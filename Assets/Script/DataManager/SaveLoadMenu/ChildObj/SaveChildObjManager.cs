using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveChildObjManager : MonoBehaviour
{

    public List<GameObject> excludeList;

    private void OnEnable()
    {
        foreach (var value in excludeList)
        {
            value.SetActive(false);
        }
    }

    public void CoverFile()
    {
        GlobalData.CurrentSaveFileButton.GetComponent<SaveFileButton>().CoverFile();
    }

    public void Cancel()
    {
        GlobalData.CurrentSaveFileButton.GetComponent<SaveFileButton>().Cancel();
    }
    
    public void SaveFile()
    {
        GlobalData.CurrentSaveFileButton.GetComponent<SaveFileButton>().SaveFile();
    }

}
