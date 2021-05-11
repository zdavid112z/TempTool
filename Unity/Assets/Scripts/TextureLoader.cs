using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureLoader : MonoBehaviour
{
    void Start()
    {
        string fileId = "DOGECOIN";
        var cloud = CloudAPI.CloudAPIManager.GetInstance().cloud;
        StartCoroutine(cloud.GetFileDetailed(fileId,
            (CloudAPI.FileInfoDetailed info, long code) =>
            {
                Debug.Log($"Succeeded {info}");
                StartCoroutine(cloud.GetFileParameter(fileId, info.parameters[0],
                    (float[,,,] data, long code) =>
                    {
                        Debug.Log($"Succeeded {data}");
                    },
                    (CloudAPI.ErrorDetails error) =>
                    {
                        Debug.Log(error);
                    }
                ));
            },
            (CloudAPI.ErrorDetails error) =>
            {
                Debug.Log(error);
            }
        ));
    }

    void Update()
    {
        
    }
}
