using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class AudioLine
    {
        public string Name;
        public AudioClip Clip;
    }
    public List<AudioLine> AudioLines;
    private AudioSource AudioSource;
    public AudioSource HeavyClick;
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    public void AudioPlayer(string Name)
    {

        foreach (AudioLine line in AudioLines)
        {
            if (line.Name == Name)
            {
                AudioSource.clip = line.Clip;
                AudioSource.Play();
            }

        }


    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(HeavyClick.isPlaying) HeavyClick.Stop();
            HeavyClick.Play();
        }
    }
}





