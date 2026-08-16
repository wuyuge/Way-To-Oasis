using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioEffectType
{
    None,
    剑击打
}

public class AdditionAudioEffect : MonoBehaviour
{
    public AudioSource audioSource;
    [System.Serializable]
    public class EffectList
    {
        public AudioEffectType type;
        public AudioClip clip;
    }
    public List<EffectList> effectList = new List<EffectList>();

    private void Awake()
    {
        GlobalData.AudioEffect = this;
    }


    public void Play(AudioEffectType effectType)
    {
        if (effectType == AudioEffectType.None)
        {
            return;
        }

        foreach (var value in effectList)
        {
            if (effectType == value.type)
            {
                audioSource.clip = value.clip;
                audioSource.Play();
            }
        }
        
    }
    


}
