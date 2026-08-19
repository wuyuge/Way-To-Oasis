using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaiwenAnimator : MonoBehaviour
{
    public List<Animator> itemAnims;
    public GameObject itemContainer;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        foreach (Transform value in itemContainer.transform)
        {
            itemAnims.Add(value.gameObject.GetComponent<Animator>());
        }
    }

    public void SetOff()
    {
        foreach (var value in itemAnims)
        {
            value.SetTrigger("end");
        }
        _animator.SetTrigger("end");
    }
    
}
