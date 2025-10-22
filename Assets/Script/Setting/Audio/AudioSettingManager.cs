using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class AudioSettingManager : MonoBehaviour,SettingInitialize
{
    public AudioMixer AudioMixer;
    public string VolumeParameter;
    private SettingDataManager Manager;

    public void Initialize(SettingDataManager manager)
    {
        Manager = manager;
        float Volume = 0;
        if(VolumeParameter == "Master") Volume = manager.setting.MainVolume;
        else if(VolumeParameter == "Audio") Volume = manager.setting.AudioVolume;
        else if(VolumeParameter == "Effect") Volume = manager.setting.EffectVolume;

        AudioMixer.SetFloat(VolumeParameter, Volume);
        GetComponent<Slider>().value = (int)Volume;
        
    }



    public void SetVolume(float volume)
    {
        AudioMixer.SetFloat(VolumeParameter, volume);
        if (VolumeParameter == "Master") Manager.setting.MainVolume = (int)volume;
        else if (VolumeParameter == "Audio") Manager.setting.AudioVolume = (int)volume;
        else if (VolumeParameter == "Effect") Manager.setting.EffectVolume = (int)volume;
    }



}
