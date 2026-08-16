using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingsManager : MonoBehaviour
{

    public GameObject MainCharacter;
    public Animator black;
    public List<GameObject> AllCharacter = new List<GameObject>();

    private void Awake()
    {
        GlobalData.EndingsManager = this;
    }

    public IEnumerator ToEnd(string SceneName)
    {
        black.SetTrigger("OnlyBlack");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneName);
        
    }

    

    public bool CheckEnding()
    {
        // 一次性获取所有需要的统计数据
        int currentNotDeadNum = CheckNotDeadNum();
        int eatNum = CheckEatNum();
        bool mainCharacterEat = CheckMainCharacterEat();
        int notComfort = CheckNotComfort();

        // 计算派生数据
        int notEatNum = currentNotDeadNum - eatNum;

        // 优先检查反抗条件，可能更紧急
        if (notComfort >= 2)
        {
            StartCoroutine(ToEnd("Be3"));
            return true; // 触发此结局后直接返回，避免后续判断
        }

        // 主角是否进食的分支处理
        if (mainCharacterEat)
        {
            if (eatNum == 1)
            {
                StartCoroutine(ToEnd("Be2"));
                return true;
            }
            // 处理主角进食的情况
            if (eatNum <= notEatNum)
            {
                StartCoroutine(ToEnd("Be1"));
                return true;
            }
            
        }
        else
        {
            // 处理主角未进食的情况
            if (currentNotDeadNum == 0)
            {
                return true; // 没有死亡角色，无需处理
            }

            // 检查全员未进食的特殊情况
            if (eatNum == 0 && currentNotDeadNum >= 3)
            {
                StartCoroutine(ToEnd("Be7"));
                return true;
            }
            // 检查只有主角存活的情况
            else if (currentNotDeadNum == 1)
            {
                StartCoroutine(ToEnd("Be5"));
                return true;
            }
            // 其他主角饿死的情况
            else
            {
                if (currentNotDeadNum == 3)
                {
                    StartCoroutine(ToEnd("Be4-1"));
                    return true;
                }
                else if (currentNotDeadNum > 3)
                {
                    StartCoroutine(ToEnd("Be4-2"));
                    return true;
                }
            }
        }
        return false;
    }

    int CheckNotDeadNum()
    {
        int Value = 0;
        foreach (GameObject g in AllCharacter)
        {
            if (!g.GetComponent<Character>().Dead)
            {
                Value++;
            }
        }
        return Value;
    }

    int CheckNotComfort()
    {
        int Value = 0;
        foreach (GameObject g in AllCharacter)
        {
            if (g.GetComponent<Character>().NotComfort)
            {
                Value++;

            }
        }
        return Value;
    }


    int CheckEatNum()
    {
        int Value = 0;
        foreach (GameObject g in AllCharacter)
        {
            if (g.GetComponent<Character>().eat)
            {
                Value++;
            }
        }
        return Value;
    }




    bool CheckMainCharacterEat()
    {
        return MainCharacter.GetComponent<Character>().eat;
    }






}
