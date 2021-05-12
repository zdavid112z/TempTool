using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureLoader : MonoBehaviour
{
    public void SetMaterialData(CloudAPI.FileParameterDataBin data, int layer)
    {
        int num_dates = data.info.num_dates;
        int w = data.info.width;
        int h = data.info.height;
        int l = data.info.num_layers;
        Texture2DArray array = new Texture2DArray(w, h, num_dates, TextureFormat.RFloat, false, false);
        for (int t = 0; t < num_dates; t++)
            array.SetPixelData<float>(data.data, 0, t, l * w * h * t);
        array.Apply(false);
        var material = GetComponentInParent<Renderer>().material;
        material.SetTexture("Texture2DArray_28e7e253cb18486a8c14899af2143136", array); // Textures
        material.SetVector("Vector2_540929bd5a9e4b91803993f7ccde8e73", new Vector4(data.info.lon_min, data.info.lon_max)); // LonMinMax
        material.SetVector("Vector2_a03a182a418a49fd8146e64479d05bb6", new Vector4(data.info.lat_min, data.info.lat_max)); // LatMinMax
        material.SetFloat("Vector1_8211db379e8f4f01b70f108ef594b658", data.info.width); // Width
        material.SetFloat("Vector1_51fb8cd46af548f6af8aef823b6bffa7", 0); // Overlay Opacity
    }

    void Start()
    {
        string fileId = "SHIBAINUCOIN";
        var cloud = CloudAPI.CloudAPIManager.GetInstance().cloud;
        StartCoroutine(cloud.GetFileDetailed(fileId,
            (CloudAPI.FileInfoDetailed info, long code) =>
            {
                Debug.Log($"Succeeded {info}");
                StartCoroutine(cloud.GetFileParameter(fileId, info.parameters[0],
                    (CloudAPI.FileParameterDataBin data, long code) =>
                    {
                        Debug.Log($"Succeeded {data}");
                        SetMaterialData(data, 0);
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
