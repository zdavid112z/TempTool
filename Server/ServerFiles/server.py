from flask import Flask, jsonify

app = Flask(__name__)


@app.route('/api/files', methods=['GET', 'POST'])
def upload_file():
    resp = jsonify({'message': 'OK'})
    resp.status_code = 200
    return resp


@app.route('/api/files/<filename>', methods=['GET', 'DELETE'])
def actions(filename):
    resp = jsonify({'message': 'OK'})
    resp.status_code = 200
    return resp


@app.route('/api/files/<filename>/<parameter>', methods=['GET'])
def get_parameter(filename, parameter):
    resp = jsonify({'message': 'OK'})
    resp.status_code = 200
    return resp


@app.route('/api/login', methods=['POST'])
def login():
    resp = jsonify({'message': 'OK'})
    resp.status_code = 200
    return resp


@app.route('/api/admins', methods=['GET'])
def get_admins():
    resp = jsonify({'message': 'OK'})
    resp.status_code = 200
    return resp


@app.route('/api/admins', methods=['POST'])
def add_admin():
    resp = jsonify({'message': 'OK'})
    resp.status_code = 200
    return resp


@app.route('/api/admins', methods=['DELETE'])
def delete_admin():
    resp = jsonify({'message': 'OK'})
    resp.status_code = 200
    return resp


if __name__ == '__main__':
    app.run(debug=True, port=8080)
