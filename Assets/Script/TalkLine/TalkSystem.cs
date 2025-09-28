using Coffee.UIExtensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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
    public GameObject charabar, black, MainCanvas, MainCTalkBar;
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


    private void Awake()
    {
        Daytime = DaytimeOBJ.GetComponent<Progress>().day_num;
        PlayerNameText.text = PlayerName.TxtLine[0];
        if (Daytime == 0)
        {
            on = false;
            Invoke("SetStartTalk", 1.5f);

        }
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
            if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
            // 点击条件：鼠标左键按下 + 交互开启（on） + 超过点击间隔
            {
                if ( on && timeSinceLastClick >= ClickInterval && !_IsShowingText)
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
            Character.text = "";
            Chara_Name.text = "";
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
                case "/CheckAmandeKillself"://死了转分支1,没死分支2
                    if(amandeKillself.GeneralBool)
                    {
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
                case "/CheckShopEvent":

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

                case "/CheckEveryOneLive":
                    foreach(GameObject g in CharacterList)
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
                case "/CheckBoBody":
                    foreach(string s in DeadName.TxtLine)
                    {
                        if(s == "博金森")
                        {
                            HandleChoice();

                            return;
                        }
                        else if(s == "博金森Used")
                        {
                            HandleChoice(ban:1);
                            return;
                        }
                        
                    }
                    return;
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
                    await Task.Delay(200);
                    ShopTextBar.transform.parent.gameObject.GetComponent<Animator>().SetTrigger("Down");
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
                        await Task.Delay(200);
                        ShopTextBar.transform.parent.gameObject.GetComponent<Animator>().SetTrigger("Down");


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
                        SetTechMode(ComfortObj,Comforttext);
                        TeachComfort.GeneralBool = true;
                    }
                    PlusLine();
                    
                    return;
                case "techmenu":
                    
                    string Menutext = SetTechText("Menu");
                    SetTechMode(Menu, Menutext);
                    on = true;
                    PlusLine();
                    
                    return;
                case "techclik":

                    string Cliktext = SetTechText("Clik");
                    SetTechMode(gameObject, Cliktext);
                    PlusLine();
                    
                    
                    return;

                case "techtalk":
                    string Talktext = SetTechText("Talk");
                    SetTechMode(DownBar.transform.Find("MaskLayer").gameObject,Talktext);
                    on = true;
                    PlusLine();
                    DaytimeOBJ.GetComponent<Button>().enabled = true;
                    
                    return;

                case "techright":
                    string Righttext = SetTechText("Right");
                    SetTechMode(DaytimeOBJ.transform.parent.gameObject, Righttext);
                    on = true;
                    DaytimeOBJ.GetComponent<Button>().enabled = false;
                    PlusLine();
                    
                    return;

                case "techclose":
                    string Closetext = SetTechText("Close");
                    SetTechMode(DaytimeOBJ, Closetext);
                    PlusLine();
                    _ = ShowText(true);
                    return;
                case "techfood":
                    string Foodtext = SetTechText("Food");
                    inTech = true;
                    SetTechMode(DownBar.transform.Find("MaskLayer").gameObject, Foodtext);
                    PlusLine();
                    Invoke("CloseMask", 2);
                    _ = ShowText(true);
                    return;

                case "techweight":
                    string Weighttext = SetTechText("Weight");
                    SetTechMode(DownBar.transform.Find("MaskLayer").gameObject, Weighttext);
                    PlusLine();
                    Invoke("CloseMask", 3);
                    _ = ShowText(true);
                    return;
                //教程结束
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
                    if(Daytime - BoDeadTime.Weight > 1)
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



                case "luo_peoplechoice":
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
                    anim.SetTrigger("down");
                    Invoke("SetCharBar", 1.5f);
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

                    
                case "upcbar":
                    charaBar.GetComponent<Animator>().SetTrigger("Up");
                    PlusLine();
                    if(!inTech)this.CharacterImageManager.CloseImage();
                    
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
                    black.SetActive(false);
                    black.SetActive(true);
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
                        MainCTalkBar.SetActive(false);
                        transform.parent.gameObject.SetActive(false);
                        ResetLine();
                    }
                    DaytimeOBJ.GetComponent<Progress>().SwtichProgress();
                    return;
                case "twicedeadchoice":
                    if ((DeadName.TxtLine.Count == 1 && DeadName.TxtLine[0] == "Leader" && UsedBody.TxtLine.Count == 0)|| 
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

                case "lock":
                    DaytimeOBJ.GetComponent<Progress>().CanSwitch = false;
                    PlusLine();

                    _ = ShowText(true);
                    return;

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
            }

            // 处理正常对话文本（角色说话）
            if (!PlayerTalking)
            {
                CanSkip = true;
                bool charaOver = false;
                string charaName = "";
                string dialogueContent = "";
                if (curText.Contains("{PlayerName}"))
                {
                    curText = curText.Replace("{PlayerName}", PlayerName.TxtLine[0]);
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


                
                if (!_inshop && !inTech)
                { _ = ShowCharacter(charaName); }
                Chara_Name.text = charaName;


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
                            Character.text = dialogueContent;
                            Chara_Name.text = charaName;
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
                    if (_inshop)
                    {
                        ShopTextBar.GetComponent<TextMeshProUGUI>().text += c;
                        if (ShowName)
                            ShopName.text = "商人";
                        else ShopName.text = "";
                    }
                    else
                    {
                        Chara_Name.text = charaName;
                        Character.text += c;
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
                on = true;
                BreakText = false;
                _IsShowingText = false;
            }
            // 处理玩家说话
            else
            {
                CanSkip = true;
                
                if (curText.Contains("："))
                {
                    line--;
                    _ = ShowText();
                    return;
                }
                on = false;
                _IsShowingText = true;
                
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
        
        foreach (GameObject g in CharacterList)
        {
            g.GetComponent<Character>().have_talk = true;
        }
        if (Daytime == 2)
        {
            Day2ShopEvent.GeneralBool = true;
            amandeKillself.GeneralBool = true;
            if (CharacterList[4].GetComponent<Character>().Special2) return;

            CharacterList[4].GetComponent<Character>().Special1 = true;
            CharacterList[4].transform.Find("Attention").gameObject.SetActive(true);
        }





    }


    public void ShowKillTalk()
    {


        foreach (GameObject g in CharacterList)
        {
            g.GetComponent<Character>().have_talk = true;
        }
        if (Daytime == 2)
        {
            Day2ShopEvent.GeneralBool = true;
            amandeKillself.GeneralBool = true;
            CharacterList[4].GetComponent<Character>().Special2 = true;
            CharacterList[4].transform.Find("Attention").gameObject.SetActive(true);
            CharacterList[5].GetComponent<Character>().Special2 = true;
            CharacterList[5].transform.Find("Attention").gameObject.SetActive(true);
            if (DeadName.TxtLine[DeadName.TxtLine.Count - 1] == "博金森")
            {
                Debug.Log("第一次商店杀死博金森");
                CharacterList[1].GetComponent<Character>().Special2 = true;
                CharacterList[1].transform.Find("Attention").gameObject.SetActive(true);

            }
            if (DeadName.TxtLine[DeadName.TxtLine.Count - 1] == "艾米莉")
            {
                Debug.Log("第一次商店杀死艾米莉");
                CharacterList[3].GetComponent<Character>().Special2 = true;
                CharacterList[3].transform.Find("Attention").gameObject.SetActive(true);

            }



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

    private void CloseMask()
    {
        mask.transform.parent.gameObject.SetActive(false);

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

}