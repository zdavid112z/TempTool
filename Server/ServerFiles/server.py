import json
import os
import time
import uuid

from flask_jwt_extended import JWTManager, create_access_token, jwt_required, get_jwt_identity, verify_jwt_in_request
from google.cloud import firestore
from google.cloud import storage
from flask import Flask, jsonify, render_template, request
from file_struct import File
import gcsfs

db = firestore.Client()

ALLOWED_EXTENSIONS = {'nc'}
app = Flask(__name__)


def allowed_file(filename):
    return '.' in filename and filename.rsplit('.', 1)[1].lower() in ALLOWED_EXTENSIONS


def get_generic_data(doc):
    field = json.loads('{}')
    field.update({u'name': doc.to_dict().get(u'name')})
    field.update({u'id': doc.to_dict().get(u'source_id')})
    field.update({u'size': doc.to_dict().get(u'size')})
    field.update({u'upload_date': doc.to_dict().get(u'upload_date')})
    field.update({u'last_used_date': doc.to_dict().get(u'last_used_date')})
    field.update({u'uploaded_by': doc.to_dict().get(u'uploaded_by')})
    field.update({u'is_permanent': doc.to_dict().get(u'is_permanent')})
    return field


def delete_collection(coll_ref, batch_size):
    docs = coll_ref.limit(batch_size).stream()
    deleted = 0

    for doc in docs:
        print(f'Deleting doc {doc.id} => {doc.to_dict()}')
        doc.reference.delete()
        deleted = deleted + 1

    if deleted >= batch_size:
        return delete_collection(coll_ref, batch_size)


def file_already_exists(filename):
    documents = db.collection(u'files')
    for doc in documents.stream():
        if doc.to_dict()['name'] == filename:
            return True

    return False

@app.route('/')
def form():
    return render_template('form.html')

@app.route('/api/files', methods=['GET'])
def get_files():
    documents = db.collection(u'files').stream()

    fields = []
    for doc in documents:
        fields.append(get_generic_data(doc))

    resp = jsonify(fields)
    resp.status_code = 200
    return resp


@app.route('/api/files', methods=['POST'])
# @jwt_required(optional=True)
def upload_file():
    verify_jwt_in_request(optional=True)
    current_user = get_jwt_identity()
    is_admin = db.collection(u'admins').document(str(current_user)).get().exists
    # is_admin = False

    filename = request.json['filename']
    data_b64 = request.json['data']

    if filename is None or data_b64 is None or filename == '':
        resp = jsonify({'message': 'No file part in the request'})
        resp.status_code = 400
        return resp

    if allowed_file(filename):
        data_comp = base64.b64decode(data_b64)
        data = zlib.decompress(data_comp)
        size = len(data)

        unique_id = str(uuid.uuid1())

        db.collection(u'orig_files').document(unique_id).set({u'data': u''})

        f = open('tmp.nc', "wb")
        f.write(data)
        f.close()
        uploaded_file = File(filename, size, time.time(), time.time(),
                             None, is_admin, unique_id)
        uploaded_file.convert('tmp.nc')

        for param in uploaded_file.get_parameters():
            param.convert_parameters(db, unique_id)

        db.collection(u'files').document(unique_id).set(uploaded_file.to_dict(db))

        uploaded_file.close()
        os.remove('tmp.nc')

        resp = jsonify({"id": unique_id})
        resp.status_code = 201
        return resp

    resp = jsonify({'message': 'Allowed file type is netcdf4'})
    resp.status_code = 401
    return resp


@app.route('/api/files/<fileid>', methods=['DELETE'])
def delete_specific_file(fileid):
    verify_jwt_in_request(optional=True)
    if db.collection(u'files').document(fileid).get().exists:

        if db.collection(u'files').document(fileid).get().to_dict().get(u'is_permanent') and not db.collection(u'admins').document(str(get_jwt_identity())).get().exists: 
            return make_response("Unauthorized", 401)
        for doc in db.collection(u'files').document(fileid).collection('parameters').stream():
            db.collection('param_data').document(fileid + '_' + doc.id).delete()

        storage_client = storage.Client()
        bucket = storage_client.get_bucket(u'temptool_database_param_data')
        blobs = bucket.list_blobs(prefix=('param_' + str(fileid)))
        for blob in blobs:
            blob.delete()

        doc_param = db.collection(u'files').document(fileid).collection('parameters')
        delete_collection(doc_param, max(1, len(list(doc_param.get()))))
        db.collection(u'orig_files').document(fileid).delete()
        db.collection(u'files').document(fileid).delete()

        resp = jsonify({'message': 'OK'})
        resp.status_code = 200
        return resp
    else:
        resp = jsonify({'message': 'Not Found'})
        resp.status_code = 404
        return resp


@app.route('/api/files/<fileid>', methods=['GET'])
def get_specific_data(fileid):
    if db.collection(u'files').document(fileid).get().exists:
        doc = db.collection(u'files').document(fileid)
        doc.update({'last_used_date': time.time()})
        field = get_generic_data(doc.get())

        parameters = [{name: document.to_dict().get(name) for name in document.to_dict().keys() if name != "data_ref"}
                      for document in doc.collection(u'parameters').stream()]

        field.update({u'parameters': parameters})
        resp = jsonify(field)
        resp.status_code = 200
        return resp
    else:
        resp = jsonify({'message': 'Not Found'})
        resp.status_code = 404
        return resp


@app.route('/api/files/<fileid>/<parameter>', methods=['GET'])
def get_parameter(fileid, parameter):
    if db.collection(u'files').document(fileid).collection(u'parameters').document(parameter).get().exists:
        doc = db.collection(u'files').document(fileid)
        doc.update({'last_used_date': time.time()})

        gcs_file_system = gcsfs.GCSFileSystem()
        gcs_json_path = "gs://temptool_database_param_data/" + "param_" + str(fileid) + "_" + str(parameter) + ".json"
        with gcs_file_system.open(gcs_json_path) as file:
            field = json.load(file)

        resp = jsonify({ "data" : field.decode("utf-8") })
        resp.status_code = 200
        return resp
    
    resp = jsonify({'message': 'Not Found'})
    resp.status_code = 404
    return resp


@app.route('/api/files/original/<fileid>', methods=['GET'])
def get_data(fileid):
    if db.collection(u'files').document(fileid).get().exists:
        doc = db.collection(u'orig_files').document(fileid)
        data = doc.get().to_dict().get(u'data')
        resp = jsonify(data)
        resp.status_code = 200
        return resp

    resp = jsonify({'message': 'Not Found'})
    resp.status_code = 404
    return resp


@app.route('/api/login', methods=['POST'])
def login():
    auth = request.form

    if not auth or not auth.get('email'):
        return make_response('Could not verify', 
                401,
                {'WWW-Authenticate' : 'Basic realm ="Login required !!"'}
        )

    if not auth.get('login_code'):
        response = login_user(auth.get('email'), db)
        return make_response('Mailjet response', response.status_code)

    user = db.collection(u'users').document(auth.get('email'))

    # returns 401 if email is wrong
    if not (user.get()).exists:
        return make_response(
            'Could not verify',
            401,
            {'WWW-Authenticate' : 'Basic realm ="User does not exist !!"'}
        )

    print(auth.get('login_code'))
    if check_password_hash((user.get().to_dict().get(u'login_code')), auth.get('login_code')):
        token = create_access_token(identity=auth.get('email'))
        return make_response(jsonify({'token' : token}, {"user_id" : auth.get('email')}), 201)
    # returns 403 if login code is wrong
    return make_response(
        'Could not verify',
        403,
        {'WWW-Authenticate' : 'Basic realm ="Wrong Password !!"'}
    )


@app.route('/api/admins', methods=['GET'])
def get_admins():
    resp = jsonify({'message': 'OK'})
    resp.status_code = 200
    return resp


@app.route('/api/admins', methods=['POST'])
def add_admin():
    current_user = get_jwt_identity()

    if db.collection(u'admins').document(str(current_user)).get().exists:

        new_admin = request.form.get(u'admin')
        if db.collection(u'admins').document(str(new_admin)).get().exists:
            return make_response("Conflict", 403)

        db.collection(u'admins').document(str(new_admin)).set({})
        resp = jsonify({'message': 'OK', 'result': fields})
        resp.status_code = 200
        return resp

    return make_response("Unauthorized", 401)


@app.route('/api/admins', methods=['DELETE'])
def delete_admin():
    current_user = get_jwt_identity()
    if db.collection(u'admins').document(str(current_user)).get().exists:

        new_admin = request.form.get(u'admin')
        if db.collection(u'admins').document(str(new_admin)).get().exists:
            db.collection(u'admins').document(str(new_admin)).delete()
            resp = jsonify({'message': 'OK', 'result': fields})
            resp.status_code = 200
            return resp

        return make_response("Not Found!", 404)

    return make_response("Unauthorized", 401)


# if __name__ == '__main__':
#     db = firestore.Client()
#     app.run(debug=True, port=8080)
