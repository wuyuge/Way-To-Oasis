using UnityEngine;
using UnityEngine.Serialization;


//开发者存档修改器
public class DeveloperSaveChanger : MonoBehaviour
{
    private SaveManager _saveManager;
    [SerializeField]
    private PlayerSaveData _saveData;
    public int saveIndex;
    private void Awake()
    {
        _saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        _saveData = new PlayerSaveData();
    }

    /// <summary>
    /// 接收并处理游戏存档数据，根据类型更新玩家存档中的相应字段。
    /// </summary>
    public void RcvData(string data, string type)
    {

    }

    public void Apply()
    {

        gameObject.transform.parent.GetComponent<DeveloperSaveChanger>().SaveData();

    }


    public void SaveData()
    {
        _saveManager.DeveloperSaveData(saveIndex,_saveData);
    }
    
    
    
}
