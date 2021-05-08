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

        public IEnumerator GetFiles(Action<FileData[], long> onSuccess, Action<ErrorDetails> onError)
        {
            return WebRequest(
                RequestType.kGET,
                $"{baseURI}/files",
                ConvertOnSuccess(onSuccess),
                onError);
        }

        public IEnumerator PostFile(string fileName, byte[] fileData, 
            Action<long> onSuccess, Action<ErrorDetails> onError)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetFileOriginal(string fileName, 
            Action<byte[], long> onSuccess, Action<ErrorDetails> onError)
        {
            return WebRequest(
                RequestType.kGET,
                $"{baseURI}/files/{fileName}",
                onSuccess,
                onError,
                null,
                new (string, string)[] { ("original", "true") });
        }

        public IEnumerator GetFileDetailed(string fileName, 
            Action<FileDataDetailed, long> onSuccess, Action<ErrorDetails> onError)
        {
            return WebRequest(
                RequestType.kGET,
                $"{baseURI}/files/{fileName}",
                ConvertOnSuccess(onSuccess),
                onError,
                null,
                new (string, string)[] { ("original", "false") });
        }

        public IEnumerator DeleteFile(string fileName, 
            Action<long> onSuccess, Action<ErrorDetails> onError)
        {
            return WebRequest(
                RequestType.kDELETE,
                $"{baseURI}/files/{fileName}",
                ConvertOnSuccess(onSuccess),
                onError);
        }

        public IEnumerator GetFileParameter(string fileName, FileParameterData parameterData,
            Action<float[,,,], long> onSuccess, Action<ErrorDetails> onError)
        {
            return WebRequest(
                RequestType.kGET,
                $"{baseURI}/files/{fileName}/{parameterData.name}",
                (byte[] body, long responseCode) => {
                    onSuccess(
                        DataConverter.FromBytes(body,
                        parameterData.num_dates,
                        parameterData.number_layers,
                        parameterData.height,
                        parameterData.width),
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
