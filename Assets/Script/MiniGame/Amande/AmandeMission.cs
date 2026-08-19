using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AmandeMission", menuName = "创建数据/新建阿曼德任务")]
public class AmandeMission : ScriptableObject
{
    [System.Serializable]
    public class MissionState
    {
        public MedicineType medicine;
        public bool composed;
        public string targetCn;
        public string targetEn;
    }
    public List<MissionState> missions;
}
