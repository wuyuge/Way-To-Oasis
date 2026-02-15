using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OppositeActive : MonoBehaviour
{
    public GameObject linkObject;
    public void OnClick()
    {
        linkObject.SetActive(!linkObject.activeSelf);
    }
}
