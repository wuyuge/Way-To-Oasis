using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MiniGameIntroManager : MonoBehaviour
{
    [System.Serializable]
    public class MiniGameData
    {
        public string name;
        public int day;          // 所属天数
        public string characterName;
        public bool canPlay;     // 仅当天有效
        public bool random;      // 是否为随机互斥小游戏
        public bool isPlayed;    // 标记：该小游戏当天是否被玩过
    }

    public List<MiniGameData> miniGameData;  // 所有小游戏配置
    public List<int> limit;                 // 各天的随机小游戏可玩次数（索引对应day）
    // 记录每一天的随机小游戏是否已完成随机（避免重复处理）
    private Dictionary<int, bool> _isDayRandomHandled = new Dictionary<int, bool>();

    private void Awake()
    {
        GlobalData.MiniGameManager = this;
        
        // 初始化所有标记：默认不可玩、未玩过
        foreach (var gameData in miniGameData)
        {
            gameData.canPlay = false;
            gameData.isPlayed = false;
        }
        
        // 按天数分组，初始化每一天的基础状态
        Dictionary<int, List<MiniGameData>> dayGameDict = new Dictionary<int, List<MiniGameData>>();
        foreach (var gameData in miniGameData)
        {
            if (!dayGameDict.ContainsKey(gameData.day))
            {
                dayGameDict[gameData.day] = new List<MiniGameData>();
                _isDayRandomHandled[gameData.day] = false; // 初始化天数处理标记
            }
            dayGameDict[gameData.day].Add(gameData);
        }

        // 初始化第一天的小游戏状态（正常随机）
        foreach (var kvp in dayGameDict)
        {
            InitDayGameState(kvp.Key, false);
        }
    }

    /// <summary>
    /// 初始化某一天的小游戏状态
    /// </summary>
    /// <param name="targetDay">目标天数</param>
    /// <param name="isFromNextDay">是否从下一天切换过来（true=未玩过则直接设为可玩）</param>
    private void InitDayGameState(int targetDay, bool isFromNextDay)
    {
        List<MiniGameData> gamesOfDay = miniGameData.FindAll(g => g.day == targetDay);
        if (gamesOfDay.Count == 0)
        {
            Debug.LogWarning($"第 {targetDay} 天没有配置任何小游戏");
            return;
        }

        List<MiniGameData> randomGames = gamesOfDay.FindAll(g => g.random);
        List<MiniGameData> nonRandomGames = gamesOfDay.FindAll(g => !g.random);

        // 1. 非随机小游戏：始终当天可玩（跨天会被重置）
        foreach (var nonRandomGame in nonRandomGames)
        {
            nonRandomGame.canPlay = true;
            nonRandomGame.isPlayed = false;
        }

        // 2. 处理随机小游戏（核心逻辑）
        if (randomGames.Count > 0)
        {
            bool hasLimit = targetDay >= 0 && targetDay < limit.Count && limit[targetDay] > 0;

            // 场景1：从下一天切换过来 → 检查是否玩过
            if (isFromNextDay)
            {
                // 只要有一个随机小游戏没玩过 → 全部直接设为可玩（不随机、不消耗limit）
                bool hasUnplayedRandomGame = randomGames.Exists(g => !g.isPlayed);
                if (hasUnplayedRandomGame)
                {
                    foreach (var game in randomGames)
                    {
                        game.canPlay = true; // 未玩过 → 直接标记为可玩
                        game.isPlayed = false; // 重置玩过标记
                    }
                    Debug.Log($"第 {targetDay} 天的随机小游戏未玩过，下一天直接设为可玩");
                }
                else
                {
                    // 已玩过 → 按正常逻辑随机（消耗limit）
                    DoRandomForDayGames(targetDay, randomGames);
                }
            }
            // 场景2：首次初始化/已玩过 → 正常随机
            else
            {
                if (!_isDayRandomHandled[targetDay] && hasLimit)
                {
                    DoRandomForDayGames(targetDay, randomGames);
                    _isDayRandomHandled[targetDay] = true; // 标记该天已随机
                }
                else if (!hasLimit)
                {
                    // limit不足 → 随机小游戏不可玩
                    foreach (var game in randomGames)
                    {
                        game.canPlay = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 对某天的随机小游戏执行随机逻辑（互斥、消耗limit）
    /// </summary>
    private void DoRandomForDayGames(int day, List<MiniGameData> randomGames)
    {
        if (limit[day] <= 0) return;

        // 随机选一个设为可玩，其余不可玩（互斥）
        int randomIndex = Random.Range(0, randomGames.Count);
        for (int i = 0; i < randomGames.Count; i++)
        {
            randomGames[i].canPlay = (i == randomIndex);
            randomGames[i].isPlayed = false; // 初始未玩
        }
        limit[day]--; // 消耗当天次数
        Debug.Log($"第 {day} 天随机选中小游戏：{randomGames[randomIndex].name}，剩余次数：{limit[day]}");
    }

    /// <summary>
    /// 切换到下一天（核心对外方法）
    /// </summary>
    /// <param name="currentDay">当前天</param>
    /// <param name="nextDay">下一天</param>
    public void SwitchToNextDay(int currentDay, int nextDay)
    {
        // 1. 重置当前天所有小游戏状态（不可玩）
        foreach (var game in miniGameData)
        {
            if (game.day == currentDay)
            {
                game.canPlay = false; // 当天可玩的 → 下一天不可玩（基础规则）
            }
        }

        // 2. 初始化下一天状态（传入isFromNextDay=true，触发未玩过直接可玩逻辑）
        InitDayGameState(nextDay, true);
        // 重置下一天的处理标记
        if (_isDayRandomHandled.ContainsKey(nextDay))
        {
            _isDayRandomHandled[nextDay] = false;
        }
    }

    /// <summary>
    /// 标记某小游戏为“已玩过”（需在玩家游玩后调用）
    /// </summary>
    public void MarkGameAsPlayed(string gameName)
    {
        var targetGame = miniGameData.Find(g => g.name == gameName);
        if (targetGame != null)
        {
            targetGame.isPlayed = true;
            Debug.Log($"标记小游戏 {gameName} 为已玩过");
        }
        else
        {
            Debug.LogWarning($"未找到小游戏：{gameName}，无法标记已玩");
        }
    }
}