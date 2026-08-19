using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoLevel", menuName = "创建数据/新建博金森关卡")]
public class BoLevelContainer : ScriptableObject
{
    [System.Serializable]
    public class Data
    {
        public GameObject Prefab;
    }
    
    public List<Data> data = new List<Data>();
    
}

public static class BoGlobalData
{
    public static bool Complete = false;
    public static RectTransform CurrentPiece {get; set; }
    public static MiniBoTalk TalkSys {get; set; }
    public static BoItemText itemText {get; set;}
    public static BoLevelAnim anim {get; set; }
    public static GameObject Button {get; set;}
}

