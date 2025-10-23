using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Skip : MonoBehaviour
{


    private Animator anim;
    public GameObject Report;
    public GameObject SwitchStageBar;
    public TalkSystem TalkSystem;
    private bool Turn;
    public Manager CurrentDead;
    public List<AudioClip> Clips;
    void Start()
    {
        anim = GetComponent<Animator>();
        ShowText();
        Turn = true;

    }
    private void Update()
    {
        if((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))&& Turn)
        {
            Turn = false;
            TurnBright();
        }
    }

    public void TurnDark()//只变黑
    {
         anim.SetTrigger("dark"); 
        
    }

    public void TurnBright()
    {
        anim.SetTrigger("bright");
    }

    public void ShowText()
    {
        Report.GetComponent<Report>().ShowText();
    }


    public void SwitchWeight()
    {
        SwitchStageBar.SetActive(true);
        SwitchStageBar.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "分配负重阶段";
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
        Turn = true;
    }



}
