using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AdminContainer : MonoBehaviour
{
    List<CloudAPI.AdminData> users = new List<CloudAPI.AdminData>();
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
        for (int i = 0; i < users.Count; i++)
        {
            GameObject copy = Instantiate(itemTemplate);
            copy.transform.SetParent(content.gameObject.transform, false);

            copy.transform.localPosition = Vector3.zero;
            copy.GetComponentInChildren<Text>().text = users[i].name;
            copy.name = users[i].name;
            items.Add(copy);
        }
    }

    void Start()
    {
        cloud = CloudAPI.CloudAPIManager.GetInstance().cloud;
        StartCoroutine(cloud.GetAdmins(
            (CloudAPI.AdminData[] a, long responseCode) =>
            {
                users = a.ToList();
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

    public void DeleteUser(string id)
    {
        var user = users.Find(x => x.name == id);
        users.Remove(user);
        DestroyItems();
        RenderList();

        StartCoroutine(cloud.DeleteAdmin(user,
            (long responseCode) =>
            {
                Debug.Log("Admin deleted!");
            }, (CloudAPI.ErrorDetails error) =>
            {
                Debug.LogError(error);
            }));
    }

    public void AddUser(string id)
    {
        if (users.Find(x => x.name == id) == null)
        {
            var user = new CloudAPI.AdminData()
            {
                name = id
            };
            users.Add(user);
            StartCoroutine(cloud.PostAdmin(user,
                (long responseCode) =>
                {
                    Debug.Log("Admin added!");
                }, (CloudAPI.ErrorDetails error) =>
                {
                    Debug.LogError(error);
                }));
        } else
        {
            Debug.Log("already registred");
        }
        DestroyItems();
        RenderList();
    }
}
