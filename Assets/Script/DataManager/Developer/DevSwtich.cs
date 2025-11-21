using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevSwitch : MonoBehaviour
{
    public GameObject linkObj1, linkObj2;


    public void Switch()
    {
        bool open = linkObj1.activeSelf;
        
        linkObj2.SetActive(open);
        linkObj1.SetActive(!open);
        
        
    }
    



}
