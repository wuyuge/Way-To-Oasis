using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 仅负责将存档数据同步到游戏对象，或从游戏对象读取数据
/// 挂在主场景和教学场景的管理器对象上
/// </summary>
public class RestoreSence : MonoBehaviour
{
    [Header("通用引用")]
    public Manager DeadName;         // 已死亡角色名称管理器
    public Manager UsedBody;         // 已使用角色名称管理器
    public Manager AmandeKillSelf;   // Amande自杀状态管理器
    public Manager PlayerName;       // 玩家名称管理器
    public Manager HaveBody;         // 当前拥有的身体数值
    public Manager HaveFood;         // 当前拥有的食物数值
    public Manager FinalBody;        // 最终身体数值
    public Manager FinalFood;        // 最终食物数值
    public Character CarryBo;        // 携带Bo角色引用
    public Manager[] CharacterWeight = new Manager[6]; // 6个角色负重管理器
    public RandomRain RainSystem;    // 下雨系统
    public DayNightSystem DayNightSystem; // 昼夜系统
    public Manager ShopExchange, ShopKill;

    [Header("主场景引用")]
    public GameObject MainCanvas;    // 主场景UI画布
    public Progress MainProgress;    // 主场景进度管理器
    public GameObject MainMask;      // 主场景遮罩
    public List<GameObject> MainCharacters; // 主场景角色列表
    public GameObject MiniCharacterContainer; // 主场景迷你角色容器
    public GameObject MainDownBar; // 主场景下方信息栏
    public GameObject MainTalkBar; // 主场景对话信息栏
    //public GameObject MainBackground; // 主场景背景
    public GameObject Shop;

    [Header("教学场景引用")]
    public GameObject TechCanvas;    // 教学场景UI画布
    public Progress TechProgress;    // 教学场景进度管理器
    public GameObject TechMask;      // 教学场景遮罩
    public List<GameObject> TechCharacters; // 教学场景角色列表
    public GameObject TechDownBar; // 教学场景下方信息栏
    public GameObject TechTalkBar; // 教学场景对话信息栏


    ///<summary>
    /// 获取当前场景数据并返回存档数据对象
    /// </summary>
    public PlayerSaveData GetData()
    {
        PlayerSaveData data = new PlayerSaveData();

        #region 通用数据获取

        data.DeadName = DeadName.TxtLine = new List<string>();
        data.UsedName = UsedBody.TxtLine = new List<string>();
        data.AmandeKillSelf = AmandeKillSelf.GeneralBool;
        data.PlayerName = PlayerName.TxtLine[0];
        data.CarryBo = CarryBo.CantWeight;
        data.IsRaining = RainSystem.isRaining;
        data.InMain = MainCanvas.activeSelf;

        //保存角色进食数据
        for (int i = 0; i < 6; i++)
        {
            data.CharacterEat[i] = CharacterWeight[i].Day1Eat;
        }

        #endregion

        #region 需要区分教学场景和主场景的数据的获取
        if (data.InMain)
        {
            data.Day = MainProgress.day_num;
            if (MainProgress.start) data.Stage = 0;
            else if (MainProgress.talk) data.Stage = 1;
            else if (MainProgress.food) data.Stage = 2;
            #region 保存角色负重数据
            if (MainProgress.start)
            {
                int index = 0;
                foreach (Manager m in CharacterWeight)
                {
                    data.CharacterWeight[index] = m.Weight;
                    data.CharacterWeightTag[index] = m.Weight_tag;
                    index++;
                }
            }
            else
            {
                int index = 0;
                Debug.Log("非负重阶段检测负重值");
                foreach (GameObject g in MainCharacters)
                {
                    if (g.GetComponent<Character>().weight3.GetComponent<Image>().color == Color.red)
                    {
                        data.CharacterWeight[index] = 3;
                        index++;
                        continue;
                    }
                    else if (g.GetComponent<Character>().weight2.GetComponent<Image>().color == Color.red)
                    {
                        data.CharacterWeight[index] = 2;
                        index++;
                        continue;
                    }
                    else if (g.GetComponent<Character>().weight1.GetComponent<Image>().color == Color.red)
                    {
                        data.CharacterWeight[index] = 1;
                        index++;
                        continue;
                    }
                    index++;

                }



            }

            #endregion

        }
        else
        {
            data.Day = TechProgress.day_num;
            if (TechProgress.start) data.Stage = 0;
            else if (TechProgress.talk) data.Stage = 1;
            else if (TechProgress.food) data.Stage = 2;
        }

        #endregion

        #region 食物尸体数据获取
        switch (data.Stage)
        {
            case 0:
                data.Food = HaveFood.Weight;
                data.Body = HaveBody.Weight;
                break;
            case 1:
            case 2:
                data.Food = FinalFood.Weight;
                data.Body = FinalBody.Weight;
                break;
        }
        #endregion

        #region 商店数据获取
        switch (data.Day)
        {
            case 2:
            case 5:
            case 7:
                if (Shop.activeSelf) data.InShop = true; 
                break;
            default:
                data.InShop = false;
                break;
        }

        if (ShopExchange.GeneralBool) data.ShopEvent = 1;
        else if (ShopKill.GeneralBool) data.ShopEvent = 2;
        else data.ShopEvent = 0;
        #endregion

        //获取当前时间
        data.SaveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        return data;
    }

    /// <summary>
    /// 用于恢复存档数据场景
    /// </summary>
    /// <param name="data">用于加载的存档数据</param>
    public void ApplyData(PlayerSaveData data)
    {
        Debug.Log("正在恢复存档数据...");
        //恢复教学场景或主场景的画布状态
        MainCanvas.SetActive(data.InMain);
        TechCanvas.SetActive(!data.InMain);
        //执行Day0特殊设置
        if (data.Day == 0 && data.Stage == 2)
        {
            Day0SpecialSet(data);
        }

        #region 恢复Progress脚本相关数据
        Progress TempProgress;
        //设置使用的progress脚本
        if(data.InMain) TempProgress = MainProgress;
        else TempProgress = TechProgress;


        //执行Day0后特殊逻辑
        if(data.InMain) AfterDay0SpecialSet(data,TempProgress);

        //启用商店对话
        switch (data.Day)
        {
            case 2:
            case 5:
            case 7:
                MainProgress.ShopTalk = true;
                break;
        }


        


        //恢复数据
        TempProgress.day_num = data.Day;
        switch (data.Stage)
        {
            case 0:
                TempProgress.start = true;
                TempProgress.talk = false;
                TempProgress.food = false;
                StageSpecialSet(data);
                break;
            case 1:
                TempProgress.start = false;
                TempProgress.talk = true;
                TempProgress.food = false;
                StageSpecialSet(data);
                break;
            case 2:
                TempProgress.start = false;
                TempProgress.talk = false;
                TempProgress.food = true;
                TempProgress.CanSwitch = true;
                StageSpecialSet(data);
                break;
        }


        #endregion


        #region 恢复食物尸体等数据

        DeadName.TxtLine = data.DeadName;
        UsedBody.TxtLine = data.UsedName;
        HaveFood.Weight = 0;
        HaveBody.Weight = 0;
        FinalFood.Weight = 0;
        FinalBody.Weight = 0;

        switch (data.Stage)
        {
            case 0:
                HaveBody.Weight = data.Body;
                HaveFood.Weight = data.Food;
                
                break;
            case 1:
                
                FinalBody.Weight = data.Body;
                FinalFood.Weight = data.Food;
                
                break;
            case 2:
                FinalBody.Weight = data.Body;
                FinalFood.Weight = data.Food;
                #region 恢复吃饭状态
                for (int i = 0; i < 6; i++)
                {
                    MainCharacters[i].GetComponent<Character>().weight.Day1Eat = data.CharacterEat[i];
                    if (data.CharacterEat[i]) FinalFood.Weight++;
                    Debug.Log($"恢复角色{MainCharacters[i].name}的进食状态为{data.CharacterEat[i]}");
                }
                #endregion
                break;
        }

        if(data.InMain)
        {
            MainDownBar.GetComponent<ObjectManager>().Food_Text.text = data.Food.ToString();
            MainDownBar.GetComponent<ObjectManager>().Body_Text.text = data.Body.ToString();
        }
        else
        {
            TechDownBar.GetComponent<ObjectManager>().Food_Text.text = data.Food.ToString();
            TechDownBar.GetComponent<ObjectManager>().Body_Text.text = data.Body.ToString();
        }


        #endregion

        #region 恢复角色负重状态
        for (int i = 0; i < 6; i++)
        {
            CharacterWeight[i].Weight = data.CharacterWeight[i];
            CharacterWeight[i].Weight_tag = data.CharacterWeightTag[i];
            Debug.Log($"恢复角色{MainCharacters[i].name}的负重为{data.CharacterWeight[i]}，标签为{data.CharacterWeightTag[i]}");
        }
        #endregion


        switch (data.ShopEvent)
        {
            case 0:
                break;
            case 1:
                MainTalkBar.GetComponent<TalkSystem>().ShowExchangeTalk();
                break;
            case 2:
                MainTalkBar.GetComponent<TalkSystem>().ShowKillTalk();
                break;
        }

        #region 恢复商店场景
        if (data.InShop)
        {
            Shop.SetActive(true);
        }
        #endregion
    }

    /// <summary>
    /// 用于Day0分配食物时的特殊逻辑处理
    /// </summary>
    /// <param name="data">加载的存档数据</param>

    void Day0SpecialSet(PlayerSaveData data)
    {

        if (data.Day != 0) return;

        TechMask.SetActive(false);
        TechTalkBar.GetComponent<Animator>().SetTrigger("down");
        TechTalkBar.GetComponent<TalkSystem>().Talklines[0] = null;
        TechTalkBar.GetComponent<TalkSystem>().Ban = true;
        _ = Task.Delay(1000);
        TechTalkBar.GetComponent<TalkSystem>().Ban = false;
        Debug.Log("Day0特殊设置完成");
        return;





    }


    //TODO：Day1加载各阶段时的特殊逻辑

    void AfterDay0SpecialSet(PlayerSaveData data, Progress progress)
    {

        if (data.Stage == 1 || data.Stage == 2)
        {
            IntermissionManager intermission = progress.gameObject.GetComponent<IntermissionManager>();
            for (int i = 0; i < intermission.Lines.Count; i++)
            {
                if (intermission.Lines[i].Day == data.Day && intermission.Lines[i].Stage == "BeforeStart")
                {
                    intermission.Lines.RemoveAt(i);
                    MainTalkBar.GetComponent<Animator>().SetTrigger("down");

                }
            }


        }
        if(data.Day == 2)
        {
            MainTalkBar.GetComponent<Animator>().SetTrigger("down");
        }

        


    }

    /// <summary>
    /// 用于加载存档不同阶段时必要的特殊逻辑处理
    /// </summary>
    /// <param name="data">玩家存档数据</param>
    void StageSpecialSet(PlayerSaveData data)
    {
        if (!data.InMain) return;
        switch (data.Stage)
        {
            case 0:
                //Day1开始阶段特殊逻辑
                break;
            case 1:
            #region Day1对话阶段特殊逻辑
                MiniCharacterContainer.GetComponent<MiniCharacterManager>().SetWalk();
                DayNightSystem.time = DayNightSystem.Frist;

                break;
            #endregion
            case 2:
                //Day1食物阶段特殊逻辑
                break;
        }



    }



}