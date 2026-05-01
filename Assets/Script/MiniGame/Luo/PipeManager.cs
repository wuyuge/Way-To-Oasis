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
    public bool canReplace,replaced;
    public Transform itemBox;
    [SerializeField]
    public List<RectTransform> _items = new List<RectTransform>();
    private RectTransform _rectTransform;
    private PipeType _replaceType;
    private bool _isCollision;
    private GameObject _replaceItem;
    public bool isDestination,isStartPoint;
    public bool destinationConnected;
    private Pipe _activePipe;
    public Animator anim;
    private RectTransform _replaceRectTransform;
    public static GameObject Collision;
    private Image _objImage;
    private Sprite _initSprite;
    
    private void Awake()
    {
        _objImage = gameObject.GetComponent<Image>();
        _initSprite = _objImage.sprite;
        itemBox = gameObject.transform.parent.parent.GetChild(transform.parent.parent.childCount - 1);
        
        for (int i = 0; i < itemBox.childCount; i++)
        {
            _items.Add(itemBox.GetChild(i).GetComponent<RectTransform>());
        }
        _rectTransform = GetComponent<RectTransform>();
        _button = GetComponent<Button>();
        SetOff();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!canReplace) return;
        DetectCollision();

        if (_items.Count == 0)
        {
            SearchItem();
        }
        
        
    }

    void SearchItem()
    {
        _items.Clear();
        itemBox = gameObject.transform.parent.parent.GetChild(transform.parent.parent.childCount - 1);
        for (int i = 0; i < itemBox.childCount; i++)
        {
            _items.Add(itemBox.GetChild(i).GetComponent<RectTransform>());
        }
    }

    private void OnEnable()
    {
        replaced = false;
    }

    private void LateUpdate()
    {
        if (!canReplace) return;
        if (Input.GetMouseButtonUp(0) && Collision == gameObject)
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
        }

        if (anglePipe is not null && anglePipe.enabled)
        {
            anglePipe.Click();
        }
        
        if (pipe4Way is not null && pipe4Way.enabled)
        {
            pipe4Way.Click();
        }

        if (tShapePipe is not null && tShapePipe.enabled)
        {
            tShapePipe.Click();
        }

        if (blindPipe is not null && blindPipe.enabled)
        {
            blindPipe.Click();
        }
        
        System.DateTime now = System.DateTime.Now;
        int hourMinute = now.Hour * 100 + now.Minute;
        LuoStaticData.Time = hourMinute;
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
                _objImage.sprite = _initSprite;
                _objImage.color = Color.white;
                GetComponent<Button>().enabled = false;
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
        anim.SetInteger("State",state);
        tempPipe.SetState(state);
    }

    private void DetectCollision()
    {
        if (canReplace)
        {
            if (Collision == gameObject && !UiCollider.IsCollision(_rectTransform,_replaceRectTransform) && _replaceRectTransform.gameObject.activeInHierarchy)
            {
                _isCollision = false;
                _replaceItem.GetComponent<LuoDraggable>().CollisionExit();
                Collision = null;
                _replaceItem = null;
            }
            
            foreach (var rect in _items)
            {
                try
                {
                    if (UiCollider.IsCollision(_rectTransform, rect) && rect.gameObject.activeInHierarchy)
                    {
                        var temp = rect.GetComponent<LuoDraggable>();
                        temp.CollisionEnter();
                        _isCollision = true;
                        _replaceType = temp.itemType;
                        _replaceItem = rect.gameObject;
                        _replaceRectTransform = rect;
                        Collision = gameObject;
                        return;
                    }
                }
                catch (MissingReferenceException e)
                {
                    UpdateItemList();
                    Console.WriteLine(e);
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
            replaced = true;
            Collision = null;
            GetComponent<Button>().enabled = true;
            SetOpen(_replaceType);
        }
    }

    public void CheckConnectivity()
    {
        if(_activePipe is null) return;
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
        anim.SetInteger("State",0);
    }

    public void SetDestinationConnect(bool value)
    {
        destinationConnected = value;
    }

    public void UpdateItemList()
    {
        _items.Clear();
        for (int i = 0; i < itemBox.childCount; i++)
        {
            _items.Add(itemBox.GetChild(i).GetComponent<RectTransform>());
        }
    }

    public void ResetConnection()
    {
        if (_activePipe == null)
        {
            return;
        }
        _activePipe.RestConnection();
    }

}
