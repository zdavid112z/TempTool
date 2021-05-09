using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Text valueText;
    public Slider slider;
    public void OnSliderChanged(float value)
    {
        valueText.text = value.ToString("F2");
        valueText.transform.position += new Vector3(slider.value, 0.0f, 0.0f);
        Debug.Log(slider.value);
    }
}
