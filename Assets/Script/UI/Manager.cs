using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New TextData",menuName = "创建数据/新建对话数据")]
public class Manager : ScriptableObject
{
    
    public List<string> TxtLine = new List<string>();
    [System.Serializable]
    public class TextData
    {
        [Header("文本设置")] 
        public string cn;
        public string en;
        
        [Header("文本分支")] 
        public bool cnHaveBranch;
        public List<string> cnBranch;
        public bool enHaveBranch;
        public List<string> enBranch;
        
        [Space(5)]
        [Header("============说话人标记===========")]
        public bool isPlayerTalking;
        public bool isAside;
        [Space(5)]
        [Header("标记符设置")]
        public List<Code> codes;
        public bool onlyCode;
        [Header("说话人设置")]
        public CharacterType speaker;
        [Range(0,7)]public int expression;
        [Header("CG设置")] public bool showCg;
        [Range(0,12)]public int cgNum;
        [Header("迷你游戏设置")]
        public CharacterType minigameType;
        [Header("未安抚")]
        public CharacterType uncomfortType;
        [Header("播放音效设置")]
        public AudioEffectType audioEffect;

    }

    public List<TextData> data = new List<TextData>();
    
    
    public Manager Option1;
    public Manager Option2;
    public Manager Option3;
    public Manager SpecialTalk;
    public Manager SpecialTalk2;
    public int Weight;
    public int Weight_tag;
    [FormerlySerializedAs("Day1Eat")] public bool Eat;
    public bool GeneralBool;
    public bool isEn;

}


public enum Code
{
    
    CheckAimiDead,
    CheckAimiEat,
    CheckAmandeKillSelf,
    CheckBoDead,
    CheckBoDeadTime,
    CheckDay0Talk,
    CheckDeadSex,
    CheckHaveBoBody,
    CheckMiniA,
    CheckMiniB,
    CheckMiniL,
    CheckMinia,
    CheckMinil,
    CheckOnlyAmandeDead,
    CheckShopEvent,
    CheckSomeBodyDead,
    Choice,
    ClickCloseMaskOff,
    ClickCloseMaskOn,
    CloseCharacterImage,
    CloseCharacterTalkBox,
    DisableMiniCharacter,
    DownTalkBox,
    End,
    ExchangeBody,
    KillSomeBody,
    LockSkip,
    LockSwitchStage,
    NextDay,
    OffCampLight,
    OffMiniCharacterImage,
    OffMiniMode,
    OffShop,
    OnMiniMode,
    OnShop,
    OpenSwitchStage,
    SetAmanadeTwiceTalkOff,
    SetAmanadeTwiceTalkOn,
    SetDay0SwitchStage,
    SetMiniCharacterSit,
    ShopChoice,
    ShowCharaName,
    ShowCharacterTalkBox,
    SpecialChoice,
    SurviveAlone,
    SurviveTwo,
    SwitchToMain,
    TechClick,
    TechClose,
    TechComfort,
    TechFood,
    TechMenu,
    TechOver,
    TechRight,
    TechTalk,
    TechWeight,
    TogetherGoDark,
    TurnDark,
    UnLockSkip,
    UpCharacterBox,
    UseBoBody,
    HideCg,
    UpTalkBox
}

public enum CharacterType
{
    Player,
    阿曼德,
    艾米莉,
    博金森,
    莱文,
    洛尔坎,
    商人
}