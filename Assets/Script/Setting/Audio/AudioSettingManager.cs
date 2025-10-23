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
        GetComponent<Slider>().value = (int)Volume;
        Invoke("Set", 0.0001f);
    }

    void Set()
    {
        float Volume = 0;
        if (VolumeParameter == "Master") Volume = Manager.setting.MainVolume;
        else if (VolumeParameter == "Audio") Volume = Manager.setting.AudioVolume;
        else if (VolumeParameter == "Effect") Volume = Manager.setting.EffectVolume;
        GetComponent<Slider>().value = Volume;
        SetVolume(Volume);
    }


    public void SetVolume(float volume)
    {
        AudioMixer.SetFloat(VolumeParameter, volume);
        if (VolumeParameter == "Master") Manager.setting.MainVolume = (int)volume;
        else if (VolumeParameter == "Audio") Manager.setting.AudioVolume = (int)volume;
        else if (VolumeParameter == "Effect") Manager.setting.EffectVolume = (int)volume;
    }



}
