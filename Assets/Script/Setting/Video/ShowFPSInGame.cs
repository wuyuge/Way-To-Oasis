using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFPSInGame : MonoBehaviour
{

    public GameObject FPSText;


    public void Clik(int Value)
    {

        if(Value == 0)
        {
            FPSText.SetActive(false);
        }
        else if(Value == 1)
        {
            FPSText.SetActive(true);
        }
        


        
    }




}
