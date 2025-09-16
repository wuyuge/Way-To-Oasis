using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodChoice : MonoBehaviour
{
    private GameObject End;
    public Manager Have_Food;
    public bool have;
    public GameObject Food_Text;
    
    void Start()
    {
        End = GameObject.Find("End");

        

    }

    // Update is called once per frame
    void Update()
    {
        
        if (!End.GetComponent<Progress>().food)
        {
           
            gameObject.GetComponent<Toggle>().isOn = false;
            
        }


    }


    public void OnClik(bool choice)
    {
        
        if(End.GetComponent<Progress>().food)
        {
            if (Have_Food.Weight < 1 && !have)
            {
                gameObject.GetComponent<Toggle>().isOn = false;
                return;
            }
            

            if (Have_Food.Weight >= 1 && choice)
            {
                Have_Food.Weight -= 1;
                have = true;
                gameObject.transform.parent.GetComponent<Character>().eat = true;
                if(gameObject.transform.parent.GetComponent<Character>().end.GetComponent<Progress>().day_num == 1)
                { this.gameObject.transform.parent.GetComponent<Character>().weight.Day1Eat = true; }
            }
            if (!choice && have)
            {
                Have_Food.Weight += 1;
                gameObject.transform.parent.GetComponent<Character>().eat = false;
                if (gameObject.transform.parent.GetComponent<Character>().end.GetComponent<Progress>().day_num == 1)
                { this.gameObject.transform.parent.GetComponent<Character>().weight.Day1Eat = false; }
                have = false;
            }
            Food_Text.GetComponent<TextMeshProUGUI>().text = Have_Food.Weight.ToString();
        }

    }




}
