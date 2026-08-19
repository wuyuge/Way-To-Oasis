//
using UnityEngine;
using UnityEngine.UI;

public class SwitchToggle : SwitchCommand
{
    private TalkSystem _talkSys;
    #region 对话栏与角色栏引用
    private GameObject TalkPanel => _talkSys.gameObject;
    private GameObject CharacterPanel => _talkSys.charabar;
    #endregion

    private TalkSysShowText _showText;
    [SerializeField]
    private Button skipButton;
    
    private bool MiniMode
    {
        get => _talkSys.MiniMode; 
        set => _talkSys.MiniMode = value;
    }


    /// <summary>
    /// 初始化SwitchToggle实例，设置TalkSystem引用。
    /// </summary>
    /// <param name="talkSys">用于对话管理的TalkSystem实例。</param>
    public override void Init(TalkSystem talkSys)
    {
        _talkSys = talkSys;
        _showText = talkSys.showText;
    }

    /// <summary>
    /// 执行指定功能代码对应的操作。
    /// </summary>
    /// <param name="function">要执行的功能枚举值。</param>
    public override void Execute(FunctionCode.Function function)
    {
        switch (function)
        {
            case FunctionCode.Function.A:
                //迷你对话模式布尔值
                MiniMode = !MiniMode;
                break;
            case FunctionCode.Function.B:
                //对话框动画控制
                Debug.LogWarning("枚举值设置方法为空,请选择子枚举",this);
                break;
            case FunctionCode.Function.Ba://上升
                MoveUI(UIElement.Talk, UIMovement.Up);
                if(_talkSys.useNewSys) GlobalData.NewTalkSysShowText.UnLockOutPut();
                _showText.CanShowText = true;
                _showText.SetEmptyText();
                break;
            case FunctionCode.Function.Bb://下降
                MoveUI(UIElement.Talk,UIMovement.Down);
                if(_talkSys.useNewSys)
                {
                    GlobalData.NewTalkSysShowText.LockOutPut();
                }
                _showText.CanShowText = false;
                _showText.SetEmptyText();
                TutorialManager.CharacterIsTalking = false;
                break;
            
            case FunctionCode.Function.C:
                //角色框动画控制
                Debug.LogWarning("枚举值设置方法为空,请选择子枚举",this);
                break;
            case FunctionCode.Function.Ca://上升
                MoveUI(UIElement.Character, UIMovement.Up);
                break;
            case FunctionCode.Function.Cb://下降
                MoveUI(UIElement.Character, UIMovement.Down);
                break;
            
            case FunctionCode.Function.D:
                //切换下一个对话数据
                _talkSys.Talklines[_talkSys.Daytime] = _talkSys.Talklines[_talkSys.Daytime].Option1;
                _talkSys.line = 0;
                if (_talkSys.useNewSys)
                {
                    GlobalData.NewTalkSysShowText.SetChoiceLine(0,false);
                }
                break;
            case FunctionCode.Function.E:
                //开关角色分配食物按钮
                Debug.LogWarning("枚举值设置方法为空,请选择子枚举",this);
                break;
            case FunctionCode.Function.Ea://开
                foreach (var g in _talkSys.CharacterList)
                {
                    g.transform.Find("Toggle").gameObject.GetComponent<Toggle>().interactable = true;
                }
                break;
            case FunctionCode.Function.Eb://关
                foreach (var g in _talkSys.CharacterList)
                {
                    g.transform.Find("Toggle").gameObject.GetComponent<Toggle>().interactable = false;
                }
                break;
            
            case FunctionCode.Function.F:
                //关营火动画
                _talkSys.MiniCharacterManager.gameObject.GetComponent<MiniCharacterManager>().OffLight();
                break;
            case FunctionCode.Function.G:
                //禁止点击继续对话
                _talkSys.on = false;
                break;
            case FunctionCode.Function.H:
                //开关迷你角色图像
                Debug.LogWarning("枚举值设置方法为空,请选择子枚举",this);
                break;
            case FunctionCode.Function.Ha://开
                _talkSys.MiniCharacterManager.gameObject.GetComponent<MiniCharacterManager>().ShowMiniCharacter();
                break;
            case FunctionCode.Function.Hb://关
                _talkSys.MiniCharacterManager.gameObject.GetComponent<MiniCharacterManager>().CloseMiniCharacter();
                break;
            
            case FunctionCode.Function.I:
                //关闭遮罩
                break;
            case FunctionCode.Function.J:
                //全屏黑屏   
                _talkSys.black.GetComponent<Animator>().SetTrigger("Black");
                break;
            case FunctionCode.Function.K:
                //Demo结束
                GameObject.Find("EndingsManager").GetComponent<EndingsManager>().ToEnd("Demo-End");
                break;
            case FunctionCode.Function.L:
                //从教学场景切换到主场景
                _talkSys.MainCanvas.SetActive(true);
                _talkSys.transform.parent.gameObject.SetActive(false);
                break;
            case FunctionCode.Function.M:
                //开启关闭角色立绘对话框
                break;
            case FunctionCode.Function.Ma://开
                _talkSys.charabar.SetActive(true);
                break;
            case FunctionCode.Function.Mb://关
                _talkSys.charabar.SetActive(false);
                _talkSys.CharacterImageManager.CloseImage();
                break;
            case FunctionCode.Function.N:
                //自动切换下一天
                Progress tempProgress = _talkSys.DaytimeOBJ.GetComponent<Progress>();
                tempProgress.CanSwitch = true;
                tempProgress.SwitchProgress();
                break;

        }
    }

    
    #region UI移动逻辑

    private enum UIMovement
    {
        Up,
        Down
    }
    private enum UIElement
    {
        Talk,
        Character
    }


    private void MoveUI(UIElement element,UIMovement moveValue)
    {
        GameObject temp = element == UIElement.Talk ? TalkPanel : CharacterPanel;
        Animator anim = temp.GetComponent<Animator>();
        
        switch (moveValue)
        {
            case UIMovement.Up:
                anim.SetTrigger("Up");
                break;
            case UIMovement.Down:
                anim.SetTrigger("Down");
                break;
            
        }
        
        
    }
    
    

    #endregion
    
    
    
    
}
