using TMPro;
using UnityEngine;

public class DevBodyManager : MonoBehaviour
{
    private TMP_Dropdown _dropdown;

    public void AddBody()
    {
        if (_dropdown == null) _dropdown = gameObject.transform.parent.GetComponent<TMP_Dropdown>();
        gameObject.transform.parent.parent.Find("BodyList").GetComponent<DevBodyList>().OpenBody(_dropdown.value);
        
        _dropdown.value = 0;
    }

    
    
}
