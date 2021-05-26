using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxField : MonoBehaviour
{
    private InputField minField;
    private InputField maxField;

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
                int maxNumber, minNumber;

                if (int.TryParse(maxField.text, out maxNumber)) {
                    if (int.TryParse(minField.text, out minNumber))
                    {
                        if(minNumber <= maxNumber)
                        {
                            
                        } else
                        {
                            maxField.text = null;
                        }
                    }
                }
                else
                {
                    maxField.text = null;
                }
                    
            }

            
        }

    }

}