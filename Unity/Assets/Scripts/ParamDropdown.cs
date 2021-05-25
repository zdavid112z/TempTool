using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ParamDropdown : MonoBehaviour
{
    private Dropdown dropdown;
    public TextureLoader textureLoader;

    private CloudAPI.FileInfoDetailed file;

    public CloudAPI.FileInfoDetailed File
    {
        get
        {
            return file;
        }
        set
        {
            file = value;
            var paramsList = file.parameters.Select(
                f => new Dropdown.OptionData()
                {
                    text = f.name
                }).ToList();
            dropdown.options = new List<Dropdown.OptionData>() {
                new Dropdown.OptionData() { text = "<Unselected>" }
            };
            dropdown.options.AddRange(paramsList);
            dropdown.value = 0;
        }
    }

    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(delegate { OnParamChanged(); });
        dropdown.value = 0;
        dropdown.options = new List<Dropdown.OptionData>() {
            new Dropdown.OptionData() { text = "<Unselected>" }
        };
    }
    private void OnParamChanged()
    {
        int index = dropdown.value;
        if (index == 0)
        {
            textureLoader.SetDefaultMaterial();
            return;
        }
        int paramIndex = index - 1;
        StartCoroutine(CloudAPI.CloudAPIManager.GetInstance().cloud.GetFileParameter(
            file.id, file.parameters[paramIndex],
            (CloudAPI.FileParameterDataBin data, long response) =>
            {
                textureLoader.SetMaterialData(data, 0);
            },
            (CloudAPI.ErrorDetails error) =>
            {
                Debug.LogError(error);
            }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
