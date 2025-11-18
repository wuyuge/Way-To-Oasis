using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadChildObjManager : MonoBehaviour
{

    public void LoadFile()
    {
        gameObject.transform.parent.parent.gameObject.GetComponent<LoadFileButton>().Load();
    }

    public void Cancel()
    {
        gameObject.transform.parent.parent.gameObject.GetComponent<LoadFileButton>().Cancel();
    }
    
}
