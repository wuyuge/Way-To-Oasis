using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookButton : MonoBehaviour
{
    public Book book;
    public bool isPre;
    public Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        anim.SetBool("Close", false);
        anim.SetBool("Open", false);
    }

    private void Update()
    {
        if (isPre)
        {
            if (book.currentPage <= 1)
            {
                anim.SetBool("Close", true);
                anim.SetBool("Open", false);
            }
            else
            {
                anim.SetBool("Close", false);
                anim.SetBool("Open", true);
            }
        }
        else
        {
            if (book.currentPage > book.bookPages.Count - 3)
            {
                anim.SetBool("Close", true);
                anim.SetBool("Open", false);
            }
            else
            {
                anim.SetBool("Close", false);
                anim.SetBool("Open", true);
            }
        }
    }
}
