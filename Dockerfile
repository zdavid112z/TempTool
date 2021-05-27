FROM zdavid112z/temptool:0.3
USER root

ENV TEMPTOOL_PROD 1
ENV GOOGLE_APPLICATION_CREDENTIALS "/www/backend/firestore_key.json"
ENV GCLOUD_PROJECT "temptool"
ENV FLASK_APP "server.py"
ENV FLASK_ENV "development"
ENV FLASK_DEBUG "0"

COPY build/WebGL/WebGL /www/data
COPY Server /www/backend
COPY default.conf /etc/nginx/conf.d

RUN chmod +x /www/backend/start.sh
RUN chmod +x /www/backend/run_backend.sh

RUN pip3 install -r /www/backend/requirements.txt

