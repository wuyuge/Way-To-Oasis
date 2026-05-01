using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DeleteSave : MonoBehaviour
{
    
    [FormerlySerializedAs("_saveManager")] public SaveManager saveManager;
    private ISaveMenuInterface _saveButtonRefresher;
    public int num;
    [FormerlySerializedAs("linkObj")] public GameObject deleteTips;
    public Button fatherObj;
    public bool isSave;
    public bool isSaveScene;
    /// <summary>
    /// 获取存档管理器
    /// 从父对象获取接口
    /// </summary>
    private void Awake()
    {
        if(saveManager is null) saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        
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
        saveManager.DeleteData(num);
        _saveButtonRefresher.UpdateSaveMenu();
        deleteTips.SetActive(false);
        Cancel();
        if (isSaveScene)
        {
            fatherObj.GetComponent<SaveFileButton>().FileExists();
        }
    }


    #region 子对象调用
    public void UpLinkObj()
    {
        GlobalData.CurrentSaveFileButton = gameObject;
        deleteTips.SetActive(true);
        gameObject.GetComponent<Button>().enabled = false;
        fatherObj.enabled = false;
    }
    
    public void Cancel()
    {
        deleteTips.SetActive(false);
        gameObject.GetComponent<Button>().enabled = true;
        fatherObj.enabled = true;
    }

    #endregion
    
    
    
    
    
    
}
