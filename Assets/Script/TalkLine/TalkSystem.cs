using Coffee.UIExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using static TechTextList;

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
    private bool ShowName;
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
    public Manager aimi;
    [Header("角色立绘控制器")]
    public CharacterImageManager CharacterImageManager;

    [Header("新手引导用对象")]
    public GameObject mask;
    public GameObject DownBar;
    public Manager Day0_Talk,TeachComfort;
    public GameObject Menu;

   
    public bool inTech = false;
    
    public bool CanSkip = true;
    [Header("点击间隔配置")]
    [Tooltip("点击后允许再次点击的间隔时间（毫秒），建议设置200-500ms")]
    public float ClickInterval = 300f; // 默认300毫秒，可根据体验调整
    private float _lastClickTime; // 记录上次有效点击的时间戳（单位：秒）
    [Header("文字显示速度（毫秒）")]
    public int TextSpeedI = 100;
    public bool _IsShowingText = false; // 标记当前是否正在显示文本
    public bool BreakText = false; // 标记是否请求中断文本显示
    [Header("玩家名字")]
    public Manager PlayerName;
    [Header("阿曼德自杀判断")]
    public Manager amandeKillself;
    [Header("商店事件判断")]
    public Manager Day2ShopEvent;
    [Header("博金森死亡时间判断")]
    public Manager BoDeadTime;

    [Header("教程文本列表")]
    [SerializeField]
    private TechTextList TechTextList;
    public Manager ShowTech;
    [Header("小人对话管理")]
    public MiniCharacterTalkSys MiniCharacterManager;
    private bool MiniMode = false;

    [Header("噪点遮罩")]
    public GameObject NoiseMask;
    //一起黑屏控制
    private bool TogetherClose;


    [Header("立绘表情控制")]
    public List<string> Expressions = new List<string>();
    private CharacterExpression expression;

    public AudioSource Type;


    private void Awake()
    {
        Daytime = DaytimeOBJ.GetComponent<Progress>().day_num;
        PlayerNameText.text = PlayerName.TxtLine[0];
        if (Daytime == 0)
        {
            on = false;
            Invoke("SetStartTalk", 1.5f);

        }
        Type = gameObject.GetComponent<AudioSource>();
    }

    void SetStartTalk()
    {
        Invoke("SetOn", 1f);
        _ = ShowText(true);
        
    }

    void SetOn()
    {
        on = true;
    }

    /// <summary>
    /// 初始化对话系统
    /// </summary>
    void Start()
    {
        
        charaBar = gameObject.transform.parent.Find("DownBar").gameObject;
        anim = gameObject.GetComponent<Animator>();
        // 清空UI文本框
        CleanUI();
        // 隐藏选择按钮（初始状态不需要）
        UpButton.gameObject.SetActive(false);
        DownButton.gameObject.SetActive(false);
        // 从第0行开始显示对话
        ResetLine();
        // 从进度管理器获取当前天数
        
        if (on && DaytimeOBJ.GetComponent<Progress>().talk)
        {
            // 隐藏选择按钮（点击文本时关闭选择界面）
            UpButton.gameObject.SetActive(false);
            DownButton.gameObject.SetActive(false);

           
        }
        expression = CharacterImageManager.gameObject.GetComponent<CharacterExpression>();
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

                    UpButton.gameObject.SetActive(false);
                    DownButton.gameObject.SetActive(false);
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




    /// <summary>
    /// 异步显示文本（支持逐字显示和中途取消）
    /// </summary>
    public async Task ShowText(bool isroll = false, string addtiontext = null, bool ClearText = true,bool ShowMask = false)
    {

        
        // 清空文本框（原有逻辑保留）
        if (!PlayerTalking && ClearText)
        {
            //Character.text = "";
            //Chara_Name.text = "";
        }
        else
        {
            Player.text = "";
        }
        if (_inshop)
        {
            ShopTextBar.GetComponent<TextMeshProUGUI>().text = "";
        }
        
        

        try
        {
            string curText = Talklines[Daytime].TxtLine[line];
            
            CanSkip = false;
            inTech = false;
            switch (curText)
            {
                
                case "choice":
                    
                    HandleChoice();
                    

                    return;
                //管理小人对话
                case "/MiniModeOn":
                    MiniMode = true;
                    PlusLine();
                    _ = ShowText(true);
                    return;
                case "/MiniModeOff":
                    MiniMode = false;
                    MiniCharacterManager.CompleteTalk();
                    PlusLine();
                    _ = ShowText(true);
                    return;
                case "/OffLight":
                    MiniCharacterManager.gameObject.GetComponent<MiniCharacterManager>().OffLight();
                    PlusLine();
                    _ = ShowText(true);
                    return;
                case "/Laiwensuccess"://安抚成功/失败预留
                    CharacterList[5].GetComponent<Character>().NotComfort = false;
                    PlusLine();
                    _ = ShowText(true);
                    return;
                case "/Laiwenfail":
                    CharacterList[5].GetComponent<Character>().NotComfort = true;
                    PlusLine();
                    _ = ShowText(true);
                    return;
                
                case "/CheckShopEvent"://杀人或换尸体后转分支1，否则转分支2

                    if (Day2ShopEvent.GeneralBool)
                    {
                        TurnOption1();
                        ResetLine();
                        _ = ShowText(true);
                        return;
                    }
                    else
                    {
                        TurnOption2();
                        ResetLine();
                        _ = ShowText(true);
                        return;
                    }

                
                case "/checktalk":
                    if (Day0_Talk.Weight == 3)
                    {
                        PlusLine();
                        _ = ShowText(true);
                        return;
                    }
                    else
                    {
                        TurnOption3();
                        ResetLine();
                        _ = ShowText(true);
                        return;
                    }
                //艾米莉特殊指令
                case "/CheckBoBody":
                    if (DeadName.TxtLine == null)
                    {
                        HandleChoice(ban: 1);
                        return;
                    }
                    bool Have_BoBody = false;
                    foreach (string s in DeadName.TxtLine)
                    {
                        if(s.Contains("博金森"))
                        {
                            Have_BoBody = true;
                            break;
                            
                        }
                        else
                        {
                            Have_BoBody = false;
                        }
                        
                    }

                    if (Have_BoBody)
                    {
                        HandleChoice();
                        return;
                    }
                    else
                    {
                        HandleChoice(ban: 1);
                        return;
                    }


                case "/UseBo":
                    int index = -1;
                    foreach(string s in DeadName.TxtLine)
                    {
                        index++;
                        if(s == "博金森")
                        {
                            UsedBody.TxtLine.Add("博金森Used");
                            DeadName.TxtLine.RemoveAt(index);
                        }

                    }
                    CharacterList[1].GetComponent<Character>().CantWeight = true;
                    PlusLine();
                    _ = ShowText(true);
                    return;

                case "/aimifail":
                    CharacterList[1].GetComponent<Character>().NotComfort = true;
                    PlusLine();
                    _ = ShowText(true);
                    return;


                case "/BanClik":
                    on = false;
                    PlusLine();
                    _ = ShowText(true);
                    return;
                case "/specialchoice":

                    HandleChoice(true);
                   

                    return;
                case "/showname":
                    SetShowName();
                    PlusLine();
                    _ = ShowText(true);
                    return;
                case "/ResetTrigger":
                    this.CharacterImageManager.ResetTrigger();
                    PlusLine();
                    _ = ShowText(true);
                    return;
                case "/noname":
                    SetNoName();
                    PlusLine();
                    _ = ShowText(true);
                    return;
                case "/KillSomeOne":
                    Day2_Shop_KillSomeOne.GeneralBool = true;

                    ShopCharaBar.SetActive(true);
                    ShopCharaBar.GetComponent<Animator>().SetTrigger("Up");
                    ShopCharaBar.GetComponent<ShopCharacterManager>().KillSB();

                    on = false;
                    return;

                case "/exchangeBody":
                    
                    Day2_Shop_Exchange.GeneralBool = true;
                    _inshop = true;

                    if (!ShopManager.GetComponent<ShopManager>().ExchangeFood())
                    {
                        ShowExchangeTalk();
                        ShopCharaBar.SetActive(true);
                        ShopCharaBar.GetComponent<Animator>().SetTrigger("Up");
                        ShopCharaBar.GetComponent<ShopCharacterManager>().SelectBody();
                        on = false;
                        


                    }
                    else
                    {
                        ShowExchangeTalk();
                        
                        PlusLine();
                        _ = ShowText(true);

                    }


                    return;
                case "/closeshop":
                    ShopManager.SetActive(false);
                    PlusLine();                         // 重置对话行索引
                    _inshop = false;                    // 标记退出商店场景
                    on = false;                          
                    _ = ShowText(true);
                    return;

                case "/shop":

                    _inshop = true;
                    PlusLine();

                    _ = ShowText(true);
                    return;
                case "CloseMask":
                    mask.transform.parent.gameObject.SetActive(false);
                    PlusLine();
                    _ = ShowText(true);
                    return;
                //教程部分命令
                case "techcomfort":
                    if (!ShowTech.GeneralBool)
                    {
                        PlusLine();
                        _ = ShowText(true);
                        return;
                    }
                    string Comforttext = SetTechText("Comfort");
                    
                    if (!TeachComfort.GeneralBool)
                    {
                        GameObject ComfortObj = null;
                        foreach (GameObject Character in CharacterList)
                        {
                            Character chara = Character.GetComponent<Character>();
                            if (!chara.Dead && (chara.Special1 || chara.Special2))
                            {
                                ComfortObj = Character;
                                break;
                            }
                        }
                        on = true;
                        if(ComfortObj != null)SetTechMode(ComfortObj,Comforttext);
                        TeachComfort.GeneralBool = true;
                        
                    }
                    PlusLine();

                    return;
                case "techmenu":
                    if (!ShowTech.GeneralBool)
                    {
                        PlusLine();
                        _ = ShowText(true);
                        return;
                    }

                    string Menutext = SetTechText("Menu");
                    SetTechMode(Menu, Menutext);
                    on = true;

                    PlusLine();
                    
                    return;
                case "techclik":
                    if (!ShowTech.GeneralBool)
                    {
                        PlusLine();
                        _ = ShowText(true);
                        return;
                    }

                    string Cliktext = SetTechText("Clik");
                    SetTechMode(transform.Find("MaskLayer").gameObject, Cliktext);
                    PlusLine();
                    
                    
                    return;

                case "techtalk":
                    if (!ShowTech.GeneralBool)
                    {
                        PlusLine();
                        _ = ShowText(true);
                        return;
                    }
                    string Talktext = SetTechText("Talk");
                    SetTechMode(DownBar.transform.Find("MaskLayer").gameObject,Talktext);
                    on = true;
                    PlusLine();
                    DaytimeOBJ.GetComponent<Button>().enabled = true;
                    

                    return;
                case "techover":
                    if (!ShowTech.GeneralBool)
                    {
                        PlusLine();
                        _ = ShowText(true);
                        return;
                    }
                    mask.transform.parent.gameObject.GetComponent<MaskManager>().SetClik(true);
                    PlusLine();
                    _ = ShowText(true);
                    return;

                case "techright":
                    if (!ShowTech.GeneralBool)
                    {
                        PlusLine();
                        _ = ShowText(true);
                        return;
                    }

                    string Righttext = SetTechText("Right");
                    SetTechMode(DaytimeOBJ.transform.parent.gameObject, Righttext);
                    on = true;
                    DaytimeOBJ.GetComponent<Button>().enabled = false;
                    PlusLine();
                    mask.transform.parent.gameObject.GetComponent<MaskManager>().SetClik(false);
                    
                    return;

                case "techclose":
                    if (!ShowTech.GeneralBool)
                    {
                        PlusLine();
                        _ = ShowText(true);
                        return;
                    }
                    string Closetext = SetTechText("Close");
                    SetTechMode(DaytimeOBJ, Closetext);
                    PlusLine();
                    _ = ShowText(true);
                    return;
                case "techfood":
                    if (!ShowTech.GeneralBool)
                    {
                        PlusLine();
                        _ = ShowText(true);
                        return;
                    }
                    string Foodtext = SetTechText("Food");
                    inTech = true;
                    SetTechMode(DownBar.transform.Find("MaskLayer").gameObject, Foodtext);
                    PlusLine();
                    
                    _ = ShowText(true);
                    return;

                case "techweight":
                    if (!ShowTech.GeneralBool)
                    {
                        PlusLine();
                        _ = ShowText(true);
                        return;
                    }
                    string Weighttext = SetTechText("Weight");
                    SetTechMode(DownBar.transform.Find("MaskLayer").gameObject, Weighttext);
                    PlusLine();
                    
                    _ = ShowText(true);
                    return;
                //教程结束



                //小人图像控制
                case "/OffMiniCharacter":
                    MiniCharacterManager.gameObject.GetComponent<Animator>().enabled = true;
                    MiniCharacterManager.gameObject.GetComponent<MiniCharacterManager>().CloseMiniCharacter();
                    PlusLine();
                    _= ShowText(true);
                    return;

                case "/OnMiniCharacter":
                    MiniCharacterManager.gameObject.GetComponent<MiniCharacterManager>().ShowMiniCharacter();
                    PlusLine();
                    _ = ShowText(true);
                    return;



                case "showdeadname":
                    
                    string showtext = string.Concat(DeadName.TxtLine); // 简化字符串拼接
                    if (showtext.Contains("Leader"))
                    {
                        showtext = showtext.Replace("Leader", string.Empty);
                    }
                    line += 1;

                    _ = ShowText(true, showtext);
                    return;

                



                case "down":
                    DaytimeOBJ.GetComponent<Progress>().CanSwitch = true;
                    anim.SetTrigger("down");
                    if(MiniMode)Invoke("SetCharBar", 1.5f);
                    PlusLine();
                    
                    
                    _ = ShowText(true);
                    return;
                case "aimieat":
                    if (aimi.Day1Eat)
                    {
                        TurnOption1();
                    }
                    else
                    {
                        TurnOption2();
                    }
                    ResetLine();

                    _ = ShowText(true);
                    return;
                case "up":
                    anim.SetTrigger("up");
                    PlusLine();
                    transform.position = new Vector2(transform.position.x, -5000); // 简化gameObject访问

                    _ = ShowText(true);
                    return;
                //死亡判断分支

                case "/CheckEveryOneLive"://全部存活 转分支1，有人死转分支2
                    foreach (GameObject g in CharacterList)
                    {
                        if (g.GetComponent<Character>().Dead)
                        {


                            if (g.GetComponent<Character>().CharacterName == "阿曼德" && amandeKillself.GeneralBool) continue;
                            TurnOption2();
                            ResetLine();
                            _ = ShowText(true);
                            return;

                        }

                    }
                    TurnOption1();
                    ResetLine();
                    _ = ShowText(true);
                    return;

                case "luo_peoplechoice"://洛尔坎判断死人性别 单人死亡 男性转分支1 女性转分支2 多人转分支3
                    if (DeadName.TxtLine.Count == 1)
                    {
                        string name = DeadName.TxtLine[0];
                        if (name == "莱文" || name == "博金森")
                            TurnOption1();
                        else if (name == "艾米莉" || name == "阿曼德")
                            TurnOption2();
                    }
                    else
                    {
                        TurnOption3();
                    }
                    ResetLine();

                    _ = ShowText(true);
                    return;

                case "deadfu"://没死分支1,死了转分支2
                    bool hasBokinson = false;
                    foreach (string s in DeadName.TxtLine)
                    {
                        if (s == "博金森")
                        {
                            hasBokinson = true;
                            break;
                        }
                    }
                    if (!hasBokinson)
                    {
                        foreach (string s in UsedBody.TxtLine)
                        {
                            if (s.Contains("博金森"))
                            {
                                hasBokinson = true;
                                break;
                            }
                        }
                    }
                    Talklines[Daytime] = hasBokinson ? Talklines[Daytime].Option2 : Talklines[Daytime].Option1;
                    ResetLine();

                    _ = ShowText(true);
                    return;

                case "/CheckBoDeadTime"://博金森死超过一天转分支2，否则转分支1
                    if (Daytime - BoDeadTime.Weight > 1)
                    {
                        TurnOption2();
                        ResetLine();
                        _ = ShowText(true);
                        return;
                    }
                    else
                    {
                        TurnOption1();
                        ResetLine();
                        _ = ShowText();
                        return;
                    }

                case "dead"://死了转分支1，没死分支2

                    Debug.Log("判断之前是否死过人");
                    if(DeadName.TxtLine.Count > 1 || UsedBody.TxtLine.Count > 1 || DeadName.TxtLine.Count + UsedBody.TxtLine.Count > 1)
                    {
                        Debug.Log("有死人");
                        TurnOption1();
                        ResetLine();
                        _ = ShowText(true);
                        return;
                    }
                    else if (DeadName.TxtLine.Count == 1 && DeadName.TxtLine[0] == "Leader" && UsedBody.TxtLine.Count == 0)
                    {
                        Debug.Log("没有死人");
                        TurnOption2();
                        ResetLine();
                        _ = ShowText(true);
                        return;
                    }
                    else if (DeadName.TxtLine.Count == 0 && UsedBody.TxtLine.Count == 1 && (UsedBody.TxtLine[0] == "LeaderUesd" || UsedBody.TxtLine[0] == "LeaderAbondoned"))
                    {
                        Debug.Log("没有死人");
                        TurnOption2();
                        ResetLine();
                        _ = ShowText(true);
                        return;
                    }

                    return;
                case "twicedeadchoice"://死了转分支2，没死分支1
                    if ((DeadName.TxtLine.Count == 1 && DeadName.TxtLine[0] == "Leader" && UsedBody.TxtLine.Count == 0) ||
                        (DeadName.TxtLine.Count == 0 && UsedBody.TxtLine.Count == 1 && UsedBody.TxtLine[0] == "LeaderUesd"))
                    {
                        TurnOption1();
                    }
                    else
                    {
                        TurnOption2();
                    }
                    ResetLine();

                    _ = ShowText(true);
                    return;

                case "/CheckAmandeKillself"://死了转分支1,没死分支2
                    if (amandeKillself.GeneralBool)
                    {
                        Debug.Log("阿曼德自杀");
                        TurnOption1();
                        ResetLine();
                        _ = ShowText(true);
                    }
                    else
                    {
                        TurnOption2();
                        ResetLine();
                        _ = ShowText(true);
                    }
                    return;
                //死亡判断分支结束

                case "upcbar":
                    charaBar.GetComponent<Animator>().SetTrigger("Up");
                    PlusLine();
                    if(!inTech && !TogetherClose) this.CharacterImageManager.CloseImage();
                    NoiseMask.SetActive(false);
                    if (TogetherClose) TogetherClose = false;
                    await Task.Delay(800);

                    _ = ShowText(true);
                    DaytimeOBJ.GetComponent<Progress>().CanSwitch = true;
                    return;

                case "downcbar":
                    charaBar.GetComponent<Animator>().SetTrigger("Down");
                    PlusLine();

                    _ = ShowText(true);
                    return;

                case "notover":
                    amande.GetComponent<Character>().have_talk = false;
                    PlusLine();
                    DaytimeOBJ.GetComponent<Progress>().CanSwitch = false;

                    _ = ShowText(true);
                    return;

                case "allover":
                    DaytimeOBJ.GetComponent<Progress>().CanSwitch = true;
                    PlusLine();
                    ShowName = true;
                    _ = ShowText(true);
                    return;
                
                case "showcharabar":
                    charabar.SetActive(true);
                    PlusLine();

                    Debug.Log("显示角色文本框");
                    _ = ShowText(true);
                    return;

                case "closecharabar":
                    charabar.SetActive(false);
                    PlusLine();
                    this.CharacterImageManager.CloseImage();
                    Debug.Log("关闭角色文本框");
                    _ = ShowText(true);
                    return;

                case "black":
                    black.GetComponent<Animator>().SetTrigger("Black");
                    PlusLine();
                    _ = ShowText(true);
                    return;

                case "next":
                    Debug.Log("进入正常场景");
                    PlusLine();

                    _ = ShowText(true);
                    if (Daytime == 0)
                    {
                        MainCanvas.SetActive(true);
                        charabar.SetActive(false);
                        transform.parent.gameObject.SetActive(false);
                        ResetLine();
                    }
                    DaytimeOBJ.GetComponent<Progress>().SwtichProgress();
                    return;

                case "/TogetherDark":
                    this.CharacterImageManager.CloseImage();
                    DaytimeOBJ.GetComponent<Progress>().skip.transform.Find("Report").gameObject.SetActive(false);
                    DaytimeOBJ.GetComponent<Progress>().skip.GetComponent<Animator>().SetTrigger("dark");
                    PlusLine();
                    TogetherClose = true;
                    _ = ShowText(true);
                    return;


                case "/ToDemoEnd":
                    GameObject.Find("EndingsManager").GetComponent<EndingsManager>().ToEnd("Demo-End");
                    return;

                case "lock":
                    DaytimeOBJ.GetComponent<Progress>().CanSwitch = false;
                    PlusLine();

                    _ = ShowText(true);
                    return;

                case "/TalkLock":
                    if( DaytimeOBJ.GetComponent<Progress>().skip.GetComponent<Image>().color.a != 0)
                    {
                        return;
                    }
                    else
                    {
                        PlusLine();

                        _ = ShowText(true);
                        return;
                    }

                case "canskip":
                    DaytimeOBJ.GetComponent<Progress>().can_skip = true;
                    PlusLine();

                    _ = ShowText(true);
                    return;

                case "p":
                    PlayerTalking = true;
                    PlusLine();
                    await Task.Yield();
                    _ = ShowText(true);
                    return;

                case "c":
                    PlayerTalking = false;
                    PlusLine();
                    await Task.Yield();
                    _ = ShowText(true);
                    return;

                case "end":
                    TurnOption1();
                    ResetLine();

                    _ = ShowText(true);
                    return;

                //day0用toggle判断
                case "/locktoggle":
                    foreach (GameObject g in CharacterList)
                    {
                        g.transform.Find("Toggle").gameObject.GetComponent<Toggle>().enabled = false;
                        g.transform.Find("Toggle").Find("Background").Find("Checkmark").gameObject.SetActive(false);
                    }
                    PlusLine();
                    _ = ShowText(true);
                    return;
                case "/unlocktoggle":
                    foreach (GameObject g in CharacterList)
                    {
                        g.transform.Find("Toggle").gameObject.GetComponent<Toggle>().enabled = true;
                        g.transform.Find("Toggle").Find("Background").Find("Checkmark").gameObject.SetActive(true);
                    }
                    PlusLine();
                    _ = ShowText(true);
                    return;


                //小人坐下状态
                case "/SetSit":
                    MiniCharacterManager.gameObject.GetComponent<MiniCharacterManager>().SetSit();
                    PlusLine();
                    _ = ShowText();
                    return;

                case "/SetStand":
                    MiniCharacterManager.gameObject.GetComponent<MiniCharacterManager>().SetStand();
                    PlusLine();
                    _ = ShowText();
                    return;




            }

            // 处理正常对话文本（角色说话）
            if (!PlayerTalking)
            {
                CanSkip = true;
                bool charaOver = false;
                string charaName = "";
                string dialogueContent = "";

                Character.text = "";
                Chara_Name.text = "";

                if (!NoiseMask.activeSelf && !_inshop && !MiniMode) NoiseMask.SetActive(true);


                if (curText.Contains("{PlayerName}"))
                {
                    curText = curText.Replace("{PlayerName}", PlayerName.TxtLine[0]);
                }

                if(!_inshop && !MiniMode && !charabar.activeSelf)
                {
                    charabar.SetActive(true);
                }


                // 解析角色名和对话内容（中文冒号分隔）
                foreach (char c in curText)
                {
                    if (_inshop)
                    {
                        dialogueContent += c;
                        continue;
                    }
                    if (c == '：')
                        charaOver = true;
                    else if (!charaOver)
                        charaName += c;
                    else
                        dialogueContent += c;
                }
                if (addtiontext != null)
                {
                    dialogueContent = addtiontext + dialogueContent;
                }

                


                if (!_inshop && !inTech && !MiniMode)
                { _ = ShowCharacter(charaName); }
                Chara_Name.text = charaName;

                foreach (string e in Expressions)
                {

                    if (dialogueContent.Contains('{' + $"{e}" + '}'))
                    {
                        dialogueContent = dialogueContent.Replace('{' + $"{e}" + '}', string.Empty);
                        SwitchExpression(charaName, e);
                    }


                }

                on = false;
                _IsShowingText = true;
                // 逐字显示对话
                foreach (char c in dialogueContent)
                {

                    if (BreakText)
                    {
                        PlusLine();
                        if(!_inshop)
                        {
                            if (MiniMode)
                            {
                                MiniCharacterManager.ShowText(charaName, dialogueContent);
                                
                                
                            }
                            else
                            {
                                Character.text = dialogueContent;
                                Chara_Name.text = charaName;
                            }
                        }
                            
                        else
                        { 
                            ShopTextBar.GetComponent<TextMeshProUGUI>().text = dialogueContent;
                            if (ShowName)
                                ShopName.text = "商人";
                        }
                        _IsShowingText = false;
                        on = true;
                        BreakText = false;
                        return;
                    }

                    await Task.Delay(TextSpeedI);
                    Type.Play();
                    if (_inshop)
                    {
                        ShopTextBar.GetComponent<TextMeshProUGUI>().text += c;
                        if (ShowName)
                            ShopName.text = "商人";
                        else ShopName.text = "";
                    }
                    else
                    {
                        if (MiniMode)
                        {
                            Character.text += c;
                            MiniCharacterManager.ShowText(charaName, Character.text);

                            
                        }
                        else
                        {
                            Chara_Name.text = charaName;
                            Character.text += c;
                        }
                    }
                    
                }
                if (dialogueContent == "你没事吧，■■？别太难过了。")
                {
                    on = false;
                    await Task.Delay(500);
                    PlusLine();
                    _ = ShowText(true,ClearText:false);
                    Invoke("SetOn", 0.5f);
                }
                Type.Stop();
                on = true;
                BreakText = false;
                _IsShowingText = false;
            }
            // 处理玩家说话
            else
            {
                CanSkip = true;
                Player.text = "";
                if (curText.Contains("："))
                {
                    line--;
                    _ = ShowText();
                    return;
                }
                on = false;
                _IsShowingText = true;


                //如果句尾没有。加。
                if (!string.IsNullOrEmpty(curText))
                {
                    // 获取最后一个字符（只计算一次，提升效率）
                    char lastChar = curText[curText.Length - 1];

                    // 定义不需要补句号的结尾字符集合
                    HashSet<char> endChars = new HashSet<char> { '。', '…', '?', '!', '？', '}', '{' };

                    // 如果最后一个字符不在集合中，则补句号
                    if (!endChars.Contains(lastChar))
                    {
                        curText += '。';
                    }
                }

                foreach (char c in curText)
                {
                    if (BreakText)
                    {
                        PlusLine();
                        if (!_inshop) Player.text = curText;
                        else ShopTextBar.GetComponent<TextMeshProUGUI>().text = curText;
                        if(ShowName) PlayerNameText.text = PlayerName.TxtLine[0];
                        else PlayerNameText.text = "";
                        _IsShowingText = false;
                        on = true;
                        BreakText = false;
                        return;
                    }
                    await Task.Delay(TextSpeedI);
                    Type.Play();
                    if (_inshop)
                    {
                        ShopTextBar.GetComponent<TextMeshProUGUI>().text += c;
                        if (ShowName)
                            ShopName.text = PlayerName.TxtLine[0];
                        else ShopName.text = "";
                    }
                    else
                    {
                        Player.text += c;
                        if (ShowName) PlayerNameText.text = PlayerName.TxtLine[0];
                        else PlayerNameText.text = "";
                    }
                    
                }
                Type.Stop();
                on = true;
                BreakText = false;
                _IsShowingText = false;
            }

            // 标记完成并准备下一行
            
            PlusLine();
        }
        finally
        {
            
            
        }
    }


    /// <summary>
    /// 处理选择分支逻辑
    /// </summary>
    private void HandleChoice(bool _isSpecial = false,int ban = 0)
    {

        if (!_inshop)//正常状态
        // 设置按钮文本（显示选项内容）
        {
            UpButton.GetComponent<Image>().color = Color.white;
            UpButton.GetComponent<Button>().enabled = true;
            DownButton.GetComponent<Image>().color = Color.white;
            DownButton.GetComponent<Button>().enabled = true;
            // 显示选择按钮
            PlayerTalkBackGround.transform.parent.gameObject.SetActive(false);
            UpButton.gameObject.SetActive(true);
            DownButton.gameObject.SetActive(true);
            UpButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = Talklines[Daytime].Option1.TxtLine[0];
            DownButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = Talklines[Daytime].Option2.TxtLine[0];
            // 绑定选项数据到按钮（选择后切换对话分支）
            UpButton.GetComponent<ButtonSelect>().textbox = Talklines[Daytime].Option1;

            DownButton.GetComponent<ButtonSelect>().textbox = Talklines[Daytime].Option2;
            if (ban == 1)
            {
                UpButton.GetComponent<Image>().color = Color.gray;
                UpButton.GetComponent<Button>().enabled = false;
            }
            else if (ban == 2)
            {
                DownButton.GetComponent<Image>().color = Color.gray;
                DownButton.GetComponent<Button>().enabled = false;
            }
        }
        else//商店状态
        {
            ShopRButton.gameObject.SetActive(true);
            ShopLButton.gameObject.SetActive(true);
            ShopLButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = Talklines[Daytime].Option1.TxtLine[0];
            ShopRButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = Talklines[Daytime].Option2.TxtLine[0];
            ShopLButton.GetComponent<ButtonSelect>().textbox = Talklines[Daytime].Option1;
            ShopRButton.GetComponent<ButtonSelect>().textbox = Talklines[Daytime].Option2;
            ShopName.text = "";
            if (_isSpecial)
            {
                ShopMButton.gameObject.SetActive(true);
                ShopMButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = Talklines[Daytime].Option2.TxtLine[0];
                ShopMButton.GetComponent<ButtonSelect>().textbox = Talklines[Daytime].Option2;
                ShopRButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = Talklines[Daytime].Option3.TxtLine[0];
                ShopRButton.GetComponent<ButtonSelect>().textbox = Talklines[Daytime].Option3;


            }
            if(ShopLButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text == "换尸体" && DeadName.TxtLine.Count == 0)
            {
                ShopLButton.GetComponent<Button>().enabled = false;
                ShopLButton.GetComponent<Image>().color = Color.gray;
            }


        }
        // 清空玩家文本框
        Player.text = "";
        if (_inshop)
        {
            ShopTextBar.GetComponent<TextMeshProUGUI>().text = "";
        }

        // 暂时关闭交互（等待玩家选择按钮）
        on = false;

    }

    /// <summary>
    /// 选择分支后的回调方法（由选择按钮调用）
    /// </summary>
    /// <param name="linebox">选择的对话分支数据</param>
    public void Setchoice(Manager linebox)
    {

        // 重置行数到0（从分支的第一行开始显示）
        ResetLine();
        // 隐藏选择按钮
        if (!_inshop)
        {
            PlayerTalkBackGround.transform.parent.gameObject.SetActive(true);
            UpButton.gameObject.SetActive(false);
            DownButton.gameObject.SetActive(false);

        }
        else
        {
            ShopLButton.gameObject.SetActive(false);
            ShopRButton.gameObject.SetActive(false);
            if (ShopMButton.gameObject.activeSelf)
            {
                ShopMButton.gameObject.SetActive(false);
            }
        }
        // 切换当前天的对话数据为选择的分支
        Talklines[Daytime] = linebox;
        PlusLine();
        // 重新开启交互
        on = true;
        // 开始显示选择分支的对话
        _ = ShowText(true);
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


        if (Daytime == 2)
        {
            Day2ShopEvent.GeneralBool = true;
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
            CharacterList[4].transform.Find("Attention").gameObject.SetActive(true);
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

        if (Daytime == 2)
        {
            Day2ShopEvent.GeneralBool = true;
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
            CharacterList[4].transform.Find("Attention").gameObject.SetActive(true);
            CharacterList[5].GetComponent<Character>().Special2 = true;
            CharacterList[5].GetComponent<Character>().EnableTalk();
            CharacterList[5].transform.Find("Attention").gameObject.SetActive(true);
            if (DeadName.TxtLine[DeadName.TxtLine.Count - 1] == "博金森")
            {
                Debug.Log("第一次商店杀死博金森");
                CharacterList[1].GetComponent<Character>().Special2 = true;
                
                CharacterList[1].GetComponent<Character>().EnableTalk();
                CharacterList[1].transform.Find("Attention").gameObject.SetActive(true);

            }
            if (DeadName.TxtLine[DeadName.TxtLine.Count - 1] == "艾米莉")
            {
                Debug.Log("第一次商店杀死艾米莉");
                CharacterList[3].GetComponent<Character>().Special2 = true;
                CharacterList[3].GetComponent<Character>().NotComfort = true;
                CharacterList[3].GetComponent<Character>().EnableTalk();
                CharacterList[3].transform.Find("Attention").gameObject.SetActive(true);

            }



        }
        foreach (GameObject g in CharacterList)
        {
            if (!g.GetComponent<Character>().Special1 && !g.GetComponent<Character>().Special2)
            g.GetComponent<Character>().DisableTalk();
        }

    }




    /// <summary>
    /// 显示角色立绘（需根据项目需求实现具体逻辑）
    /// </summary>
    /// <param name="name">角色名称</param>
    async Task ShowCharacter(string name)
    {
        await Task.Delay(100);
        
        this.CharacterImageManager.SetImage(name);

    }


    public void ShowBar()
    {

        anim.SetTrigger("up");



    }

    

    private void SetCharBar()
    {
        charabar.SetActive(true);

    }


    void TurnOption1()
    {
        Talklines[Daytime] = Talklines[Daytime].Option1;
    }
    
    void TurnOption2()
    {
        Talklines[Daytime] = Talklines[Daytime].Option2;
    }

    void TurnOption3()
    {
         Talklines[Daytime] = Talklines[Daytime].Option3;
    }

    void ResetLine()
    {
         line = 0;
    }

    void PlusLine()
    {
        line++;
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
        mask.transform.parent.gameObject.SetActive(true);
        mask.GetComponent<Unmask>().m_FitTarget = MaskGameObj.GetComponent<RectTransform>();
        mask.transform.parent.Find("TechText").GetComponent<TextMeshProUGUI>().text = TechText;


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




}