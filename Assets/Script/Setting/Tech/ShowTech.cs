using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowTech : MonoBehaviour
{
    public Manager Tech;
    private void Awake()
    {
        GetComponent<Toggle>().isOn = Tech.GeneralBool;
    }


    public void Change(bool value)
    {
        Tech.GeneralBool = value;
        GetComponent<Toggle>().isOn = value;
    }




}
