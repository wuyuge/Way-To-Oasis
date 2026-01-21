using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Level",menuName = "Create MiniGame Level/Luo")]
public class LuoLevelFile : ScriptableObject
{
    [System.Serializable]
    public class PipeTypeC
    {
        public PipeType type;
        public int state;
        public bool isStartPoint,startIsVertical,isDestination,destinationIsVertical;
    }
    public PipeTypeC[] pipeTypeList = new PipeTypeC[16];
    public List<GameObject> replaceItems;
}
