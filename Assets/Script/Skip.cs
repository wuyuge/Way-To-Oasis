using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Skip : MonoBehaviour
{


    private Animator _anim;
    public GameObject Report;
    public GameObject SwitchStageBar;
    public TalkSystem TalkSystem;
    private bool _turn;
    public Manager CurrentDead;
    public List<AudioClip> Clips;
    public GameObject Tips;
    private Image _image;
    private Progress _progress;
    void Start()
    {
        _anim = GetComponent<Animator>();
        _image = GetComponent<Image>();
        ShowText();
        _turn = true;
        _progress = TalkSystem.DaytimeOBJ.GetComponent<Progress>();

    }
    private void Update()
    {
        if(Input.anyKeyDown && _turn)
        {
            _turn = false;
            TurnBright();
        }
        if(_image.color.a == 0 && Tips.activeSelf)
        {
            Tips.SetActive(false);
        }
    }

    public void TurnDark()//只变黑
    {
         _anim.SetTrigger("dark"); 
        
    }

    public void TurnBright()
    {
        _anim.SetTrigger("bright");
    }

    public void ShowText()
    {
        Report.GetComponent<Report>().ShowText();
    }


    public void ShowTips()
    {
        Tips.SetActive(true);
    }

    public void HideTips()
    {
        Tips.SetActive(false);
    }

    
    /// <summary>
    /// 设置不同阶段的提示文字
    /// </summary>
    public void SwitchWeight()
    {
        
        SwitchStageBar.SetActive(true);
    }

    public void SetOn()
    {
        TalkSystem.on = true;
    }

    public void SetOff()
    {
        TalkSystem.on = false;
    }

    public void PlayAudio()
    {
        if(CurrentDead.TxtLine.Count != 0)
        {
            GetComponent<AudioSource>().clip = Clips[1];
        }
        else
        {
            GetComponent<AudioSource>().clip = Clips[0];
        }


        GetComponent<AudioSource>().Play();
    }

    public void SetTurn()
    {
        _turn = true;
    }



}
