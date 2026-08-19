using UnityEngine;

public class DevApplyBody : MonoBehaviour
{


    public void Apply()
    {
        
        gameObject.transform.parent.GetComponent<DevBodyList>().SendData();
        
    }
    
    
}
