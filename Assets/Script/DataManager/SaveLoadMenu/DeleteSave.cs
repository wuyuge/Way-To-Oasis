using UnityEngine;
using UnityEngine.UI;

public class DeleteSave : MonoBehaviour
{
    
    private SaveManager _saveManager;
    private ISaveMenuInterface _saveButtonRefresher;
    public int num;
    public GameObject linkObj;

    /// <summary>
    /// 获取存档管理器
    /// 从父对象获取接口
    /// </summary>
    private void Awake()
    {
        _saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        _saveButtonRefresher = gameObject.transform.parent.parent.parent.parent.parent.GetComponent<ISaveMenuInterface>();
        
    }

    /// <summary>
    /// 删除存档后刷新存档界面
    /// </summary>
    public void Delete()
    {
        _saveManager.DeleteData(num);
        _saveButtonRefresher.UpdateSaveMenu();
        linkObj.SetActive(false);
        
    }


    #region 子对象调用
    public void UpLinkObj()
    {
        linkObj.SetActive(true);
        gameObject.GetComponent<Button>().enabled = false;
        gameObject.transform.parent.GetComponent<Button>().enabled = false;
    }
    
    public void Cancel()
    {
        linkObj.SetActive(false);
        gameObject.GetComponent<Button>().enabled = true;
        gameObject.transform.parent.GetComponent<Button>().enabled = true;
    }

    #endregion
    
    
    
    
    
    
}
