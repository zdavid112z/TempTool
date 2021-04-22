#!/bin/bash

while true; do
    cd /www/backend
    # Run server
    # python3 -m swagger_server
    sleep 100000
    echo "Backend crashed with exit code $?.  Respawning.." >&2
    sleep 1
done
