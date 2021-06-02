using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextValueLabel : MonoBehaviour
{
    private Text label;
    private Text value;
    // Start is called before the first frame update
    void Start()
    {
        label = transform.Find("Text").GetComponent<Text>();
        value = transform.Find("Value").GetComponent<Text>();
    }

    public void SetValueText(string val)
    {
        value.text = val;
    }

    public void SetValueSize(long numBytes)
    {
        double mb = numBytes / (1024.0 * 1024.0);
        value.text = string.Format("{0:N2}", mb) + " MiB";
    }

    public void SetValueDate(long date)
    {
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(date).ToLocalTime();
        string t = dtDateTime.ToLocalTime().ToString("dd-MM-yyyy");
        value.text = t;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
