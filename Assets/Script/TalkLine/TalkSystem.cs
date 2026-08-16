
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


/// <summary>
/// 对话系统核心类：负责文本显示、角色切换、选择分支等对话逻辑
/// 支持说话途中点击立即显示剩余文本
/// </summary>
public class TalkSystem : MonoBehaviour
{
    [Header("每天的文本线")]
    [Tooltip("存储每一天的对话数据列表")]
    public List<Manager> Talklines = new List<Manager>();
    public Manager DeadName,UsedBody;
    [Header("管理时间,目前行数")]
    [Tooltip("关联到进度管理器对象，用于获取当前天数")]
    public GameObject DaytimeOBJ;
    [Tooltip("当前天数索引")]
    public int Daytime;
    [Tooltip("当前正在显示的对话行索引")]
    public int line;
    [Header("文本框位置")]
    [Tooltip("玩家对话显示的文本框")]
    public TextMeshProUGUI Player;
    public GameObject PlayerTalkBackGround;
    public TextMeshProUGUI PlayerNameText;
    [Tooltip("角色对话显示的文本框")]
    public TextMeshProUGUI Character;
    [Tooltip("角色名称显示的文本框")]
    public TextMeshProUGUI Chara_Name;
    [Header("商店用对象")]
    [Tooltip("商店")]
    public GameObject ShopManager;
    public GameObject ShopTextBar;
    public GameObject ShopCharaBar;
    public TextMeshProUGUI ShopName;
    public GameObject ShopLButton, ShopRButton, ShopMButton;
    public Manager Day2_Shop_Exchange, Day2_Shop_KillSomeOne;
    #pragma warning disable CS0414
    public bool ShowName = true;
    [Tooltip("判断是否是商店场景")]
    public bool _inshop;
    [Header("辨别是谁在说话")]
    [Tooltip("标记当前是否是玩家在说话（true=玩家，false=角色）")]
    public bool PlayerTalking = false;
    [Header("按钮位置")]
    [Tooltip("选择分支时的左按钮")]
    public Button UpButton;
    [Tooltip("选择分支时的右按钮")]
    public Button DownButton;
    [Tooltip("是否允许对话交互（防止重复点击）")]
    public bool on = true;
    [Tooltip("文本逐字显示的速度（毫秒/字）")]
    public int TextSpeed;
    [Tooltip("动画控制")]
    public Animator anim;
    [Header("各个游戏对象")]
    private GameObject charaBar;
    public GameObject amande;
    public GameObject charabar, black, MainCanvas;
    public List<GameObject> CharacterList;
    public List<Character> characterComponentList;
    public Manager aimi;
    [Header("角色立绘控制器")]
    public CharacterImageManager CharacterImageManager;
    [Header("新手引导用对象")]
    public GameObject DownBar;
    public Manager Day0_Talk,TeachComfort;
    public GameObject Menu;
    public bool inTech = false;
    public bool CanSkip = true;
    [Header("点击间隔配置")]
    [Tooltip("点击后允许再次点击的间隔时间（毫秒），建议设置200-500ms")]
    public float ClickInterval = 300f; // 默认300毫秒，可根据体验调整
    private float _lastClickTime; // 记录上次有效点击的时间戳（单位：秒）
    [Header("文字显示速度（秒）")] public float TextSpeedI;
    public bool _IsShowingText = false; // 标记当前是否正在显示文本
    public bool BreakText = false; // 标记是否请求中断文本显示
    [Header("玩家名字")]
    public Manager PlayerName;
    [Header("阿曼德自杀判断")]
    public Manager amandeKillself;
    [FormerlySerializedAs("Day2ShopEvent")] [Header("商店事件判断")]
    public Manager shopEvent;
    [Header("博金森死亡时间判断")]
    public Manager BoDeadTime;
    [Header("教程文本列表")]
    [SerializeField]
    public TechTextList TechTextList;
    public Manager ShowTech;
    [Header("小人对话管理")]
    public MiniCharacterTalkSys MiniCharacterManager;
    public bool MiniMode;
    [Header("噪点遮罩")]
    public GameObject NoiseMask;
    //一起黑屏控制
    private bool TogetherClose;
    [Header("立绘表情控制")]
    public List<string> Expressions = new List<string>();
    private CharacterExpression expression;
    public AudioSource Type;
    /*private bool CharaBarShow = true;*/
    [Header("子脚本")] 
    public TalkSysSwitch switchManager;
    public TalkSysShowText showText;
    public TalkSysUIButtonFunc buttonFunc;
    public NewTalkSysShowText newTalkSysShowText;

    public Manager reload;
    public Manager language;
    public bool useNewSys;
    
    /// <summary>
    /// 用于存档加载时暂时停止对话系统调用
    /// </summary>
    public bool Ban
    {
        get;
        set;
    }
    
    


    private void Awake()
    {
        Daytime = DaytimeOBJ.GetComponent<Progress>().day_num;
        anim = gameObject.GetComponent<Animator>();
        PlayerNameText.text = PlayerName.TxtLine[0];
        Type = gameObject.GetComponent<AudioSource>();
        GlobalData.TalkSystem = this;
        GlobalData.Language = language;
    }
    
    

    void SetStartTalk()
    {
        Invoke("SetOn", 1f);
        _ = ShowText();
        
    }

    void SetOn()
    {
        on = true;
    }

    private void OnEnable()
    {
        buttonFunc.Init(this);
        switchManager.Init(this);
        showText.Init(this);
        newTalkSysShowText.Init(this);
        TalkSysStaticData.TalkSys = this;
        if (characterComponentList.Count == 0)
        {
            foreach (var value in CharacterList)
            {
                characterComponentList.Add(value.GetComponent<Character>());
            }
        }
    }


    /// <summary>
    /// 初始化对话系统
    /// </summary>
    void Start()
    {
        
        charaBar = gameObject.transform.parent.Find("DownBar").gameObject;
        
        // 清空UI文本框
        CleanUI();
        // 隐藏选择按钮（初始状态不需要）
        UpButton.gameObject.SetActive(false);
        DownButton.gameObject.SetActive(false);
        // 从第0行开始显示对话
        ResetLine();
        // 从进度管理器获取当前天数
        expression = CharacterImageManager.gameObject.GetComponent<CharacterExpression>();
        
        if (on && DaytimeOBJ.GetComponent<Progress>().talk)
        {
            // 隐藏选择按钮（点击文本时关闭选择界面）
            UpButton.gameObject.SetActive(false);
            DownButton.gameObject.SetActive(false);
        }

        if (Daytime == 0 && !reload.GeneralBool && GlobalData.Progress.talk)
        {
            Invoke(nameof(SetStartTalk),1f);
        }
                
    }

    /// <summary>
    /// 帧更新：检测鼠标点击并处理对话交互
    /// </summary>
    void Update()
    {
        if (DaytimeOBJ.GetComponent<Progress>().talk || DaytimeOBJ.GetComponent<Progress>().CanTalk || _inshop)
        {
            Daytime = DaytimeOBJ.GetComponent<Progress>().day_num;

            // 计算当前时间与上次点击的间隔（转换为毫秒便于比较）
            float timeSinceLastClick = (Time.time - _lastClickTime) * 1000f;

            // 点击条件：鼠标左键按下 或 空格键按下 + 交互开启（on） + 超过点击间隔

            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
            {
                
                if (on && timeSinceLastClick >= ClickInterval && !_IsShowingText)
                {
                    
                    // 更新上次点击时间戳为当前时间
                    _lastClickTime = Time.time;
                    _ = ShowText();
                    // 仅当不在显示文本时，才开始新的文本显示
                }
                else if (_IsShowingText && !BreakText)
                {
                    BreakText = true;
                }
            }
        }
    }


    public int ShowText()
    {
        if (!useNewSys)
        {
            showText.ShowText();
        }
        else
        {
            newTalkSysShowText.ShowText();
        }


        return 0;
    }
    
    
    /// <summary>
    /// 清空所有UI文本框
    /// </summary>
    void CleanUI()
    {
        Player.text = "";
        Character.text = "";
        Chara_Name.text = "";
    }

    public void ShowExchangeTalk()
    {
        
        bool AmandeDead = false;

        Daytime = DaytimeOBJ.GetComponent<Progress>().day_num;
        if (Daytime == 2)
        {
            shopEvent.GeneralBool = true;
            foreach (string s in DeadName.TxtLine)
            {
                if (s.Contains("阿曼德"))
                {
                    AmandeDead=true;
                    break;
                }
            }
            foreach (string s in UsedBody.TxtLine)
            {
                if (s.Contains("阿曼德"))
                {
                    AmandeDead = true;
                    break;
                }
            }
            if (!AmandeDead)
                amandeKillself.GeneralBool = true;
            if (CharacterList[4].GetComponent<Character>().Special2) return;

            CharacterList[4].GetComponent<Character>().Special1 = true;
            CharacterList[4].GetComponent<Character>().EnableTalk();
            CharacterList[4].GetComponent<Character>().Attention.SetActive(true);
            Debug.Log("Done");
        }

        foreach (GameObject g in CharacterList)
        {
            if (!g.GetComponent<Character>().Special1 && !g.GetComponent<Character>().Special2)
                g.GetComponent<Character>().DisableTalk();

        }



    }


    public void ShowKillTalk()
    {

        bool AmandeDead = false;
        Daytime = DaytimeOBJ.GetComponent<Progress>().day_num;
        if (Daytime == 2)
        {
            shopEvent.GeneralBool = true;
            foreach (string s in DeadName.TxtLine)
            {
                if (s.Contains("阿曼德"))
                {
                    AmandeDead = true;
                    break;
                }
            }
            foreach (string s in UsedBody.TxtLine)
            {
                if (s.Contains("阿曼德"))
                {
                    AmandeDead = true;
                    break;
                }
            }
            if (!AmandeDead)
                amandeKillself.GeneralBool = true;
            CharacterList[4].GetComponent<Character>().Special2 = true;
            CharacterList[4].GetComponent<Character>().EnableTalk();
            CharacterList[4].GetComponent<Character>().Attention.SetActive(true);
            CharacterList[5].GetComponent<Character>().Special2 = true;
            CharacterList[5].GetComponent<Character>().EnableTalk();
            CharacterList[5].GetComponent<Character>().Attention.SetActive(true);
            if(DeadName.TxtLine != null)
            {   
                if (DeadName.TxtLine[DeadName.TxtLine.Count - 1] == "博金森")
                {
                    Debug.Log("第一次商店杀死博金森");
                    CharacterList[1].GetComponent<Character>().Special2 = true;
                
                    CharacterList[1].GetComponent<Character>().EnableTalk();
                    CharacterList[1].GetComponent<Character>().Attention.SetActive(true);

                }
                if (DeadName.TxtLine[DeadName.TxtLine.Count - 1] == "艾米莉")
                {
                        Debug.Log("第一次商店杀死艾米莉");
                        CharacterList[3].GetComponent<Character>().Special2 = true;
                        CharacterList[3].GetComponent<Character>().EnableTalk();
                        CharacterList[3].GetComponent<Character>().Attention.SetActive(true);
                }
            }
        }
        foreach (GameObject g in CharacterList)
        {
            if (!g.GetComponent<Character>().Special1 && !g.GetComponent<Character>().Special2) g.GetComponent<Character>().DisableTalk();
        }
    }
    public void ShowBar()
    {
        if (anim is null)
        {
            anim = GetComponent<Animator>();
        }
        anim.SetTrigger("Up");
    }
    public void HideBar()
    {
        anim = GetComponent<Animator>();
        anim.SetTrigger("Down");
    }
    public void ResetLine()
    {
         line = 0;
    }
    private void SetTechMode(GameObject MaskGameObj ,string TechText)
    {
        if (!ShowTech.GeneralBool)
        {
            //PlusLine();
            //_ = ShowText(true);
            return;
        }
        inTech = true;
    }
    private string SetTechText(string Comment)
    {
        foreach (TechTextList.TechText techText in TechTextList.TextList)
        {
            if (techText.name == Comment)
            {
                return techText.text;
            }
        }
        return "";
    }
    public void Mask(GameObject MaskGameObj, string Comment)
    {
        string ShowText = SetTechText(Comment);
        SetTechMode(MaskGameObj, ShowText);
    }
    public void SetShowName()
    {
        ShowName = true;
        PlayerNameText.text = PlayerName.TxtLine[0];
    }
    public void SetNoName()
    {
        ShowName = false;
        PlayerNameText.text = "";
    }
    public void SwitchExpression(string CharaName,string Expression)
    {
        expression.SetExpression(CharaName,Expression);
    }
    public void SwitchExpression(string CharaName,int index)
    {
        expression.SetExpression(CharaName,index);
    }
    public void SwitchLine(TalkLine switchLine)
    {

        switch (switchLine)
        {
            case  TalkLine.Line1:
                Talklines[Daytime] = Talklines[Daytime].Option1;
                break;
            case TalkLine.Line2:
                Talklines[Daytime] = Talklines[Daytime].Option2;
                break;
            case TalkLine.Line3:
                Talklines[Daytime] = Talklines[Daytime].Option3;
                break;
            default:
                Debug.Log("预期外的line");
                break;
        }
        line = 0;
        if (useNewSys)
        {
            newTalkSysShowText.SetChoiceLine(0,false);
        }
    }
    public void SetTextBox(Manager textBox)
    {
        Talklines[Daytime] = textBox;
        showText.CanShowText = true;
    }
}



public enum TalkLine
{
    Line1,Line2,Line3
}

public static class TalkSysStaticData
{
    public static TalkSystem TalkSys;
    public static TalkSysShowText TalkSysShowText;
}


