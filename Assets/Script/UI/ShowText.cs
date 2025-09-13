using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class ShowText : MonoBehaviour
{
    public Manager Text;
    public bool start;
    


    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        foreach (string s in Text.TxtLine)
        {
            foreach (char c in s)
            {
                gameObject.GetComponent<TextMeshProUGUI>().text += c;
            }
            gameObject.GetComponent<TextMeshProUGUI>().text = "";


        }




    }
}
