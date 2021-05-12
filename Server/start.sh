#!/bin/bash

printenv FIRESTORE_KEY > /www/backend/firestore_key.json

/www/backend/run_backend.sh &
