using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QteSliderManager : MonoBehaviour
{
    private Slider _slider;
    [Tooltip("反抗的强度开始后会自动除以60以适配60次调用")]
    public float maxIntensity;
    public float initialIntensity;
    public float currentIntensity;
    public float playerIntensity;
    private float _usedTime = 0f;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        maxIntensity = maxIntensity / 60;
        initialIntensity = initialIntensity / 60;
        currentIntensity = initialIntensity;
        
    }


    private void FixedUpdate()
    {
        if (currentIntensity < maxIntensity && _usedTime < 1)
        {
            currentIntensity += Mathf.Lerp(initialIntensity, maxIntensity, _usedTime / 1f);
            _usedTime += Time.deltaTime;
        }
        _slider.value -= currentIntensity;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _slider.value += playerIntensity;
        }
    }
}
