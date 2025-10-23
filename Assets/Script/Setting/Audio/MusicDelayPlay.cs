using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicDelayPlay : MonoBehaviour
{

    private void Start()
    {
        Invoke("Play",0.5f);
    }

    void Play()
    {
        GetComponent<AudioSource>().Play();
    }


}
