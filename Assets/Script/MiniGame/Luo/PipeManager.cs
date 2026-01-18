using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PipeType
{
    StraightPipe,AnglePipe,Pipe4Way,BlindPipe,TShapePipe
}

public class PipeManager : MonoBehaviour
{
    public StraightPipe straightPipe;
    public AnglePipe anglePipe;
    public Pipe4Way pipe4Way;
    public BlindPipe blindPipe;
    public TShapePipe tShapePipe;
    private Button _button;
    public bool isStartPoint,startIsVertical,isDestination;
    
    

    private void Awake()
    {
        _button = GetComponent<Button>();
        SetOff();
        
    }


    private void SetOff()
    {
        straightPipe.enabled = false;
        anglePipe.enabled = false;
        pipe4Way.enabled = false;
        blindPipe.enabled = false;
        tShapePipe.enabled = false;
    }

    public void Click()
    {
        if (straightPipe is not null && straightPipe.enabled)
        {
            straightPipe.Click();
            return;
        }

        if (anglePipe is not null && anglePipe.enabled)
        {
            anglePipe.Click();
            return;
        }
        
        if (pipe4Way is not null && pipe4Way.enabled)
        {
            pipe4Way.Click();
            return;
        }

        if (tShapePipe is not null && tShapePipe.enabled)
        {
            tShapePipe.Click();
        }
    }

    public void SetOpen(PipeType type)
    {
        SetOff();
        _button.enabled = true;
        Pipe tempPipe;
        switch (type)
        {
            case PipeType.StraightPipe:
                straightPipe.enabled = true;
                tempPipe = straightPipe;
                break;
            case PipeType.AnglePipe:
                anglePipe.enabled = true;
                tempPipe = anglePipe;
                break;
            case PipeType.Pipe4Way:
                pipe4Way.enabled = true;
                tempPipe = pipe4Way;
               break;
            case PipeType.TShapePipe:
                tShapePipe.enabled = true;
                tempPipe = tShapePipe;
                break;
            case PipeType.BlindPipe:
                blindPipe.enabled = true;
                tempPipe = blindPipe;
                break;
            default:
                return;
        }
        
        tempPipe.isStartPoint = isStartPoint;
        tempPipe.startIsVertical = startIsVertical;
        tempPipe.isDestination = isDestination;
        
        
    }
}
