using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace CloudAPI
{
    internal abstract class RequestBody
    {
        public abstract UploadHandler GetUploadHandler();
    }

    internal class JSONRequestBody : RequestBody
    {
        public JSONRequestBody(string json)
        {
            this.json = json;
        }

        public static JSONRequestBody FromObject<T>(T obj)
        {
            return new JSONRequestBody(JsonUtility.ToJson(obj));
        }

        public override UploadHandler GetUploadHandler()
        {
            return new UploadHandlerRaw(Encoding.UTF8.GetBytes(json))
            {
                contentType = MIMEType.kJSON
            };
        }

        private readonly string json;
    }

    internal class FileRequestBody : RequestBody
    {
        public FileRequestBody(string filePath)
        {
            this.filePath = filePath;
        }

        public override UploadHandler GetUploadHandler()
        {
            return new UploadHandlerFile(filePath);
        }

        private readonly string filePath;
    }
}