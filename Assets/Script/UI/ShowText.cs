using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;


public class ShowText : MonoBehaviour
{
    [FormerlySerializedAs("Text")] public Manager text;
    public bool start;
    // Update is called once per frame
    void FixedUpdate()
    {
        
        foreach (string s in text.TxtLine)
        {
            foreach (char c in s)
            {
                gameObject.GetComponent<TextMeshProUGUI>().text += c;
            }
            gameObject.GetComponent<TextMeshProUGUI>().text = "";


        }




    }
}
