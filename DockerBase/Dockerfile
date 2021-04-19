FROM nginx:latest
MAINTAINER zdavid112z zdavid112z@gmail.com
USER root
ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get upgrade -y
RUN apt-get install software-properties-common python3.7 build-essential python3.7-distutils libhdf5-103 libhdf5-dev libnetcdf-c++4 libnetcdf-c++4-dev libnetcdf-dev -y
RUN curl https://bootstrap.pypa.io/get-pip.py -o get-pip.py
RUN python3.7 get-pip.py
RUN rm -rf get-pip.py
RUN mkdir -p /usr/src/app
RUN pip3 install flask_testing coverage nose pluggy py randomize tox connexion python_dateutil setuptools flask pytest google-cloud-firestore numpy cftime netCDF4 mailjet_rest pyjwt

RUN rm /etc/nginx/conf.d/default.conf
RUN mkdir -p /www/data
RUN mkdir -p /www/backend
RUN sed -n -i 'p;2a sh /www/backend/start.sh ####"' /docker-entrypoint.sh
RUN sed -n -i 'p;2a echo "#### Running sh /www/backend/start.sh ####"' /docker-entrypoint.sh
