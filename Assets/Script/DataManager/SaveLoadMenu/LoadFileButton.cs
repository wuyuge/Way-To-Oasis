using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadFileButton : MonoBehaviour
{
    private SaveManager _saveManager;
    public int saveIndex;
    public GameObject linkObj;
    public Manager autoSaveIsOn;

    private void Awake()
    {
        _saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
    }
    
    
    //在开启读档界面时禁止自动存档
    private void OnEnable()
    {
        if (autoSaveIsOn.GeneralBool)
        {
            autoSaveIsOn.GeneralBool = false;
        }
    }


    public void Load()
    {
        //获取挂载这个脚本的对应存档
        autoSaveIsOn.GeneralBool = true;
        _saveManager.LoadData(saveIndex);
    }

    public void UpLinkObj()
    {
        linkObj.SetActive(true);
        gameObject.GetComponent<Button>().enabled = false;
        gameObject.transform.Find("Delete").GetComponent<Button>().enabled = false;
    }


    public void Cancel()
    {
        linkObj.SetActive(false);
        gameObject.GetComponent<Button>().enabled = true;
        gameObject.transform.Find("Delete").GetComponent<Button>().enabled = true;
    }



}
