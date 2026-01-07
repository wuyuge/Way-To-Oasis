using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGeneralFunction : MonoBehaviour
{
    public GameObject orderObject;

    public void SetState()
    {
        orderObject.SetActive(!orderObject.activeSelf);
    }
    
}
