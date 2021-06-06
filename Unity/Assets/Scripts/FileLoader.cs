using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using WebGLFileUploader;

public class FileLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("WebGLFileUploadManager.getOS: " + WebGLFileUploadManager.getOS);
        Debug.Log("WebGLFileUploadManager.isMOBILE: " + WebGLFileUploadManager.IsMOBILE);
        Debug.Log("WebGLFileUploadManager.getUserAgent: " + WebGLFileUploadManager.GetUserAgent);

        WebGLFileUploadManager.SetDebug(true);
        WebGLFileUploadManager.Show(false);
        WebGLFileUploadManager.SetDescription("Select netcdf files (.nc)");
        WebGLFileUploadManager.SetImageEncodeSetting(true);
        WebGLFileUploadManager.SetAllowedFileName("\\.(nc)$");
        WebGLFileUploadManager.onFileUploaded += OnFileUploaded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnUpload()
    {
        WebGLFileUploadManager.PopupDialog(null, "Select netcdf files (.nc)");
    }

    private void OnFileUploaded(UploadedFileInfo[] result)
    {
        if (result.Length == 0)
        {
            Debug.Log("File upload Error!");
        }
        else
        {
            Debug.Log("File upload success! (result.Length: " + result.Length + ")");
        }

        foreach (UploadedFileInfo file in result)
        {
            if (file.isSuccess)
            {
                Debug.Log("file.filePath: " + file.filePath + " exists:" + File.Exists(file.filePath));

                byte[] byteArray = File.ReadAllBytes(file.filePath);
                for (int i = 0; i < 16; i++)
                {
                    if (i < byteArray.Length)
                        Debug.Log((int)byteArray[i]);
                }

                Debug.Log("File.ReadAllBytes : byte[].Length: " + byteArray.Length);
                Debug.Log(file.name);

                var text = GameObject.Find("Upload Status Label").GetComponent<Text>();
                text.text = "Uploading...";
                var cloud = CloudAPI.CloudAPIManager.GetInstance().cloud;
                StartCoroutine(cloud.PostFile(file.name, byteArray,
                    (long code) =>
                    {
                        text.text = "File uploaded successfully!";
                        Debug.Log("File uploaded successfully!");
                    }, (CloudAPI.ErrorDetails err) =>
                    {
                        text.text = "File upload failed";
                        Debug.LogError(err);
                    }));
                break;
            }
        }
    }
}
