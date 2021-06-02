from mailjet_rest import Client
import os
import uuid

from  werkzeug.security import generate_password_hash

api_key = os.environ['MJ_APIKEY_PUBLIC']
api_secret = os.environ['MJ_APIKEY_PRIVATE']
mailjet = Client(auth=(api_key, api_secret))

def login_user(recipient, db):
    unique_code = uuid.uuid1().int % 900000 + 100000
    print("inaint de send")
    data = {
          'FromEmail': 'rares_stefan.epure@stud.acs.upb.ro',
          'FromName': 'Temptool',
          'Subject': 'Temptool Code', #Subject
          'Text-part': 'Dear User,\n Welcome to Temptool! Your code is: ' + str(unique_code),
          'Recipients': [{ "Email": str(recipient)}]
    }
    db.collection(u'users').document(recipient).set({u'login_code' : str(unique_code)})
    result = mailjet.send.create(data=data)
    return result