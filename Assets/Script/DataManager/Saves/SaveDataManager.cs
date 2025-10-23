using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[System.Serializable]
public class PlayerSave
{
    public string Name;
    public string Day_Num;
    public List<string> DeadName;
    public List<string> UsedBody;
    public int Have_Body;
    public int Have_Food;
    public bool AmandeKillSelf;

    public string Stage;

}


public class SaveDataManager : MonoBehaviour
{
    public List<PlayerSave> Saves;

    private string encryptionKey = "Way_To_Oasis";  // 加密密钥
    public string FilePath;

    public void Awake()
    {
        FilePath = Application.streamingAssetsPath + "/Saves/Saves";
    }



    public void Save()
    {
        string json = JsonUtility.ToJson(Saves, true);


    }

    public void Load()
    {

    }



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
