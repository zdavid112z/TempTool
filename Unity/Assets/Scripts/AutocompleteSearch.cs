using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutocompleteSearch : MonoBehaviour {
    List<string> files = new List<string>() { "a", "aa", "aaa", "aaron", "ab", "abandoned", "abc", "aberdeen", "abilities", "ability", "able", "about", "above", "abraham", "abroad",
       "a1", "aa2", "aaa3", "aaron1", "ab1", "abandoned1", "abc1", "aberdeen1", "abilities1", "ability1", "able1", "about1", "above1", "abraham1", "abroad1" };
    string text = "";
    public InputField field;
    public Image container;

    public GameObject itemTemplate;
    public GameObject content;

    public Sprite search;
    public Sprite searchResult;

    List<GameObject> items = new List<GameObject>();
    int height = 0;

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
    }

    void Update()//HandleSearchValue()
    {
        string oldString = text;
        text = field.text;

        if (!string.IsNullOrEmpty(text) && text != oldString)
        {
            List<string> found = files.FindAll(w => w.ToLower().StartsWith(text.ToLower()));
            if (found.Count > 0)
            {
                DestroyItems();
                for (int i = 0; i < found.Count; i++)
                {
                    GameObject copy = Instantiate(itemTemplate);
                    copy.transform.SetParent(content.gameObject.transform, false);
                    
                    copy.transform.localPosition = Vector3.zero;
                    copy.GetComponentInChildren<Text>().text = found[i];
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

        if (text == "")
        {
           DestroyItems();
            height = 0;
            field.image.sprite = search;
        }
        container.rectTransform.sizeDelta = new Vector2(256, height);
    }
}