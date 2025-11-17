using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    
    private SaveManager saveManager;

    private void Awake()
    {
        saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
    }



    public void Save(int num)
    {

        saveManager.SaveData(num);


    }


    public void Load(int num)
    {
        saveManager.LoadData(num);
    }


    public void Delete(int num)
    {
        saveManager.DeleteData(num);

    }

}
