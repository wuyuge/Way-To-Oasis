using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextSizeManager : MonoBehaviour
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
    public Slider OtherSlider;


    public void SetTextSize(float size)
    {
        OtherSlider.value = size;
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
