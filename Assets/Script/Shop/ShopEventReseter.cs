using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopEventReseter : MonoBehaviour
{
    public List<Manager> eventList;

    public void ResetEvent()
    {
        foreach (var m in eventList)
        {
            m.GeneralBool = false;
        }
    }
    
    
    
}
