using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 存档菜单管理器：检测存档文件并控制对应按钮的激活状态
/// </summary>
public class SaveMenu : MonoBehaviour
{
    [Header("存档按钮配置")]
    [Tooltip("存储所有存档按钮的列表（自动填充，无需手动赋值）")]
    public List<GameObject> saveFileButtons = new List<GameObject>();
    
    [Tooltip("存档按钮的父容器（必须包含名为 S_0 ~ S_5 的子物体）")]
    public GameObject fileContainer;

    [Header("存档配置（与 SaveConstants 同步）")]
    [Tooltip("最大存档槽数量")]
    public int maxSaveSlots = 6;

    private void OnEnable()
    {
        // 初始化存档按钮列表
        InitializeSaveButtons();
        
        // 检测存档文件并更新按钮状态
        UpdateSaveButtonStates();
    }

    /// <summary>
    /// 初始化存档按钮列表（从父容器中查找所有存档按钮）
    /// </summary>
    private void InitializeSaveButtons()
    {
        saveFileButtons.Clear();

        // 容错检查：父容器为空时提示错误
        if (fileContainer == null)
        {
            Debug.LogError($"[{nameof(SaveMenu)}] 存档按钮父容器未赋值！请在Inspector面板中指定");
            return;
        }

        // 遍历所有存档槽，查找对应的按钮
        for (int i = 0; i < maxSaveSlots; i++)
        {
            string buttonName = $"S_{i}";
            Transform buttonTransform = fileContainer.transform.Find(buttonName);

            // 容错检查：找不到对应按钮时提示警告
            if (buttonTransform == null)
            {
                Debug.LogWarning($"[{nameof(SaveMenu)}] 在父容器中未找到名为 {buttonName} 的存档按钮");
                continue;
            }

            saveFileButtons.Add(buttonTransform.gameObject);
        }
    }

    /// <summary>
    /// 检测存档文件是否存在，并更新对应按钮的激活状态
    /// </summary>
    private void UpdateSaveButtonStates()
    {
        // 容错检查：存档文件夹路径未配置时提示错误
        if (string.IsNullOrEmpty(SaveConstants.SaveFolderPath))
        {
            Debug.LogError($"[{nameof(SaveMenu)}] 存档文件夹路径未配置！请检查 SaveConstants 类");
            return;
        }

        // 确保存档文件夹存在（避免首次运行时路径不存在的问题）
        if (!Directory.Exists(SaveConstants.SaveFolderPath))
        {
            Directory.CreateDirectory(SaveConstants.SaveFolderPath);
            Debug.Log($"[{nameof(SaveMenu)}] 已创建存档文件夹：{SaveConstants.SaveFolderPath}");
        }

        // 遍历所有存档按钮，检测对应存档文件
        for (int i = 0; i < saveFileButtons.Count; i++)
        {
            GameObject saveButton = saveFileButtons[i];
            string fileName = SaveConstants.SaveFileNameTemplate.Replace("{Field}", i.ToString());
            string filePath = Path.Combine(SaveConstants.SaveFolderPath, fileName);

            // 启用/禁用按钮（存在存档则启用）
            if (saveButton != null)
            {
                saveButton.SetActive(File.Exists(filePath));
            }
        }
    }

    /// <summary>
    /// 外部调用：强制刷新存档按钮状态（例如存档/读档后）
    /// </summary>
    public void RefreshSaveButtons()
    {
        UpdateSaveButtonStates();
    }
}
