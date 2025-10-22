using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

[System.Serializable]
public class PlayerSetting
{
    public int ScreenMode;
    public bool ShowFPS;
    public int Anti;
    public int Vsync;
    public int RefreshFPS;
    public float TextSpeed;
    public float TextSize;
    public int MainVolume;
    public int AudioVolume;
    public int EffectVolume;
}

public class SettingDataManager : MonoBehaviour
{
    public PlayerSetting setting;
    public bool encrypt = true;  // 默认启用加密
    private string encryptionKey = "Way_To_Oasis";  // 加密密钥
    
    public List<GameObject> InitializeObj;

    private void Awake()
    {
        // 确保保存目录存在
        string directory = Application.streamingAssetsPath + "/Saves/Setting/";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        Load();

        foreach (var obj in InitializeObj)
        {
            Debug.Log($"初始化{obj.name}");
            obj.GetComponent<SettingInitialize>().Initialize(this);
        }

    }

    void SaveSetting()
    {
        string json = JsonUtility.ToJson(setting, true);
        string filePath = Application.streamingAssetsPath + "/Saves/Setting/PlayerSetting.json";

        if (encrypt)
        {
            // 加密后保存
            byte[] encryptedData = EncryptData(Encoding.UTF8.GetBytes(json));
            File.WriteAllBytes(filePath, encryptedData);
        }
        else
        {
            // 不加密直接保存
            File.WriteAllText(filePath, json);
        }
    }
    public void Save()
    {
        SaveSetting();
        Debug.Log("设置已保存");
    }

    public void Load()
    {
        string filePath = Application.streamingAssetsPath + "/Saves/Setting/PlayerSetting.json";

        // 如果文件不存在，初始化新设置
        if (!File.Exists(filePath))
        {
            setting = new PlayerSetting();
            Save();
            return;
        }

        try
        {
            if (encrypt)
            {
                // 解密后读取
                byte[] encryptedData = File.ReadAllBytes(filePath);
                byte[] decryptedData = DecryptData(encryptedData);
                string json = Encoding.UTF8.GetString(decryptedData);
                setting = JsonUtility.FromJson<PlayerSetting>(json);
            }
            else
            {
                // 不加密直接读取
                string json = File.ReadAllText(filePath);
                setting = JsonUtility.FromJson<PlayerSetting>(json);
            }
            Debug.Log("设置已读取");
        }
        catch
        {
            // 读取失败时初始化新设置
            Debug.LogWarning("设置文件损坏，已重置为默认设置");
            setting = new PlayerSetting();
            Save();
        }
    }

    // 加密数据（异或加密）
    private byte[] EncryptData(byte[] data)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);
        byte[] result = new byte[data.Length];

        for (int i = 0; i < data.Length; i++)
        {
            // 使用密钥进行异或运算
            result[i] = (byte)(data[i] ^ keyBytes[i % keyBytes.Length]);
        }

        return result;
    }

    // 解密数据（与加密算法相同，异或运算可逆）
    private byte[] DecryptData(byte[] data)
    {
        // 异或加密的解密算法与加密相同
        return EncryptData(data);
    }
}