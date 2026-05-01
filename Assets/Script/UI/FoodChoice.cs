using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FoodChoice : MonoBehaviour
{
    public GameObject End;
    public Manager Have_Food;
    public bool have;
    public GameObject Food_Text;
    public Character character;
    private bool Ban;
    private Toggle _toggle;


    void Awake()
    {
        _toggle = gameObject.GetComponent<Toggle>();
        Invoke("open",0.8f);
        if (character is null)
        {
            character = gameObject.transform.parent.gameObject.GetComponent<Character>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalData.Progress is not null)
        {
            if (!GlobalData.Progress.food)
            {
                _toggle.isOn = false;
            }
        }
        else
        {
            _toggle.enabled = true;
        }

        have = character.weight.Eat;
        _toggle.isOn = character.weight.Eat;

    }


    public void OnClik(bool choice)
    {
        if (Ban) return;
        
        if(GlobalData.Progress.food)
        {
            if (Have_Food.Weight < 1 && !have)
            {
                _toggle.isOn = false;
                return;
            }

            if (Have_Food.Weight >= 1 && choice)
            {
                Have_Food.Weight -= 1;
                have = true;
                character.eat = true;
                character.weight.Eat = true; 
            }
            if (!choice && have)
            {
                Have_Food.Weight += 1;
                character.eat = false;
                character.weight.Eat = false; 
                have = false;
            }
            Food_Text.GetComponent<TextMeshProUGUI>().text = Have_Food.Weight.ToString();
        }

    }

    void open()
    {
        Ban = false;
    }
    


}
