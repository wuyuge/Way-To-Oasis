using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DownBar : MonoBehaviour
{
    [Tooltip("开始时在下面还是上面")]
    public bool StartDown;
    [Header("背包")]
    public Manager Final_Food, Final_Body;
    public GameObject FoodText, BodyText;
    public bool SetText;
    void Start()
    {
        if (StartDown)
        {
            gameObject.GetComponent<Animator>().SetTrigger("Down");
            SetOffAnimator();
            Invoke("SetOnAnimator",5f);

        }
        if (SetText)
        {
            Invoke("Set_Text", 1f);
            
        }
    }

    void Set_Text()
    {
        FoodText.GetComponent<TextMeshProUGUI>().text = Final_Food.Weight.ToString();
        BodyText.GetComponent<TextMeshProUGUI>().text = Final_Body.Weight.ToString();
    }




    // Update is called once per frame
    void SetOffAnimator()
    {
        gameObject.GetComponent<Animator>().enabled = false;
    }
    void SetOnAnimator()
    {
        gameObject.GetComponent<Animator>().enabled = true;
    }
}
