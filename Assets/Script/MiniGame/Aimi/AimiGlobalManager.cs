
using UnityEngine;

public static class AimiGlobalManager 
{
    public static AimiPlayer Player{ get; set; }
    public static RectTransform LineTransform { get; set; }
    public static AimiManager TalkManager { get; set;}
    public static RectTransform LineColl { get; set; }
    public static int ObjectNums { get; set; }
    public static int CheckNums { get; set; }
    public static bool Failed { get; set; }
    public static AimiEffectPlayer EffectPlayer {get; set; }
}
