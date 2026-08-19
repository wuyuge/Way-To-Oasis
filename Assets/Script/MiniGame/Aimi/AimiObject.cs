using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimiObject : MonoBehaviour
{
    public bool search;
    public bool check;
    public Animator anim;
    private static RectTransform Line => AimiGlobalManager.LineColl;
    private RectTransform _rectTransform;
    public AimiData data;
    public Sprite checkSprite;
    private Image _image;
    public Manager language;
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
    }

    private void LateUpdate()
    {
        if (check)
        {
            return;
        }
        if (!search && !check)
        {
            if (UiCollider.IsCollision(Line,_rectTransform))
            {
                anim.SetBool("search", true);
                search = true;
            }
        }
        else if (!check)
        {
            anim.SetBool("search", false);
            search = false;
        }

        if (UiCollider.IsCollision(AimiGlobalManager.Player.rectTransform, _rectTransform))
        {
            check = true;
            anim.SetTrigger("checked");
            _image.sprite = checkSprite;
            AimiGlobalManager.CheckNums++;
            AimiGlobalManager.EffectPlayer.PlayerSound(AimiEffectPlayer.EffectType.Collect);
            if (AimiGlobalManager.CheckNums >= AimiGlobalManager.ObjectNums)
            {
                return;
            }
            if (AimiGlobalManager.CheckNums == AimiGlobalManager.ObjectNums / 2)
            {
                return;
            }
            AimiGlobalManager.TalkManager.SetText(language.isEn ? data.en : data.description,data.expression);
            
            
        }
    }
    
    
}
