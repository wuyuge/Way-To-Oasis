using System.IO;
using UnityEngine;

public static class SaveConstants
{
    // 存档路径（统一拼接，避免重复书写）
    public static string SaveFolderPath => Path.Combine(Application.streamingAssetsPath, "Saves", "Saves");
    public static string SaveFileNameTemplate => "player_data{Field}.save";
    public static string EncryptionKey => "Way_To_Oasis"; // 加密密钥
    public static string LoadingSceneName => "Loading"; // 加载场景名称
}
