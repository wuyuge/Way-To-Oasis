using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadChildObjManager : MonoBehaviour
{

    public List<GameObject> excludeList;
    private void OnEnable()
    {
        foreach (var value in excludeList)
        {
            value.SetActive(false);
        }
    }
    
    public void LoadFile()
    {
        GlobalData.CurrentLoadFileButton.GetComponent<LoadFileButton>().Load();
    }

    public void Cancel()
    {
        GlobalData.CurrentLoadFileButton.GetComponent<LoadFileButton>().Cancel();
    }
    
}
