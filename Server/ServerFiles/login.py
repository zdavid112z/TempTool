from mailjet_rest import Client
import os
import uuid

from  werkzeug.security import generate_password_hash

api_key = os.environ['MJ_APIKEY_PUBLIC']
api_secret = os.environ['MJ_APIKEY_PRIVATE']
mailjet = Client(auth=(api_key, api_secret))

def login_user(recipient, db):
  unique_code = uuid.uuid1();
  print("inaint de send")
  data = {
          'FromEmail': 'rares_stefan.epure@stud.acs.upb.ro',
          'FromName': 'Temptool',
          'Subject': 'Temptool Code', #Subject
          'Text-part': 'Dear User,\n Welcome to Temptool! Your code is: ' + str(unique_code),
           'Recipients': [{ "Email": str(recipient)}]
            }
  print("date ")
  db.collection(u'users').document(recipient).set({u'login_code' : generate_password_hash(unique_code)});
  print("ia baza")
  result = mailjet.send.create(data=data)
  print("trimite")
  print (result.status_code)
  # print (result.json())
  return result