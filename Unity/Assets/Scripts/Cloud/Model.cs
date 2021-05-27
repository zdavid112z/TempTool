using System;
using UnityEngine.Assertions;

namespace CloudAPI
{
    [Serializable]
    public class Wrapper<T>
    {
        public T obj;
    }

    [Serializable]
    public class FileInfo
    {
        public string id;
        public string name;
        public int size;
        public long upload_date;
        public long last_used_date;
        public string uploaded_by;
        public bool is_permanent;
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
        public long start_date;
        public long end_date;
        public float lon_max;
        public float lon_min;
        public float lat_max;
        public float lat_min;
        public float layer_max;
        public float layer_min;
        public float missing_value;
    }

    [Serializable]
    public class FileInfoDetailed
    {
        public string id;
        public string filename;
        public long upload_date;
        public long last_used_date;
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

    public class FileParameterDataBin
    {
        public float vmin, vmax;
        public FileParameterInfo info;
        public float[] data;

        public void UpdateMinMax()
        {
            vmin = float.MaxValue;
            vmax = float.MinValue;
            foreach (float f in data)
            {
                if (f != info.missing_value)
                {
                    vmin = Math.Min(vmin, f);
                    vmax = Math.Max(vmax, f);
                }
            }
        }

        public float this[int t, int l, int y, int x]
        {
            get
            {
                Assert.IsTrue(
                    t >= 0 && t < info.num_dates &&
                    l >= 0 && l < info.num_layers &&
                    y >= 0 && y < info.height &&
                    x >= 0 && x < info.width);
                return data[t * info.num_layers * info.height * info.width +
                    l * info.height * info.width + y * info.width + x];
            }
            set
            {
                Assert.IsTrue(
                    t >= 0 && t < info.num_dates &&
                    l >= 0 && l < info.num_layers &&
                    y >= 0 && y < info.height &&
                    x >= 0 && x < info.width);
                data[t * info.num_layers * info.height * info.width +
                    l * info.height * info.width + y * info.width + x] = value;
            }
        }

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
        public long expiration_date;
    }

    [Serializable]
    public class AdminData
    {
        public string email;
    }
}

