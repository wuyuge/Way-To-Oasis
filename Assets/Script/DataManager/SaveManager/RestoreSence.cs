using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 仅负责将存档数据同步到游戏对象，或从游戏对象读取数据
/// 挂在主场景和教学场景的管理器对象上
/// </summary>
public class RestoreSence : MonoBehaviour
{
    public Manager food , finalFood , body , finalBody , playerName , amandeKillSelfTag , deadBodyContainer,currentDead,usedBody;
    public GameObject mainCanvas, teachCanvas;
    public Progress mainProgress, teachProgress;
    public TalkSystem mainTalkSys, teachTalkSys;
    public IntermissionManager mainInterMission;
    private PlayerSaveData _curData;
    public DayNightSystem mainDayNightSys;
    public MiniGameIntroManager miniGameIntro;
    public List<Button> interactButtons;
    ///<summary>
    /// 获取当前场景数据并返回存档数据对象
    /// </summary>
    public PlayerSaveData GetData()
    {
        PlayerSaveData data = new PlayerSaveData();
        data.saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        data.playerName = playerName.TxtLine[0];
        data.day = GlobalData.Day;
        data.stage = GlobalData.Stage;
        data.amandeKillSelfTag = amandeKillSelfTag.GeneralBool;
        data.afterShop = GlobalData.AfterShop;
        if (GlobalData.AfterShop)
        {
            //商店后角色反抗对话数据保存
            //0为没有反抗对话，1是sp1，2是sp2，3是进行过安抚对话
            for (int i = 0; i < GlobalData.TalkSystem.characterComponentList.Count; i++)
            {
                data.characterComfortState[i] = GlobalData.TalkSystem.characterComponentList[i].NotComfort;
                if (GlobalData.TalkSystem.characterComponentList[i].Special1)
                {
                    data.characterSpTalkState[i] = 1;
                    continue;
                }
                if (GlobalData.TalkSystem.characterComponentList[i].Special2)
                {
                    data.characterSpTalkState[i] = 2;
                    continue;
                }
                if (GlobalData.TalkSystem.characterComponentList[i].AfterSpecialTalk)
                {
                    data.characterSpTalkState[i] = 3;
                    continue;
                }
                data.characterSpTalkState[i] = 0;
            }
        }
        else
        {
            data.characterComfortState = new bool[6];
            data.characterSpTalkState = new int[6];
        }

        for (int i = 0; i < currentDead.TxtLine.Count; i++)
        {
            data.currentDead[i] = currentDead.TxtLine[i];
        }
        
        for (int i = 0; i < deadBodyContainer.TxtLine.Count; i++)
        {
            data.deadBodyContainer[i] = deadBodyContainer.TxtLine[i];
        }

        for (int i = 0; i < usedBody.TxtLine.Count; i++)
        {
            data.usedBodyContainer[i] = usedBody.TxtLine[i];
        }
        //TODO:保存小游戏是否可玩状态
        

        //食物和尸体的赋值
        switch (data.stage)
        {
            case 0:
                data.food = food.Weight;
                data.body = body.Weight;
                break;
            case 1:
            case 2:
                data.food = finalFood.Weight;
                data.body = finalBody.Weight;
                break;
            default:
                data.food = food.Weight;
                data.body = body.Weight;
                break;
        }

        for (int i = 0; i < GlobalData.TalkSystem.characterComponentList.Count; i++)
        {
            data.characterDeadState[i] = GlobalData.TalkSystem.characterComponentList[i].Dead;
            data.characterCarry[i] = GlobalData.TalkSystem.characterComponentList[i].weight.Weight;
            data.characterEatState[i] = GlobalData.TalkSystem.characterComponentList[i].weight.Eat;
            data.characterCarryTag[i] = GlobalData.TalkSystem.characterComponentList[i].weight.Weight_tag;
        }
        
        return data;
    }

    /// <summary>
    /// 用于恢复存档数据场景
    /// </summary>
    /// <param name="data">用于加载的存档数据</param>
    public void ApplyData(PlayerSaveData data)
    {
        _curData = data;
        playerName.TxtLine[0] = data.playerName;//设定玩家名称
        
        deadBodyContainer.TxtLine.Clear();
        for (int i = 0; i < data.deadBodyContainer.Length; i++)
        {
            if (data.deadBodyContainer[i] == string.Empty)
            {
                continue; 
            }
            deadBodyContainer.TxtLine.Add(data.deadBodyContainer[i]);//恢复持有尸体列表
        }

        usedBody.TxtLine.Clear();
        for (int i = 0; i < data.usedBodyContainer.Length; i++)//恢复使用尸体列表
        {
            if (data.usedBodyContainer[i] == string.Empty)
            {
                continue; 
            }
            usedBody.TxtLine.Add(data.usedBodyContainer[i]);
        }
        
        currentDead.TxtLine.Clear();
        for (int i = 0; i < data.currentDead.Length; i++)
        {
            if (data.currentDead[i] == string.Empty)
            {
                continue; 
            }
            currentDead.TxtLine.Add(data.currentDead[i]);//恢复幕间死亡者提示
        }
        
        amandeKillSelfTag.GeneralBool = data.amandeKillSelfTag;//恢复阿曼德自杀标记
        
        switch (data.stage)//设定食物，尸体数量
        {
            case 0:
                food.Weight = data.food;
                body.Weight = data.body;
                finalFood.Weight = 0;
                finalBody.Weight = 0;
                break;
            case 1:
            case 2:
                finalFood.Weight = data.food;
                finalBody.Weight = data.body;
                food.Weight = 0;
                body.Weight = 0;
                if (data.day == 0)
                {
                    finalFood.Weight = 21;
                    finalBody.Weight = 1;
                }
                break;
        }
        
        if (data.day == 0)
        {
            teachCanvas.SetActive(true);
            mainCanvas.SetActive(false);
            SetTeach();
        }
        else
        {
            teachCanvas.SetActive(false);
            mainCanvas.SetActive(true);
            SetMain();
        }
        
        foreach (var value in interactButtons)//设定教程手册等按钮为可点击状态
        {
            value.interactable = true;
        }
        
    }

    private void SetMain()
    {
        Debug.Log("设定主画布");
        mainProgress.day_num = _curData.day;//设定时间
        mainProgress.SetStage(_curData.stage);//设定阶段
        GlobalData.NewTalkSysShowText.LockOutPut();
        for (int i = 0; i < mainTalkSys.characterComponentList.Count; i++)//设定角色死亡，负重,吃东西状态
        {
            mainTalkSys.characterComponentList[i].Dead = _curData.characterDeadState[i];
            mainTalkSys.characterComponentList[i].weight.Weight = _curData.characterCarry[i];
            mainTalkSys.characterComponentList[i].weight.Eat = _curData.characterEatState[i];
            if (_curData.characterEatState[i] && _curData.stage == 2)
            {
                finalFood.Weight++;
            }
            mainTalkSys.characterComponentList[i].weight.Weight_tag = _curData.characterCarryTag[i];
        }

        
        if (_curData.day == 1 && (_curData.stage == 1 || _curData.stage == 2)) //day1对话阶段，禁止负重阶段起始对话
        {
            mainTalkSys.HideBar();
            mainInterMission.Lines.RemoveAt(0);
            mainTalkSys.showText.CanShowText = false;
            mainProgress.CanSwitch = true;
        }

        TutorialManager.TutorialIsShow = _curData.day == 1 && _curData.stage == 0;

        if (_curData.day == 2)
        {
            mainTalkSys.HideBar();
            mainTalkSys.showText.CanShowText = false;
            mainProgress.CanSwitch = true;
            mainProgress.ShopTalk = true;
        }

        if (_curData.day == 3)
        {
            if (_curData.stage != 0)
            {
                mainInterMission.Lines.RemoveAt(2);
                mainTalkSys.HideBar();
            }
            mainTalkSys.showText.CanShowText = false;
            mainProgress.CanSwitch = true;
        }
        
        if (_curData.stage == 2)//设定时间状态
        {
            mainDayNightSys.SetSecond();
        }
        else if (_curData.stage == 1)
        {
            mainDayNightSys.SetFirst(); 
        }

        if (_curData.afterShop)
        {
            for (int i = 0; i < _curData.characterSpTalkState.Length; i++)
            {
                GlobalData.TalkSystem.characterComponentList[i].NotComfort = _curData.characterComfortState[i];
                if (_curData.characterSpTalkState[i] == 0)
                {
                    continue;
                }

                if (_curData.characterSpTalkState[i] == 1)
                {
                    GlobalData.TalkSystem.characterComponentList[i].Special1 = true;
                    continue;
                }
                
                if (_curData.characterSpTalkState[i] == 2)
                {
                    GlobalData.TalkSystem.characterComponentList[i].Special2 = true;
                    continue;
                }
                
                if (_curData.characterSpTalkState[i] == 3)
                {
                    GlobalData.TalkSystem.characterComponentList[i].NotComfort = true;
                }
            }
        }
        
    }
    
    private void SetTeach()
    {
        Debug.Log("设定教学画布");
        teachProgress.day_num = _curData.day;//设定时间
        teachProgress.SetStage(_curData.stage);//设定阶段
        if (_curData.stage == 2)
        {
            teachTalkSys.showText.CanShowText = false;
            teachTalkSys.HideBar();
            teachProgress.CanSwitch = true;
            teachTalkSys.CharacterImageManager.gameObject.SetActive(false);
        }
    }
    

}