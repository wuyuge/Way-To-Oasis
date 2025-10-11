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
    [Header("ÅÐ¶ÏÌõ¼þ")]
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


    Manager CheckCondition(TextLine textLine)
    {

        if(textLine.Day == 3 && textLine.Stage == "BeforeStart")//day3ÅÐ¶Ï°¢ÂüµÂ×ÔÉ±
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
        if (textLine.Day == 3 && textLine.Stage == "AfterFood")//day3°¬Ã×ÀòË½ÁÄ
        {
            if (AmandeKillSelf.GeneralBool)
            {
                int NotDead = 0;
                foreach (Character c in characters)
                {
                    if(c.CharacterName == "°¬Ã×Àò" || c.CharacterName == "²©½ðÉ­")
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
