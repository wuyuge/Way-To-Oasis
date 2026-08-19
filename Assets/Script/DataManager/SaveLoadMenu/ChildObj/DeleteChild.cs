using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteChild : MonoBehaviour
{
    
    public List<GameObject> excludeList;
    private void OnEnable()
    {
        foreach (var value in excludeList)
        {
            value.SetActive(false);
        }
    }
    public void DeleteFile()
    {
        GlobalData.CurrentSaveFileButton.GetComponent<DeleteSave>().Delete();
    }

    public void CancelDelete()
    {
        GlobalData.CurrentSaveFileButton.GetComponent<DeleteSave>().Cancel();
    }
}
