using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonName
{
    Left,
    Right,
    ShopLeft,
    ShopRight,
    ShopMiddle
}

public enum ButtonAction
{
    Enable,
    Disable,
    Hide
}

public class TalkSysUIButtonFunc : MonoBehaviour, ITalkSysCore
{
    #region 按钮缓存字段（替代懒加载属性，避免重复 GetComponent）
    // 普通场景
    private Button _leftButton;
    private Image _leftButtonImage;
    private TextMeshProUGUI _leftButtonText;
    private ButtonSelect _leftButtonTextBox;

    private Button _rightButton;
    private Image _rightButtonImage;
    private TextMeshProUGUI _rightButtonText;
    private ButtonSelect _rightButtonTextBox;

    // 商店场景
    private Button _shopLeftButton;
    private Image _shopLeftButtonImage;
    private TextMeshProUGUI _shopLeftButtonText;
    private ButtonSelect _shopLeftButtonTextBox;

    private Button _shopRightButton;
    private Image _shopRightButtonImage;
    private TextMeshProUGUI _shopRightButtonText;
    private ButtonSelect _shopRightButtonTextBox;

    private Button _shopMiddleButton;
    private Image _shopMiddleButtonImage;
    private TextMeshProUGUI _shopMiddleButtonText;
    private ButtonSelect _shopMiddleButtonTextBox;
    #endregion

    #region 便捷访问属性（对外暴露，内部使用缓存字段）
    // 普通场景
    private Button LeftButton => _leftButton;
    private Image LeftButtonImage => _leftButtonImage;
    private TextMeshProUGUI LeftButtonText => _leftButtonText;
    private ButtonSelect LeftButtonTextBox => _leftButtonTextBox;

    private Button RightButton => _rightButton;
    private Image RightButtonImage => _rightButtonImage;
    private TextMeshProUGUI RightButtonText => _rightButtonText;
    private ButtonSelect RightButtonTextBox => _rightButtonTextBox;

    // 商店场景
    private Button ShopLeftButton => _shopLeftButton;
    private Image ShopLeftButtonImage => _shopLeftButtonImage;
    private TextMeshProUGUI ShopLeftButtonText => _shopLeftButtonText;
    private ButtonSelect ShopLeftButtonTextBox => _shopLeftButtonTextBox;

    private Button ShopRightButton => _shopRightButton;
    private Image ShopRightButtonImage => _shopRightButtonImage;
    private TextMeshProUGUI ShopRightButtonText => _shopRightButtonText;
    private ButtonSelect ShopRightButtonTextBox => _shopRightButtonTextBox;

    private Button ShopMiddleButton => _shopMiddleButton;
    private Image ShopMiddleButtonImage => _shopMiddleButtonImage;
    private TextMeshProUGUI ShopMiddleButtonText => _shopMiddleButtonText;
    private ButtonSelect ShopMiddleButtonTextBox => _shopMiddleButtonTextBox;
    #endregion

    private TalkSystem _talkSys;

    public void Init(TalkSystem talkSys)
    {
        
        _talkSys = talkSys;
        // 初始化组件缓存（一次性获取，后续直接使用）
        CacheAllButtons();
    }

    /// <summary>
    /// 执行与指定按钮和动作相关的操作。
    /// </summary>
    /// <param name="buttonName">要执行操作的按钮名称。</param>
    /// <param name="action">在指定按钮上执行的动作。</param>
    /// <param name="textBox">传入已经转换好选项分支的manager</param>
    public void SwitchButtonState(ButtonName buttonName, ButtonAction action,Manager textBox = null)
    {
        if (textBox == null)
        {
            Debug.LogError("传入文本为空");
            return;
        }
        Button tempBotton;
        Image tempImage;
        TextMeshProUGUI tempText;
        ButtonSelect tempTextBox;
        
        switch (buttonName)
        {
            case ButtonName.Left:
                tempBotton = LeftButton;
                tempImage = LeftButtonImage;
                tempText = LeftButtonText;
                tempTextBox = LeftButtonTextBox;
                break;
            case ButtonName.Right:
                tempBotton = RightButton;
                tempImage = RightButtonImage;
                tempText = RightButtonText;
                tempTextBox = RightButtonTextBox;
                break;
            case ButtonName.ShopLeft:
                tempBotton = ShopLeftButton;
                tempImage = ShopLeftButtonImage;
                tempText = ShopLeftButtonText;
                tempTextBox = ShopLeftButtonTextBox;
                break;
            case ButtonName.ShopRight:
                tempBotton = ShopRightButton;
                tempImage = ShopRightButtonImage;
                tempText = ShopRightButtonText;
                tempTextBox = ShopRightButtonTextBox;
                break;
            case ButtonName.ShopMiddle:
                tempBotton = ShopMiddleButton;
                tempImage = ShopMiddleButtonImage;
                tempText = ShopMiddleButtonText;
                tempTextBox = ShopMiddleButtonTextBox;
                break;
            default:
                Debug.LogError($"未处理的按钮类型：{buttonName}");
                return;
        }

        // 精准空引用校验
        if (tempBotton == null)
        {
            Debug.LogError($"按钮 {buttonName} 的 Button 组件为空！");
            return;
        }
        if (tempImage == null)
        {
            Debug.LogError($"按钮 {buttonName} 的 Image 组件为空！");
            return;
        }
        if (tempText == null)
        {
            Debug.LogError($"按钮 {buttonName} 的 TextMeshProUGUI 组件为空！");
            return;
        }
        if (tempTextBox == null)
        {
            Debug.LogError($"按钮 {buttonName} 的 ButtonSelect 组件为空！");
            return;
        }

        // 动作逻辑（语义与逻辑一致）
        switch (action)
        {
            case ButtonAction.Enable:
                tempBotton.interactable = true;
                SetTextBox(textBox,tempTextBox);
                tempImage.gameObject.SetActive(true);
                break;
            case ButtonAction.Disable:
                tempImage.gameObject.SetActive(true);
                tempBotton.interactable = false;
                break;
            case ButtonAction.Hide:
                tempImage.gameObject.SetActive(false);
                tempBotton.interactable = false;
                break;
        }
        
    }

    private void SetTextBox(Manager textBox,ButtonSelect buttonSelect)
    {
        buttonSelect.SetTextBox(textBox);

    }
    
    
    
    #region 按钮组件获取

    /// <summary>
    /// 缓存所有按钮组件，避免重复 GetComponent
    /// </summary>
    private void CacheAllButtons()
    {
        CacheNormalSceneButtons();
        if (_talkSys.Daytime != 0)
        {
            CacheShopSceneButtons();
        }
        
    }

    /// <summary>
    /// 缓存普通场景按钮组件
    /// </summary>
    private void CacheNormalSceneButtons()
    {
        // 左按钮
        _leftButton = _talkSys.UpButton;
        _leftButtonImage = _leftButton.gameObject.GetComponent<Image>();
        _leftButtonText = _leftButton.gameObject.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        _leftButtonTextBox = _leftButton.gameObject.GetComponent<ButtonSelect>();

        // 右按钮
        _rightButton = _talkSys.DownButton;
        _rightButtonImage = _rightButton.gameObject.GetComponent<Image>();
        _rightButtonText = _rightButton.gameObject.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        _rightButtonTextBox = _rightButton.gameObject.GetComponent<ButtonSelect>();
    }

    /// <summary>
    /// 缓存商店场景按钮组件
    /// </summary>
    private void CacheShopSceneButtons()
    {
        // 商店左按钮
        _shopLeftButton = _talkSys.ShopLButton.GetComponent<Button>();
        _shopLeftButtonImage = _talkSys.ShopLButton.GetComponent<Image>();
        _shopLeftButtonText = _talkSys.ShopLButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        _shopLeftButtonTextBox = _talkSys.ShopLButton.GetComponent<ButtonSelect>();

        // 商店右按钮
        _shopRightButton = _talkSys.ShopRButton.GetComponent<Button>();
        _shopRightButtonImage = _talkSys.ShopRButton.GetComponent<Image>();
        _shopRightButtonText = _talkSys.ShopRButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        _shopRightButtonTextBox = _talkSys.ShopRButton.GetComponent<ButtonSelect>();

        // 商店中间按钮
        _shopMiddleButton = _talkSys.ShopMButton.GetComponent<Button>();
        _shopMiddleButtonImage = _talkSys.ShopMButton.GetComponent<Image>();
        _shopMiddleButtonText = _talkSys.ShopMButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        _shopMiddleButtonTextBox = _talkSys.ShopMButton.GetComponent<ButtonSelect>();
    }

    #endregion

    public void SetBoBodySpecialChoice(bool have)
    {
        SwitchButtonState
        (
            ButtonName.Left,
            have? ButtonAction.Enable:ButtonAction.Disable,
            _talkSys.Talklines[_talkSys.Daytime].Option1
        );
        SwitchButtonState
        (
            ButtonName.Right,
            ButtonAction.Enable,
            _talkSys.Talklines[_talkSys.Daytime].Option2
        );
    }
    
    
    
    
}