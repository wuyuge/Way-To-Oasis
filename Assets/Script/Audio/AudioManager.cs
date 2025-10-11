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




}
