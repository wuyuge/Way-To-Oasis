using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextSizeManager : MonoBehaviour , SettingInitialize
{
    [System.Serializable]
    public class TextSize
    {
        public TextMeshProUGUI TextUI;
        public float DefultSize;
        public float MaxSize;
        public float MinSize;
        
    }

    
    public List<TextSize> TextSizes = new List<TextSize>();
    private SettingDataManager Manager;

    public void Initialize(SettingDataManager manager)
    {
        Manager = manager;

        SetTextSize(manager.setting.TextSize);
        GetComponent<Slider>().value = manager.setting.TextSize;
        Debug.Log("文字大小初始化");


    }



    public void SetTextSize(float size)
    {
        GetComponent<Slider>().value = size;
        Manager.setting.TextSize = size;



        foreach (var Text in TextSizes)
        {
            float CurrentSize;
            float DiffSize = Text.MaxSize - Text.MinSize;
            DiffSize *= size;
            CurrentSize = Text.MinSize + DiffSize;
            if (CurrentSize > Text.DefultSize)
            {
                Text.TextUI.fontSizeMax = CurrentSize;
                Text.TextUI.fontSizeMin = Text.MinSize;
            }
            else
            {
                Text.TextUI.fontSizeMax = CurrentSize;
                Text.TextUI.fontSizeMin = CurrentSize;
            }
            


        }

    }

}
