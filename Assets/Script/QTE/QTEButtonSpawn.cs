using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QteButtonSpawn : MonoBehaviour
{
    public GameObject buttonPrefab;
    private Queue<GameObject> _buttons = new Queue<GameObject>();
    public int maxButtonCount;
    
    
    public bool IsSpawning
    {
        get;
        set;
    }

    /// <summary>
    /// 初始化时预先实例化并缓存指定数量的按钮，这些按钮初始状态为非激活状态，并被添加到队列中
    /// </summary>
    private void Awake()
    {
        for (int i = 0; i < maxButtonCount; i++)
        {
            GameObject tempButton = Instantiate(buttonPrefab, transform);
            tempButton.SetActive(false);
            _buttons.Enqueue(tempButton);
        }
        IsSpawning = true;
    }

    private void Start()
    {
        StartCoroutine(ContinueCreateButton());
    }

    /// <summary>
    /// 从队列中移除一个按钮并激活它
    /// 如果当前队列中的按钮数量不在允许范围内（为空或超过最大限制），则不会执行任何操作
    /// </summary>
    void SpawnButton()
    {
        if (_buttons.Count == 0)
        {
            return;
        }
        GameObject tempButton = _buttons.Dequeue();
        tempButton.SetActive(true);

    }

    /// <summary>
    /// 向队列中添加一个按钮。
    /// 在添加前会检查队列大小是否在允许范围内，如果超出范围则不会添加。
    /// </summary>
    /// <param name="button">要添加到队列中的按钮对象</param>
    public void AddButton(GameObject button)
    {
        if (_buttons.Count >= maxButtonCount)
        {
            return ;
        }
        _buttons.Enqueue(button);
    }


    private IEnumerator ContinueCreateButton()
    {
        while (IsSpawning)
        {
            
            SpawnButton();
            yield return new WaitForSeconds(1);
            
        }
    }
    
    
    public void StopSpawn()
    {
        IsSpawning = false;
        StopCoroutine(ContinueCreateButton());
    }

    public void StartSpawn()
    {
        IsSpawning = true;
        StartCoroutine(ContinueCreateButton());
    }
}
