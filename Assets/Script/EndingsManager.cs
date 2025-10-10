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

        int CurrentNotDeadNum = CheckNotDeadNum(); 
        int EatNum = CheckEatNum();
        int NotEatNum = CurrentNotDeadNum - EatNum;
        bool MainCharacterEat = CheckMainCharacterEat();
        int NotComfort = CheckNotComfort();

        if(MainCharacterEat && EatNum <= NotEatNum)//得到食物的人数（包括主角）（两人以上）小于等于没有得到食物的人
        {
            ToEnd("Be1");
        }

        if(MainCharacterEat && EatNum == 1)//玩家被叛乱杀死（只有主角得到食物，且没得到食物的人大于等于两个）
        {
            ToEnd("Be2");
        }

        if(NotComfort >= 2) // 玩家被反对杀死（一天结束时，拥有反抗心理的人大于等于两个）
        {
            ToEnd("Be3");
        }



        if(!MainCharacterEat && CurrentNotDeadNum != 0)//be4玩家被饿死
        {
            if(CurrentNotDeadNum == 3)
            {
                ToEnd("Be4-1");

            }
            else if(CurrentNotDeadNum > 3)
            {
                ToEnd("Be4-2");
            }


        }

        if(!MainCharacterEat && CurrentNotDeadNum == 1)//玩家被饿死（没有其他队友存活）
        {
            ToEnd("Be5");
        }


        if(!MainCharacterEat && EatNum == 0 && CurrentNotDeadNum == 6)//玩家一个食物也不分配大家一起饿死
        {
            ToEnd("Be7");
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
