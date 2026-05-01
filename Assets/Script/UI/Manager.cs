using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New TextData",menuName = "创建数据/新建对话数据")]
public class Manager : ScriptableObject
{
    
    public List<string> TxtLine = new List<string>();
    public Manager Option1;
    public Manager Option2;
    public Manager Option3;
    public Manager SpecialTalk;
    public Manager SpecialTalk2;
    public int Weight;
    public int Weight_tag;
    [FormerlySerializedAs("Day1Eat")] public bool Eat;
    public bool GeneralBool;


}
