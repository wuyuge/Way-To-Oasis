using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssResources : MonoBehaviour
{
    public Progress End;
    public bool Food;
    public bool Body,Weight_Food;
    public Toggle Food_Toogle, Body_Toogle;

    // Update is called once per frame
    private void Update()
    {
        if (End.start != true)
        {
            
            Food_Toogle.enabled = false;
            Body_Toogle.enabled = false;
        }
        else if (End.start)
        {

            Food_Toogle.enabled = true;
            Body_Toogle.enabled = true;


        }

        Switch();

        

        if (!End.start)
        {
            Body = false;
            Weight_Food = false;
            Body_Toogle.isOn = false;
            Food_Toogle.isOn = false;
        }
    }
    


    
    void Switch()
    {
        if (Food_Toogle.enabled)
        {
            if (Food_Toogle.isOn)
            {
                Weight_Food = true;
                Body = false;
                
            }
            else if(Body_Toogle.isOn)
            {
                Weight_Food = false;
                Body = true;
                
            }
            else
            {
                Weight_Food = false;
                Body = false;
            }
        }
    }






}
