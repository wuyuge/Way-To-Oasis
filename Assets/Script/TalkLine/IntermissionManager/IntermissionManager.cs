using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermissionManager : MonoBehaviour
{

    [System.Serializable]
    public class TextLine
    {
        public int Day;
        public Manager Text;
        public string Stage;
        public bool Have_Condition;
        public bool Have_Show;
    }


    public List<TextLine> Lines;
    [Header("判断条件")]
    public Manager AmandeKillSelf;
    public List<Character> characters;
    private Progress AddObj;

    private void Start()
    {
        AddObj = GetComponent<Progress>();
    }

    public Manager AddTextLine(string Stage)
    {
        AddObj = GetComponent<Progress>();
        int day_num = AddObj.day_num;

        foreach (TextLine line in Lines)
        {

            if(line.Day == day_num && line.Stage == Stage)
            {
                if (!line.Have_Condition && !line.Have_Show)
                {
                    line.Have_Show = true;
                    return line.Text;
                }
                else if (!line.Have_Show) return CheckCondition(line);
            }

        }
        return null;

    }


    /// <summary>
    /// 用于删除某天的某个对话项
    /// 需要参数 字符串类型 阶段 整数类型 天数
    /// </summary>
    public void DeleteLine(string Stage ,int Day)
    {
        int index = 0;
        foreach (TextLine textLine in Lines)
        {
            if(textLine.Day == Day && textLine.Stage == Stage)
            {
                Lines.RemoveAt(index);
                return;
            }
            index++;
        }

        Debug.LogError("加载存档删除前置对话失败 没有对应对话数据");

    }




    Manager CheckCondition(TextLine textLine)
    {

        if(textLine.Day == 3 && textLine.Stage == "BeforeStart")//day3判断阿曼德自杀
        {
            if (AmandeKillSelf.GeneralBool)
            {
                textLine.Have_Show = true;
                return textLine.Text;
            }
            else
            {
                return null;
            }
        }
        if (textLine.Day == 3 && textLine.Stage == "AfterFood")//day3艾米莉私聊
        {
            if (AmandeKillSelf.GeneralBool)
            {
                int NotDead = 0;
                foreach (Character c in characters)
                {
                    if(c.CharacterName == "艾米莉" || c.CharacterName == "博金森")
                    {
                        if (!c.Dead) NotDead++;

                    }
                }
                if (NotDead == 2)
                {
                    textLine.Have_Show = true;
                    return textLine.Text; 
                }
                else return null;
            }
            else
            {
                return null;
            }
        }



        return null;


    }







}
