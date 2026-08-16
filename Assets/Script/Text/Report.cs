using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Report : MonoBehaviour
{
    public Manager Food,Body,Food2,Body2,CurrentDead;
    public Progress Day;
    [SerializeField]
    private TextMeshProUGUI Text;
    private string DefultText = "<size=200>Day {day}</size>\n\n{dead}剩余食物数量 {food}\n剩余尸体数量 <color=#00ff00ff>{body}</color>";
    private string DefultTextEn = "<size=200>Day {day}</size>\n\n{dead}Remaining Food {food}\nRemaining Body <color=#00ff00ff>{body}</color>";
    public Manager AmandeKillSelf,killAimi;

    public void ShowText()
    {
        Text.text = GlobalData.Language.isEn ? DefultTextEn : DefultText;
        
        string showdead = string.Empty;
        Text.text = Text.text.Replace("{day}", Day.day_num.ToString());
        
        if (CurrentDead.TxtLine.Count != 0)
        {
            showdead += "<color=#ff0000ff>";
            var addText = string.Empty;
            foreach (string s in CurrentDead.TxtLine)
            {
                
                if (s.Contains("博金森"))
                {
                    addText = GlobalData.Language.isEn ? "You killed " : "你杀死了";
                    if (showdead.Contains(addText))
                    {
                        continue;
                    }
                    if(s != CurrentDead.TxtLine[CurrentDead.TxtLine.Count - 1])
                        showdead += addText + GetCharaName(s) + "\n";
                    else
                        showdead += addText + GetCharaName(s);
                }
                else if (s.Contains("阿曼德"))
                {
                    if (AmandeKillSelf.GeneralBool)
                    {
                        addText = GlobalData.Language.isEn ? "dead" : "死亡了";
                        if (s != CurrentDead.TxtLine[CurrentDead.TxtLine.Count - 1])
                            showdead +=  GetCharaName(s) + $"{addText}\n";
                        else showdead += GetCharaName(s) + addText;
                    }
                    else
                    {
                        addText = GlobalData.Language.isEn ? " was starved to death by you" : "被你饿死了";
                        if (s != CurrentDead.TxtLine[CurrentDead.TxtLine.Count - 1])
                            showdead += GetCharaName(s) + $"{addText}\n";
                        else showdead += GetCharaName(s) + addText;
                    }
                }
                else if (s.Contains("艾米莉"))
                {
                    
                    if (killAimi.GeneralBool)
                    {
                        addText = GlobalData.Language.isEn ? "You killed" : "你杀死了";
                        if (s != CurrentDead.TxtLine[CurrentDead.TxtLine.Count - 1])
                            showdead += addText + GetCharaName(s) + "\n";
                        else showdead += addText + GetCharaName(s);
                    }
                    else
                    {
                        addText = GlobalData.Language.isEn ? " was starved to death by you" : "被你饿死了";
                        if (s != CurrentDead.TxtLine[CurrentDead.TxtLine.Count - 1])
                            showdead += GetCharaName(s) + $"{addText}\n";
                        else showdead += GetCharaName(s) + addText;
                    }
                }
                else
                {
                    addText = GlobalData.Language.isEn ? " was starved to death by you" : "被你饿死了";
                    if (s != CurrentDead.TxtLine[CurrentDead.TxtLine.Count - 1])
                        showdead += GetCharaName(s) + $"{addText}\n";
                    else showdead += GetCharaName(s) + addText;
                }
            }
            showdead += "</color>";
            Text.text = Text.text.Replace("{dead}", showdead + "\n");
        }
        else
        {
            Text.text = Text.text.Replace("{dead}", string.Empty);
        }
        string FoodText = Mathf.Max(Food.Weight, Food2.Weight).ToString();
        string BodyText = Mathf.Max(Body.Weight, Body2.Weight).ToString();

        Text.text = Text.text.Replace("{food}", "<color=#00ff00ff>" + FoodText  + "</color>");
        if(Mathf.Max(Food.Weight, Food2.Weight) > 9)
        {
            Text.text = Text.text.Replace("{body}", " " + BodyText );
        }
        else Text.text = Text.text.Replace("{body}", BodyText );
        
    }

    // 建议把映射表做成静态只读，避免每次方法调用重复创建
    private static readonly Dictionary<string, string> _charaEnMap = new Dictionary<string, string>()
    {
        {"阿曼德", "Amanda"},
        {"博金森", "Bokinson"},
        {"艾米莉", "Emily"},
        {"洛尔坎", "Lorquin"},
        {"莱文", "Levine"}
    };

    private string GetCharaName(string txt)
    {
        // 遍历所有角色中文名
        foreach (var pair in _charaEnMap)
        {
            string cnName = pair.Key;
            if (txt.Contains(cnName))
            {
                return GlobalData.Language.isEn ? pair.Value : cnName;
            }
        }
        return string.Empty;
    }
    
    
}
