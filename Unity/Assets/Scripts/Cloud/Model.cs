using System;

namespace CloudAPI
{
    [Serializable]
    public class FileInfo
    {
        public string id;
        public string name;
        public int size;
        public int upload_date;
        public int last_used_date;
        public string uploaded_by;
        public bool is_permanent;
    }

    [Serializable]
    public class FileUploadData
    {
        public string filename;
        public string data;
    }

    [Serializable]
    public class FileParameterInfo
    {
        public string name;
        public string description;
        public string type;
        public string unit;
        public int element_bytes;
        public int width;
        public int height;
        public int num_layers;
        public int num_dates;
        public int start_date;
        public int end_date;
        public float lon_max;
        public float lon_min;
        public float lat_max;
        public float lat_min;
        public float layer_max;
        public float layer_min;
    }

    [Serializable]
    public class FileInfoDetailed
    {
        public string id;
        public string filename;
        public int upload_date;
        public int last_used_date;
        public string uploaded_by;
        public bool is_permanent;
        public int size;
        public FileParameterInfo[] parameters;
    }

    [Serializable]
    public class FileParameterData
    {
        public string data;
    }

    [Serializable]
    public class LoginRequestData
    {
        public string email;
        public string login_code;
    }

    [Serializable]
    public class LoginResponse
    {
        public int expiration_date;
    }

    [Serializable]
    public class AdminData
    {
        public string email;
    }
}

