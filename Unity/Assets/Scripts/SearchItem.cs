using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchItem : MonoBehaviour
{
    public GameObject paramDropdown;
    public AutocompleteSearch search;
    public CloudAPI.FileInfo file;

    public void OnClick()
    {
        search.KeepExtended();
        StartCoroutine(CloudAPI.CloudAPIManager.GetInstance().cloud.GetFileDetailed(file.id,
            (CloudAPI.FileInfoDetailed details, long response) =>
            {
                paramDropdown.GetComponent<ParamDropdown>().File = details;
                search.field.text = file.name;
                search.Collapse();
            }, (CloudAPI.ErrorDetails error) =>
            {
                Debug.LogError(error);
                search.Collapse();
            }));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
