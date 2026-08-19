using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// 游戏进度管理器
/// 负责控制游戏阶段切换（开始阶段/对话阶段/进食阶段）、日期更新、幕间对话触发、背景滚动状态控制等核心逻辑
/// </summary>
public class Progress : MonoBehaviour
{
    // 当前游戏天数
    public int day_num = 1;
    [Header("判断状态 - 游戏当前所处阶段标记")]
    // 是否处于【开始阶段】（初始阶段，未进入对话/进食）
    public bool start;
    // 是否处于【对话阶段】
    public bool talk;
    // 是否处于【进食阶段】
    public bool food;
    //教学关是否可以跳过阶段
    public bool can_skip;
    // 是否允许切换游戏阶段（用于控制阶段切换的触发条件）
    public bool CanSwitch;
    // 是否允许触发对话（控制对话系统的激活状态）
    public bool CanTalk;
    //是否触发商店对话
    public bool ShopTalk;
    [Tooltip("文字描述 - 关联UI文本组件，显示重量、对话、进食、日期信息")]
    // 重量显示文本（UI组件）
    public Animator state; 
    // 日期显示文本（UI组件，格式：Day X）
    public TextMeshProUGUI day;
    [Header("游戏对象 - 关联核心功能预制体/组件")]
    // 跳过按钮/明暗控制对象（用于切换场景明暗状态）
    public GameObject skip;
    // 底部功能栏（可能包含背包、道具管理等UI）
    public GameObject DownBar;
    // 背景对象（关联背景滚动脚本 BackGroundMoving）
    public GameObject background;
    // 对话栏（关联对话系统脚本 TalkSystem）
    public GameObject TalkBar;
    // 对象管理器（管理道具携带、背包状态等）
    public GameObject ObjectManager;
    public GameObject Shop;
    [Tooltip("幕间切换延迟")]
    public float Delay;
    [Header("插在各个阶段的幕间 - 不同阶段触发的对话数据列表")]
    //【开始阶段前】的幕间对话列表（按天数索引，对应每天开始前的对话）
    public List<Manager> beforeStart;
    // 【进食阶段前】的幕间对话列表（按天数索引，对应每天进入进食前的对话）
    public List<Manager> beforeFood;
    // 【进食阶段后】的幕间对话列表（按天数索引，对应每天进食后的对话）
    public List<Manager> afterFood;
    [Header("背包 - 道具重量管理相关")]
    // 最终达到的食物重量
    public Manager Final_Food;
    // 最终达到的尸体重量
    public Manager Final_Body;
    // 当前拥有的食物重量
    public Manager Have_Food;
    // 当前的尸体数量
    public Manager Have_Body;
    public GameObject Mask;
    public DayNightSystem DNSys;
    // 用于判断day0遮罩是否用过
    private bool Day0MaskUsed = false;
    private TalkSystem talkSys;
    [Header("阿曼德自杀判断")]
    public Manager AmandeKillSelf;
    private AudioManager AudioManager;
    [Header("转换阶段提示条")]
    public GameObject SwitchStageBar;
    [Header("黑幕")]
    public Manager CurrentDead;
    public VideoPlayer ShopSwitch;
    private bool SwitchPlaying;
    private bool Shake; //震动管理
    [Header("抛弃物品提示")]
    public AbandonTips abandon;
    public Manager AbandonBool;
    public MiniCharacterManager MiniCharacter;
    [Header("跳过对话提示")]
    public GameObject TalkTips;
    public Manager TalkTipsBool;
    [Header("饿死队员提示")]
    public GameObject killTips;
    public Manager killTipsBool;
    public GameObject day0TalkTips;
    public GameObject day0FoodTalkTips;
    public GameObject comfortTips;
    //重置每天的商店事件
    public ShopEventReseter shopEventReseter;
    private Button _thisButton;
    public GameObject shopIntro;

    /// <summary>
    /// 初始化方法 - 游戏启动时执行
    /// 1. 绑定UI文本组件 2. 设置初始阶段的文本颜色 3. 触发当天开始前的幕间对话
    /// </summary>
    void Start()
    {
        GlobalData.Day = day_num;
        _thisButton = this.GetComponent<Button>();
        AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        talkSys = TalkBar.GetComponent<TalkSystem>();
        // 初始化日期显示（格式：Day X）
        day.text = "Day " + day_num;
        
        if (day_num != 0)
            beforeStart[day_num] = GetComponent<IntermissionManager>().AddTextLine("BeforeStart");
        else
        {
            state.SetTrigger("Switch");
        }
        // 触发当天开始前的幕间对话（若存在对应天数的对话数据）
        if (beforeStart[day_num] != null)
        {
            CanTalk = true;  // 允许触发对话
            // 将当前天数的对话数据赋值给对话系统
            talkSys.Talklines[day_num] = beforeStart[day_num];
            talkSys.line = 0;  // 重置对话行数到第一行
            /*_ = talkSys.ShowText();  // 启动对话显示（异步执行）*/
            DownBar.GetComponent<Animator>().SetTrigger("Down");  // 底部栏播放"向下"动画（可能隐藏底部栏）
        }

    }

    private void OnEnable()
    {
        GlobalData.Progress = this;
        StartCoroutine(RefreshStage());
    }

    private IEnumerator RefreshStage()
    {
        while (true)
        {
            if (start)
            {
                GlobalData.Stage = 0;
            }

            if (talk)
            {
                GlobalData.Stage = 1;
            }

            if (food)
            {
                GlobalData.Stage = 2;
            }

            yield return new WaitForSeconds(1);
        }
        
    }

    private void Update()
    {
        if (day_num != 0)
        {
            _thisButton.interactable = CanSwitch;
        }
        
    }


    public void FixedUpdate()
    {
        if(day_num == 0 && food && !Day0MaskUsed)
        {
            DownBar.GetComponent<ObjectManager>().CheckEat(false);
        }

        /*if(SwitchPlaying)
        {
            
            if (ShopSwitch.frame == (long)ShopSwitch.clip.frameCount - 1)
            {
                
                SwitchPlaying = false;
                ShopSwitch.gameObject.SetActive(false);

            }
        }*/
    }
    
    /// <summary>
    /// 阶段切换核心方法
    /// 根据当前游戏阶段（start/talk/food），切换到下一个阶段，并执行对应逻辑
    /// 包含：状态标记更新、UI文本颜色更新、背景滚动控制、幕间对话触发、日期更新等
    /// </summary>
    public void SwitchProgress(bool skipTips = false)
    {
        
        if (TutorialManager.TutorialIsShow)
        {
            Debug.Log("显示教程中禁止转换阶段");
            return;
        }
        AudioManager.AudioPlayer("Click");
        if(day_num > 3)
        {
            GlobalData.EndingsManager.ToEnd("Demo-End");
        }
        // 1. 从【开始阶段】切换到【对话阶段】
        if (start && CanSwitch)
        {
            if (DNSys.time < DNSys.Frist - 0.005f) return;
            // 通知对象管理器更新道具携带数量（可能同步背包显示）
            if (!skipTips)
            {
                if (!AbandonBool.GeneralBool)
                {
                    bool allCarry = ObjectManager.GetComponent<ObjectManager>().SetCarryNum(true);
                    if (!allCarry)
                    {
                        abandon.gameObject.SetActive(true);
                        return;
                    }
                }
            }
            else
                ObjectManager.GetComponent<ObjectManager>().SetCarryNum();

           
            
            // 激活背景滚动（调用 BackGroundMoving 脚本的 open 状态）
            background.GetComponent<BackGroundMoving>().open = true;
            if(day_num != 0) MiniCharacter.SetWalk();
            // 更新UI文本颜色（对话阶段高亮，开始阶段半透明）
            start = false;  // 退出开始阶段
            talk = true;  // 进入对话阶段
            state.SetTrigger("Switch");
        }
        
        else if (talk && !CanSwitch && day_num == 0 && !GlobalData.ShowText.CanShowText)
        {
            day0TalkTips.SetActive(true);
        }

        // 2. 从【对话阶段】切换到【进食阶段】
        else if (talk && CanSwitch)
        {
            if (DNSys.time < DNSys.Second - 0.005f && day_num != 0) return;

            if (!skipTips && !GlobalData.AfterShop)
            {
                bool checkTalk = true;
                foreach (var value in GlobalData.TalkSystem.characterComponentList)
                {
                    if (value.CharacterName == "主角")
                    {
                        continue;
                    }
                    if (!value.have_talk && !value.Dead)
                    {
                        checkTalk = false;
                    }
                }
                if (!checkTalk && day_num != 0)
                {
                    if (!TalkTipsBool.GeneralBool)
                    {
                        TalkTips.SetActive(true);
                        return;
                    }

                }
            }
            

            if (!CheckComfort())
            {
                comfortTips.SetActive(true);
                return;
            }



            // 商店页面调用 还会加上转场效果
            if (day_num == 2 || day_num == 5 || day_num == 7)
            {
                if(ShopTalk)
                {
                    /*if(!Shake)
                    {
                        Shake = true;
                        GetComponent<UIshake>().StartShake(); 
                    }
                    else
                    {
                        ShopSwitch.Play();
                        SwitchPlaying = true;
                        Invoke("SetShop", 5.5f);
                    }*/

                    GlobalData.AfterShop = true;
                    var shopAnim = shopIntro.GetComponent<Animator>();
                    shopIntro.SetActive(true);
                    shopAnim.SetInteger("Phase",shopAnim.GetInteger("Phase") + 1);
                    return;
                }
            }
            
            if(day_num != 0) beforeFood[day_num] = GetComponent<IntermissionManager>().AddTextLine("BeforeFood");
            // 触发进食前的幕间对话（若存在对应天数的对话数据）
            if (beforeFood[day_num] != null)
            {
                CanTalk = true;  // 允许触发对话
                // 赋值对话数据到对话系统
                talkSys.Talklines[day_num] = beforeFood[day_num];
                talkSys.line = 0;  // 重置对话行数
                talkSys.showText.CanShowText = true;
                if (talkSys.useNewSys)
                {
                    GlobalData.NewTalkSysShowText.UnLockOutPut();
                    GlobalData.NewTalkSysShowText.SetChoiceLine(0,true);
                }
                _ = talkSys.ShowText();  // 启动对话显示
                TalkBar.GetComponent<Animator>().SetTrigger("Up");
                if (TalkBar.transform.position.y == 0)
                {
                    DownAnim();
                }  // 底部栏隐藏动画
                  // 对话栏显示动画（向上弹出）
            }
            

            // 更新UI文本颜色（进食阶段高亮，对话阶段半透明）
            state.SetTrigger("Switch");
            talk = false;  // 退出对话阶段
            food = true;   // 进入进食阶段

            // 若背景正在滚动，停止滚动并将场景设为暗色（可能表示进入静态场景）
            if (background.GetComponent<BackGroundMoving>().open)
            {
                background.GetComponent<BackGroundMoving>().open = false;
                 // 调用 skip 的暗化方法
            }
            DownBar.GetComponent<ObjectManager>().ResetEat();
            SetComfort();
            GlobalData.AfterShop = false;
            SwitchStageBar.SetActive(true);
            
            

        }

        // 3. 从【进食阶段】切换到【下一天的开始阶段】
        else if (food && CanSwitch)
        {
            if (DNSys.on) return;
            if (!skipTips)
            {
                if (!CheckEatState()) return; //饿死队员提示
            }
            
            
            
            bool allEat = false;
            CurrentDead.TxtLine.Clear();
            // 特殊逻辑：第0天（可能是教程天）的进食完成判断
            if (day_num == 0)
            {
                // 检查是否满足进食条件（若未满足，不执行阶段切换）
                if (!DownBar.GetComponent<ObjectManager>().CheckEat(false))
                {
                    day0FoodTalkTips.SetActive(true);
                    return;  // 退出方法，不切换阶段
                }

                // 第0天进食完成后，更新重量数据（将目标重量赋值给当前拥有重量，重置目标重量）
                Have_Body.Weight = Final_Body.Weight;
                Have_Food.Weight = Final_Food.Weight;
                Final_Body.Weight = 0;
                Final_Food.Weight = 0;
            }
            else
            { allEat = DownBar.GetComponent<ObjectManager>().CheckEat(true); } // 判断哪个角色没有进食并将其状态设为死亡}
            if (day_num != 0)
                afterFood[day_num] = GetComponent<IntermissionManager>().AddTextLine("AfterFood");

            // 触发进食后的幕间对话（若存在对应天数的对话数据）
            if (afterFood[day_num] != null)
            {
                CanTalk = true;  // 允许触发对话
                // 赋值对话数据到对话系统
                if (day_num == 1)
                {
                    if (allEat)
                    {
                        talkSys.Talklines[day_num] = afterFood[day_num];
                        talkSys.line = 0;  // 重置对话行数
                        talkSys.showText.CanShowText = true;
                        if (talkSys.useNewSys)
                        {
                            GlobalData.NewTalkSysShowText.UnLockOutPut();
                            GlobalData.NewTalkSysShowText.SetChoiceLine(0,true);
                        }
                        _ = talkSys.ShowText();  // 启动对话显示
                        TalkBar.GetComponent<Animator>().SetTrigger("Up");// 对话栏显示动画
                        afterFood[day_num] = null;  // 清空当前天数的对话数据（避免重复触发）
                        return;  // 先显示对话，暂不执行后续阶段切换（对话结束后需重新触发切换）
                    }

                }
                else
                {
                    talkSys.Talklines[day_num] = afterFood[day_num];
                    talkSys.line = 0;  // 重置对话行数
                    talkSys.showText.CanShowText = true;
                    if (talkSys.useNewSys)
                    {
                        GlobalData.NewTalkSysShowText.UnLockOutPut();
                        GlobalData.NewTalkSysShowText.SetChoiceLine(0,true);
                    }
                    _ = talkSys.ShowText();  // 启动对话显示
                    TalkBar.GetComponent<Animator>().SetTrigger("Up");  // 对话栏显示动画
                    if (TalkBar.transform.position.y == 0)
                    {
                        DownAnim();
                    }
                    afterFood[day_num] = null;  // 清空当前天数的对话数据（避免重复触发）
                    return;  // 先显示对话，暂不执行后续阶段切换（对话结束后需重新触发切换）
                }
                
            }
            else
            {
                if (day_num == 3)
                {
                    if(! GlobalData.EndingsManager.CheckEnding())
                    {
                        StartCoroutine(GlobalData.EndingsManager.ToEnd("Demo-End"));
                        return;
                    }
                }
            }

            if (AmandeKillSelf.GeneralBool)
            {
                KillAmande();
            }
            int notcomfort = 0;
            foreach (GameObject g in talkSys.CharacterList)
            {
                if (g.GetComponent<Character>().NotComfort)
                {
                    notcomfort += 1;
                }
            }

            if (notcomfort == 1)
            {
                //艾米莉 博金森反抗死亡
                foreach (GameObject g in talkSys.CharacterList)
                {
                    var tempCharacter = g.GetComponent<Character>();
                    if (tempCharacter.NotComfort && (tempCharacter.CharacterName == "艾米莉" || tempCharacter.CharacterName == "博金森"))
                    {
                        tempCharacter.SetKilled();
                    }
                }
            }

            // 更新UI文本颜色（开始阶段高亮，进食阶段半透明）
            state.SetTrigger("Switch");
            start = true;  // 进入下一天的开始阶段
            talk = false;  // 退出对话阶段
            food = false;  // 退出进食阶段
            day_num += 1;  // 天数+1（进入下一天）
            day.text = "Day " + day_num;  // 更新日期显示
            if(day_num == 2 || day_num == 5 || day_num == 7)
            {
                ShopTalk = true;
            }
            

            if (shopEventReseter != null)
            {
                shopEventReseter.ResetEvent();
            }
            
            // 非第0天的特殊处理：场景明暗切换（先暗后亮，模拟昼夜交替）
            //重置安抚状态
            foreach (var value in talkSys.CharacterList)
            {
                value.GetComponent<Character>().ResetComfort();
            }
            
            
            GlobalData.Day = day_num;

            // 通知对象管理器重置道具携带状态（可能将携带道具放回背包）
            ObjectManager.GetComponent<ObjectManager>().ReturnCarry();
            ObjectManager.GetComponent<ObjectManager>().RestTag();
            

            // 重置背景状态（调用 BackGroundMoving 的 Re_set 方法，还原背景初始位置）

            foreach (GameObject g in DownBar.GetComponent<ObjectManager>().Character_List)
            {
                g.GetComponent<Character>().EnableTalk();
            }
            
            if (day_num != 0)
                beforeStart[day_num] = GetComponent<IntermissionManager>().AddTextLine("BeforeStart");

            // 触发下一天开始前的幕间对话（若存在对应天数的对话数据）
            if (beforeStart[day_num] != null)
            {
                Debug.Log($"开始前对话 DayNum：{day_num}");
                CanTalk = true;  // 允许触发对话
                // 赋值对话数据到对话系统
                talkSys.Talklines[day_num] = beforeStart[day_num];
                talkSys.line = 0;// 重置对话行数
                talkSys.on = true;
                talkSys.showText.CanShowText = true;
                if (talkSys.useNewSys)
                {
                    GlobalData.NewTalkSysShowText.UnLockOutPut();
                    GlobalData.NewTalkSysShowText.SetChoiceLine(0,true);
                }
                talkSys.ShowText();
                // 启动对话显示
                TalkBar.GetComponent<Animator>().SetTrigger("start");  // 对话栏显示动画
            }

            if (day_num != 0)
            {
                if(skip.GetComponent<Image>().color.a == 0) skip.GetComponent<Skip>().TurnDark();  // 若未暗化，执行暗化
                skip.transform.Find("Report").GetComponent<Report>().ShowText();
                skip.transform.Find("Report").gameObject.SetActive(true);
                
                Invoke("ResetBack", 0.4f);
                Invoke("TurnLight", 1.8f);  // 延迟1秒后执行亮化（Invoke 用于延迟调用方法）
            }

            

        }
        
        if (start)
        {
            GlobalData.Stage = 0;
        }

        if (talk)
        {
            GlobalData.Stage = 1;
        }

        if (food)
        {
            GlobalData.Stage = 2;
        }
    }

    private void KillAmande()
    {
        Final_Body.Weight += 1;
        talkSys.DeadName.TxtLine.Add("阿曼德Poison");
        CurrentDead.TxtLine.Add("阿曼德");
        talkSys.CharacterList[4].GetComponent<Character>().Dead = true;
        talkSys.CharacterList[4].transform.Find("Toggle").gameObject.SetActive(false);
    }

    void SetComfort()
    {
        foreach (GameObject g in DownBar.GetComponent<ObjectManager>().Character_List)
        {
            if(!g.GetComponent<Character>().AfterSpecialTalk && (g.GetComponent<Character>().Special1 || g.GetComponent<Character>().Special2))
            {
                if(g.GetComponent<Character>().CharacterName != "阿曼德")
                {
                    g.GetComponent<Character>().NotComfort = true;
                    g.transform.Find("Attention").gameObject.SetActive(false);
                    g.transform.Find("Attention2").gameObject.SetActive(true);
                }
                else
                {
                    g.transform.Find("Attention").gameObject.SetActive(false);
                    g.transform.Find("Attention2").gameObject.SetActive(false);
                }
            }
        }
    }

    private bool CheckEatState()
    {
        if (killTipsBool.GeneralBool || day_num == 0)
        {
            return true;
        }

        for (int i = 0; i < GlobalData.Characters.Count; i++)
        {
            if (!GlobalData.Characters[i].Dead && !GlobalData.Characters[i].weight.Eat)
            {
                killTips.SetActive(true);
                return false;
            }
        }

        return true;
    }

    private bool CheckComfort()//检查角色安抚对话状态
    {
        foreach (var value in GlobalData.Characters)
        {
            if (value.NotComfort || value.Dead)
            {
                continue;
            }
            if (value.Special1 || value.Special2)
            {
                return false;
            }
        }

        return true;
    }
    

    /// <summary>
    /// 场景亮化方法（延迟调用）
    /// 用于模拟昼夜交替中的"天亮"效果，调用 skip 对象的亮化方法
    /// </summary>
    void TurnLight()
    {
        talkSys.MiniCharacterManager.gameObject.GetComponent<MiniCharacterManager>().SetStand();
    }

    void ResetBack()
    {
        background.GetComponent<BackGroundMoving>().Re_set();
    }

    void ColseMask()
    {
        Mask.transform.parent.gameObject.SetActive(false);
    }
    
    void DownAnim()
    {
        //DownBar.GetComponent<Animator>().SetTrigger("Down");  // 底部栏隐藏动画
    }

    public void SetShop()
    {
        
        ShopTalk = false;
        talkSys.showText.CanShowText = true;
        if(talkSys.useNewSys)
        {
            GlobalData.NewTalkSysShowText.UnLockOutPut();
            GlobalData.NewTalkSysShowText.SetChoiceLine(0,true);
            GlobalData.NewTalkSysShowText.SetShopStatus(true);
        }
        Shop.gameObject.SetActive(true);
    }

    public void TalkStage()
    {
        if(day_num == 0)
            return;
        SwitchStageBar.SetActive(true);
    }

    //由存档系统调用设定阶段
    public void SetStage(int stage)
    {
        switch (stage)
        {
            case 0:
                start = true;
                talk = false;
                food = false;
                break;
            case 1:
                if (day_num == 0 )
                {
                    return;
                }
                start = false;
                talk = true;
                food = false;
                state.SetTrigger("SetTalk");
                if(day_num != 0) MiniCharacter.SetWalk();
                break;
            case 2:
                start = false;
                talk = false;
                food = true;
                state.SetTrigger("SetFood");
                break;
                
        }
    }
    
    

}

public static class GlobalData
{

    public static int Day { get; set; }
    public static MiniGameIntroManager MiniGameManager { get; set; }
    public static TalkSysShowText ShowText { get; set; }
    public static bool InShop { get; set; }
    public static GameObject CurrentSaveFileButton { get; set; }
    public static GameObject CurrentLoadFileButton { get; set; }
    public static TalkSystem TalkSystem { get; set; }
    public static Progress Progress { get; set; }
    public static List<Character> Characters => TalkSystem.characterComponentList;

    public static SkipText SkipButton { get; set; }
    public static int Stage { get; set; }

    public static bool AfterShop { get; set; }
    
    public static Manager Language { get; set; }
    public static NewTalkSysShowText NewTalkSysShowText { get; set; }
    
    public static HistoryManager History { get; set; }
    public static bool OnMiniGame { get; set; }
    public static AdditionAudioEffect AudioEffect { get; set; }
    public static EndingsManager EndingsManager {get; set; }
}

