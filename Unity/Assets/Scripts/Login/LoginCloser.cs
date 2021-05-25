using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginCloser : MonoBehaviour
{
    public GameObject Panel;
    public void ClosePanel()
    {
        if (Panel != null)
        {
            Panel.SetActive(false);
        }
    }
}
