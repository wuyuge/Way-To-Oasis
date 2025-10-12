using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingManager : MonoBehaviour
{
    public AudioMixer AudioMixer;
    public string VolumeParameter;

    public void SetVolume(float volume)
    {
        AudioMixer.SetFloat(VolumeParameter, volume);
    }



}
