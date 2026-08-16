using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimiEffectPlayer : MonoBehaviour
{
    public enum EffectType
    {
        Collect,
    }

    [System.Serializable]
    public class Effect
    {
        public EffectType type;
        public AudioClip clip;
    }
    public AudioSource aS;
    public List<Effect> effects;

    private void OnEnable()
    {
        AimiGlobalManager.EffectPlayer = this;
    }


    public void PlayerSound(EffectType type)
    {
        foreach (var value in effects)
        {
            if (value.type == type)
            {
                aS.clip = value.clip;
                aS.Play();
            }
        }
    }

}
