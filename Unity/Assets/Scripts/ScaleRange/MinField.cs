using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinField : MonoBehaviour
{
    private InputField minField;
    private InputField maxField;
    public TextureLoader textureLoader;

    private void Start()
    {
        minField = GameObject.Find("InputField_min").GetComponent<InputField>();
        maxField = GameObject.Find("InputField_max").GetComponent<InputField>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (maxField.text != null)
            {
                float maxNumber, minNumber;

                if (float.TryParse(minField.text, out minNumber))
                {
                    if (float.TryParse(maxField.text, out maxNumber))
                    {
                        if (minNumber <= maxNumber)
                        {
                            textureLoader.SetRange(minNumber, maxNumber);
                        }
                        else
                        {
                            minField.text = null;
                        }
                    }
                }
                else
                {
                    minField.text = null;
                }
            }
        }

    }
}
