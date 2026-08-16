
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_BoData", menuName = "创建数据/新建博金森对话数据")]
public class BoItemData : ScriptableObject
{
    [System.Serializable]
    public class Data
    {
        public string cn;
        public string en;
        [Range(1,3)]public int expression;
    }
    public List<Data> data = new List<Data>();
    public Data description;
}
