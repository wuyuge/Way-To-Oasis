using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextSpeedManager : MonoBehaviour, IPointerUpHandler
{
    public TalkSystem talkSystem,sys2;
    public int MinSpeed,DefultSpeed,MaxSpeed;
    public Slider OtherSlider;
    public TextPreview Preview;
    private int PreviewSpeed;
    

    public void SetTextSpeed(float speed)
    {
        int Difference = MaxSpeed - MinSpeed;
        OtherSlider.value = speed;
        Difference = (int)(Difference * speed);
        if (talkSystem != null)
        talkSystem.TextSpeedI = MinSpeed + Difference;
        if (sys2 != null)
        sys2.TextSpeedI = MinSpeed + Difference;
        PreviewSpeed = MinSpeed + Difference;



    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("UI元素上鼠标抬起");
        _ = Preview.ResetSpeed(PreviewSpeed);
    }



}
