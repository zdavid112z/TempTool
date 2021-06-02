using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class Submiter : MonoBehaviour
{
    public GameObject inputField;
    private InputField emailField;
    private string lastEmail = "";
    public Color okColor = Color.white;
    public Color waitingColor = new Color(0.7f, 0.7f, 0.7f);
    public Color wrongColor = new Color(1.0f, 0.8f, 0.8f);
    public Image image;

    private const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
            + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
              + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
            + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

    private static bool IsEmail(string email)
    {
        if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
        else return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        emailField = GameObject.Find("EmailField").GetComponent<InputField>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (emailField.text != lastEmail)
        {
            image.color = okColor;
        }
        if (Input.GetKeyUp(KeyCode.Return) && emailField.text != lastEmail)
        {
            lastEmail = emailField.text;
            if (IsEmail(emailField.text))
            {
                image.color = waitingColor;
                StartCoroutine(CloudAPI.CloudAPIManager.GetInstance().cloud.PostLogin(
                    new CloudAPI.LoginRequestData()
                    {
                        email = emailField.text
                    },
                    (CloudAPI.LoginResponse respose, long code) =>
                    {
                        inputField.SetActive(true);
                        image.color = okColor;
                    }, (CloudAPI.ErrorDetails error) =>
                    {
                        image.color = wrongColor;
                        Debug.LogError(error);
                    }));
            } else
            {
                image.color = wrongColor;
                Debug.Log("incorect email");
            }
        }
    }
}
