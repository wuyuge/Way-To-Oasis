using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 负责存档数据的保存、加载、删除
/// </summary>
public class SaveManager : MonoBehaviour
{
    public RestoreSence restoreSence; // 场景同步管理器引用
    private PlayerSaveData _currentSaveData; // 当前存档数据对象
    public bool EnableEncrypt = true; // 是否启用加密
    public Manager Reload;
    
    private string _xorEncryptionKey = SaveConstants.EncryptionKey;

    /// <summary>
    /// 用于在场景加载时检查是否需要加载存档数据
    /// </summary>
    void Awake()
    {
        if (Reload.GeneralBool)
        {
            Debug.Log("检测到需要加载存档数据，正在加载...");
            LoadData(Reload.Weight);
            restoreSence.ApplyData(_currentSaveData);
            Invoke("SetReloadBool", 0.5f);
            Reload.Weight = 0;
        }
    }

    void SetReloadBool()
    {
        Reload.GeneralBool = false;
    }



    /// <summary>
    /// 获取当前游戏数据
    /// </summary>
    void GetCurrentSaveData()
    {
        if (restoreSence != null)
        {
            _currentSaveData = restoreSence.GetData();
        }
        else
        {
            Debug.LogError("GetCurrentSaveData: restoreSence 引用未设置！");
        }
    }

    /// <summary>
    /// 保存当前存档数据到指定编号的文件
    /// </summary>
    public void SaveData(int num)
    {
        GetCurrentSaveData();

        if (_currentSaveData == null)
        {
            Debug.LogError("SaveData: _currentSaveData 为 null，无法保存。");
            return;
        }

        string fileName = SaveConstants.SaveFileNameTemplate.Replace("{Field}", num.ToString());
        string filePath = Path.Combine(SaveConstants.SaveFolderPath, fileName);

        try
        {
            string jsonData = JsonUtility.ToJson(_currentSaveData, prettyPrint: true);

            if (EnableEncrypt)
            {
                jsonData = XOREncrypt(jsonData);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, jsonData);

            Debug.Log($"存档 {num} 保存成功！路径: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"SaveData: 保存存档 {num} 时发生错误: {e.Message}");
        }
    }

    /// <summary>
    /// 从指定编号加载存档数据（仅加载，不同步到场景）
    /// </summary>
    public bool LoadData(int num)
    {

        if(!Reload.GeneralBool)
        {
            Reload.GeneralBool = true;
            Reload.Weight = num;
            Time.timeScale = 1f;
            SceneManager.LoadScene("main");
            return false;

        }

        string fileName = SaveConstants.SaveFileNameTemplate.Replace("{Field}", num.ToString());
        string filePath = Path.Combine(SaveConstants.SaveFolderPath, fileName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"LoadData: 存档文件 {filePath} 不存在。");
            _currentSaveData = null;
            return false;
        }

        try
        {
            string fileContent = File.ReadAllText(filePath);

            if (EnableEncrypt)
            {
                fileContent = XORDecrypt(fileContent);
                if (string.IsNullOrEmpty(fileContent))
                {
                    Debug.LogError($"LoadData: 存档 {num} 解密失败。");
                    _currentSaveData = null;
                    return false;
                }
            }

            PlayerSaveData loadedData = JsonUtility.FromJson<PlayerSaveData>(fileContent);

            if (loadedData != null)
            {
                _currentSaveData = loadedData;
                Debug.Log($"存档 {num} 加载成功！");
                return true;
            }
            else
            {
                Debug.LogError($"LoadData: 存档 {num} 反序列化失败。");
                _currentSaveData = null;
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"LoadData: 加载存档 {num} 时发生错误: {e.Message}");
            _currentSaveData = null;
            return false;
        }
    }

    /// <summary>
    /// 删除指定编号的存档
    /// </summary>
    public bool DeleteData(int num)
    {
        string fileName = SaveConstants.SaveFileNameTemplate.Replace("{Field}", num.ToString());
        string filePath = Path.Combine(SaveConstants.SaveFolderPath, fileName);

        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                Debug.Log($"存档 {num} 已删除。");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"DeleteData: 删除存档 {num} 时发生错误: {e.Message}");
                return false;
            }
        }
        else
        {
            Debug.LogWarning($"DeleteData: 要删除的存档 {num} 不存在。");
            return false;
        }
    }

    /// <summary>
    /// 检查是否存在任何存档
    /// </summary>
    public bool CheckAnySave()
    {
        if (!Directory.Exists(SaveConstants.SaveFolderPath))
        {
            return false;
        }

        string searchPattern = SaveConstants.SaveFileNameTemplate.Replace("{Field}", "*");
        string[] saveFiles = Directory.GetFiles(SaveConstants.SaveFolderPath, searchPattern);

        return saveFiles.Length > 0;
    }

    /// <summary>
    /// 获取当前加载的存档数据的副本
    /// </summary>
    public PlayerSaveData GetLoadedSaveData()
    {
        return _currentSaveData;
    }

    /// <summary>
    /// 异或加密字符串。
    /// </summary>
    private string XOREncrypt(string data)
    {
        if (string.IsNullOrEmpty(data)) return string.Empty;
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        byte[] keyBytes = Encoding.UTF8.GetBytes(_xorEncryptionKey);
        byte[] resultBytes = ProcessXor(dataBytes, keyBytes);
        return Convert.ToBase64String(resultBytes);
    }

    /// <summary>
    /// 异或解密字符串。
    /// </summary>
    private string XORDecrypt(string data)
    {
        if (string.IsNullOrEmpty(data)) return string.Empty;
        try
        {
            byte[] encryptedBytes = Convert.FromBase64String(data);
            byte[] keyBytes = Encoding.UTF8.GetBytes(_xorEncryptionKey);
            byte[] resultBytes = ProcessXor(encryptedBytes, keyBytes);
            return Encoding.UTF8.GetString(resultBytes);
        }
        catch (FormatException)
        {
            Debug.LogError("XORDecrypt: 输入的不是有效的Base64字符串。");
            return string.Empty;
        }
    }

    /// <summary>
    /// 执行异或操作的核心方法。
    /// </summary>
    private byte[] ProcessXor(byte[] inputBytes, byte[] keyBytes)
    {
        byte[] resultBytes = new byte[inputBytes.Length];
        for (int i = 0; i < inputBytes.Length; i++)
        {
            resultBytes[i] = (byte)(inputBytes[i] ^ keyBytes[i % keyBytes.Length]);
        }
        return resultBytes;
    }
}