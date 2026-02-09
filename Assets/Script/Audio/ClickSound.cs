using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSound : MonoBehaviour
{
    public AudioManager AudioManager;

    public void Start()
    {
        if (AudioManager is null)
        {
            AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        }
        
    }


    public void Click()
    {
        AudioManager.AudioPlayer("Click");
    }


}
