using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSpeedManager : MonoBehaviour
{
    public TalkSystem talkSystem,sys2;
    public int MinSpeed,DefultSpeed,MaxSpeed;
    public Slider OtherSlider;
    

    public void SetTextSpeed(float speed)
    {
        int Difference = MaxSpeed - MinSpeed;
        OtherSlider.value = speed;
        Difference = (int)(Difference * speed);
        talkSystem.TextSpeedI = MinSpeed + Difference;
        sys2.TextSpeedI = MinSpeed + Difference;



    }



}
