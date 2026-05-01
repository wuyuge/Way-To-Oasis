using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class FileButtonRefresh : MonoBehaviour
{

    public TextMeshProUGUI mainInfo,timeInfo,textOnImage;
    public SaveManager saveManager;
    public int num;
    public List<string> stageNames;
    public bool reportSaveIsNull = true;
    public List<string> initialText;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        try
        {
            PlayerSaveData data = saveManager.GetDataFromFile(num.ToString(),reportSaveIsNull);
            if (data == null)
            {
                Debug.Log("存档为空");
                mainInfo.text = "";
                timeInfo.text = "";
                textOnImage.text = "";
                return;
            }
            #region 替换文本

            InitialTextUI();
            
            
            mainInfo.text = mainInfo.text.Replace("{Day}",data.day.ToString());
            string stageString = string.Empty;
            switch (data.stage)
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
            timeInfo.text = timeInfo.text.Replace("{Time}",data.saveTime);
            textOnImage.text = textOnImage.text.Replace("{Day}", data.day.ToString());

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
        timeInfo.text = initialText[2];
        textOnImage.text = initialText[3];
    }
    
    
}
