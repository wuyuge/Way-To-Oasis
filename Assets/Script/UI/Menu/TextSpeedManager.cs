using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextSpeedManager : MonoBehaviour, IPointerUpHandler,SettingInitialize
{
    public TalkSystem talkSystem,sys2;
    public int MinSpeed,DefultSpeed,MaxSpeed;
    public Slider OtherSlider;
    public TextPreview Preview;
    private int PreviewSpeed;
    private SettingDataManager Manager;


    public void Initialize(SettingDataManager manager)
    {
        
        Manager = manager;
        if (SceneManager.GetActiveScene().name != "Start")
        {
            SetTextSpeed(manager.setting.TextSpeed);
        }

        GetComponent<Slider>().value = (float)manager.setting.TextSpeed;

    }


    

    public void SetTextSpeed(float speed)
    {
        int Difference = MaxSpeed - MinSpeed;
        OtherSlider.value = speed;
        Difference = (int)(Difference * speed);
        if (talkSystem != null)
        talkSystem.TextSpeedI = (MinSpeed + Difference)*0.001f;
        if (sys2 != null)
        sys2.TextSpeedI = (MinSpeed + Difference)*0.001f;
        PreviewSpeed = MinSpeed + Difference;

        Manager.setting.TextSpeed = speed;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        _ = Preview.ResetSpeed(PreviewSpeed);
    }



}
