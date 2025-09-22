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
    /// <summary>
    /// 切换状态标记UI（可能用于标记角色是否被选中/处于特殊状态）
    /// </summary>
    public GameObject toggle;

    [Header("状态 - 关联游戏进度管理器")]
    /// <summary>
    /// 游戏进度管理对象（Progress脚本挂载对象，用于判断当前游戏阶段）
    /// </summary>
    public GameObject progress;

    [Header("对话列表 - 角色专属对话数据")]
    /// <summary>
    /// 角色按天数对应的对话列表（索引对应天数，存储每天的对话数据）
    /// </summary>
    public List<Manager> textline = new List<Manager>();

    [Tooltip("挂载的对话Bar - 角色触发对话时使用的对话面板")]
    /// <summary>
    /// 对话面板对象（挂载TalkSystem脚本，用于显示角色对话）
    /// </summary>
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
    private GameObject Attention,Attention2;
    [Header("安抚对话控制")]
    public bool Special1,Special2,AfterSpecialTalk;
    public bool NotComfort;//用于判断安抚是否成功
    public bool ClikDelay = false;
    [Header("Day0用")]
    public Manager Day0_Talk;

    


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
        if (curr_num != 0)
        {
            Attention = gameObject.transform.Find("Attention").gameObject;
            Attention2 = gameObject.transform.Find("Attention2").gameObject;
            Attention.SetActive(false);
            Attention2.SetActive(false);
        }
        
        // 初始化显示角色持有的尸体数量（更新UI文本）
        gameObject.transform.parent.Find("Have_Body").GetComponent<TextMeshProUGUI>().text = body.Weight.ToString();
        // 初始化角色负重为0（初始无负重）
        weight.Weight = 0;
        // 绑定角色子对象中的3个负重进度条UI
        weight1 = gameObject.transform.Find("Weight3").gameObject;
        weight2 = gameObject.transform.Find("Weight2").gameObject;
        weight3 = gameObject.transform.Find("Weight1").gameObject;
        // 初始化显示角色持有的食物数量（更新UI文本）
        gameObject.transform.parent.Find("Have_Food").GetComponent<TextMeshProUGUI>().text = food.Weight.ToString();
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
            Attention.SetActive(false);
            Attention2.SetActive(false);
            return; 
        }
        if(AfterSpecialTalk && NotComfort)
        {
            Attention2.SetActive(true);
        }
        if (AfterSpecialTalk && !NotComfort)
        {
            Attention2.SetActive(false);
            
        }
        if(progress.GetComponent<Progress>().talk && CharacterName == "主角")
        {
            gameObject.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            gameObject.GetComponent<Image>().color = Color.white;
        }


        // 获取资源选择面板（SelectBar）的状态，判断是否处于"食物选择"模式
        if (!progress.GetComponent<Progress>().food)
        {
            // 非食物选择模式：启用角色按钮，隐藏ToggleUI
            gameObject.GetComponent<Button>().enabled = true;
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
            // 刷新负重进度条UI（重置为初始绿色）
            Refresh();
        }
    }

    /// <summary>
    /// 负重分配/取消分配核心方法
    /// 包含三种逻辑：1. 分配食物到负重 2. 分配尸体到负重 3. 取消负重分配（归还食物/尸体）
    /// </summary>
    public void Decrase()
    {
        // 【死亡状态判断】如果角色死亡，不执行任何负重操作
        if (Dead) return;

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
                if (weight.Weight >= 1) weight1.GetComponent<Image>().color = Color.red;
                if (weight.Weight >= 2) weight2.GetComponent<Image>().color = Color.red;
                if (weight.Weight == 3) weight3.GetComponent<Image>().color = Color.red;

                // 减少角色持有的食物数量（分配1个食物到负重）
                food.Weight -= 1;
                // 刷新UI显示当前剩余食物数量
                gameObject.transform.parent.Find("Have_Food").GetComponent<TextMeshProUGUI>().text = food.Weight.ToString();
            }

            // 2. 【分配尸体到负重】：判断是否处于"尸体分配"阶段，且满足分配条件
            if (gameObject.transform.parent.Find("SelectBar").GetComponent<AssResources>().Body
                && weight.Weight == 0  // 负重为空（尸体占满3格，必须无其他负重）
                && body.Weight > 0)    // 有尸体可分配
            {
                // 标记负重类型为"尸体"（0=食物，1=尸体）
                weight.Weight_tag = 1;
                // 尸体占满3格负重，直接将3个进度条设为红色
                weight1.GetComponent<Image>().color = Color.red;
                weight2.GetComponent<Image>().color = Color.red;
                weight3.GetComponent<Image>().color = Color.red;

                // 负重设为3（尸体固定占3格）
                weight.Weight = 3;
                // 减少角色持有的尸体数量（分配1个尸体到负重）
                body.Weight -= 1;
                // 刷新UI显示当前剩余尸体数量
                gameObject.transform.parent.Find("Have_Body").GetComponent<TextMeshProUGUI>().text = body.Weight.ToString();
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
                    // 刷新负重进度条UI（重置未占用的格子为绿色）
                    Refresh();
                    // 重新设置已占用的格子为红色（避免刷新后全部变绿）
                    if (weight.Weight >= 1) weight1.GetComponent<Image>().color = Color.red;
                    if (weight.Weight >= 2) weight2.GetComponent<Image>().color = Color.red;
                    if (weight.Weight == 3) weight3.GetComponent<Image>().color = Color.red;
                }
                if (weight.Weight_tag == 1)
                {
                    // 负重类型为尸体：归还1个尸体到持有数量，负重清零（尸体占3格，直接减3）
                    body.Weight += 1;
                    weight.Weight -= 3;
                    // 刷新负重进度条UI（全部重置为绿色）
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
    /// 1. 重置所有负重进度条为绿色（未占用状态）
    /// 2. 同步更新持有食物/尸体的UI文本
    /// </summary>
    void Refresh()
    {
        // 【死亡状态判断】如果角色死亡，不执行UI刷新
        if (Dead) return;

        // 重置3个负重进度条颜色为绿色（表示未占用）
        weight1.GetComponent<Image>().color = Color.green;
        weight2.GetComponent<Image>().color = Color.green;
        weight3.GetComponent<Image>().color = Color.green;

        // 同步更新UI显示当前持有食物数量
        gameObject.transform.parent.Find("Have_Food").GetComponent<TextMeshProUGUI>().text = food.Weight.ToString();
        // 同步更新UI显示当前持有尸体数量
        gameObject.transform.parent.Find("Have_Body").GetComponent<TextMeshProUGUI>().text = body.Weight.ToString();

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
        
        
        Debug.Log("禁用按钮");

        // 判断当前游戏是否处于"对话阶段"（Progress中的talk状态为true）
        if (progress.GetComponent<Progress>().talk && !ClikDelay)
        {
            TalkSystem talksys = TalkBar.GetComponent<TalkSystem>();
            talksys.on = false;
            ClikDelay = true;
            Debug.Log("触发对话");

            // 如果未触发过对话（have_talk为false）
            if (!have_talk)
            {
                if(progress.GetComponent<Progress>().day_num == 0 && CharacterName != "阿曼德")
                {
                    Day0_Talk.Weight += 1;
                }
                // 显示对话面板
                TalkBar.SetActive(true);
                
                talksys.Talklines[end.GetComponent<Progress>().day_num] = this.textline[end.GetComponent<Progress>().day_num];
                // 标记为已触发对话（避免重复触发）
                have_talk = true;
                talksys.ShowBar();
                // 父对象播放"向下"动画（可能隐藏父对象UI，突出对话面板）
                Invoke("DownAnim", Delay);
                Invoke("SetTalk", 1.5f);
                
            }
            else
            {
                TalkBar.SetActive(true);
                talksys.Talklines[end.GetComponent<Progress>().day_num] = this.textline[end.GetComponent<Progress>().day_num].Option3;
                talksys.ShowBar();
                // 父对象播放"向下"动画（可能隐藏父对象UI，突出对话面板）
                Invoke("DownAnim", Delay);
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
        
        _ = talksys.ShowText();
    }


    void DownAnim()
    {
        gameObject.transform.parent.GetComponent<Animator>().SetTrigger("Down");
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
        Invoke("DownAnim", Delay);
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
        Invoke("DownAnim", Delay);
        Invoke("SetTalk", 1.2f);




    }





}