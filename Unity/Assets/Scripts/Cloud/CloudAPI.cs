using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Net;

namespace CloudAPI
{
    internal class CloudAPI : ICloudAPI
    {
        private class SessionData
        {
            public string jwt;
        }

        public CloudAPI(string baseURI)
        {
            this.baseURI = baseURI;
            session = new SessionData()
            {
                jwt = null
            };
        }

        public IEnumerator GetFiles(Action<FileInfo[], long> onSuccess, Action<ErrorDetails> onError)
        {
            return WebRequest(
                RequestType.kGET,
                $"{baseURI}/files",
                ConvertOnSuccess(onSuccess),
                onError);
        }

        public IEnumerator PostFile(string filePath,
            Action<long> onSuccess, Action<ErrorDetails> onError)
        {
            return WebRequest(
                RequestType.kPOST,
                $"{baseURI}/files",
                ConvertOnSuccess(onSuccess),
                onError,
                new FileRequestBody(filePath));
        }

        public IEnumerator GetFileOriginal(string fileId, 
            Action<byte[], long> onSuccess, Action<ErrorDetails> onError)
        {
            return WebRequest(
                RequestType.kGET,
                $"{baseURI}/files/original/{fileId}",
                onSuccess,
                onError);
        }

        public IEnumerator GetFileDetailed(string fileId, 
            Action<FileInfoDetailed, long> onSuccess, Action<ErrorDetails> onError)
        {
            return WebRequest(
                RequestType.kGET,
                $"{baseURI}/files/{fileId}",
                ConvertOnSuccess(onSuccess),
                onError);
        }

        public IEnumerator DeleteFile(string fileId, 
            Action<long> onSuccess, Action<ErrorDetails> onError)
        {
            return WebRequest(
                RequestType.kDELETE,
                $"{baseURI}/files/{fileId}",
                ConvertOnSuccess(onSuccess),
                onError);
        }

        public IEnumerator GetFileParameter(string fileId, FileParameterInfo parameterData,
            Action<FileParameterDataBin, long> onSuccess, Action<ErrorDetails> onError)
        {
            return WebRequest(
                RequestType.kGET,
                $"{baseURI}/files/{fileId}/{parameterData.name}",
                (byte[] body, long responseCode) => {
                    FileParameterData data = 
                        JsonUtility.FromJson<FileParameterData>(
                            Encoding.UTF8.GetString(body));
                    byte[] bytesDataCompressed = Convert.FromBase64String(data.data);
                    byte[] bytesData = Compression.Inflate(bytesDataCompressed);
                    onSuccess(
                        DataConverter.FromBytes(bytesData, parameterData),
                        responseCode);
                },
                onError);
        }

        public IEnumerator PostLogin(LoginRequestData request, 
            Action<LoginResponse, long> onSuccess, Action<ErrorDetails> onError)
        {
            return WebRequest(
                RequestType.kPOST,
                $"{baseURI}/auth",
                ConvertOnSuccess(PeekLoginResponse(onSuccess)),
                onError,
                JSONRequestBody.FromObject(request));
        }

        public IEnumerator GetAdmins(Action<AdminData[], long> onSuccess, Action<ErrorDetails> onError)
        {
            return WebRequest(
                RequestType.kGET,
                $"{baseURI}/admins",
                ConvertOnSuccess(onSuccess),
                onError);
        }

        public IEnumerator PostAdmin(AdminData admin, 
            Action<long> onSuccess, Action<ErrorDetails> onError)
        {
            return WebRequest(
                RequestType.kPOST,
                $"{baseURI}/admins",
                ConvertOnSuccess(onSuccess),
                onError,
                JSONRequestBody.FromObject(admin));
        }

        public IEnumerator DeleteAdmin(AdminData admin,
            Action<long> onSuccess, Action<ErrorDetails> onError)
        {
            return WebRequest(
                RequestType.kDELETE,
                $"{baseURI}/admins",
                ConvertOnSuccess(onSuccess),
                onError,
                JSONRequestBody.FromObject(admin));
        }

        private Action<LoginResponse, long> PeekLoginResponse(Action<LoginResponse, long> onSuccess)
        {
            return (result, responseCode) =>
            {
                if (result != null && result.token != null && result.token.Length != 0)
                {
                    session.jwt = result.token;
                }
                onSuccess(result, responseCode);
            };
        }

        private Action<byte[], long> ConvertOnSuccess<T>(Action<T, long> onSuccess)
        {
            return (result, responseCode) =>
            {
                string str = Encoding.UTF8.GetString(result);
                if (typeof(T).IsArray)
                {
                    Wrapper<T> wrapper = default;
                    try
                    {
                        wrapper = JsonUtility.FromJson<Wrapper<T>>("{\"obj\":" + str + "}"); ;
                    } catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                    onSuccess(wrapper.obj, responseCode);
                }
                else
                {
                    T obj = default;
                    try
                    {
                        obj = JsonUtility.FromJson<T>(str);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                    onSuccess(obj, responseCode);
                }
            };
        }

        private Action<byte[], long> ConvertOnSuccess(Action<long> onSuccess)
        {
            return (result, responseCode) =>
            {
                onSuccess(responseCode);
            };
        }

        private void SetRequestHeaders(UnityWebRequest req)
        {
            if (session.jwt != null)
                req.SetRequestHeader("Authorization", $"Bearer {session.jwt}");
        }

        IEnumerator WebRequest(
            string method, string uri,
            Action<byte[], long> onSuccess,
            Action<ErrorDetails> onError,
            RequestBody body = null,
            (string Name, string Value)[] queryParams = null)
        {
            if (queryParams != null)
            {
                UriBuilder builder = new UriBuilder(uri);
                bool first = true;
                foreach (var (Name, Value) in queryParams)
                {
                    uri += first ? "?" : "&";
                    uri += Name + "=" + Value;
                    first = false;
                }
            }

            UnityWebRequest req = new UnityWebRequest(uri, method);
            SetRequestHeaders(req);
            req.downloadHandler = new DownloadHandlerBuffer();
            if (body != null)
                req.uploadHandler = body.GetUploadHandler();

            yield return req.SendWebRequest();
            while (!req.isDone)
                yield return null;

            byte[] response = req.downloadHandler.data;

            if (req.result != UnityWebRequest.Result.Success)
            {
                ErrorDetails error = new ErrorDetails
                {
                    result = req.result,
                    error = req.error,
                    responseCode = req.responseCode,
                    body = response
                };
                onError(error);
                yield break;
            }

            onSuccess(response, req.responseCode);
        }

        private readonly SessionData session;
        private readonly string baseURI;
    }
}
