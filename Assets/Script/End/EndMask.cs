using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndMask : MonoBehaviour
{

    public Image image;
    public Material material;
    public GameObject BackToStart;
    public GameObject Text;
    private bool Breaking;
    public float AddAmount;
    new public AudioSource audio;
    private void Start()
    {
        material = GetComponent<Image>().material;
        material.SetFloat("_BreakAmount", 0);

    }


    public void Click()
    {
        Breaking = true;
        audio.Play();

    }

    public void FixedUpdate()
    {
        if (Breaking)
        {
            material.SetFloat("_BreakAmount", material.GetFloat("_BreakAmount") +AddAmount);
            if(material.GetFloat("_BreakAmount") == 1)
            {
                Breaking = false;
            }
        }

        if (material.GetFloat("_BreakAmount") >= 1-AddAmount && Breaking)
        {

            Invoke("ActiveText",0.25f);
        }
    }

    void ActiveText()
    {
        if (!Text.activeSelf)
            Text.SetActive(true);
        if (!BackToStart.activeSelf)
            BackToStart.SetActive(true);
    }





}
