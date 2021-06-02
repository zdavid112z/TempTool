using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureLoader : MonoBehaviour
{
    private CloudAPI.FileParameterDataBin currentData;
    private int currentLayer;
    private Material material;
    public Action<float, float> onRangeChanged = null;

    public void SetDefaultMaterial()
    {
        material.SetFloat("Vector1_51fb8cd46af548f6af8aef823b6bffa7", 1); // Overlay Opacity

        currentData = null;
    }

    public void SetRange(float vmin, float vmax)
    {
        material.SetVector("Vector2_7d6901a117e948c9b13f530cee0ea746", new Vector4(vmin, vmax)); // LerpMinMax
        onRangeChanged?.Invoke(vmin, vmax);
    }

    public Vector2 ResetRange()
    {
        if (currentData != null)
        {
            SetRange(currentData.vmin, currentData.vmax);
            return new Vector2(currentData.vmin, currentData.vmax);
        }
        return new Vector2(0, 100);
    }

    public void SetMaterialData(CloudAPI.FileParameterDataBin data, int layer)
    {
        int num_dates = data.info.num_dates;
        int w = data.info.width;
        int h = data.info.height;
        int l = data.info.num_layers;
        int offset = w * h * layer;
        Texture2DArray array = new Texture2DArray(w, h, num_dates, TextureFormat.RFloat, false, false);
        for (int t = 0; t < num_dates; t++)
            array.SetPixelData<float>(data.data, 0, t, l * w * h * t + offset);
        array.Apply(false);
        material.SetTexture("Texture2DArray_28e7e253cb18486a8c14899af2143136", array); // Textures
        material.SetVector("Vector2_540929bd5a9e4b91803993f7ccde8e73", new Vector4(data.info.lon_min, data.info.lon_max)); // LonMinMax
        material.SetVector("Vector2_a03a182a418a49fd8146e64479d05bb6", new Vector4(data.info.lat_min, data.info.lat_max)); // LatMinMax
        material.SetFloat("Vector1_8211db379e8f4f01b70f108ef594b658", data.info.width); // Width
        material.SetFloat("Vector1_51fb8cd46af548f6af8aef823b6bffa7", 0); // Overlay Opacity
        // material.SetFloat("Vector1_eb79a7293bb64596949928ce17eb143b", 0); // Texture Index

        currentLayer = layer;
        currentData = data;

        ResetRange();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">Between 0 and 100</param>
    public void SetTimeProgress(float value)
    {
        if (currentData == null)
            return;
        float index = value / 100f * (currentData.info.num_dates - 1);
        material.SetFloat("Vector1_eb79a7293bb64596949928ce17eb143b", index); // Texture Index
    }

    void Start()
    {
        material = GetComponentInParent<Renderer>().material;
    }

    void Update()
    {
        
    }
}
