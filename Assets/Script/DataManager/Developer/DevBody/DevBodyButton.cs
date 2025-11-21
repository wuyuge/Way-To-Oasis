using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevBodyButton : MonoBehaviour
{
    public int index;
    public string charaName;
    private Image _image;
    private DevBodyList _devBodyList;
    private bool _added;
    private void Awake()
    {
        _image = GetComponent<Image>();
        _devBodyList = gameObject.transform.parent.parent.parent.GetComponent<DevBodyList>();  
        
        
    }

    private void OnEnable()
    {
        _image.color = Color.white;
        _added = false;
    }


    public void Click()
    {
        if (!_added)
        {
            _image.color = Color.red;
            _devBodyList.AddUsedBody(index);
            _added = true;
        }
        else
        {
            _devBodyList.DeleteBodyFinally(charaName,index);
        }
        
    }
    
    
    
    
}
