using UnityEngine;
using UnityEngine.Serialization;

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
    /// <param name="data">要更新的数据值，格式根据type参数而变化。</param>
    /// <param name="type">指定待更新的存档数据类型，"day":游戏天数, "food":食物数量, "bodyList":死亡的人名用"/"分隔, "usedBodyList":消耗尸体的名字用"/"分隔
    ///  "bodyNum":拥有尸体数量, "Amande":阿曼德自杀标记 0 false 1 true, "saveIndex":保存存档栏位等,"stage":阶段区分。</param>
    public void RcvData(string data, string type)
    {
        switch (type)
        {
            case "day":
                _saveData.Day = int.Parse(data);
                if (int.Parse(data) != 0)
                {
                    _saveData.InMain = true;
                }
                break;
            case "food":
                _saveData.Food = int.Parse(data);
                break;
            case "stage":
                switch (data)
                {
                    case "0":
                        _saveData.Stage = 0;
                        break;
                    case "1":
                        _saveData.Stage = 1;
                        break;
                    case "2":
                        _saveData.Stage = 2;
                        break;
                }
                break;
            case "bodyList":
                string[] tempData;
                _saveData.DeadName.Clear();
                if (data == "") return;
                if (data.Contains("/"))
                {
                    tempData = data.Split('/');
                    foreach (var value in tempData)
                    {
                        _saveData.DeadName.Add(value);
                    }
                }
                else
                {
                    _saveData.DeadName.Add(data);
                }
                break;
            case "usedBodyList":
                _saveData.UsedName.Clear();
                if (data == "") return;
                if (data.Contains("/"))
                {
                    tempData = data.Split('/');
                    foreach (var value in tempData)
                    {
                        _saveData.UsedName.Add(value);
                    }
                }
                else
                {
                    _saveData.UsedName.Add(data);
                }
                break;
            case "bodyNum":
                _saveData.Body = int.Parse(data);
                break;
            case "Amande":
                _saveData.AmandeKillSelf = data == "1";
                break;
            case "saveIndex":
                saveIndex = int.Parse(data);
                break;
        }
        
        
        
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
