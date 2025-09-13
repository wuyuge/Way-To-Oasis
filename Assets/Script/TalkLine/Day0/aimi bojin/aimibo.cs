using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aimibo : MonoBehaviour
{

    public GameObject LinkObj;


    

    // Update is called once per frame
    void Update()
    {
        if(LinkObj.GetComponent<Character>().have_talk == true)
        {
            gameObject.GetComponent<Character>().have_talk = true;
        }
    }
}
