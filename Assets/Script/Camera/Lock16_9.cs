using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Lock16_9 : MonoBehaviour
{
    private const float TargetAspect = 16f / 9f;

    void Start()
    {
        FixAspect();
    }

    void Update()
    {
        FixAspect();
    }

    void FixAspect()
    {
        float screenAspect = (float)Screen.width / Screen.height;
        Camera cam = Camera.main;

        // 屏幕更宽 → 左右黑边
        if (screenAspect > TargetAspect)
        {
            float w = TargetAspect / screenAspect;
            cam.rect = new Rect((1 - w) / 2, 0, w, 1);
        }
        // 屏幕更高 → 上下黑边
        else
        {
            float h = screenAspect / TargetAspect;
            cam.rect = new Rect(0, (1 - h) / 2, 1, h);
        }
    }
}