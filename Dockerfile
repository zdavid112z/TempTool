FROM zdavid112z/temptool:0.3
USER root

COPY build/WebGL/WebGL /www/data
COPY Server /www/backend
COPY default.conf /etc/nginx/conf.d
RUN chmod +x /www/backend/start.sh
RUN chmod +x /www/backend/run_backend.sh
