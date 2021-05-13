using UnityEngine;
using UnityEngine.UI;

public class SliderTextValue : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The text shown will be formatted using this string.  {0} is replaced with the actual value")]
    private string formatText = "{0}°";

    private Text tmproText;

    private void Start()
    {
        tmproText = GetComponent<Text>();

        GetComponentInParent<Slider>().onValueChanged.AddListener(HandleValueChanged);
    }

    private void HandleValueChanged(float value)
    {
        //tmproText.text = string.Format(formatText, value);
        tmproText.text = value.ToString("F2");
    }
}