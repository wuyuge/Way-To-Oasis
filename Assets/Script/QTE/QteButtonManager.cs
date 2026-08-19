using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class QteButtonManager : MonoBehaviour
{
    public Image remainingImage;
    public float decreaseSpeed;
    private KeyCode _needKey;
    public TextMeshProUGUI keyText;
    private QteButtonSpawn _qteButtonSpawn;
    private bool _canUnlocked;
    private bool _unlocked;
    public int rangeX, rangeY;
    private Transform _tempTransform;
    private Vector3 _initialPosition;

    /// <summary>
    /// 当脚本实例被加载时调用，初始化减少速度、获取QteButtonSpawn组件，并保存当前RectTransform的位置。
    /// </summary>
    private void Awake()
    {
        decreaseSpeed = decreaseSpeed / 60;
        _qteButtonSpawn = transform.parent.GetComponent<QteButtonSpawn>();
        _tempTransform = GetComponent<RectTransform>();
        _initialPosition = _tempTransform.position;
    }

    /// <summary>
    /// 当此组件启用时，初始化按钮的位置、解锁状态、剩余时间条的填充量和颜色，并随机生成需要按下的键。
    /// </summary>
    private void OnEnable()
    {
        _tempTransform.position = new Vector2(_initialPosition.x + Random.Range(-rangeX,rangeX), _initialPosition.y + Random.Range(-rangeY,rangeY));
        _unlocked = false;
        remainingImage.fillAmount = 1f;
        remainingImage.color = Color.green;
        RandomInput();
        _canUnlocked = true;
    }

    /// <summary>
    /// 在每一固定帧更新时检查并减少剩余图像的填充量，当填充量归零时解锁，并在短暂延迟后锁定。
    /// 如果当前未解锁且剩余图像的填充量大于0，则根据设定的速度减少填充量。
    /// 当填充量减少到0或以下时，重置为1，设置为已解锁状态，将剩余图像颜色变为红色，并安排在0.5秒后调用Locked方法。
    /// </summary>
    private void FixedUpdate()
    {
        if (remainingImage.fillAmount > 0 && !_unlocked)
        {
            remainingImage.fillAmount -= decreaseSpeed;
            if (remainingImage.fillAmount <= 0)
            {
                remainingImage.fillAmount = 1;
                _unlocked = true;
                remainingImage.GetComponent<Image>().color = Color.red;
                Invoke(nameof(Locked),0.25f);
            }
        }
    }

    /// <summary>
    /// 将当前游戏对象添加到按钮队列中，并设置其为非激活状态，同时禁止解锁。
    /// 该方法首先通过QteButtonSpawn实例将此游戏对象加入到队列里，然后将自身设为非激活状态，最后更新_canUnlocked标志以防止再次解锁。
    /// </summary>
    void Locked()
    {
        _qteButtonSpawn.AddButton(gameObject);
        gameObject.SetActive(false);
        _canUnlocked = false;
        
    }
    
    
    

    private void Update()
    {
        Unlock();
    }

    /// <summary>
    /// 当按下指定的键且允许解锁时，解锁当前按钮并将其添加到QTE按钮队列中，然后禁用该按钮对象。
    /// </summary>
    private void Unlock()
    {
        if (Input.GetKeyDown(_needKey) && _canUnlocked)
        {
            _unlocked = true;
            _qteButtonSpawn.AddButton(gameObject);
            gameObject.SetActive(false);
        }
        /*else if (!Input.GetKeyDown(_needKey)&& _canUnlocked)
        {
            _canUnlocked = false;
            remainingImage.color = Color.red;
            
        }*/
    }

    void RandomInput()
    {
        int randomCode = UnityEngine.Random.Range(0, 4);
        switch (randomCode)
        {
            case 0:
                _needKey = KeyCode.W;
                keyText.text = "W";
                break;
            case 1:
                _needKey = KeyCode.Q;
                keyText.text = "Q";
                break;
            case 2:
                _needKey = KeyCode.E;
                keyText.text = "E";
                break;
            case 3:
                _needKey = KeyCode.R;
                keyText.text = "R";
                break;
            
        }
    }


}
