using UnityEngine;
[CreateAssetMenu(fileName = "New_AimiData", menuName = "创建数据/新建艾米莉数据")]
public class AimiData : ScriptableObject
{
    public string description;
    public string en;
    [Range(1,5)]public int expression;
}
