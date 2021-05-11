using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace CloudAPI
{
    public class ErrorDetails
    {
        public UnityWebRequest.Result result;
        public string error;
        public long responseCode;
        public byte[] body;
    }

    public class MIMEType
    {
        public const string kJSON = "application/json";
        public const string kBinary = "application/octet-stream";
    }

    public class RequestType
    {
        public const string kGET = UnityWebRequest.kHttpVerbGET;
        public const string kPUT = UnityWebRequest.kHttpVerbPUT;
        public const string kPOST = UnityWebRequest.kHttpVerbPOST;
        public const string kDELETE = UnityWebRequest.kHttpVerbDELETE;
    }

    /// <summary>
    /// Interface to communicate with the Cloud.
    /// 
    /// All methods are coroutines. If a request succeeds, then the onSuccess
    /// function is called with the returned body (optional) and the response
    /// code, which cannot be 4XX or 5XX. The onError function is not called.
    /// 
    /// If a request fails due to any issue, including connection errors and
    /// bad response codes, the onError function is called. The 'result' field
    /// in the ErrorDetails class can be ConnectionError, ProtocolError or
    /// DataProcessingError. The 'error' field contains a human readable
    /// description of the error. The respose code and returned body (if it
    /// exists) are also included. The onSuccess function is not called.
    /// </summary>
    public interface ICloudAPI
    {
        /// <summary>
        /// Get the list of files available
        /// </summary>
        /// <param name="onSuccess">Function to call if the request succeeds.
        /// The first parameter is the list of files and the second one is the
        /// response code</param>
        /// <param name="onError">Function to call if the request fails</param>
        IEnumerator GetFiles(Action<FileInfo[], long> onSuccess, Action<ErrorDetails> onError);

        /// <summary>
        /// Uploads a file
        /// </summary>
        /// <param name="filePath">The path to the file to upload</param>
        /// <param name="onSuccess">Function to call if the request succeeds.
        /// The parameter is the response code from the request</param>
        /// <param name="onError">Function to call if the request fails</param>
        IEnumerator PostFile(string filePath,
            Action<long> onSuccess, Action<ErrorDetails> onError);

        /// <summary>
        /// Get the original data from a selected file, as it was when it was
        /// originally uploaded
        /// </summary>
        /// <param name="fileId">The id of the file to downlaod</param>
        /// <param name="onSuccess">Function to call if the request succeeds.
        /// The first parameter is an array of bytes representing the contents
        /// of the original file and the second one is the request response
        /// code</param>
        /// <param name="onError">Function to call if the request fails</param>
        IEnumerator GetFileOriginal(string fileId, 
            Action<byte[], long> onSuccess, Action<ErrorDetails> onError);

        /// <summary>
        /// Get the matrix of data points for a selected parameter
        /// </summary>
        /// <param name="fileId">The id of the file which contains the
        /// parameter</param>
        /// <param name="parameterData">The parameter to download
        /// </param>
        /// <param name="onSuccess">Function to call if the request succeeds.
        /// The first parameter is the matrix of data points and the second
        /// one is the request response code</param>
        /// <param name="onError">Function to call if the request fails</param>
        public IEnumerator GetFileParameter(string fileId, FileParameterInfo parameterData,
            Action<float[,,,], long> onSuccess, Action<ErrorDetails> onError);

        /// <summary>
        /// Get details about a selected file, including a list of its
        /// parameters
        /// </summary>
        /// <param name="fileId">The id of the file whose details to
        /// download</param>
        /// <param name="onSuccess">Function to call if the request succeeds.
        /// The first parameter contains the details of the file and the second
        /// one is the request response code</param>
        /// <param name="onError">Function to call if the request fails</param>
        IEnumerator GetFileDetailed(string fileId, 
            Action<FileInfoDetailed, long> onSuccess, Action<ErrorDetails> onError);

        /// <summary>
        /// Send a request to delete a file
        /// </summary>
        /// <param name="fileId">The id of the file to delete</param>
        /// <param name="onSuccess">Function to call if the request succeeds.
        /// The parameter is the response code from the request</param>
        /// <param name="onError">Function to call if the request fails</param>
        IEnumerator DeleteFile(string fileId, 
            Action<long> onSuccess, Action<ErrorDetails> onError);

        /// <summary>
        /// Send a login request. Leaving the 'login_code' field as null leads
        /// to, if the request succeeds, sending an email to the user at the
        /// provided email. If the field is not null, then the request returns
        /// a valid JWT which is automatically stored.
        /// </summary>
        /// <param name="request">The data of the user to authenticate</param>
        /// <param name="onSuccess">Function to call if the request succeeds.
        /// The first parameter contains the expiration date of the token, if
        /// it was returned, otherwise it is null. The second parameter is the
        /// response code from the request</param>
        /// <param name="onError">Function to call if the request fails</param>
        IEnumerator PostLogin(LoginRequestData request, 
            Action<LoginResponse, long> onSuccess, Action<ErrorDetails> onError);

        /// <summary>
        /// Get the list of all admins
        /// </summary>
        /// <param name="onSuccess">Function to call if the request succeeds.
        /// The first parameter is the list of admins and the second one is the
        /// response code from the request</param>
        /// <param name="onError">Function to call if the request fails</param>
        IEnumerator GetAdmins(Action<AdminData[], long> onSuccess, Action<ErrorDetails> onError);

        /// <summary>
        /// Add an admin if the user is authenticated
        /// </summary>
        /// <param name="admin">The admin data to add</param>
        /// <param name="onSuccess">Function to call if the request succeeds.
        /// The parameter is the response code from the request</param>
        /// <param name="onError">Function to call if the request fails</param>
        IEnumerator PostAdmin(AdminData admin, 
            Action<long> onSuccess, Action<ErrorDetails> onError);

        /// <summary>
        /// Delete an admin
        /// </summary>
        /// <param name="admin">The admin data to delete</param>
        /// <param name="onSuccess">Function to call if the request succeeds.
        /// The parameter is the response code from the request</param>
        /// <param name="onError">Function to call if the request fails</param>
        IEnumerator DeleteAdmin(AdminData admin, 
            Action<long> onSuccess, Action<ErrorDetails> onError);
    }
}
