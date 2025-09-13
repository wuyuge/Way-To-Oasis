using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAll : MonoBehaviour
{
    public List<Manager> charaeat = new List<Manager>();
    public Manager deadname;
    
    void Start()
    {
        foreach (Manager m in charaeat)
        {
            m.Day1Eat = false;
        }
        deadname.TxtLine.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
