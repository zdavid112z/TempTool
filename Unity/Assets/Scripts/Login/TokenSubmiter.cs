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

    private string lastToken= "";
    public Color okColor = Color.white;
    public Color waitingColor = new Color(0.7f, 0.7f, 0.7f);
    public Color wrongColor = new Color(1.0f, 0.8f, 0.8f);
    private Image image;
    private Submiter submiter;

    private bool IsValid(string token)
    {
        if (token.Length != 6)
            return false;
        foreach (char ch in token)
        {
            if (ch < '0' || ch > '9')
                return false;
        }
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
        image = GetComponent<Image>();
        submiter = emailField.GetComponent<Submiter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tokenField.text != lastToken)
        {
            image.color = okColor;
        }
        if (Input.GetKeyUp(KeyCode.Return) && tokenField.text != lastToken)
        {
            lastToken = tokenField.text;
            if (IsValid(tokenField.text))
            {
                image.color = waitingColor;
                submiter.image.color = waitingColor;
                StartCoroutine(CloudAPI.CloudAPIManager.GetInstance().cloud.PostLogin(
                    new CloudAPI.LoginRequestData()
                    {
                        email = emailField.text,
                        login_code = tokenField.text
                    },
                    (CloudAPI.LoginResponse respose, long code) =>
                    {
                        panel.SetActive(true);
                        loginPanel.SetActive(false);
                        loginButton.SetActive(false);
                        token.SetActive(false);
                        tokenField.text = null;
                        emailField.text = null;
                        image.color = okColor;
                        submiter.image.color = okColor;
                    }, (CloudAPI.ErrorDetails error) =>
                    {
                        image.color = wrongColor;
                        submiter.image.color = okColor;
                        Debug.LogError(error);
                    }));
            }
            else
            {
                image.color = wrongColor;
                Debug.Log("invalid token");
            }
        }

    }
}
