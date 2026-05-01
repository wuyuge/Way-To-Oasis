using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MiniGameIntroManager : MonoBehaviour
{
    [Serializable]
    public class MiniGameData
    {
        public string name;
        public List<MiniGameInfo> infos;
    }

    [Serializable]
    public class MiniGameInfo
    {
        public bool isRandom;
        public bool canPlay;
        public bool disable = true;
        public List<int> mutex;
    }

    public List<MiniGameData> miniGameData;
    public List<int> limit;

    private void Awake()
    {
        GlobalData.MiniGameManager = this;
    }

    private void Start()
    {
        SelectMiniGame();
    }

    private void SelectMiniGame()
    {
        foreach (var value in miniGameData)
        {
            for (var i = 0 ; i < value.infos.Count ; i++)
            {
                if (value.infos[i].disable)
                {
                    value.infos[i].canPlay = false;
                    continue;
                }
                
                if (!value.infos[i].isRandom)
                {
                    value.infos[i].canPlay = true;
                    continue;
                }

                if (CheckMutex(value, i))
                {
                    value.infos[i].canPlay = false;
                    continue;
                }

                if (limit[i] < 1)
                {
                    value.infos[i].canPlay = false;
                    continue;
                }
                
                var temp = Random.Range(0, 2);

                if (temp == 0)
                {
                    value.infos[i].canPlay = true;
                    limit[i] -= 1;
                }
                else
                {
                    value.infos[i].canPlay = false;
                }

                

            }
        }
        foreach (var value in miniGameData)
        {
            for (var i = 0 ; i < value.infos.Count ; i++)
            {
                if (limit[i] > 0 && !CheckMutex(value, i))
                {
                    value.infos[i].canPlay = true;
                    limit[i] -= 1;
                }
            }
        }
        
    }

    private bool CheckMutex(MiniGameData gameData, int index)
    {
        foreach (var value in gameData.infos[index].mutex)
        {
            if (gameData.infos[value].canPlay)
            {
                return true;
            }
        }

        return false;
    }
}