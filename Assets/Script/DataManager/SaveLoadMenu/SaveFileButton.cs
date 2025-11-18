using System; 
using UnityEngine;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// 挂载在存档界面的每个存档按钮上
/// 功能:
/// 保存存档
/// 检测是否覆盖存档
/// </summary>
public class SaveFileButton : MonoBehaviour
{
    public int fileNum;
    private SaveManager _saveManager;
    public FileButtonRefresh refresher;
    public GameObject coverTips,saveTips;

    /// <summary>
    /// 当存档界面打开时启用挂载这个脚本的按钮组件,
    /// 并且关闭是否覆盖选项提示,确认存档选项
    /// </summary>
    private void OnEnable()
    {
        gameObject.GetComponent<Button>().enabled = true;
        coverTips.SetActive(false);
        saveTips.SetActive(false);
    }

    /// <summary>
    /// 开始运行时获取存档管理器
    /// </summary>
    private void Start()
    {
        _saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
    }
    

    /// <summary>
    /// 用于判断是否需要覆盖文件
    /// </summary>
    /// <returns>返回布尔型,存在True,不存在False</returns>
    bool FileExists()
    {
        string fileName = SaveConstants.SaveFileNameTemplate.Replace("{Field}", fileNum.ToString()); // 生成存档文件名
        string filePath = Path.Combine(SaveConstants.SaveFolderPath, fileName); // 生成存档文件路径
        
        return File.Exists(filePath);
        
        
    }

    /// <summary>
    /// 核心方法
    /// 判断是否存在文件
    /// 不存在直接存档
    /// 存在则返回并且询问是否覆盖
    /// </summary>
    void Save()
    {
        //判断是否存在存档文件
        //若存在打开是否覆盖提示关闭挂载脚本对象的按钮组件并返回
        //若不存在则继续向下执行
        if (FileExists())
        {
            ShowCoverFileTips();
            return;
        }

        try
        {
            _saveManager.SaveData(fileNum);
            refresher.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError($"保存存档发生错误:{e}");
            throw;
        }
        
    }

    public void UpLinkObj()
    {
        if (FileExists())
        {
            ShowCoverFileTips();
            return;
                
        }
        saveTips.SetActive(true);
        gameObject.GetComponent<Button>().enabled = false;
    }
    
    

    #region 子对象调用方法


    public void SaveFile()
    {
        try
        {
            
            Save();
            refresher.Refresh();
            gameObject.GetComponent<Button>().enabled = true;
            saveTips.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.LogError($"存档保存错误{e}");
            throw;
        }
    }
    /// <summary>
    /// 由子对象直接调用直接覆盖保存
    /// </summary>
    public void CoverFile()
    {
        try
        {
            _saveManager.SaveData(fileNum);
            refresher.Refresh();
            gameObject.GetComponent<Button>().enabled = true;
            coverTips.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.LogError($"保存存档发生错误:{e}");
            throw;
        }
    }
    /// <summary>
    /// 由子对象调用取消
    /// </summary>
    public void Cancel()
    {
        gameObject.GetComponent<Button>().enabled = true;
        coverTips.SetActive(false);
        saveTips.SetActive(false);
    }
    
    #endregion

    
    
    /// <summary>
    /// 检测到是否覆盖文件时显示的内容
    /// </summary>
    public void ShowCoverFileTips()
    {
        gameObject.GetComponent<Button>().enabled = false;
        coverTips.SetActive(true);
        Debug.Log("存在文件,询问覆盖并返回");
        
    }
    
    
    
    
}
