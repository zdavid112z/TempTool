FROM zdavid112z/temptool:0.3
USER root

ARG TEMPTOOL_PROD=1
COPY build/WebGL/WebGL /www/data
COPY Server /www/backend
COPY default.conf /etc/nginx/conf.d
RUN chmod +x /www/backend/start.sh
RUN chmod +x /www/backend/run_backend.sh
RUN pip3 install -r Server/requirements.txt