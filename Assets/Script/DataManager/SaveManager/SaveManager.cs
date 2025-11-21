using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

/// <summary>
/// 负责存档数据的保存、加载、删除
/// </summary>
public class SaveManager : MonoBehaviour
{
    public RestoreSence restoreSence; // 场景同步管理器引用
    private PlayerSaveData _currentSaveData; // 当前存档数据对象
    public bool enableEncrypt = true; // 是否启用加密
    [FormerlySerializedAs("Reload")] public Manager reload;
    
    private string _xorEncryptionKey = SaveConstants.EncryptionKey;

    /// <summary>
    /// 用于在场景加载时检查是否需要加载存档数据
    /// </summary>
    void Awake()
    {
        if (reload != null)
        {
            if (reload.GeneralBool)
            {
                Debug.Log("检测到需要加载存档数据，正在加载...");
                LoadData(reload.Weight);
                restoreSence.ApplyData(_currentSaveData);
                Invoke("SetReloadBool", 0.5f);
                reload.Weight = 0;
            }
        }
        
    }

    void SetReloadBool()
    {
        reload.GeneralBool = false;
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

            if (enableEncrypt)
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

        if(!reload.GeneralBool)
        {
            reload.GeneralBool = true;
            reload.Weight = num;
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

            if (enableEncrypt)
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
                //删除对应的meta文件
                filePath += ".meta";
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
    
    /// <summary>
    /// 这个方法的主要功能是从指定的文件中读取玩家的存档数据，并进行必要的解密和反序列化操作。
    /// 如果过程中有任何问题（如文件不存在、解密失败、反序列化失败等），都会记录相应的日志并返回 null。
    /// 如果一切顺利，则返回反序列化后的 PlayerSaveData 对象。
    /// </summary>
    /// <param name="index">是一个字符串，表示存档文件的索引。</param>
    /// <param name="reportWarning">是否报告存档文件不存在问题</param>
    /// <returns></returns>
    public PlayerSaveData GetDataFormFile(string index,bool reportWarning = true)
    {
        string fileName = SaveConstants.SaveFileNameTemplate.Replace("{Field}", index); // 生成存档文件名
        string filePath = Path.Combine(SaveConstants.SaveFolderPath, fileName); // 生成存档文件路径

        if (!File.Exists(filePath))
        {
            if (reportWarning) Debug.LogWarning($"GetDataFormFile: 存档文件 {filePath} 不存在。"); // 如果文件不存在，记录警告并返回 null
            return null;
        }

        try
        {
            string fileContent = File.ReadAllText(filePath); // 读取存档文件内容

            if (enableEncrypt)
            {
                fileContent = XORDecrypt(fileContent); // 如果启用加密，解密文件内容
                if (string.IsNullOrEmpty(fileContent))
                {
                    Debug.LogError($"GetDataFormFile: 存档 {index} 解密失败。"); // 如果解密失败，记录错误并返回 null
                    return null;
                }
            }

            PlayerSaveData loadedData = JsonUtility.FromJson<PlayerSaveData>(fileContent); // 反序列化文件内容

            if (loadedData != null)
            {
                Debug.Log($"存档 {index} 加载成功！"); // 如果反序列化成功，记录日志并返回数据
                return loadedData;
            }
            else
            {
                Debug.LogError($"GetDataFormFile: 存档 {index} 反序列化失败。"); // 如果反序列化失败，记录错误并返回 null
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"GetDataFormFile: 加载存档 {index} 时发生错误: {e.Message}"); // 如果加载过程中发生异常，记录错误并返回 null
            return null;
        }
    }

    /// <summary>
    /// 用于开发者手动保存玩家数据到指定编号的存档文件中。
    /// </summary>
    /// <param name="num">要保存的存档编号。</param>
    /// <param name="data">待保存的玩家数据对象。</param>
    public void DeveloperSaveData(int num, PlayerSaveData data)
    {
        if (data == null)
        {
            Debug.LogError("SaveData: _currentSaveData 为 null，无法保存。");
            return;
        }

        string fileName = SaveConstants.SaveFileNameTemplate.Replace("{Field}", num.ToString());
        string filePath = Path.Combine(SaveConstants.SaveFolderPath, fileName);

        try
        {
            string jsonData = JsonUtility.ToJson(data, prettyPrint: true);

            if (enableEncrypt)
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
    
    
    
    
    
    
}