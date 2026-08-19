using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public static class AchievementManager
{
    
    private const bool Unlockable = true;
    
    
    public static bool UnlockAchievement(string achievement)
    {
        if (SteamManager.Initialized && Unlockable)
        {
            if(CheckAchievementStatus(achievement))
            {
                Debug.Log("成就已解锁" + achievement);
                return true;
            }
            bool success = SteamUserStats.SetAchievement(achievement);
            if (success)
            {
                SteamUserStats.StoreStats();
                Debug.Log("解锁成就" + achievement);
                return true;
            }
            Debug.Log("解锁成就失败" + achievement);

        }
        return false;
    }

    public static void ClearAchievement(string achievement)
    {
        if (SteamManager.Initialized && Unlockable)
        {
            SteamUserStats.ClearAchievement(achievement);
            Debug.Log("锁定成就：" + achievement);
        }
    }
    
    public static bool CheckAchievementStatus(string achievementID)
    {
        if (SteamManager.Initialized && Unlockable)
        {
            SteamUserStats.GetAchievement(achievementID, out bool isUnlocked);
            return isUnlocked;
        }
        return false;
    }
}





