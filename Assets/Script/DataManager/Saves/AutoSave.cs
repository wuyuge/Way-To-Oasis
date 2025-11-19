using System;
using System.Collections;
using UnityEngine;
/// <summary>
/// 挂载在存档管理器的子对象实现自动存档
/// </summary>
public class AutoSave : MonoBehaviour
{

    public int saveDelayTime;
    public Manager autoSaveIsOn;
    private SaveManager _saveManager;
    private Coroutine _coroutine;

    private void Awake()
    {
        _saveManager = gameObject.transform.parent.GetComponent<SaveManager>();
        _coroutine = StartCoroutine(AutoSaveFile());
        
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// 自动保存方法
    /// </summary>
    /// <returns></returns>
    private IEnumerator AutoSaveFile()
    {
        while (true)
        {
            if (autoSaveIsOn.GeneralBool)
            {
                try
                {
                    _saveManager.SaveData(0);
                    Debug.Log("自动保存成功");
                }
                catch (Exception e)
                {
                    Debug.LogError($"自动保存存档失败 错误:{e}");
                    throw;
                }
                
            }
            else
            {
                Debug.Log("自动保存已禁用");
            }
            yield return new WaitForSecondsRealtime(saveDelayTime);
        }
    }

    /// <summary>
    /// 停止自动保存
    /// </summary>
    public void StopAutoSave()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
    
    
    
    
    
}
