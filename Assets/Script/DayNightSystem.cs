using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class DayNightSystem : MonoBehaviour
{
    public Gradient gradient,backGradient;
    [Range(0f, 1f)] public float time;
    public bool on = true;
    public bool complete;
    public float cycleDurationInSeconds = 60f; 
    public Progress progress;
    private float timeIncrementPerFixedUpdate;
    private Light2D lightComponent;
    public BackGroundMoving BackGround;
    public float Frist, Second;
    private Image BackImage,BackImage2;
    private bool ShowStage;


    void Start()
    {
        BackImage = BackGround.backgroundLayers[0].layerObject.GetComponent<Image>();
        BackImage2 = BackGround.backgroundLayers[1].layerObject.GetComponent<Image>();
        lightComponent = GetComponent<Light2D>();
        CalculateTimeIncrement();
    }
    
    public void CalculateTimeIncrement()
    {
        timeIncrementPerFixedUpdate = 1f / (cycleDurationInSeconds * 50f);
    }

    private void FixedUpdate()
    {
        if (on && lightComponent != null)
        {
            
            if(time < Frist)
            {
                time += 0.8f * timeIncrementPerFixedUpdate;
            }
            else time += timeIncrementPerFixedUpdate;

            if (progress.start && time > Frist)
            { time = Frist; BackGround.open = false; ShowStage = false; }
            else if (progress.talk && time > Second)
            { 
                time = Second; 
                BackGround.open = false;
                if (!ShowStage)
                {
                    progress.TalkStage();
                    ShowStage = true;
                }
            }
            else if (!progress.start && !progress.food) BackGround.open = on;
            else if(progress.food) { BackGround.open = false; }
            
            if (time >= 1f)
            {
                time -= 1f; 
                on = false;
                complete = true;
            }
            lightComponent.color = gradient.Evaluate(time);
        }
    }

    
    private void OnValidate()
    {
        if (cycleDurationInSeconds < 0.1f)
        {
            cycleDurationInSeconds = 0.1f;
        }
        CalculateTimeIncrement();
    }

    public void Reset_Color()
    {
        lightComponent.color = gradient.Evaluate(time);
    }

    public void SetFirst()
    {
        time = Frist;
    }

    public void SetSecond()
    {
        time = Second;
    }
}
