/// <summary>
/// 玩家存档数据类，用于序列化存储游戏中的关键进度和状态
/// </summary>
[System.Serializable]
public class PlayerSaveData
{
    public string saveTime;          // 存档时间（用于在存档列表中显示）
    public string playerName;
    public int day;                  // 当前游戏天数（记录游戏进度阶段）
    public int stage;                // 当前游戏阶段（0:分配负重 1:对话阶段 2:分配食物）  
    public int food; 
    public int body;
    public bool amandeKillSelfTag;
    public bool[] characterDeadState = new bool[6];//角色死亡状态
    public int[] characterCarry = new int[6];
    public bool[] characterEatState = new bool[6];
    public int[] characterCarryTag = new int[6];
    public string[] deadBodyContainer = new string[7];
    public string[] currentDead = new string[5];
    public bool[] miniGamePlayState = new bool[20];
    


}
