using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetFields : MonoBehaviour
{
    public InputField minField;
    public InputField maxField;
    public TextureLoader textureLoader;

    public void Start()
    {
        textureLoader.onRangeChanged = (float vmin, float vmax) =>
        {
            minField.text = vmin.ToString();
            maxField.text = vmax.ToString();
        };
    }

    public void ResetOnClick()
    {
        Vector2 values = textureLoader.ResetRange();
        minField.text = values.x.ToString();
        maxField.text = values.y.ToString();
    }
}
