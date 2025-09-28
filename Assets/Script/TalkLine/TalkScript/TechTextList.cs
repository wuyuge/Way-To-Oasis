using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechTextList : MonoBehaviour
{
    [System.Serializable]
    public class TechText
    {
        public string name;
        public string text;
    }


    public List<TechText> TextList = new List<TechText>();
}
