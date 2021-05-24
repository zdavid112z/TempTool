using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteUser : MonoBehaviour
{
    public AdminContainer users;
    // Start is called before the first frame update
    public void DeleteUserOnClick()
    {
        users.DeleteUser(this.name);
    }
}
