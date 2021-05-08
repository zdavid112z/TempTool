using System;

namespace CloudAPI
{
    [Serializable]
    public class FileData
    {
        public string name;
        public int size;
        public int upload_date;
        public int last_used_date;
        public string uploaded_by;
        public bool is_permanent;
    }

    [Serializable]
    public class FileParameterData
    {
        public string name;
        public string description;
        public string type;
        public string unit;
        public int element_bytes;
        public int width;
        public int height;
        public int number_layers;
        public int num_dates;
        public int start_date;
        public int end_date;
    }

    [Serializable]
    public class FileDataDetailed
    {
        public string filename;
        public int upload_date;
        public int last_used_date;
        public string uploaded_by;
        public bool is_permanent;
        public int size;
        public FileParameterData[] parameters;
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

