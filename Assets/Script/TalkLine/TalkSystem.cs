using Coffee.UIExtensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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
    public Manager DeadName;
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
    public Button Lbutton;
    [Tooltip("选择分支时的右按钮")]
    public Button Rbutton;
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
    public Manager Day0_Talk;

    // 用于取消异步文本显示任务的令牌源
    private CancellationTokenSource _cts;
    // 标记当前是否正在显示文本
    public bool _isShowingText = false;
    // 存储当前正在显示的完整文本（用于中途点击显示剩余内容）
    private string _currentFullDialogue = "";
    // 标记是否已经显示完整文本（避免重复处理）
    private bool _isTextFullyDisplayed = false;

    private bool ClikDelay;
    private bool CanSkip;

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
        Lbutton.gameObject.SetActive(false);
        Rbutton.gameObject.SetActive(false);
        // 从第0行开始显示对话
        line = 0;
        // 从进度管理器获取当前天数
        Daytime = DaytimeOBJ.GetComponent<Progress>().day_num;
        if (on && DaytimeOBJ.GetComponent<Progress>().talk)
        {
            // 隐藏选择按钮（点击文本时关闭选择界面）
            Lbutton.gameObject.SetActive(false);
            Rbutton.gameObject.SetActive(false);

           
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
            if (Input.GetKeyDown(KeyCode.Mouse0) && on && !ClikDelay)
            {

                ClikDelay = true;
                Lbutton.gameObject.SetActive(false);
                Rbutton.gameObject.SetActive(false);

                

                // 仅当不在显示文本时，才开始新的文本显示
                if (!_isShowingText)
                {
                    try
                    {
                        
                        _ = ShowText();
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        //line -= 1;
                        return;
                    }
                    catch (NullReferenceException)
                    {
                        return;
                    }
                }
            }
            else if (ClikDelay && CanSkip && !_isTextFullyDisplayed)
            {
                ShowRemainingText();
                return;
            }
        }
    }

    /// <summary>
    /// 立即显示当前文本的剩余部分
    /// </summary>
    private void ShowRemainingText()
    {
        if (string.IsNullOrEmpty(_currentFullDialogue)) return;

        

        // 强制更新文本显示
        if (!PlayerTalking)
        {
            Character.text = _currentFullDialogue;
            
        }
        else
        {
            Player.text = _currentFullDialogue;
            
        }

        // 立即更新状态，允许下一次交互
        _isTextFullyDisplayed = true;
        _isShowingText = false; // 手动标记为已完成，避免状态延迟
        ClikDelay = false;
        CanSkip = false;
    }

    /// <summary>
    /// 异步显示文本（支持逐字显示和中途取消）
    /// </summary>
    public async Task ShowText(bool isroll = false, string addtiontext = null)
    {
        if (isroll) _isShowingText = false;
        if (_isShowingText) return;


        _isShowingText = true;
        _isTextFullyDisplayed = false;
        _currentFullDialogue = "";

        // 清空文本框（原有逻辑保留）
        if (!PlayerTalking)
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
            switch (curText)
            {
                case "choice":
                    
                    HandleChoice();
                    

                    return;
                case "/Laiwensuccess"://安抚成功/失败预留
                    CharacterList[5].GetComponent<Character>().Comfort = true;
                    line++;
                    _ = ShowText(true);
                    return;
                case "/Laiwenfail":
                    CharacterList[5].GetComponent<Character>().Comfort = false;
                    line++;
                    _ = ShowText(true);
                    return;
                case "/checktalk":
                    if (Day0_Talk.Weight == 3)
                    {
                        line++;
                        _ = ShowText(true);
                        return;
                    }
                    else
                    {
                        Talklines[Daytime] = Talklines[Daytime].Option3;
                        line = 0;
                        _ = ShowText(true);
                        return;
                    }


                case "/specialchoice":

                    HandleChoice(true);
                    _isShowingText = false;

                    return;
                case "/showname":
                    ShowName = true;
                    line++;
                    _ = ShowText(true);
                    return;
                case "/noname":
                    ShowName = false;
                    line++;
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
                    bool notmultiple = true;
                    Day2_Shop_Exchange.GeneralBool = true;
                    ShowExchangeTalk();
                    notmultiple = ShopManager.GetComponent<ShopManager>().ExchangeFood();
                    if (!notmultiple)
                    {
                        ShopCharaBar.SetActive(true);
                        ShopCharaBar.GetComponent<Animator>().SetTrigger("Up");
                        ShopCharaBar.GetComponent<ShopCharacterManager>().SelectBody();
                        on = false;
                        await Task.Delay(200);
                        ShopTextBar.transform.parent.gameObject.GetComponent<Animator>().SetTrigger("Down");


                    }
                    else
                    {

                        _isShowingText = false;
                        line++;
                        _ = ShowText(true);

                    }


                    return;
                case "/closeshop":
                    ShopManager.SetActive(false);
                    line = 0;
                    _inshop = false;
                    return;

                case "/shop":

                    _inshop = true;
                    line++;

                    _ = ShowText(true);
                    return;

                case "techtalk":
                    mask.transform.parent.gameObject.SetActive(true);
                    mask.GetComponent<Unmask>().m_FitTarget = DownBar.transform.Find("MaskLayer").GetComponent<RectTransform>();
                    mask.transform.parent.Find("TechText").GetComponent<TextMeshProUGUI>().text = "点击人物头像开始对话";
                    line++;
                    _ = ShowText(true);
                    return;

                case "techclose":
                    mask.transform.parent.gameObject.SetActive(true);
                    mask.GetComponent<Unmask>().m_FitTarget = DaytimeOBJ.GetComponent<RectTransform>();
                    mask.transform.parent.Find("TechText").GetComponent<TextMeshProUGUI>().text = "点击按钮结束交谈环节";
                    line++;
                    _ = ShowText(true);
                    return;
                case "techfood":
                    mask.transform.parent.gameObject.SetActive(true);
                    mask.GetComponent<Unmask>().m_FitTarget = DownBar.transform.Find("MaskLayer").GetComponent<RectTransform>();
                    mask.transform.parent.Find("TechText").GetComponent<TextMeshProUGUI>().text = "请勾选所有人物头像旁的按钮分配食物";
                    line++;
                    Invoke("CloseMask", 2);
                    _ = ShowText(true);
                    return;

                case "techweight":
                    mask.transform.parent.gameObject.SetActive(true);
                    mask.GetComponent<Unmask>().m_FitTarget = DownBar.transform.Find("MaskLayer").GetComponent<RectTransform>();
                    mask.transform.parent.Find("TechText").GetComponent<TextMeshProUGUI>().text = "选择右侧的物品然后点击人物头像分配携带物品";
                    line++;
                    Invoke("CloseMask", 3);
                    _ = ShowText(true);
                    return;

                case "deadfu":
                    bool hasBokinson = false;
                    foreach (string s in DeadName.TxtLine)
                    {
                        if (s == "博金森")
                        {
                            hasBokinson = true;
                            break;
                        }
                    }
                    Talklines[Daytime] = hasBokinson ? Talklines[Daytime].Option2 : Talklines[Daytime].Option1;
                    line = 0;

                    _ = ShowText(true);
                    return;

                case "luo_peoplechoice":
                    if (DeadName.TxtLine.Count == 1)
                    {
                        string name = DeadName.TxtLine[0];
                        if (name == "莱文" || name == "博金森")
                            Talklines[Daytime] = Talklines[Daytime].Option1;
                        else if (name == "艾米莉" || name == "阿曼德")
                            Talklines[Daytime] = Talklines[Daytime].Option2;
                    }
                    else
                    {
                        Talklines[Daytime] = Talklines[Daytime].Option3;
                    }
                    line = 0;

                    _ = ShowText(true);
                    return;

                case "showdeadname":
                    string showtext = string.Concat(DeadName.TxtLine); // 简化字符串拼接
                    line += 1;

                    _ = ShowText(true, showtext);
                    return;

                case "dead":
                    Talklines[Daytime] = DeadName.TxtLine.Count != 0
                        ? Talklines[Daytime].Option1
                        : Talklines[Daytime].Option2;

                    _ = ShowText(true);
                    return;

                case "down":
                    anim.SetTrigger("down");
                    Invoke("SetCharBar", 1.5f);
                    line++;
                    this.CharacterImageManager.CloseImage();
                    _ = ShowText(true);
                    return;
                case "aimieat":
                    if (aimi.Day1Eat)
                    {
                        Talklines[Daytime] = Talklines[Daytime].Option1;
                    }
                    else
                    {
                        Talklines[Daytime] = Talklines[Daytime].Option2;
                    }
                    line = 0;

                    _ = ShowText(true);
                    return;
                case "up":
                    anim.SetTrigger("up");
                    line++;
                    transform.position = new Vector2(transform.position.x, -5000); // 简化gameObject访问

                    _ = ShowText(true);
                    return;

                case "upcbar":
                    charaBar.GetComponent<Animator>().SetTrigger("Up");
                    line++;
                    this.CharacterImageManager.CloseImage();
                    await Task.Delay(500);

                    _ = ShowText(true);
                    DaytimeOBJ.GetComponent<Progress>().CanSwitch = true;
                    return;

                case "downcbar":
                    charaBar.GetComponent<Animator>().SetTrigger("Down");
                    line++;

                    _ = ShowText(true);
                    return;

                case "notover":
                    amande.GetComponent<Character>().have_talk = false;
                    line++;
                    DaytimeOBJ.GetComponent<Progress>().CanSwitch = false;

                    _ = ShowText(true);
                    return;

                case "allover":
                    DaytimeOBJ.GetComponent<Progress>().CanSwitch = true;
                    line++;
                    ShowName = true;
                    _ = ShowText(true);
                    return;
                
                case "showcharabar":
                    charabar.SetActive(true);
                    line++;

                    Debug.Log("关闭角色文本框");
                    _ = ShowText(true);
                    return;

                case "closecharabar":
                    charabar.SetActive(false);
                    line++;
                    this.CharacterImageManager.CloseImage();
                    Debug.Log("关闭角色文本框");
                    _ = ShowText(true);
                    return;

                case "black":
                    black.SetActive(false);
                    black.SetActive(true);
                    line++;

                    _ = ShowText(true);
                    return;

                case "next":
                    Debug.Log("进入正常场景");
                    line++;

                    _ = ShowText(true);
                    if (Daytime == 0)
                    {
                        MainCanvas.SetActive(true);
                        MainCTalkBar.SetActive(false);
                        transform.parent.gameObject.SetActive(false);
                        line = 0;
                    }
                    DaytimeOBJ.GetComponent<Progress>().SwtichProgress();
                    return;
                case "twicedeadchoice":
                    if (DeadName.TxtLine.Count == 0)
                    {
                        Talklines[Daytime] = Talklines[Daytime].Option1;
                    }
                    else
                    {
                        Talklines[Daytime] = Talklines[Daytime].Option2;
                    }
                    line = 0;

                    _ = ShowText(true);
                    return;

                case "lock":
                    DaytimeOBJ.GetComponent<Progress>().CanSwitch = false;
                    line++;

                    _ = ShowText(true);
                    return;

                case "canskip":
                    DaytimeOBJ.GetComponent<Progress>().can_skip = true;
                    line++;

                    _ = ShowText(true);
                    return;

                case "p":
                    PlayerTalking = true;
                    line++;
                    await Task.Yield();
                    _ = ShowText(true);
                    return;

                case "c":
                    PlayerTalking = false;
                    line++;
                    await Task.Yield();
                    _ = ShowText(true);
                    return;

                case "end":
                    Talklines[Daytime] = Talklines[Daytime].Option1;
                    line = 0;

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


                _currentFullDialogue = dialogueContent; // 新增：统一赋值，确保ShowRemainingText能读取

                _ = ShowCharacter(charaName);
                Chara_Name.text = charaName;

                // 逐字显示对话
                foreach (char c in dialogueContent)
                {
                    

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
            }
            // 处理玩家说话
            else
            {
                CanSkip = true;
                _currentFullDialogue = curText; // 新增：统一赋值
                if (curText.Contains("："))
                {
                    line--;
                    _ = ShowText();
                    return;
                }
                foreach (char c in curText)
                {
                    

                    if (_inshop)
                    {
                        ShopTextBar.GetComponent<TextMeshProUGUI>().text += c;
                        if (ShowName)
                            ShopName.text = "■■";
                        else ShopName.text = "";
                    }
                    else
                    {
                        Player.text += c;
                        PlayerNameText.text = "■■";
                    }
                    
                }
            }

            // 标记完成并准备下一行
            _isTextFullyDisplayed = true;
            line++;
        }
        finally
        {
            _isShowingText = false;
            ClikDelay = false;
        }
    }


    /// <summary>
    /// 处理选择分支逻辑
    /// </summary>
    private void HandleChoice(bool _isSpecial = false)
    {

        if (!_inshop)//正常状态
        // 设置按钮文本（显示选项内容）
        {
            // 显示选择按钮
            PlayerTalkBackGround.transform.parent.gameObject.SetActive(false);
            Lbutton.gameObject.SetActive(true);
            Rbutton.gameObject.SetActive(true);
            Lbutton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = Talklines[Daytime].Option1.TxtLine[0];
            Rbutton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = Talklines[Daytime].Option2.TxtLine[0];
            // 绑定选项数据到按钮（选择后切换对话分支）
            Lbutton.GetComponent<ButtonSelect>().textbox = Talklines[Daytime].Option1;

            Rbutton.GetComponent<ButtonSelect>().textbox = Talklines[Daytime].Option2;
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
        line = 0;
        // 隐藏选择按钮
        if (!_inshop)
        {
            PlayerTalkBackGround.transform.parent.gameObject.SetActive(true);
            Lbutton.gameObject.SetActive(false);
            Rbutton.gameObject.SetActive(false);

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
        line++;
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
        _inshop = false;
        foreach (GameObject g in CharacterList)
        {
            g.GetComponent<Character>().have_talk = true;
        }
        if (Daytime == 2)
        {
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
        Debug.Log($"显示{name}立绘");
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

}