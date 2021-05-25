using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class AddUser : MonoBehaviour
{
    public AdminContainer users;
    public InputField emailField;
    // Start is called before the first frame update
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
    
    public void AddUserOnClick()
    {
        if(IsEmail(emailField.text))
        {
            users.AddUser(emailField.text);
            emailField.text = null;
        } else
        {
            Debug.Log("invalid email adr");
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return) && IsEmail(emailField.text))
        {
            users.AddUser(emailField.text);
            emailField.text = null;
        }
    }
}
