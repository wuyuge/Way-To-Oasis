using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingsManager : MonoBehaviour
{

    public GameObject MainCharacter;
    public List<GameObject> AllCharacter = new List<GameObject>();

    public void ToEnd(string SceneName)
    {
        SceneManager.LoadScene(SceneName);



    }


    public void CheckEnding()
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
            ToEnd("Be3");
            return; // 触发此结局后直接返回，避免后续判断
        }

        // 主角是否进食的分支处理
        if (mainCharacterEat)
        {
            // 处理主角进食的情况
            if (eatNum <= notEatNum)
            {
                ToEnd("Be1");
            }
            else if (eatNum == 1)
            {
                ToEnd("Be2");
            }
        }
        else
        {
            // 处理主角未进食的情况
            if (currentNotDeadNum == 0)
            {
                return; // 没有存活角色，无需处理
            }

            // 检查全员未进食的特殊情况
            if (eatNum == 0 && currentNotDeadNum == 6)
            {
                ToEnd("Be7");
            }
            // 检查只有主角存活的情况
            else if (currentNotDeadNum == 1)
            {
                ToEnd("Be5");
            }
            // 其他主角饿死的情况
            else
            {
                if (currentNotDeadNum == 3)
                {
                    ToEnd("Be4-1");
                }
                else if (currentNotDeadNum > 3)
                {
                    ToEnd("Be4-2");
                }
            }
        }
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
