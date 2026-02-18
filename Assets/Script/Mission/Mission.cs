using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mission", menuName = "创建数据/新建任务数据")]
public class Mission : ScriptableObject
{
    // 单个任务的数据结构
    [System.Serializable]
    public class MissionData
    {
        public string name; // 任务名称
        public bool isComplete; // 任务完成状态
    }

    // 当天的所有任务列表
    public List<MissionData> missions = new List<MissionData>();
}
