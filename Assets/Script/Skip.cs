using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Skip : MonoBehaviour
{


    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        
        



    }



    public void TurnDark()//Ö»±äºÚ
    {
         anim.SetTrigger("dark"); 
        
    }

    public void TurnBright()
    {
        anim.SetTrigger("bright");
    }



}
