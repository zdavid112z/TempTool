#!/bin/bash

while true; do
    cd /www/backend
    python3 -m swagger_server
    echo "Backend crashed with exit code $?.  Respawning.." >&2
    sleep 1
done
