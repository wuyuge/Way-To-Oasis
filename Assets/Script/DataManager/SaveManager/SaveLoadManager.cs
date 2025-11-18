using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SaveLoadManager : MonoBehaviour
{

    public GameObject linkMenu;

    public void Open()
    {
        linkMenu.SetActive(true);
    }
    

}
