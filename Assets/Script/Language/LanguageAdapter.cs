using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageAdapter : MonoBehaviour
{
    public Manager language;
    public string cn;
    public string en;
    private TextMeshProUGUI text;
    public bool isImage;
    public Image image;
    public Sprite cnImage,enImage;
    public bool isAnim;
    public bool isOptions;
    public List<string> cnOptions;
    public List<string> enOptions;
    public Manager playerName;
    public bool replaceName;
    
    private void OnEnable()
    {
        if (text == null && !isImage && !isAnim && !isOptions)
        {
            text = GetComponent<TextMeshProUGUI>();
            text.text = language.isEn ? en : cn;
            if (replaceName)
            {
                text.text = text.text.Replace("{PlayerName}", playerName.TxtLine[0]);
            }
        }

        if (isImage)
        {
            image = GetComponent<Image>();
            image.sprite = language.isEn ? enImage : cnImage;
        }

        if (isAnim)
        {
            GetComponent<Animator>().SetBool("en",language.isEn);
        }

        if (isOptions)
        {
            var dropdown = GetComponent<TMP_Dropdown>();
            for (int i = 0; i < dropdown.options.Count; i++)
            {
                dropdown.options[i].text = language.isEn ? enOptions[i] : cnOptions[i];
            }
        }
        
    }

    private void LateUpdate()
    {
        if (isAnim)
        {
            return;
        }
        if (isImage)
        {
            image.sprite = language.isEn ? enImage : cnImage;
        }
        if (isOptions)
        {
            var dropdown = GetComponent<TMP_Dropdown>();
            for (int i = 0; i < dropdown.options.Count; i++)
            {
                dropdown.options[i].text = language.isEn ? enOptions[i] : cnOptions[i];
            }
        }

        if (!isOptions && !isAnim && !isImage)
        {
            text.text = language.isEn ? en : cn;
            if (replaceName)
            {
                text.text = text.text.Replace("{PlayerName}", playerName.TxtLine[0]);
            }
        }
    }
}
