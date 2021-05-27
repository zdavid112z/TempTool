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

        t = self.__times[:]
        start_date = datetime(1970, 1, 1) + timedelta(hours=min(1756800, max(0, int(np.min(t)))))
        end_date = datetime(1970, 1, 1) + timedelta(hours=min(1756800, max(0, int(np.max(t)))))
        data_to_upload = {
            u'name': self.__name,
            u'height': len(self.__latitudes),
            u'width': len(self.__longitudes),
            u'num_dates': len(self.__times),
            u'element_byte_size': 4,
            u'description': self.__data.long_name,
            u'unit': self.__data.units if self.__data.units is not None else '',
            u'lat_max': float(max(self.__latitudes[:])),
            u'lat_min': float(min(self.__latitudes[:])),
            u'lon_max': float(max(self.__longitudes[:])),
            u'lon_min': float(min(self.__longitudes[:])),
            u'missing_value': np.asscalar(self.__data.missing_value) if self.__data.missing_value is not None else float('-inf'),
            u'start_date': start_date.replace(tzinfo=timezone.utc).timestamp(),
            u'end_date': end_date.replace(tzinfo=timezone.utc).timestamp(),
            u'num_layers': 1,
            u'max_layer': 1,
            u'min_layer': 1,
            u'data_ref': ref_param
        }

        doc.set(data_to_upload)

        # data_array = np.array(self.__data[:])
        # list_data = base64.b64encode((np.array(data_array.tolist())).flatten())
        # compress_data = zlib.compress(list_data)
        data_to_write = base64.b64encode(zlib.compress(self.__data[:].astype(np.dtype('float32')).tobytes()))
        filename = "param_" + source_id + "_" + str(self.__name) + ".json"
        with open(filename, 'wb') as file:
            file.write(data_to_write)

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
        self.__data = None

    def close(self):
        self.__data.close()

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
        self.__data = Dataset(file, mode='r')
        data = self.__data

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

            if match or match_descript:
                time = data.variables[variable]
                var_time = variable

        if var_time and var_lat and var_long:
            for variable in data.variables.keys():
                v = data.variables[variable]
                if var_time in v.dimensions and var_long in v.dimensions and var_lat in v.dimensions and (v.dtype == np.dtype("float32") or v.dtype == np.dtype("float64")):
                    self.__parameters.append(Parameter(variable, longs, lats, time, data.variables[variable]))
