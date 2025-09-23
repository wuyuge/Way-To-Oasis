using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class New_Game : MonoBehaviour
{
    public bool is_New;
    public Manager Final_Food, Final_Body,Have_Food,Have_Body;
    public List<Manager> ResetBoolManager = new List<Manager>();
    [Header("³õÊ¼ÊýÁ¿")]
    public int food, body;
    public Manager DeadName,UesdBody;
    public Manager Day0_Talk;
    public void Clik()
    {
        if (is_New)
        {
            Final_Body.Weight = body;
            Final_Food.Weight = food;
            Have_Body.Weight = 0;
            Have_Food.Weight = 0;
            UesdBody.TxtLine.Clear();
            

        }
        foreach (Manager m in ResetBoolManager)
        {
            m.GeneralBool = false;
        }
        DeadName.TxtLine.Clear();
        DeadName.TxtLine.Add("Leader");
        Day0_Talk.Weight = 0;
    }

    




}
