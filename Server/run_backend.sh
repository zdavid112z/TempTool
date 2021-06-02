#!/bin/bash

cd /www/backend/ServerFiles

while true; do
    # Run server
    flask run --no-debugger --no-reload --port=8080
    echo "Backend crashed with exit code $?.  Respawning.." >&2
    sleep 1
done