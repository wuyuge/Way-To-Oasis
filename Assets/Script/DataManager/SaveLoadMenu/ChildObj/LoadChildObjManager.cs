using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadChildObjManager : MonoBehaviour
{

    public void LoadFile(GameObject o)
    {
        o.GetComponent<LoadFileButton>().Load();
    }

    public void Cancel(GameObject o)
    {
        o.GetComponent<LoadFileButton>().Cancel();
    }
    
}
