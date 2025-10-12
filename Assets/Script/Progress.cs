using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

/// <summary>
/// 游戏进度管理器
/// 负责控制游戏阶段切换（开始阶段/对话阶段/进食阶段）、日期更新、幕间对话触发、背景滚动状态控制等核心逻辑
/// </summary>
public class Progress : MonoBehaviour
{
    /// <summary>
    /// 当前游戏天数
    /// </summary>
    public int day_num = 1;

    [Header("判断状态 - 游戏当前所处阶段标记")]
    /// <summary>
    /// 是否处于【开始阶段】（初始阶段，未进入对话/进食）
    /// </summary>
    public bool start;
    /// <summary>
    /// 是否处于【对话阶段】
    /// </summary>
    public bool talk;
    /// <summary>
    /// 是否处于【进食阶段】
    /// </summary>
    public bool food;
    //教学关是否可以跳过阶段
    public bool can_skip;
    /// <summary>
    /// 是否允许切换游戏阶段（用于控制阶段切换的触发条件）
    /// </summary>
    public bool CanSwitch;
    /// <summary>
    /// 是否允许触发对话（控制对话系统的激活状态）
    /// </summary>
    public bool CanTalk;
    //是否触发商店对话
    public bool ShopTalk;

    [Tooltip("文字描述 - 关联UI文本组件，显示重量、对话、进食、日期信息")]
    /// <summary>
    /// 重量显示文本（UI组件）
    /// </summary>
    public TextMeshProUGUI weight;
    /// <summary>
    /// 对话阶段提示文本（UI组件）
    /// </summary>
    public TextMeshProUGUI talk_t;
    /// <summary>
    /// 进食阶段提示文本（UI组件）
    /// </summary>
    public TextMeshProUGUI food_t;
    /// <summary>
    /// 日期显示文本（UI组件，格式：Day X）
    /// </summary>
    public TextMeshProUGUI day;

    [Header("游戏对象 - 关联核心功能预制体/组件")]
    /// <summary>
    /// 跳过按钮/明暗控制对象（用于切换场景明暗状态）
    /// </summary>
    public GameObject skip;
    /// <summary>
    /// 底部功能栏（可能包含背包、道具管理等UI）
    /// </summary>
    public GameObject DownBar;
    /// <summary>
    /// 背景对象（关联背景滚动脚本 BackGroundMoving）
    /// </summary>
    public GameObject background;
    /// <summary>
    /// 对话栏（关联对话系统脚本 TalkSystem）
    /// </summary>
    public GameObject TalkBar;
    /// <summary>
    /// 对象管理器（管理道具携带、背包状态等）
    /// </summary>
    public GameObject ObjectManager;

    public GameObject Shop;
    [Tooltip("幕间切换延迟")]
    public float Delay;

    [Header("插在各个阶段的幕间 - 不同阶段触发的对话数据列表")]
    /// <summary>
    /// 【开始阶段前】的幕间对话列表（按天数索引，对应每天开始前的对话）
    /// </summary>
    public List<Manager> beforeStart;
    /// <summary>
    /// 【进食阶段前】的幕间对话列表（按天数索引，对应每天进入进食前的对话）
    /// </summary>
    public List<Manager> beforeFood;
    /// <summary>
    /// 【进食阶段后】的幕间对话列表（按天数索引，对应每天进食后的对话）
    /// </summary>
    public List<Manager> afterFood;

    [Header("背包 - 道具重量管理相关")]
    /// <summary>
    /// 最终达到的食物重量
    /// </summary>
    public Manager Final_Food;
    /// <summary>
    /// 最终达到的尸体重量
    /// </summary>
    public Manager Final_Body;
    /// <summary>
    /// 当前拥有的食物重量
    /// </summary>
    public Manager Have_Food;
    /// <summary>
    /// 当前的尸体重量
    /// </summary>
    public Manager Have_Body;

    public GameObject Mask;
    public DayNightSystem DNSys;

    // 用于判断day0遮罩是否用过
    private bool Day0MaskUsed = false;

    private bool TipsWeight = false;
    private TalkSystem talkSys;
    [Header("阿曼德自杀判断")]
    public Manager AmandeKillSelf;


    private AudioManager AudioManager;

    /// <summary>
    /// 初始化方法 - 游戏启动时执行
    /// 1. 绑定UI文本组件 2. 设置初始阶段的文本颜色 3. 触发当天开始前的幕间对话
    /// </summary>
    void Start()
    {
        AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        talkSys = TalkBar.GetComponent<TalkSystem>();
        // 初始化日期显示（格式：Day X）
        day.text = "Day " + day_num;

        // 动态绑定父对象下的UI文本组件（避免手动拖拽丢失，增强鲁棒性）
        weight = gameObject.transform.parent.Find("Weight").GetComponent<TextMeshProUGUI>();
        talk_t = gameObject.transform.parent.Find("Talk").GetComponent<TextMeshProUGUI>();
        food_t = gameObject.transform.parent.Find("Food").GetComponent<TextMeshProUGUI>();
        

        // 根据初始阶段设置文本颜色（高亮当前阶段，其他阶段半透明）
        if (start)
        {
            weight.color = new Color32(0, 0, 0, 255);   // 开始阶段（重量文本）高亮
            talk_t.color = new Color32(0, 0, 0, 120);  // 对话文本半透明
            food_t.color = new Color32(0, 0, 0, 120);  // 进食文本半透明
        }
        else if (talk)
        {
            weight.color = new Color32(0, 0, 0, 120);  // 重量文本半透明
            talk_t.color = new Color32(0, 0, 0, 255);  // 对话阶段高亮
            food_t.color = new Color32(0, 0, 0, 120);  // 进食文本半透明
        }
        else if (food)
        {
            weight.color = new Color32(0, 0, 0, 120);  // 重量文本半透明
            talk_t.color = new Color32(0, 0, 0, 120);  // 对话文本半透明
            food_t.color = new Color32(0, 0, 0, 255);  // 进食阶段高亮
        }
        if (day_num != 0)
            beforeStart[day_num] = GetComponent<IntermissionManager>().AddTextLine("BeforeStart");
        // 触发当天开始前的幕间对话（若存在对应天数的对话数据）
        if (beforeStart[day_num] != null)
        {
            CanTalk = true;  // 允许触发对话
            // 将当前天数的对话数据赋值给对话系统
            talkSys.Talklines[day_num] = beforeStart[day_num];
            talkSys.line = 0;  // 重置对话行数到第一行
            _ = talkSys.ShowText();  // 启动对话显示（异步执行）
            DownBar.GetComponent<Animator>().SetTrigger("Down");  // 底部栏播放"向下"动画（可能隐藏底部栏）
        }

    }



    public void FixedUpdate()
    {
        if(day_num == 0 && food && !Day0MaskUsed)
        {
            bool allEat = DownBar.GetComponent<ObjectManager>().CheckEat(false);
            if(allEat)
            {
                Day0MaskUsed = true;
                Mask.transform.parent.Find("TechText").GetComponent<TextMeshProUGUI>().text = "点击按钮结束分配食物环节";
                Mask.GetComponent<Unmask>().fitTarget = gameObject.GetComponent<RectTransform>();
                Mask.transform.parent.gameObject.SetActive(true);
                

            }
        }
    }


    /// <summary>
    /// 阶段切换核心方法
    /// 根据当前游戏阶段（start/talk/food），切换到下一个阶段，并执行对应逻辑
    /// 包含：状态标记更新、UI文本颜色更新、背景滚动控制、幕间对话触发、日期更新等
    /// </summary>
    public void SwtichProgress()
    {
        AudioManager.AudioPlayer("Click");
        if(day_num > 3)
        {
            GameObject.Find("EndingsManager").GetComponent<EndingsManager>().ToEnd("Demo-End");
        }
        // 1. 从【开始阶段】切换到【对话阶段】
        if (start && CanSwitch)
        {
            // 通知对象管理器更新道具携带数量（可能同步背包显示）
            if (!TipsWeight)
            {
                bool allCarry = ObjectManager.GetComponent<ObjectManager>().SetCarryNum(true);
                if (!allCarry)
                {
                    TipsWeight = true;
                    Mask.GetComponent<Unmask>().fitTarget = DownBar.transform.Find("MaskLayer").GetComponent<RectTransform>();
                    Mask.transform.parent.Find("TechText").GetComponent<TextMeshProUGUI>().text = "没有被分配的物品将会被丢弃";
                    Mask.transform.parent.gameObject.SetActive(true);
                    
                    return;
                }
            }
            else
                ObjectManager.GetComponent<ObjectManager>().SetCarryNum();

            if (DNSys.time < DNSys.Frist - 0.005f) return;
            Debug.Log("开启背景滚动");
            // 激活背景滚动（调用 BackGroundMoving 脚本的 open 状态）
            background.GetComponent<BackGroundMoving>().open = true;

            // 更新UI文本颜色（对话阶段高亮，开始阶段半透明）
            weight.color = new Color32(0, 0, 0, 120);
            start = false;  // 退出开始阶段

            
            

            talk = true;  // 进入对话阶段
            talk_t.color = new Color32(0, 0, 0, 255);  // 对话文本高亮
        }

        // 2. 从【对话阶段】切换到【进食阶段】
        else if (talk && CanSwitch)
        {
            if (DNSys.time < DNSys.Second - 0.005f) return;
            // 商店页面调用 还会加上转场效果（待做）
            if (day_num == 2 || day_num == 5 || day_num == 7)
            {
                if(ShopTalk)
                {
                    ShopTalk = false;
                    Shop.gameObject.SetActive(true);
                    return;
                }
            }




            if (day_num == 0 && !can_skip)
            {

                Mask.transform.parent.gameObject.SetActive(true);
                Mask.transform.parent.Find("TechText").GetComponent<TextMeshProUGUI>().text = "请与所有人的对话一次";
                
                return;

            }
            
            if(day_num != 0)
            beforeFood[day_num] = GetComponent<IntermissionManager>().AddTextLine("BeforeFood");
            // 触发进食前的幕间对话（若存在对应天数的对话数据）
            if (beforeFood[day_num] != null)
            {
                CanTalk = true;  // 允许触发对话
                // 赋值对话数据到对话系统
                talkSys.Talklines[day_num] = beforeFood[day_num];
                talkSys.line = 0;  // 重置对话行数
                _ = talkSys.ShowText();  // 启动对话显示
                TalkBar.GetComponent<Animator>().SetTrigger("up");
                if (TalkBar.transform.position.y == 0)
                {
                    DownAnim();
                }  // 底部栏隐藏动画
                  // 对话栏显示动画（向上弹出）
            }
            

            // 更新UI文本颜色（进食阶段高亮，对话阶段半透明）
            talk_t.color = new Color32(0, 0, 0, 120);
            food_t.color = new Color32(0, 0, 0, 255);
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
        }

        // 3. 从【进食阶段】切换到【下一天的开始阶段】
        else if (food && CanSwitch)
        {
            if (DNSys.on) return;
            bool allEat = false;
            // 特殊逻辑：第0天（可能是教程天）的进食完成判断
            if (day_num == 0)
            {
                // 检查是否满足进食条件（若未满足，不执行阶段切换）
                if (!DownBar.GetComponent<ObjectManager>().CheckEat(false))
                {
                    talkSys.Mask(DownBar.transform.Find("MaskLayer").gameObject,"AllEat");
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
                        _ = talkSys.ShowText();  // 启动对话显示
                        TalkBar.GetComponent<Animator>().SetTrigger("up");// 对话栏显示动画
                        //if (TalkBar.transform.position.y == 0)
                        //{
                        //    DownAnim();
                        //}  // 底部栏隐藏动画

                        afterFood[day_num] = null;  // 清空当前天数的对话数据（避免重复触发）
                        return;  // 先显示对话，暂不执行后续阶段切换（对话结束后需重新触发切换）
                    }

                }
                else
                {
                    talkSys.Talklines[day_num] = afterFood[day_num];
                    talkSys.line = 0;  // 重置对话行数
                    _ = talkSys.ShowText();  // 启动对话显示
                    TalkBar.GetComponent<Animator>().SetTrigger("up");  // 对话栏显示动画
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
                if (day_num > 3)
                {
                    GameObject.Find("EndingsManager").GetComponent<EndingsManager>().ToEnd("Demo-End");
                }
            }
            if (day_num != 0)
            {

                skip.GetComponent<Skip>().TurnDark();  // 若未暗化，执行暗化
                Invoke("ResetBack",0.4f);
                Invoke("TurnLight", 0.8f);  // 延迟1秒后执行亮化（Invoke 用于延迟调用方法）
            }

            // 更新UI文本颜色（开始阶段高亮，进食阶段半透明）
            food_t.color = new Color32(0, 0, 0, 120);
            weight.color = new Color32(0, 0, 0, 255);
            start = true;  // 进入下一天的开始阶段
            talk = false;  // 退出对话阶段
            food = false;  // 退出进食阶段
            day_num += 1;  // 天数+1（进入下一天）
            day.text = "Day " + day_num;  // 更新日期显示
            if(day_num == 2 || day_num == 5 || day_num == 7)
            {
                ShopTalk = true;
            }

            // 非第0天的特殊处理：场景明暗切换（先暗后亮，模拟昼夜交替）
            if (AmandeKillSelf.GeneralBool)
            {
                KillAmande();
            }
            if (day_num != 0)
                beforeStart[day_num] = GetComponent<IntermissionManager>().AddTextLine("BeforeStart");

            // 触发下一天开始前的幕间对话（若存在对应天数的对话数据）
            if (beforeStart[day_num] != null)
            {
                CanTalk = true;  // 允许触发对话
                // 赋值对话数据到对话系统
                talkSys.Talklines[day_num] = beforeStart[day_num];
                talkSys.line = 0;// 重置对话行数
                Invoke("Talk", 0.2f);
                  
                
                // 启动对话显示
                TalkBar.GetComponent<Animator>().SetTrigger("start");  // 对话栏显示动画
                if (TalkBar.transform.position.y == 0)
                {
                    DownAnim();
                }
                

            }
            //else
            //{
            //    TalkBar.GetComponent<Animator>().SetTrigger("down");
            //}
            
            
            // 通知对象管理器重置道具携带状态（可能将携带道具放回背包）
            ObjectManager.GetComponent<ObjectManager>().ReturnCarry();
            ObjectManager.GetComponent<ObjectManager>().RestTag();
            // 重置背景状态（调用 BackGroundMoving 的 Re_set 方法，还原背景初始位置）
            
            foreach(GameObject g in DownBar.GetComponent<ObjectManager>().Character_List)
            {
                g.GetComponent<Character>().EnableTalk();
            }



        }
    }


    private void KillAmande()
    {
        Final_Body.Weight += 1;
        talkSys.DeadName.TxtLine.Add("阿曼德Poison");
        talkSys.CharacterList[4].GetComponent<Character>().Dead = true;
        talkSys.CharacterList[4].transform.Find("Toggle").gameObject.SetActive(false);
    }

    void SetComfort()
    {
        foreach (GameObject g in DownBar.GetComponent<ObjectManager>().Character_List)
        {
            if(!g.GetComponent<Character>().AfterSpecialTalk && (g.GetComponent<Character>().Special1 || g.GetComponent<Character>().Special2))
            {
                g.GetComponent<Character>().NotComfort = true;
                g.transform.Find("Attention").gameObject.SetActive(false);
                g.transform.Find("Attention2").gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 场景亮化方法（延迟调用）
    /// 用于模拟昼夜交替中的"天亮"效果，调用 skip 对象的亮化方法
    /// </summary>
    void TurnLight()
    {
        skip.GetComponent<Skip>().TurnBright();
    }

    void ResetBack()
    {
        background.GetComponent<BackGroundMoving>().Re_set();
    }

    void ColseMask()
    {
        Mask.transform.parent.gameObject.SetActive(false);
    }

    void Talk()
    {
        _ = talkSys.ShowText();
    }
    void DownAnim()
    {
        //DownBar.GetComponent<Animator>().SetTrigger("Down");  // 底部栏隐藏动画
    }

}