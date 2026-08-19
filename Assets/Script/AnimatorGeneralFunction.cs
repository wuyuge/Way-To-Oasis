using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorGeneralFunction : MonoBehaviour
{
    public void CloseGameObject()
    {
        gameObject.SetActive(false);
    }
}
