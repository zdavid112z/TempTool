using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteFile : MonoBehaviour
{
    public FilesContainer files;
    public string fileId;
    // Start is called before the first frame update
    public void DeleteFileOnClick()
    {
        files.DeleteFile(this.name);
        var cloud = CloudAPI.CloudAPIManager.GetInstance().cloud;
        StartCoroutine(cloud.DeleteFile(fileId,
            (long response) =>
            {
                Debug.Log("File deleted successfully!");
            },
            (CloudAPI.ErrorDetails err) =>
            {
                Debug.LogError(err);
            }));
    }
}
