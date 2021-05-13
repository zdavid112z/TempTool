#!/bin/bash

cd /www/backend

while true; do
    # Run server
    # python3 -m swagger_server
<<<<<<< HEAD
    python3 ServerFiles/server.py
<<<<<<< Updated upstream
    sleep 3
=======
=======
    python3 ServerFile/server.py
    echo "Backend crashed with exit code $?.  Respawning.." >&2
    sleep 1
done