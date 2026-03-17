using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteChild : MonoBehaviour
{
    public void DeleteFile(GameObject o)
    {
        o.GetComponent<DeleteSave>().Delete();
    }

    public void CancelDelete(GameObject o)
    {
        o.GetComponent<DeleteSave>().Cancel();
    }
}
