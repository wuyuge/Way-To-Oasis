using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class New_Game : MonoBehaviour
{
    public bool is_New;
    public Manager Final_Food, Final_Body;
    [Header("³õÊ¼ÊýÁ¿")]
    public int food, body;
    public Manager DeadName;
    public Manager Day0_Talk;
    public void Clik()
    {
        if (is_New)
        {
            Final_Body.Weight = body;
            Final_Food.Weight = food;

        }
        DeadName.TxtLine.Clear();
        DeadName.TxtLine.Add("Leader");
        Day0_Talk.Weight = 0;
    }

    




}
