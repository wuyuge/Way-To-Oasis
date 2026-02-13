using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class DayNightSystem : MonoBehaviour
{
    [Tooltip("������ҹ�仯����ɫ����")]
    public Gradient gradient,backGradient;

    [Tooltip("��ǰʱ�����(0-1)��0��ʾ���1��ʾ����")]
    [Range(0f, 1f)] public float time;

    [Tooltip("�Ƿ�������ҹϵͳ")]
    public bool on = true;
    public bool complete;
    [Tooltip("���һ����ҹѭ�����������")]
    public float cycleDurationInSeconds = 60f; // Ĭ��60�����һ��ѭ��
    public Progress progress;
    private float timeIncrementPerFixedUpdate;
    private Light2D lightComponent;
    public BackGroundMoving BackGround;
    [Header("���们��")]
    public float Frist, Second;

    private Image BackImage,BackImage2;

    private bool ShowStage;


    void Start()
    {
        BackImage = BackGround.backgroundLayers[0].layerObject.GetComponent<Image>();
        BackImage2 = BackGround.backgroundLayers[1].layerObject.GetComponent<Image>();
        // ��ȡLight2D��������棬�����ظ���ȡ
        lightComponent = GetComponent<Light2D>();

        // ����ÿ֡��ʱ��������ʹ��ҹѭ������ָ�����������
        CalculateTimeIncrement();
    }

    // ����ʱ�������ķ��������޸�ѭ��ʱ�������ֶ�����
    public void CalculateTimeIncrement()
    {
        // FixedUpdateÿ���Լִ��50��
        // ��֡�� = ѭ������ * 50
        // ÿ֡���� = 1 / ��֡��
        timeIncrementPerFixedUpdate = 1f / (cycleDurationInSeconds * 50f);
    }

    private void FixedUpdate()
    {
        if (on && lightComponent != null)
        {
            // ����ʱ�����
            if(time < Frist)
            {
                time += 0.8f * timeIncrementPerFixedUpdate;
            }
            else
            time += timeIncrementPerFixedUpdate;

            if (progress.start && time > Frist)//�峿
            { time = Frist; BackGround.open = false; ShowStage = false; }
            else if (progress.talk && time > Second)//�ƻ�
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

              
            // ȷ��ʱ����0-1��Χ��ѭ��
            if (time >= 1f)
            {
                time -= 1f; // ��ȥ1��������Ϊ0������ѭ����������
                on = false;
                complete = true;
            }

            // Ӧ����ɫ�仯
            lightComponent.color = gradient.Evaluate(time);
        }
    }

    // ��ѭ��ʱ��ı�ʱ�Զ����¼�������
    private void OnValidate()
    {
        // ȷ��ѭ��ʱ�䲻��С��0.1�룬������ֵ����
        if (cycleDurationInSeconds < 0.1f)
        {
            cycleDurationInSeconds = 0.1f;
        }

        // �ڱ༭��ģʽ��Ҳ����ʱ������
        CalculateTimeIncrement();
    }

    public void Reset_Color()
    {
        lightComponent.color = gradient.Evaluate(time);
    }


}
