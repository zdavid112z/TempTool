using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

public class FileDownloader : MonoBehaviour
{
    [DllImport("__Internal")] private static extern void AddClickListenerForFileDialog();
    [DllImport("__Internal")] private static extern void FocusFileUploader();

    void Start()
    {
        AddClickListenerForFileDialog();
    }

    public void FileDialogResult(string fileUrl)
    {
        Debug.Log(fileUrl);
        StartCoroutine(LoadBlob(fileUrl));
    }

    IEnumerator LoadBlob(string url)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            // Get text content like this:
            Debug.Log(webRequest.downloadHandler.text);
        } else
        {
            Debug.Log($"Error: {webRequest.error}");
        }
    }
}
