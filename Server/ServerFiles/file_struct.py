import codecs
import json
import os
import re
import base64
import zlib
from datetime import datetime, timedelta, timezone

import numpy as np
from google.cloud import storage
from netCDF4 import Dataset


class Parameter:
    def __init__(self, name, longitudes, latitudes, times, data):
        self.__name = name
        self.__longitudes = longitudes
        self.__latitudes = latitudes
        self.__times = times
        self.__data = data
        
    def get_name(self):
        return self.__name

    def convert_parameters(self, db, source_id):
        ref_param = db.collection(u'param_data').document(source_id + '_' + self.__name)
        doc = db.collection(u'files').document(source_id).collection(u'parameters').document(self.__name)

        start_date = datetime(1900, 1, 1) + timedelta(hours=int(min(self.__times[:])))
        end_date = datetime(1900, 1, 1) + timedelta(hours=int(max(self.__times[:])))
        data_to_upload = {
            u'name': self.__name,
            u'height': len(self.__latitudes),
            u'width': len(self.__longitudes),
            u'element_byte_size': 4,
            u'description': self.__data.long_name,
            u'unit': self.__data.units,
            u'lat_max': float(max(self.__latitudes[:])),
            u'lat_min': float(min(self.__latitudes[:])),
            u'long_max': float(max(self.__longitudes[:])),
            u'long_min': float(min(self.__longitudes[:])),
            u'start_date': start_date.replace(tzinfo=timezone.utc).timestamp(),
            u'end_date': end_date.replace(tzinfo=timezone.utc).timestamp(),
            u'num_layers': 1,
            u'max_layer': 1,
            u'min_layer': 1,
            u'data_ref': ref_param
        }

        doc.set(data_to_upload)

        data_array = np.array(self.__data[:])
        list_data = base64.b64encode((np.array(data_array.tolist())).flatten())
        compress_data = zlib.compress(list_data)
        filename = "param_" + source_id + "_" + str(self.__name) + ".json"
        with open(filename, 'wb') as file:
        	file.write(compress_data);

        storage_client = storage.Client()
        bucket = storage_client.get_bucket(u'temptool_database_param_data')
        blob = bucket.blob(filename)
        with open(filename, 'rb') as json_file:
            blob.upload_from_file(json_file)

        ref_param.set({u'data': ""})
        os.remove(filename)


class File():
    def __init__(self, name=None, size=0, upload_date=None, last_used_date=None,
                 uploaded_by=None, is_permanent=False, source_id=None):

        self.__name = name
        self.__size = size
        self.__upload_date = upload_date
        self.__last_used_date = last_used_date
        self.__uploaded_by = uploaded_by
        self.__is_permanent = is_permanent
        self.__id = source_id
        self.__parameters = []

    def get_name(self):
        return self.__name

    def get_size(self):
        return self.__size

    def get_upload_date(self):
        return self.__upload_date

    def get_last_used_date(self):
        return self.__last_used_date

    def set_last_used_data(self, last_used_date):
        self.__last_used_date = last_used_date

    def get_uploaded_by(self):
        return self.__uploaded_by

    def get_is_permanent(self):
        return self.__is_permanent

    def get_parameters(self):
        return self.__parameters

    def to_dict(self, db):
        ref = db.collection(u'orig_files').document(self.__name)
        data = {
            u'source_id': self.__id,
            u'name': self.__name,
            u'size': self.__size,
            u'upload_date': self.__upload_date,
            u'last_used_date': self.__last_used_date,
            u'uploaded_by': self.__uploaded_by,
            u'is_permanent': self.__is_permanent,
            u'orig_data_ref': ref
        }

        return data

    def convert(self, file):
        type(file)
        data = Dataset(file, mode='r')

        match_descript = None
        time = None
        lats = None
        longs = None
        var_time = None
        var_lat = None
        var_long = None

        for variable in data.variables.keys():
            match = re.search("lon.*", variable)

            if match:
                longs = data.variables[variable]
                var_long = variable

            match = re.search("lat.*", variable)
            if match:
                lats = data.variables[variable]
                var_lat = variable

            match = re.search("time.*", variable)

            if hasattr(data.variables[variable], 'long_name'):
                match_descript = re.search('time.*', data.variables[variable].long_name)

            if match and match_descript:
                time = data.variables[variable]
                var_time = variable

        if var_time and var_lat and var_long:
            for variable in data.variables.keys():
                if var_time in data.variables[variable].dimensions and var_long in data.variables[variable].dimensions and var_lat in data.variables[variable].dimensions:
                    self.__parameters.append(Parameter(variable, longs, lats, time, data.variables[variable]))
