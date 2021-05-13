using System;
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

    public class WebRequests
    {
        public static string GetStatusDescription(int code)
        {
            if (code >= 100 && code < 600)
            {
                int i = code / 100;
                int j = code % 100;

                if (j < s_HTTPStatusDescriptions[i].Length)
                    return s_HTTPStatusDescriptions[i][j];
            }

            return string.Empty;
        }

        private static readonly string[][] s_HTTPStatusDescriptions = new string[][]
        {
            null,

            new string[]
            { 
                /* 100 */"Continue",
                /* 101 */ "Switching Protocols",
                /* 102 */ "Processing"
            },

            new string[]
            { 
                /* 200 */"OK",
                /* 201 */ "Created",
                /* 202 */ "Accepted",
                /* 203 */ "Non-Authoritative Information",
                /* 204 */ "No Content",
                /* 205 */ "Reset Content",
                /* 206 */ "Partial Content",
                /* 207 */ "Multi-Status"
            },

            new string[]
            { 
                /* 300 */"Multiple Choices",
                /* 301 */ "Moved Permanently",
                /* 302 */ "Found",
                /* 303 */ "See Other",
                /* 304 */ "Not Modified",
                /* 305 */ "Use Proxy",
                /* 306 */ string.Empty,
                /* 307 */ "Temporary Redirect"
            },

            new string[]
            { 
                /* 400 */"Bad Request",
                /* 401 */ "Unauthorized",
                /* 402 */ "Payment Required",
                /* 403 */ "Forbidden",
                /* 404 */ "Not Found",
                /* 405 */ "Method Not Allowed",
                /* 406 */ "Not Acceptable",
                /* 407 */ "Proxy Authentication Required",
                /* 408 */ "Request Timeout",
                /* 409 */ "Conflict",
                /* 410 */ "Gone",
                /* 411 */ "Length Required",
                /* 412 */ "Precondition Failed",
                /* 413 */ "Request Entity Too Large",
                /* 414 */ "Request-Uri Too Long",
                /* 415 */ "Unsupported Media Type",
                /* 416 */ "Requested Range Not Satisfiable",
                /* 417 */ "Expectation Failed",
                /* 418 */ string.Empty,
                /* 419 */ string.Empty,
                /* 420 */ string.Empty,
                /* 421 */ string.Empty,
                /* 422 */ "Unprocessable Entity",
                /* 423 */ "Locked",
                /* 424 */ "Failed Dependency"
            },

            new string[]
            { 
                /* 500 */"Internal Server Error",
                /* 501 */ "Not Implemented",
                /* 502 */ "Bad Gateway",
                /* 503 */ "Service Unavailable",
                /* 504 */ "Gateway Timeout",
                /* 505 */ "Http Version Not Supported",
                /* 506 */ string.Empty,
                /* 507 */ "Insufficient Storage"
            }
        };
    }
}