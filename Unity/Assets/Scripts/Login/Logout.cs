using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logout : MonoBehaviour
{
    public GameObject loginButton;
    public GameObject dashboard;
    public void LogoutOnClick()
    {
        loginButton.SetActive(true);
        dashboard.SetActive(false);
    }
}
