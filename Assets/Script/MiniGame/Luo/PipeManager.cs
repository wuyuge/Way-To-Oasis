using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum PipeType
{
    StraightPipe,AnglePipe,Pipe4Way,BlindPipe,TShapePipe,ReplacePipe
}

public class PipeManager : MonoBehaviour
{
    public StraightPipe straightPipe;
    public AnglePipe anglePipe;
    public Pipe4Way pipe4Way;
    public BlindPipe blindPipe;
    public TShapePipe tShapePipe;
    private Button _button;
    public bool canReplace;
    public Transform itemBox;
    [SerializeField]
    private List<RectTransform> _items = new List<RectTransform>();
    private RectTransform _rectTransform;
    private PipeType _replaceType;
    private bool _isCollision;
    private GameObject _replaceItem;
    public bool isDestination,isStartPoint;
    private Pipe _activePipe;
    private Animator _anim;
    //TODO:拖拽物品逻辑优化
    private void Awake()
    {
        itemBox = gameObject.transform.parent.parent.GetChild(transform.parent.parent.childCount - 1);
        for (int i = 0; i < itemBox.childCount; i++)
        {
            _items.Add(itemBox.GetChild(i).GetComponent<RectTransform>());
        }
        _rectTransform = GetComponent<RectTransform>();
        _button = GetComponent<Button>();
        SetOff();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        DetectCollision();
        if (Input.GetMouseButtonUp(0))
        {
            OnPointerUp();
        }
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            OnPointerUp();
        }
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

    public void SetOpen(PipeType type,int state = 0,bool isStartPoint = false,bool startIsVertical = false,
        bool isDestination = false,bool destinationIsVertical = false)
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
            case PipeType.ReplacePipe:
                canReplace = true;
                return;
            default:
                return;
        }

        _activePipe = tempPipe;
        this.isDestination = isDestination;
        this.isStartPoint = isStartPoint;
        tempPipe.isStartPoint = isStartPoint;
        tempPipe.startIsVertical = startIsVertical;
        tempPipe.isDestination = isDestination;
        tempPipe.destinationIsVertical = destinationIsVertical;
        _anim.SetInteger("State",state);
        tempPipe.SetState(state);
    }

    protected void DetectCollision()
    {
        if (canReplace)
        {
            foreach (var rect in _items)
            {
                if (UiCollider.IsCollision(_rectTransform, rect))
                {
                    var temp = rect.GetComponent<LuoDraggable>();
                    temp.CollisionEnter();
                    _isCollision = true;
                    _replaceType = temp.itemType;
                    _replaceItem = rect.gameObject;
                    
                    return;
                }
            }
        }
    }

    public void OnPointerUp()
    {
        if (_isCollision)
        {
            _replaceItem.SetActive(false);
            _isCollision = false;
            canReplace = false;
            SetOpen(_replaceType);
        }
    }

    public void CheckConnectivity()
    {
        _activePipe.CheckConnectivity();
    }

    public bool HaveInterface(Pipe.PipeTowards towards)
    {
        if (_activePipe is null)
        {
            return false;
            
        }
        return _activePipe.HaveInterface(towards);
    }

    public void SetConnect(bool value)
    {
        if (_activePipe is null)
        {
            return;
        }
        _activePipe.isConnected = value;
    }

    public void StateSetOver()
    {
        _anim.SetInteger("State",0);
    }
    
    
}
