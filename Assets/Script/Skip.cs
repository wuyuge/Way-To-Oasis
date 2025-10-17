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
    void Start()
    {
        anim = GetComponent<Animator>();
        
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


}
