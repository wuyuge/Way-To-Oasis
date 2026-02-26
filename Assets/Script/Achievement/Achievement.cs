using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementList",menuName = "创建数据/成就列表")]
public class Achievement : ScriptableObject
{
    public List<string> achievements;
}