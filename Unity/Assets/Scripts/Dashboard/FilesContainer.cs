using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FilesContainer : MonoBehaviour
{
    List<CloudAPI.FileInfo> files = new List<CloudAPI.FileInfo>();
    public Image container;

    public GameObject itemTemplate;
    public GameObject content;


    List<GameObject> items = new List<GameObject>();

    private CloudAPI.ICloudAPI cloud;

    private void DestroyItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i]);
        }
        items.Clear();
    }

    private void RenderList()
    {
        for (int i = 0; i < files.Count; i++)
        {
            GameObject copy = Instantiate(itemTemplate);
            copy.transform.SetParent(content.gameObject.transform, false);

            copy.transform.localPosition = Vector3.zero;
            copy.GetComponentInChildren<Text>().text = files[i].name + " " + files[i].id;
            copy.name = files[i].id;
            items.Add(copy);
        }
    }

    void Start()
    {
        cloud = CloudAPI.CloudAPIManager.GetInstance().cloud;
        StartCoroutine(cloud.GetFiles(
            (CloudAPI.FileInfo[] f, long responseCode) =>
            {
                files = f.ToList();
                DestroyItems();
                RenderList();
            }, (CloudAPI.ErrorDetails error) =>
            {
                Debug.LogError(error);
            }));
    }

    void Update()//HandleSearchValue()
    {

    }

    public void DeleteFile(string id)
    {
        CloudAPI.FileInfo file = files.Find(x => x.id == id);
        files.Remove(file);
        DestroyItems();
        RenderList();
    }
}
