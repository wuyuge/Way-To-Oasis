using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Pipe : MonoBehaviour
{
    public bool isConnected;
    public bool isStartPoint,isDestination;
    public GameObject above,below,left,right;
    public int pipeNumber;
    protected Coroutine CurrentCoroutine;
    protected Pipe AboveComponent, LeftComponent,BelowComponent,RightComponent;
    private Animator _animator;

    /// <summary>
    /// 当游戏对象被初始化时调用。此方法设置了管道的编号，并根据当前管道的位置确定其上下左右相邻的管道对象。如果某个方向上没有相邻管道，则相应属性设置为null。
    /// </summary>
    private void Awake()
    {
        pipeNumber = transform.GetSiblingIndex();
        above = pipeNumber - 4 >= 0  ? transform.parent.GetChild(pipeNumber - 4).gameObject : null;
        below = pipeNumber + 4 < transform.parent.childCount ? gameObject.transform.parent.GetChild(pipeNumber + 4).gameObject : null;
        var rightIndex = pipeNumber + 1;
        right = (rightIndex < transform.parent.childCount) && (pipeNumber % 4 != 3) 
            ? transform.parent.GetChild(rightIndex).gameObject 
            : null;
        var leftIndex = pipeNumber - 1;
        left = (leftIndex >= 0) && (pipeNumber % 4 != 0) 
            ? transform.parent.GetChild(leftIndex).gameObject 
            : null;
        if (above is not null)
        {
            AboveComponent = above.GetComponent<Pipe>();
        }
        if (below is not null)
        {
            BelowComponent = below.GetComponent<Pipe>();
        }
        if (left is not null)
        {
            LeftComponent = left.GetComponent<Pipe>();
        }
        if (right is not null)
        {
            RightComponent = right.GetComponent<Pipe>();
        }
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 初始化管道组件。如果该管道是起点，则将其颜色设置为蓝色，并开始尝试链接其他管道形成通路；如果该管道是终点，则将其颜色设置为红色。
    /// </summary>
    void Start()
    {
        if (isStartPoint)
        {
            GetComponent<Image>().color = Color.blue;
            isConnected = true;
            CurrentCoroutine = StartCoroutine(TryLinkOtherPipe());
        }   
        if (isDestination)
        {
            GetComponent<Image>().color = Color.red;
        }
    }

    public abstract bool CheckLinked(PipeTowards towards);

    /// <summary>
    /// 尝试链接其他管道以形成从起点到终点的通路。此方法在每次迭代中检查当前管道是否已连接，如果未连接，则停止尝试链接的过程。
    /// </summary>
    /// <returns>返回一个IEnumerator对象，允许该方法作为协程运行。</returns>
    protected abstract IEnumerator TryLinkOtherPipe();
    
    public enum PipeTowards
    {
        Above,
        Below,
        Left,
        Right
    }

    public void Click()
    {
        _animator.SetTrigger("rotate");
    }

    public abstract void SetState();

}
