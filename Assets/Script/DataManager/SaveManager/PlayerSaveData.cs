using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家存档数据类，用于序列化存储游戏中的关键进度和状态
/// 标记为[System.Serializable]以支持Unity的JsonUtility序列化
/// </summary>
[System.Serializable]
public class PlayerSaveData
{
    public int SaveID;               // 存档唯一标识ID（可用于区分多个存档）
    public string SaveTime;          // 存档时间（用于在存档列表中显示）
    public string PlayerName;        // 玩家名称（用于存档标识或游戏内显示）
    public int Day;                  // 当前游戏天数（记录游戏进度阶段）
    public int Body;                 // 身体相关数值（可能对应负重、体力等核心属性）
    public int Food;                 // 食物数量（生存类游戏核心资源）
    public bool AmandeKillSelf;      // Amande角色是否自杀的剧情标记
    public bool CarryBo;             // 是否携带Bo角色的状态标记（影响剧情/玩法）
    public int Stage;                // 当前游戏阶段（0:分配负重 1:对话阶段 2:分配食物）
    public List<string> DeadName = new List<string>();    // 已死亡角色名称列表（用于恢复剧情状态）
    public List<string> UsedName = new List<string>();    // 已使用（消耗）的角色名称列表（可能关联资源消耗）
    public bool InMain;              // 是否在主场景（区分主场景和教学场景的状态恢复）

    public int[] CharacterWeight = new int[6]; // 6个角色的负重标记数组（索引对应具体角色）
    public bool[] CharacterEat = new bool[6];  // 6个角色是否进食的标记数组（索引对应具体角色）
    public bool IsRaining;          // 当前是否下雨的环境状态标记
    public int[] CharacterWeightTag = new int[6]; // 角色负重标签列表（可能用于调试或显示）
    public bool InShop; //用于判断是否在商店场景
    public int ShopEvent; //判断商店是否发生事件

}
