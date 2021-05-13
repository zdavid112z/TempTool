using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AutocompleteSearch : MonoBehaviour {
    List<CloudAPI.FileInfo> files = new List<CloudAPI.FileInfo>();
    string text = null;
    bool keepExtended = false;
    int framesUnfocused;
    public const int maxFramesUnfocused = 40;
    public InputField field;
    public Image container;

    public GameObject itemTemplate;
    public GameObject content;

    public Sprite search;
    public Sprite searchResult;

    List<GameObject> items = new List<GameObject>();
    int height = 0;

    private CloudAPI.ICloudAPI cloud;

    private void DestroyItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i]);
        }
    }

    void Start()
    {
        // field.OnSubmit.AddListener(HandleSearchValue);
        framesUnfocused = maxFramesUnfocused;
        cloud = CloudAPI.CloudAPIManager.GetInstance().cloud;
        StartCoroutine(cloud.GetFiles(
            (CloudAPI.FileInfo[] f, long responseCode) =>
            {
                files = f.ToList();
            }, (CloudAPI.ErrorDetails error) =>
            {
                Debug.LogError(error);
            }));
    }

    void Update()//HandleSearchValue()
    {
        string oldString = text;
        string newString = field.text;

        if (!field.isFocused)
        {
            framesUnfocused = Mathf.Min(framesUnfocused + 1, maxFramesUnfocused);
        }
        else
        {
            framesUnfocused = 0;
        }
        if (keepExtended)
        {
            framesUnfocused = 0;
        }

        if (framesUnfocused < maxFramesUnfocused &&
            (newString != oldString ||
                (string.IsNullOrEmpty(newString) && items.Count == 0)))
        {
            text = newString;
            List<CloudAPI.FileInfo> found = files.FindAll(w => w.name.ToLower().Contains(text.ToLower()));
            if (found.Count > 0)
            {
                DestroyItems();
                for (int i = 0; i < found.Count; i++)
                {
                    GameObject copy = Instantiate(itemTemplate);
                    copy.transform.SetParent(content.gameObject.transform, false);
                    
                    copy.transform.localPosition = Vector3.zero;
                    copy.GetComponent<SearchItem>().file = found[i];
                    copy.GetComponent<SearchItem>().search = this;
                    copy.GetComponentInChildren<Text>().text = found[i].name;
                    items.Add(copy);
                }
                height = Mathf.Min(40 * found.Count, 150);
                field.image.sprite = searchResult;
            }
            else
            {
                DestroyItems();
                height = 0;
                field.image.sprite = search;
            }
        }

        if (framesUnfocused >= maxFramesUnfocused)
        {
            text = null;
            DestroyItems();
            height = 0;
            field.image.sprite = search;
        }
        container.rectTransform.sizeDelta = new Vector2(256, height);
    }

    public void KeepExtended()
    {
        keepExtended = true;
    }

    public void Collapse()
    {
        keepExtended = false;
        framesUnfocused = maxFramesUnfocused;
    }
}