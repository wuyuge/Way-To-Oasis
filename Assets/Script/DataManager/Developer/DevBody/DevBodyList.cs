using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DevBodyList : MonoBehaviour
{
    public List<GameObject> bodyContainer;
    private string _bodyName,_usedBodyName;
    private int _bodyNum;
    public TextMeshProUGUI bodyNumText;
    public string initialText;
    public bool showLog;
    [SerializeField]
    private DeveloperSaveChanger _saveChanger;

    void Awake()
    {
        _saveChanger = gameObject.transform.parent.GetComponent<DeveloperSaveChanger>();
    }

    /// <summary>
    /// 当此脚本启用时，初始化身体容器的状态和相关文本。
    /// 该方法将所有身体容器设置为不可见，并重置记录的身体名称、已使用身体名称及数量。
    /// 同时更新UI上显示的身体数量。
    /// </summary>
    void OnEnable()
    {
        
        foreach (var value in bodyContainer)
        {
            value.SetActive(false);
        }
        _bodyName = string.Empty;
        _usedBodyName = string.Empty;
        _bodyNum = 0;
        bodyNumText.text = bodyNumText.text.Replace("{Body}", _bodyNum.ToString());
        _saveChanger.RcvData(_bodyNum.ToString(),"bodyNum");
    }

    /// <summary>
    /// 启用指定索引的身体容器，并更新身体名称和数量显示。
    /// </summary>
    /// <param name="index">要启用的身体容器索引，范围从0到5。</param>
    public void OpenBody(int index)
    {
        bodyContainer[index].SetActive(true);
        
        switch (index)
        {
           case 0:
               if (_bodyName != string.Empty) _bodyName += "/"; 
               if(!_bodyName.Contains("Leader")) _bodyName += "Leader";
               _bodyNum++;
               break;
           case 1:
               if (_bodyName != string.Empty) _bodyName += "/";
               if(!_bodyName.Contains("艾米莉")) _bodyName += "艾米莉";
               _bodyNum++;
               break;
           case 2:
               if (_bodyName != string.Empty) _bodyName += "/";
               if(!_bodyName.Contains("博金森")) _bodyName += "博金森";
               _bodyNum++;
               break;
           case 3:
               if (_bodyName != string.Empty) _bodyName += "/";
               if(!_bodyName.Contains("洛尔坎")) _bodyName += "洛尔坎";
               _bodyNum++;
               break;
           case 4:
               if (_bodyName != string.Empty) _bodyName += "/";
               if(!_bodyName.Contains("阿曼德")) _bodyName += "阿曼德";
               _bodyNum++;
               break;
           case 5:
               if (_bodyName != string.Empty) _bodyName += "/";
               if(!_bodyName.Contains("莱文")) _bodyName += "莱文";
               _bodyNum++;
               break;

        }
        _bodyName = _bodyName.Replace("//", "/");
        if (_bodyName.StartsWith("/"))
        {
            _bodyName = _bodyName.Substring(1);
        }
        if (_bodyName.EndsWith("/"))
        {
            _bodyName = _bodyName.Substring(0, _bodyName.Length - 1);
        }
        bodyNumText.text = initialText;
        bodyNumText.text = bodyNumText.text.Replace("{Body}", _bodyNum.ToString());
        _saveChanger.RcvData(_bodyNum.ToString(),"bodyNum");
        if (showLog) Debug.Log(_bodyName);

    }

    /// <summary>
    /// 从_bodyName中删除指定名称，并更新_bodyName字符串以移除不必要的斜杠。
    /// </summary>
    /// <param name="delName">要从_bodyName中删除的名称。</param>
    private void DeleteBody(string delName)
    {
        // 更新 _bodyName
        _bodyName = _bodyName.Replace(delName, string.Empty);
        _bodyName = _bodyName.Replace("//", "/");
        if (_bodyName.StartsWith("/"))
        {
            _bodyName = _bodyName.Substring(1);
        }
        if (_bodyName.EndsWith("/"))
        {
            _bodyName = _bodyName.Substring(0, _bodyName.Length - 1);
        }
        if (showLog) Debug.Log($"Deleted: {delName}, Remaining: {_bodyName}");
        else
        {
            if (showLog) Debug.Log($"Body with name '{delName}' not found.");
        }
    }

    /// <summary>
    /// 将指定索引的身体添加到已使用列表中，并从可选列表中移除。
    /// </summary>
    /// <param name="index">要添加的身体索引，范围从0到5。</param>
    public void AddUsedBody(int index)
    {
        switch (index)
        {
            case 0:
                if (_usedBodyName != string.Empty) _usedBodyName += "/"; 
                if(!_usedBodyName.Contains("Leader")) _usedBodyName += "Leader";
                _bodyNum--;
                DeleteBody("Leader");
                break;
            case 1:
                if (_usedBodyName != string.Empty) _usedBodyName += "/";
                if(!_usedBodyName.Contains("艾米莉")) _usedBodyName += "艾米莉";
                _bodyNum--;
                DeleteBody("艾米莉");
                break;
            case 2:
                if (_usedBodyName != string.Empty) _usedBodyName += "/";
                if(!_usedBodyName.Contains("博金森")) _usedBodyName += "博金森";
                _bodyNum--;
                DeleteBody("博金森");
                break;
            case 3:
                if (_usedBodyName != string.Empty) _usedBodyName += "/";
                if(!_usedBodyName.Contains("洛尔坎")) _usedBodyName += "洛尔坎";
                _bodyNum--;
                DeleteBody("洛尔坎");
                break;
            case 4:
                if (_usedBodyName != string.Empty) _usedBodyName += "/";
                if(!_usedBodyName.Contains("阿曼德")) _usedBodyName += "阿曼德";
                _bodyNum--;
                DeleteBody("阿曼德");
                break;
            case 5:
                if (_usedBodyName != string.Empty) _usedBodyName += "/";
                if(!_usedBodyName.Contains("莱文")) _usedBodyName += "莱文";
                _bodyNum--;
                DeleteBody("莱文");
                break;

        }
        bodyNumText.text = initialText;
        bodyNumText.text = bodyNumText.text.Replace("{Body}", _bodyNum.ToString());
        _saveChanger.RcvData(_bodyNum.ToString(),"bodyNum");
        
    }


    /// <summary>
    /// 从已使用的身体列表中删除指定名称的身体，并更新显示。
    /// </summary>
    /// <param name="delName">要删除的身体名称。</param>
    public void DeleteBodyFinally(string delName,int index)
    {
        // 更新 _usedBodyName
        _usedBodyName = _usedBodyName.Replace(delName, string.Empty);
        _usedBodyName = _usedBodyName.Replace("//", "/");
        if (_usedBodyName.StartsWith("/"))
        {
            _usedBodyName = _usedBodyName.Substring(1);
        }
        if (_usedBodyName.EndsWith("/"))
        {
            _usedBodyName = _usedBodyName.Substring(0, _usedBodyName.Length - 1);
        }
        if (showLog) Debug.Log($"Deleted: {delName}, Remaining: {_usedBodyName}");
        else
        {
            if (showLog) Debug.Log($"Body with name '{delName}' not found.");
        }
        
        bodyContainer[index].SetActive(false);
        _saveChanger.RcvData(_bodyNum.ToString(),"bodyNum");
        
    }

    public void SendData()
    {
        if (_bodyName != null)
        {
            _saveChanger.RcvData(_bodyName,"bodyList");
        }

        if (_usedBodyName != null)
        {
            _saveChanger.RcvData(_usedBodyName,"usedBodyList");
        }
        _saveChanger.RcvData(_bodyNum.ToString(),"bodyNum");
    }
    
    
    

}
