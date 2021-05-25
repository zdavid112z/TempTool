using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetFields : MonoBehaviour
{
    public InputField minField;
    public InputField maxField;

    public void ResetOnClick()
    {
        maxField.text = null;
        minField.text = null;
    }
}
