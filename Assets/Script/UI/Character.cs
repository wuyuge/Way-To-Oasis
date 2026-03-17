using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色管理器脚本
/// 负责单个角色的核心逻辑：负重管理（食物/尸体分配）、UI状态刷新、对话触发、死亡状态控制等
/// </summary>
public class Character : MonoBehaviour
{
    
    /// <summary>
    /// 角色本地数据管理
    /// weight：当前角色的负重值（0-3，对应3个负重进度条）
    /// food：角色持有的食物数量
    /// body：角色持有的尸体数量
    /// </summary>
    public Manager weight, food, body;
    [Header("保存角色名称")]
    public string CharacterName;
    /// <summary>
    /// 负重进度条UI（3个进度条分别对应1/2/3格负重）
    /// weight1：第1格负重条
    /// weight2：第2格负重条
    /// weight3：第3格负重条
    /// end：游戏结束/进度管理根对象（用于获取当前天数）
    /// </summary>
    public GameObject weight1, weight2, weight3, end;

    /// <summary>
    /// 当前天数记录（用于判断是否进入下一天，触发负重重置）
    /// </summary>
    public int curr_num;

    [Header("UI - 角色相关UI组件")]
    // 切换状态标记UI（可能用于标记角色是否被选中/处于特殊状态）
    public GameObject toggle;

    [Header("状态 - 关联游戏进度管理器")]
    // 游戏进度管理对象（Progress脚本挂载对象，用于判断当前游戏阶段）
    public Progress progress;

    [Header("对话列表 - 角色专属对话数据")]
    // 角色按天数对应的对话列表（索引对应天数，存储每天的对话数据）
    public List<Manager> textline = new List<Manager>();

    [Tooltip("挂载的对话Bar - 角色触发对话时使用的对话面板")]
    // 对话面板对象（挂载TalkSystem脚本，用于显示角色对话）
    public GameObject TalkBar;

    /// <summary>
    /// 对话状态标记（true：已触发过对话；false：未触发对话）
    /// </summary>
    public bool have_talk = false;

    /// <summary>
    /// 角色死亡状态标记（true：角色死亡，所有功能失效；false：角色存活，功能正常）
    /// </summary>
    public bool Dead;

    public Texture2D DeadImage;

    /// <summary>
    /// 角色进食状态标记（预留，暂未在现有逻辑中使用）
    /// </summary>
    public bool eat;
    public float Delay = 1;
    public GameObject Mask;
    [Tooltip("安抚提示")]
    public GameObject Attention,Attention2;
    [Header("安抚对话控制")]
    public bool Special1,Special2,AfterSpecialTalk;
    public bool NotComfort;//用于判断安抚是否成功
    public bool ClikDelay = false;
    [Header("Day0用")]
    public Manager Day0_Talk;

    public GameObject Background;
    /// <summary>
    /// 用于商店后限制对话触发
    /// </summary>

    public bool CanTalk = true;


    /// <summary>
    /// 是否无法负重（true：无法负重，禁用负重操作；false：可正常负重）
    /// </summary>

    public bool CantWeight = false;

    public List<GameObject> Child;
    public List<GameObject> CharacterList;

    public bool ShowInfo = false;
    public bool Have_ShowInfo = false;

    private int InfoClick = 0;

    private Image _weight1Image,_weight2Image,_weight3Image;


    private void Awake()
    {
        Attention = gameObject.transform.Find("Attention").gameObject;
        Attention2 = gameObject.transform.Find("Attention2").gameObject;
        //在初始加载且不是重新加载存档的场景刷新负重等状态
        if (!GameObject.Find("SaveManager").GetComponent<SaveManager>().reload.GeneralBool)
        {
            weight.Weight = 0;
            weight.Weight_tag = 0;
            weight.Day1Eat = false;
            
            
        }
    }



    /// <summary>
    /// 初始化方法 - 游戏启动时执行
    /// 1. 绑定UI组件 2. 初始化角色数据 3. 刷新初始UI显示
    /// </summary>
    void Start()
    {
        Delay = 1;
        // 绑定角色子对象中的ToggleUI（状态标记）
        toggle = transform.Find("Toggle").gameObject;
        // 查找全局的进度管理根对象（End）
        end = GameObject.Find("End");
        // 记录初始天数（与Progress中的day_num同步）
        curr_num = end.GetComponent<Progress>().day_num;
        // 绑定角色子对象中的3个负重进度条UI
        weight1 = gameObject.transform.Find("Weight3").gameObject;
        weight2 = gameObject.transform.Find("Weight2").gameObject;
        weight3 = gameObject.transform.Find("Weight1").gameObject;
        _weight1Image = weight1.GetComponent<Image>();
        _weight2Image = weight2.GetComponent<Image>();
        _weight3Image = weight3.GetComponent<Image>();
        if(end.GetComponent<Progress>().day_num != 0)
        Background = GameObject.Find("BackgroundContainer").gameObject;
        for (int i = 0; i < transform.childCount;i++)
        {
            Child.Add(transform.GetChild(i).gameObject);
        }
        CharacterList = gameObject.transform.parent.gameObject.GetComponent<ObjectManager>().Character_List;
        StartRestWeightImage();

        Attention2.SetActive(false);
        Attention.SetActive(false);



    }

    // Update is called once per frame
    /// <summary>
    /// 帧更新方法 - 每帧执行
    /// 1. 控制角色按钮启用/禁用状态 2. 检测是否进入下一天，触发负重重置
    /// </summary>
    void Update()
    {
        if (Dead && CharacterName == "主角") return;
        // 【死亡状态判断】如果角色死亡，不执行任何Update逻辑
        if (Dead) 
        {
            Sprite NewSprite = Sprite.Create(DeadImage, new Rect(0, 0, DeadImage.width, DeadImage.height),new Vector2(0.5f,0.5f));
            gameObject.GetComponent<Image>().sprite = NewSprite;
            foreach (GameObject child in Child)
            {
                child.SetActive(false);
            }
            return; 
        }
        
        if((Special1 || Special2) && !AfterSpecialTalk) Attention.SetActive(true);
        
        
        if(NotComfort && progress.day_num == 2)
        {
            Attention2.SetActive(true);
        }
        else if (AfterSpecialTalk && !NotComfort)
        {
            Attention2.SetActive(false);
            
        }
        else if (progress.day_num != 0)
        {
            Attention2.SetActive(false);
        }
        if(progress.talk && CharacterName == "主角")
        {
            gameObject.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            gameObject.GetComponent<Image>().color = Color.white;
        }


        // 获取资源选择面板（SelectBar）的状态，判断是否处于"食物选择"模式
        if (!progress.food)
        {
            bool InfoOn = false;
            // 非食物选择模式：启用角色按钮，隐藏ToggleUI
            foreach (GameObject g in CharacterList)
            {
                if (g.GetComponent<Character>().ShowInfo)
                {
                    InfoOn = true;
                    break;
                }
            }
            if (!InfoOn) 
            gameObject.GetComponent<Button>().enabled = true;
            else gameObject.GetComponent<Button>().enabled = false;
            toggle.SetActive(false);
        }
        else
        {
            // 食物选择模式：禁用角色按钮，显示ToggleUI（标记当前模式）
            toggle.SetActive(true);
            gameObject.GetComponent<Button>().enabled = false;
        }

        // 检测是否进入下一天（当前记录的天数与Progress中的天数不一致）
        if (curr_num != end.GetComponent<Progress>().day_num)
        {
            // 重置角色负重为0（新的一天初始无负重）
            weight.Weight = 0;
            // 更新当前天数记录（与Progress同步）
            curr_num = end.GetComponent<Progress>().day_num;
            // 刷新负重进度条UI（重置为初始白色）
            Refresh();
        }
        if (CantWeight)
        {
            weight.Weight = 3;
            _weight1Image.color = new Color32(95,47,54,255);;
            _weight2Image.color = new Color32(95,47,54,255);;
            _weight3Image.color = new Color32(95,47,54,255);;
        }

        


        //关闭角色资料逻辑
        if(ShowInfo && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space)) && !Have_ShowInfo && progress.GetComponent<Progress>().day_num == 0)
        {
            if(CharacterName == "博金森" || CharacterName == "艾米莉")
            {
                if (InfoClick == 0)
                {
                    InfoClick = 1;
                    return;
                }
                else
                {
                    GameObject.Find("CharacterInfo").GetComponent<CharacterInfoManager>().CloseInfo();
                    TalkBar.GetComponent<TalkSystem>().on = true;

                    Have_ShowInfo = true;

                    OnTalk();

                    Invoke("SetBool", 1.5f);
                }
            }
            else
            {
                GameObject.Find("CharacterInfo").GetComponent<CharacterInfoManager>().CloseInfo();
                TalkBar.GetComponent<TalkSystem>().on = true;
            
                Have_ShowInfo = true;
            
                OnTalk();

                Invoke("SetBool", 1.5f);
            }

        }


    }

    void SetBool()
    {
        ShowInfo = false;
    }



    /// <summary>
    /// 负重分配/取消分配核心方法
    /// 包含三种逻辑：1. 分配食物到负重 2. 分配尸体到负重 3. 取消负重分配（归还食物/尸体）
    /// </summary>
    public void Decrase()
    {
        // 【死亡状态判断】如果角色死亡，不执行任何负重操作
        if (Dead) return;
        if (CantWeight) return;
        if (!progress.start) return;

        // 获取资源选择面板（SelectBar）的状态，判断是否处于"食物选择"模式
        if (!gameObject.transform.parent.Find("SelectBar").GetComponent<AssResources>().Food)
        {
            // 非食物选择模式：启用角色按钮，隐藏ToggleUI
            gameObject.GetComponent<Button>().enabled = true;
            toggle.SetActive(false);

            // 1. 【分配食物到负重】：判断是否处于"食物负重分配"阶段，且满足分配条件
            if (gameObject.transform.parent.Find("SelectBar").GetComponent<AssResources>().Weight_Food
                && weight.Weight < 3  // 负重未满（最大3格）
                && weight.Weight >= 0 // 负重不为负
                && food.Weight > 0)   // 有食物可分配
            {
                // 增加1格负重
                weight.Weight += 1;
                // 标记负重类型为"食物"（0=食物，1=尸体）
                weight.Weight_tag = 0;

                // 根据当前负重数，更新负重进度条颜色（红色表示已占用）
                if (weight.Weight >= 1) _weight1Image.color = new Color32(95,47,54,255);;
                if (weight.Weight >= 2) _weight2Image.color = new Color32(95,47,54,255);;
                if (weight.Weight == 3) _weight3Image.color = new Color32(95,47,54,255);;

                // 减少角色持有的食物数量（分配1个食物到负重）
                food.Weight -= 1;
            }

            // 2. 【分配尸体到负重】：判断是否处于"尸体分配"阶段，且满足分配条件
            if (gameObject.transform.parent.Find("SelectBar").GetComponent<AssResources>().Body
                && weight.Weight == 0  // 负重为空（尸体占满3格，必须无其他负重）
                && body.Weight > 0)    // 有尸体可分配
            {
                // 标记负重类型为"尸体"（0=食物，1=尸体）
                weight.Weight_tag = 1;
                // 尸体占满3格负重，直接将3个进度条设为红色
                _weight1Image.color = new Color32(95,47,54,255);;
                _weight2Image.color = new Color32(95,47,54,255);;
                _weight3Image.color = new Color32(95,47,54,255);;

                // 负重设为3（尸体固定占3格）
                weight.Weight = 3;
                // 减少角色持有的尸体数量（分配1个尸体到负重）
                body.Weight -= 1;
            }

            // 3. 【取消负重分配】：判断是否处于"非分配阶段"，且当前有负重
            if (!gameObject.transform.parent.Find("SelectBar").GetComponent<AssResources>().Weight_Food
                && !gameObject.transform.parent.Find("SelectBar").GetComponent<AssResources>().Body
                && weight.Weight > 0)
            {
                // 根据负重类型，归还对应资源（食物/尸体）
                if (weight.Weight_tag == 0)
                {
                    // 负重类型为食物：归还1个食物到持有数量，减少1格负重
                    food.Weight += 1;
                    weight.Weight -= 1;
                    // 刷新负重进度条UI（重置未占用的格子为白色）
                    Refresh();
                    // 重新设置已占用的格子为红色（避免刷新后全部变白）
                    if (weight.Weight >= 1) _weight1Image.color = new Color32(95,47,54,255);;
                    if (weight.Weight >= 2) _weight2Image.color = new Color32(95,47,54,255);;
                    if (weight.Weight == 3) _weight3Image.color = new Color32(95,47,54,255);;
                }
                if (weight.Weight_tag == 1)
                {
                    // 负重类型为尸体：归还1个尸体到持有数量，负重清零（尸体占3格，直接减3）
                    body.Weight += 1;
                    weight.Weight -= 3;
                    // 刷新负重进度条UI（全部重置为白色）
                    Refresh();
                }
            }
        }
        else
        {
            // 食物选择模式：禁用角色按钮，显示ToggleUI
            toggle.SetActive(true);
            gameObject.GetComponent<Button>().enabled = false;
        }
    }

    /// <summary>
    /// 负重UI刷新方法
    /// 1. 重置所有负重进度条为白色（未占用状态）
    /// 2. 同步更新持有食物/尸体的UI文本
    /// </summary>
    void Refresh()
    {
        // 【死亡状态判断】如果角色死亡，不执行UI刷新
        if (Dead) return;
        

        // 重置3个负重进度条颜色为白色（表示未占用）
        _weight1Image.color = Color.white;
        _weight2Image.color = Color.white;
        _weight3Image.color = Color.white;


        AfterSpecialTalk = false;
        toggle.GetComponent<Toggle>().isOn = false;
        NotComfort = false;
    }

    /// <summary>
    /// 角色对话触发方法
    /// 仅在游戏处于"对话阶段"且未触发过对话时执行
    /// </summary>
    public void OnTalk()
    {
        // 【死亡状态判断】如果角色死亡，不执行对话触发
        if (Dead) return;
        if (!CanTalk) return;
        if (AfterSpecialTalk) return;
        if (GlobalData.Day == 0 && have_talk && CharacterName == "阿曼德") return;
        if (TutorialManager.TutorialIsShow && !TutorialManager.TutorialWeight)
        {
            TutorialManager.Controller.Shake();
            return;
        }
        
        TalkSystem talksys = TalkBar.GetComponent<TalkSystem>();
        TutorialManager.CharacterIsTalking = true;
        if (talksys.Daytime != 0)
        { if (Background.GetComponent<BackGroundMoving>().open) return; }
        
        talksys.SetShowName();
        if (Special1 && !ClikDelay)
        {
            ClikDelay = true;
            Special1 = false;
            Invoke("SetSpecialTalk1", 0.2f);
            
            Invoke("SetClik", 5f);
            return;
        }
        if (Special2 && !ClikDelay)
        {
            ClikDelay = true;
            Special2 = false;
            Invoke("SetSpecialTalk2", 0.2f);
            Invoke("SetClik", 5f);
            return;
        }
        // 用于d0的新手引导
        if (end.GetComponent<Progress>().day_num == 0)
        {
            Mask.gameObject.SetActive(false);
        }

        //day0角色资料显示逻辑
        if (end.GetComponent<Progress>().day_num == 0 && !Have_ShowInfo)
        {

            if(CharacterName == "艾米莉" || CharacterName == "博金森")
            {
                gameObject.GetComponent<Aimibo>().ShowInfo("艾米莉");
                ShowInfo = true;
                return;
            }


            GameObject.Find("CharacterInfo").GetComponent<CharacterInfoManager>().ShowInfo(CharacterName);
            ShowInfo = true;
            return;

        }



        

        // 判断当前游戏是否处于"对话阶段"（Progress中的talk状态为true）
        if (progress.GetComponent<Progress>().talk && !ClikDelay)
        {
            progress.GetComponent<Progress>().CanSwitch = false;
            talksys.on = false;
            ClikDelay = true;
            

            // 如果未触发过对话（have_talk为false）
            if (!have_talk)
            {
                
                if (progress.GetComponent<Progress>().day_num == 0 && CharacterName != "阿曼德")
                {
                    Day0_Talk.Weight += 1;
                }
                // 显示对话面板
                TalkBar.SetActive(true);
                
                talksys.Talklines[end.GetComponent<Progress>().day_num] = this.textline[end.GetComponent<Progress>().day_num];
                talksys.showText.CanShowText = true;
                // 标记为已触发对话（避免重复触发）
                have_talk = true;
                talksys.ShowBar();
                // 父对象播放"向下"动画（可能隐藏父对象UI，突出对话面板）
                
                Invoke("SetTalk", 1.5f);
                
            }
            else
            {
                TalkBar.SetActive(true);
                talksys.Talklines[end.GetComponent<Progress>().day_num] = this.textline[end.GetComponent<Progress>().day_num].Option3;
                talksys.ShowBar();
                talksys.showText.CanShowText = true;
                
                // 显示对话面板（调用TalkSystem的ShowBar方法，可能包含动画）
                Invoke("SetTalk", 1.5f);
                
            }
            Invoke("SetClik", 5f);
        }
        
    }

    void SetClik()
    {
        ClikDelay = false;
        gameObject.GetComponent<Button>().enabled = true;
        TalkBar.GetComponent<TalkSystem>().on = true;
    }

    void SetTalk()
    {
        TalkSystem talksys = TalkBar.GetComponent<TalkSystem>();
        talksys.on = true;
        talksys._inshop = false;
        // 显示对话面板（调用TalkSystem的ShowBar方法，可能包含动画）
        
        // 重置对话行数到第一行
        talksys.line = 0;
        // 启动对话文本显示（异步执行，避免UI卡顿）
        talksys.showText.CanShowText = true;
        _ = talksys.ShowText();
    }
    

    public void SetSpecialTalk1()
    {
        TalkBar.GetComponent<TalkSystem>().Talklines[end.GetComponent<Progress>().day_num] = this.textline[end.GetComponent<Progress>().day_num].SpecialTalk;
        TalkBar.GetComponent<TalkSystem>().on = true;

        TalkBar.SetActive(true);
        Attention.SetActive(false);
        
        // 显示对话面板（调用TalkSystem的ShowBar方法，可能包含动画）
        TalkBar.GetComponent<TalkSystem>().ShowBar();
        
        // 父对象播放"向下"动画（可能隐藏父对象UI，突出对话面板）
        AfterSpecialTalk = true;
        
        Invoke("SetTalk", 1.2f);

    }


    public void SetSpecialTalk2()
    {
        TalkBar.GetComponent<TalkSystem>().Talklines[end.GetComponent<Progress>().day_num] = this.textline[end.GetComponent<Progress>().day_num].SpecialTalk2;
        TalkBar.GetComponent<TalkSystem>().on = true;
        TalkBar.SetActive(true);
        Attention.SetActive(false);
        
        // 显示对话面板（调用TalkSystem的ShowBar方法，可能包含动画）
        TalkBar.GetComponent<TalkSystem>().ShowBar();
        
        // 父对象播放"向下"动画（可能隐藏父对象UI，突出对话面板）
        AfterSpecialTalk = true;
        Invoke("SetTalk", 1.2f);




    }

    /// <summary>
    /// 管理角色对话触发权限
    /// </summary>
    public void DisableTalk()
    {
        CanTalk = false;

    }
    /// <summary>
    /// 管理角色对话触发权限
    /// </summary>
    public void EnableTalk()
    {
        CanTalk = true;
    }



    void StartRestWeightImage()
    {

        switch(weight.Weight)
        {
            case 0:
                _weight1Image.color = Color.white;
                _weight2Image.color = Color.white;
                _weight3Image.color = Color.white;
                break;
            case 1:
                _weight1Image.color = new Color32(95,47,54,255);
                _weight2Image.color = Color.white;
                _weight3Image.color = Color.white;
                break;
            case 2:
                _weight1Image.color = new Color32(95,47,54,255);;
                _weight2Image.color = new Color32(95,47,54,255);;
                _weight3Image.color = Color.white;
                break;
            case 3:
                _weight1Image.color = new Color32(95,47,54,255);;
                _weight2Image.color = new Color32(95,47,54,255);;
                _weight3Image.color = new Color32(95,47,54,255);;
                break;



        }

    }




}