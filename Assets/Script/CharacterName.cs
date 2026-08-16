

public static class CharacterName
{
    public static string GetCharaName(CharacterType type)
    {
        var text = string.Empty;
        switch (type)
        {
            case CharacterType.Player:
                return GlobalData.TalkSystem.PlayerName.TxtLine[0];
            case CharacterType.阿曼德:
                text = GlobalData.Language.isEn ? "Amanda" : "阿曼德";
                break;
            case CharacterType.艾米莉:
                text = GlobalData.Language.isEn ? "Emily" : "艾米莉";
                break;
            case CharacterType.博金森:
                text = GlobalData.Language.isEn ? "Bokinson" : "博金森";
                break;
            case CharacterType.莱文:
                text = GlobalData.Language.isEn ? "Levine" : "莱文";
                break;
            case CharacterType.洛尔坎:
                text = GlobalData.Language.isEn ? "Lorquin" : "洛尔坎";
                break;
        }

        return text;
    }
    
    public static string GetCharaName(string type,bool returnCn = false)
    {
        var text = string.Empty;
        switch (type)
        {
            case "Player":
                return GlobalData.TalkSystem.PlayerName.TxtLine[0];
            case "阿曼德":
                text = GlobalData.Language.isEn ? "Amanda" : "阿曼德";
                if (returnCn) text = "阿曼德";
                break;
            case "艾米莉":
                text = GlobalData.Language.isEn ? "Emily" : "艾米莉";
                if (returnCn) text = "艾米莉";
                break;
            case "博金森":
                text = GlobalData.Language.isEn ? "Bokinson" : "博金森";
                if (returnCn) text = "博金森";
                break;
            case "莱文":
                text = GlobalData.Language.isEn ? "Levine" : "莱文";
                if (returnCn) text = "莱文";
                break;
            case "洛尔坎":
                text = GlobalData.Language.isEn ? "Lorquin" : "洛尔坎";
                if (returnCn) text = "洛尔坎";
                break;
        }

        return text;
    }
}
