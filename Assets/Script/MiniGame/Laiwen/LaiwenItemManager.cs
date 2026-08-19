using UnityEngine;
[CreateAssetMenu(fileName = "New LaiwenData",menuName = "创建数据/新建莱文游戏数据")]
public class LaiwenItemManager : ScriptableObject
{
    public string dataName;
    public string context;
    public string enContext;
    [Range(0,5)]public int expression;


}


