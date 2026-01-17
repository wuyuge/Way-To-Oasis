using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeManager : MonoBehaviour
{
    public StraightPipe straightPipe;
    public AnglePipe anglePipe;

    private void Awake()
    {
        straightPipe = GetComponent<StraightPipe>();
        anglePipe = GetComponent<AnglePipe>();
    }

    public void Click()
    {
        if (straightPipe is not null)
        {
            straightPipe.Click();
            return;
        }

        if (anglePipe is not null)
        {
            anglePipe.Click();
        }
    }
    
}
