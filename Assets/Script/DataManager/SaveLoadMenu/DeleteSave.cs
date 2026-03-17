using System;
using UnityEngine;
using UnityEngine.UI;

public class DeleteSave : MonoBehaviour
{
    
    private SaveManager _saveManager;
    private ISaveMenuInterface _saveButtonRefresher;
    public int num;
    public GameObject linkObj;
    public Button fatherObj;
    public bool isSave;

    /// <summary>
    /// 获取存档管理器
    /// 从父对象获取接口
    /// </summary>
    private void Awake()
    {
        _saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        
    }

    private void Start()
    {
        if (isSave)
        {
            _saveButtonRefresher = SLManager.SaveMenu;
        }
        else
        {
            _saveButtonRefresher = SLManager.LoadMenu;
        }
    }


    /// <summary>
    /// 删除存档后刷新存档界面
    /// </summary>
    public void Delete()
    {
        _saveManager.DeleteData(num);
        _saveButtonRefresher.UpdateSaveMenu();
        linkObj.SetActive(false);
        Cancel();
    }


    #region 子对象调用
    public void UpLinkObj()
    {
        linkObj.SetActive(true);
        gameObject.GetComponent<Button>().enabled = false;
        fatherObj.enabled = false;
    }
    
    public void Cancel()
    {
        linkObj.SetActive(false);
        gameObject.GetComponent<Button>().enabled = true;
        fatherObj.enabled = true;
    }

    #endregion
    
    
    
    
    
    
}
