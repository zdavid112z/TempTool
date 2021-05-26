#!/bin/bash

cd /www/backend

while true; do
    # Run server
    # python3 -m swagger_server
    python3 ServerFiles/server.py

    sleep 3


    echo "Backend crashed with exit code $?.  Respawning.." >&2
    sleep 1
done
