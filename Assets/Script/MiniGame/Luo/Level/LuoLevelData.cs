using System.Collections.Generic;
using UnityEngine;

public enum LuoPipeType
{
    Straight,
    Tee,
    FourWay,
    Angle,
    Blind,
    Item,
    None
}

public enum Toward
{
    Up,
    Down,
    Left,
    Right,
}

[CreateAssetMenu(fileName = "LuoLevelData", menuName = "创建数据/新建洛尔坎关卡")]
public class LuoLevelData :ScriptableObject
{
    [System.Serializable]
    public class LevelData
    {
        [Header("管道设置")]
        public LuoPipeType pipe;
        [Range(0,3)] public int toward;
        [Header("起点设置")]
        public bool isStart;
        public Toward startPos;
        [Header("终点设置")]
        public bool isDestination;
        public Toward destinationPos;
    }
    public List<LevelData> level;
    public List<LuoPipeType> items;
    [Header("源头位置左右两边从上到下0-3")]
    [Range(0,3)] public int source1;
    [Range(0,3)] public int source2;
    [Range(1,2)] public int rank;

}
