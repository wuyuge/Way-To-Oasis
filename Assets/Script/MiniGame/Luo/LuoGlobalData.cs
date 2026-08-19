
using System.Collections.Generic;
using UnityEngine;

public static class LuoGlobalData
{
    public static Pipe[] PipeList =  new Pipe[16];
    public static List<RectTransform> ItemList = new List<RectTransform>();

    public static void AddItem(RectTransform item)
    {
        ItemList.Add(item);
    }
    
    public static void ClearItem()
    {
        ItemList.Clear();
    }

    public static LuoLevelLoader LevelLoader;

    public static Pipe StartPipe {get; set; }
    public static Pipe DestinationPipe {get; set; }

    public static List<Pipe> LinkedPipeList = new List<Pipe>();
    public static LuoTalkSys TalkSys;
    public static int TotalRollTime {get; set; }
    public static int MaxCorrect {get; set; }

}
