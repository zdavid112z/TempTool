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
            throw new NotImplementedException();
        }

        public IEnumerator GetFileOriginal(string fileId, 
            Action<byte[], long> onSuccess, Action<ErrorDetails> onError)
        {
            return WebRequest(
                RequestType.kGET,
                $"{baseURI}/files/{fileId}",
                onSuccess,
                onError,
                null,
                new (string, string)[] { ("original", "true") });
        }

        public IEnumerator GetFileDetailed(string fileId, 
            Action<FileInfoDetailed, long> onSuccess, Action<ErrorDetails> onError)
        {
            return WebRequest(
                RequestType.kGET,
                $"{baseURI}/files/{fileId}",
                ConvertOnSuccess(onSuccess),
                onError,
                null,
                new (string, string)[] { ("original", "false") });
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
                    byte[] bytesData = Convert.FromBase64String(data.data);
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
                $"{baseURI}/login",
                ConvertOnSuccess(onSuccess),
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

        private Action<byte[], long> ConvertOnSuccess<T>(Action<T, long> onSuccess)
        {
            return (result, responseCode) =>
            {
                onSuccess(JsonUtility.FromJson<T>(
                    Encoding.UTF8.GetString(result)), responseCode);
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
                WebClient client = new WebClient();
                foreach (var (Name, Value) in queryParams)
                {
                    client.QueryString.Add(Name, Value);
                }
                uri = client.DownloadString(uri);
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