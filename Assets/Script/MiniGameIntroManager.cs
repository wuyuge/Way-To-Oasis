using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MiniGameIntroManager : MonoBehaviour
{
    [System.Serializable]
    public class MiniGameData
    {
        public string name;
        public int day;
        public string characterName;
        public bool canPlay;
        public bool random;
    }
    
    public List<MiniGameData> miniGameData;

    private void Awake()
    {
        GlobalData.MiniGameManager = this;
        foreach (var value in miniGameData)
        {
            if (value.random)
            {
                value.canPlay = Random.value > 0.5f;
            }
            else
            {
                value.canPlay = true;
            }
        }
    }
}
