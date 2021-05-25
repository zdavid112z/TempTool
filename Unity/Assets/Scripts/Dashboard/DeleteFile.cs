using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteFile : MonoBehaviour
{
    public FilesContainer files;
    // Start is called before the first frame update
    public void DeleteFileOnClick()
    {
        files.DeleteFile(this.name);
    }
}
