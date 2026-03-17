using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OppositeActive : MonoBehaviour
{
    public GameObject linkObject;
    private Animator _anim;
    public void OnClick()
    {
        if (_anim is null)
        {
            _anim = linkObject.GetComponent<Animator>();
        }

        if (!linkObject.activeSelf)
        {
            linkObject.SetActive(true);
        }
        else
        {
            _anim.SetTrigger("Close");
        }
        
    }
}
