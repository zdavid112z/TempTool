using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdminContainer : MonoBehaviour
{
    List<string> users = new List<string>(new string [] {"user1@yahoo.com", "strawberry_love123@yahoo.com", "cat_lover65@gmail.com", "weatherman@magicschoolbus.org"});
    public Image container;

    public GameObject itemTemplate;
    public GameObject content;


    List<GameObject> items = new List<GameObject>();
    int count = 0;

    private CloudAPI.ICloudAPI cloud;

    private void DestroyItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i]);
        }
    }

    private void RenderList()
    {
        for (int i = 0; i < users.Count; i++)
        {
            GameObject copy = Instantiate(itemTemplate);
            copy.transform.SetParent(content.gameObject.transform, false);

            copy.transform.localPosition = Vector3.zero;
            copy.GetComponentInChildren<Text>().text = users[i];
            copy.name = users[i];
            items.Add(copy);
        }
    }

    void Start()
    {
        
    }

    void Update()//HandleSearchValue()
    {
        if (count != users.Count)
        {
            DestroyItems();
            RenderList();
            count = users.Count;
        }
    }

    public void DeleteUser(string id)
    {
        string user = users.Find(x => x == id);
        users.Remove(user);
        DestroyItems();
        RenderList();
    }

    public void AddUser(string id)
    {
        if (users.Find(x => x == id) == null)
        {
            users.Add(id);
        } else
        {
            Debug.Log("already registred");
        }
        DestroyItems();
        RenderList();
    }
}
