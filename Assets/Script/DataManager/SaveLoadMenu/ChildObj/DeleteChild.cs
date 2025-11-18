using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteChild : MonoBehaviour
{
    public void DeleteFile()
    {
        gameObject.transform.parent.parent.gameObject.GetComponent<DeleteSave>().Delete();
    }

    public void CancelDelete()
    {
        gameObject.transform.parent.parent.gameObject.GetComponent<DeleteSave>().Cancel();
    }
}
