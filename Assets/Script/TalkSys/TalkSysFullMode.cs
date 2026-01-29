using UnityEngine;
using UnityEngine.UI;

public class TalkSysFullMode : MonoBehaviour
{
    private Button _btn;
    public CgManager cgManager;
    private bool _fullModeOn = false;
    private void Awake()
    {
        _btn = GetComponent<Button>();
    }

    private void Update()
    {
        _btn.enabled = FullModeState.GetValue();
        if (_fullModeOn && Input.GetMouseButtonDown(0))
        {
            Click();
        }
    }
    
    public void Click()
    {
        if (cgManager is null)
        {
            Debug.LogError("cg管理器为空",this);
            return;
        }
        _fullModeOn = !_fullModeOn;
        cgManager.SetFullMode(_fullModeOn);
        FullModeState.SetValue(_fullModeOn,true);
        
    }
}


public static class FullModeState
{
    private static bool _cgIsShowing;
    private static bool _fullModeOn;
    
    public static bool GetValue(bool isFullMode = false)
    {
        if (isFullMode)
        {
            return _fullModeOn;
        }
        return _cgIsShowing;
    }
    
    public static void SetValue(bool value,bool isFullMode = false)
    {
        if (isFullMode)
        {
            _fullModeOn = value;
            return;
        }
        _cgIsShowing = value;
    }
}

