using System;
using UnityEngine;
using UnityEngine.Serialization;

public class AimiPlayer : MonoBehaviour
{
    public RectTransform rectTransform;
    public float moveSpeed = 500f;
    public RectTransform originPoint;
    public int radius = 200;

    [Header("静止检测设置")]
    public float idleInterval = 60f;
    public float moveThreshold = 1f;

    private Vector2 _lastPosition;
    private float _idleTimer;
    public Manager start;

    private void Awake()
    {
        AimiGlobalManager.Player = this;
    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        _lastPosition = rectTransform.anchoredPosition;
        _idleTimer = 0f;
    }

    private void Update()
    {
        if (start.GeneralBool && !AimiGlobalManager.Failed)
        {
            Movement();
            CheckIdleLoopTrigger();
        }
    }

    private void Movement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector2 targetPos = rectTransform.anchoredPosition;
        targetPos.x += horizontal * moveSpeed * Time.deltaTime;
        targetPos.y += vertical * moveSpeed * Time.deltaTime;

        Vector2 dir = targetPos - originPoint.anchoredPosition;
        float distance = dir.magnitude;
        if (distance >= radius)
        {
            targetPos = originPoint.anchoredPosition + dir.normalized * radius;
        }
        rectTransform.anchoredPosition = targetPos;
    }

    private void CheckIdleLoopTrigger()
    {
        Vector2 currentPos = rectTransform.anchoredPosition;
        bool isMoving = Vector2.Distance(currentPos, _lastPosition) > moveThreshold;

        if (isMoving)
        {
            _idleTimer = 0f;
        }
        else
        {
            _idleTimer += Time.deltaTime;
            if (_idleTimer >= idleInterval)
            {
                OnIdleLoopEvent();
                _idleTimer = 0f;
            }
        }
        _lastPosition = currentPos;
    }

    private void OnIdleLoopEvent()
    {
        Debug.Log("静止触发事件");
        AimiGlobalManager.TalkManager.SetTimeOut();
    }
}