using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokenSubmiter : MonoBehaviour
{
    private GameObject loginPanel;
    private GameObject loginButton;
    private InputField tokenField;
    private InputField emailField;
    private GameObject token;
    public GameObject panel;

    private bool IsValid(string token)
    {
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        tokenField = GameObject.Find("TokenField").GetComponent<InputField>();
        token = GameObject.Find("TokenField");
        emailField = GameObject.Find("EmailField").GetComponent<InputField>();
        loginPanel = GameObject.Find("Login");
        loginButton = GameObject.Find("Login Button");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (IsValid(tokenField.text))
            {
                panel.SetActive(true);
                loginPanel.SetActive(false);
                loginButton.SetActive(false);
                token.SetActive(false);
                tokenField.text = null;
                emailField.text = null;
            }
            else
            {
                Debug.Log("invalid token");
            }
        }

    }
}
