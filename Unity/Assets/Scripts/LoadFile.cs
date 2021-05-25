using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LoadFile : MonoBehaviour
{
    string path;
    public void OpenExplorer()
    {
       // path = EditorUtility.OpenFilePanel("Overwrite with nc", "", "nc");
        //GetFile();
    }

    void GetFile()
    {
        if (path != null)
        {
            UploadFile();
        }
    }

    void UploadFile()
    {

    }
}
