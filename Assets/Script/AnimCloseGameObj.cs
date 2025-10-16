using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCloseGameObj : MonoBehaviour
{
    public void CloseGameObj()
    {
        gameObject.SetActive(false);
    }
}
