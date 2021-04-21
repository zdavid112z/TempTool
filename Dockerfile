FROM zdavid112z/temptool:0.2

COPY build/WebGL/WebGL /www/data
COPY Server /www/backend
COPY default.conf /etc/nginx/conf.d
