using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class FileButtonRefresh : MonoBehaviour
{

    public TextMeshProUGUI mainInfo,secInfo,timeInfo,textOnImage;
    private SaveManager _saveManager;
    public int num;
    public List<string> stageNames;
    public bool reportSaveIsNull = true;
    public List<string> initialText;

    private void Awake()
    {
        _saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        
    }

    private void OnEnable()
    {
        Refresh();

    }

    public void Refresh()
    {
        try
        {
            PlayerSaveData data = _saveManager.GetDataFormFile(num.ToString(),reportSaveIsNull);
            if (data == null)
            {
                Debug.Log("存档为空");
                mainInfo.text = "";
                secInfo.text = "";
                timeInfo.text = "";
                textOnImage.text = "";
                return;
            }
            #region 替换文本

            InitialTextUI();
            
            
            mainInfo.text = mainInfo.text.Replace("{Day}",data.Day.ToString());
            string stageString = string.Empty;
            switch (data.Stage)
            {
                case 0:
                    stageString = stageNames[0];
                    break;
                case 1:
                    stageString = stageNames[1];
                    break;
                case 2:
                    stageString = stageNames[2];
                    break;
            }
            mainInfo.text = mainInfo.text.Replace("{Stage}", stageString);
            secInfo.text = secInfo.text.Replace("{Food}", data.Food.ToString());
            secInfo.text = secInfo.text.Replace("{Body}",data.Body.ToString());
            timeInfo.text = timeInfo.text.Replace("{Time}",data.SaveTime);
            textOnImage.text = textOnImage.text.Replace("{Day}", data.Day.ToString());

            #endregion

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    
    
    /// <summary>
    /// 重新将UI文本设为初始值
    /// </summary>
    void InitialTextUI()
    {
        mainInfo.text = initialText[0];
        secInfo.text = initialText[1];
        timeInfo.text = initialText[2];
        textOnImage.text = initialText[3];
    }
    
    
}
