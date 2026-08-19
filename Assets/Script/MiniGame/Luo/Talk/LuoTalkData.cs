using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new LuoTalk", menuName = "创建数据/新建洛尔坎对话数据")]
public class LuoTalkData : ScriptableObject
{
    [System.Serializable]
    public class TalkData
    {
        public string cn;
        public string en;
        public Express express;
        public bool cnHaveBranch;
        public List<string> cnBranch;
        public bool enHaveBranch;
        public List<string> enBranch;
    }
    
    public enum Express
    {
        平静,
        嫌弃,
        紧张
    }
    
    public List<TalkData> talkDatas = new List<TalkData>();
    public bool showed;
}
