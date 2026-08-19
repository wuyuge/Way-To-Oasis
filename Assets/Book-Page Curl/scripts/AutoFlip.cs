using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Book))]
public class AutoFlip : MonoBehaviour {
    public FlipMode Mode;
    public float PageFlipTime = 1;
    public float TimeBetweenPages = 1;
    public float DelayBeforeStarting = 0;
    public bool AutoStartFlip=true;
    public Book ControledBook;
    public int AnimationFramesCount = 40;
    bool isFlipping = false;

    // 新增：从右侧中间向左翻页
    [Header("新增功能")]
    public bool useMiddleRightFlip = false; // 启用右侧中间翻页

    // Use this for initialization
    void Start () {
        if (!ControledBook)
            ControledBook = GetComponent<Book>();
        if (AutoStartFlip)
            StartFlipping();
        ControledBook.OnFlip.AddListener(new UnityEngine.Events.UnityAction(PageFlipped));
	}

    void PageFlipped()
    {
        isFlipping = false;
    }

	public void StartFlipping()
    {
        StartCoroutine(FlipToEnd());
    }

    // 向右翻页（原右下角）
    public void FlipRightPage()
    {
        if (isFlipping) return;
        if (ControledBook.currentPage + 2 >= ControledBook.TotalPageCount) return;
        isFlipping = true;
        float frameTime = PageFlipTime / AnimationFramesCount;
        float xc = (ControledBook.EndBottomRight.x + ControledBook.EndBottomLeft.x) / 2;
        float xl = ((ControledBook.EndBottomRight.x - ControledBook.EndBottomLeft.x) / 2) * 0.9f;
        float h = Mathf.Abs(ControledBook.EndBottomRight.y) * 0.9f;
        float dx = (xl)*2 / AnimationFramesCount;
        StartCoroutine(FlipRTL(xc, xl, h, frameTime, dx));
    }

    // 向左翻页（原左下角）
    public void FlipLeftPage()
    {
        if (isFlipping) return;
        if (ControledBook.currentPage - 2 <= 0) return;
        isFlipping = true;
        float frameTime = PageFlipTime / AnimationFramesCount;
        float xc = (ControledBook.EndBottomRight.x + ControledBook.EndBottomLeft.x) / 2;
        float xl = ((ControledBook.EndBottomRight.x - ControledBook.EndBottomLeft.x) / 2) * 0.9f;
        float h = Mathf.Abs(ControledBook.EndBottomRight.y) * 0.9f;
        float dx = (xl) * 2 / AnimationFramesCount;
        StartCoroutine(FlipLTR(xc, xl, h, frameTime, dx));
    }

    // 新增：从右侧中间向左翻页
    public void FlipRightMiddlePage()
    {
        if (isFlipping) return;
        if (ControledBook.currentPage + 2 >= ControledBook.TotalPageCount) return;
        isFlipping = true;
        float frameTime = PageFlipTime / AnimationFramesCount;
        float xc = (ControledBook.EndBottomRight.x + ControledBook.EndBottomLeft.x) / 2;
        float xl = ((ControledBook.EndBottomRight.x - ControledBook.EndBottomLeft.x) / 2) * 0.9f;
        
        // 核心修改：高度改为页面中间（y=0），不再是底部
        float h = 0; 
        float dx = (xl) * 2 / AnimationFramesCount;
        
        StartCoroutine(FlipRTL_Middle(xc, xl, h, frameTime, dx));
    }

    IEnumerator FlipToEnd()
    {
        yield return new WaitForSeconds(DelayBeforeStarting);
        float frameTime = PageFlipTime / AnimationFramesCount;
        float xc = (ControledBook.EndBottomRight.x + ControledBook.EndBottomLeft.x) / 2;
        float xl = ((ControledBook.EndBottomRight.x - ControledBook.EndBottomLeft.x) / 2)*0.9f;
        float h = Mathf.Abs(ControledBook.EndBottomRight.y)*0.9f;
        float dx = (xl)*2 / AnimationFramesCount;

        switch (Mode)
        {
            case FlipMode.RightToLeft:
                while (ControledBook.currentPage < ControledBook.TotalPageCount)
                {
                    // 判断：启用中间翻页则调用新函数
                    if (useMiddleRightFlip)
                    {
                        FlipRightMiddlePage();
                    }
                    else
                    {
                        StartCoroutine(FlipRTL(xc, xl, h, frameTime, dx));
                    }
                    
                    yield return new WaitForSeconds(TimeBetweenPages);
                }
                break;

            case FlipMode.LeftToRight:
                while (ControledBook.currentPage > 0)
                {
                    StartCoroutine(FlipLTR(xc, xl, h, frameTime, dx));
                    yield return new WaitForSeconds(TimeBetweenPages);
                }
                break;
        }
    }

    // 原版：右下角翻页
    IEnumerator FlipRTL(float xc, float xl, float h, float frameTime, float dx)
    {
        float x = xc + xl;
        float y = (-h / (xl * xl)) * (x - xc) * (x - xc);

        ControledBook.DragRightPageToPoint(new Vector3(x, y, 0));
        for (int i = 0; i < AnimationFramesCount; i++)
        {
            y = (-h / (xl * xl)) * (x - xc) * (x - xc);
            ControledBook.UpdateBookRTLToPoint(new Vector3(x, y, 0));
            yield return new WaitForSeconds(frameTime);
            x -= dx;
        }
        ControledBook.ReleasePage();
    }

    // 新增：右侧中间向左翻页（水平直线翻页，无弧度）
    IEnumerator FlipRTL_Middle(float xc, float xl, float h, float frameTime, float dx)
    {
        float x = xc + xl;
        float y = 0; // 固定中间高度

        ControledBook.DragRightPageToPoint(new Vector3(x, y, 0));
        for (int i = 0; i < AnimationFramesCount; i++)
        {
            ControledBook.UpdateBookRTLToPoint(new Vector3(x, y, 0));
            yield return new WaitForSeconds(frameTime);
            x -= dx;
        }
        ControledBook.ReleasePage();
    }

    // 原版：左下角翻页
    IEnumerator FlipLTR(float xc, float xl, float h, float frameTime, float dx)
    {
        float x = xc - xl;
        float y = (-h / (xl * xl)) * (x - xc) * (x - xc);
        ControledBook.DragLeftPageToPoint(new Vector3(x, y, 0));
        for (int i = 0; i < AnimationFramesCount; i++)
        {
            y = (-h / (xl * xl)) * (x - xc) * (x - xc);
            ControledBook.UpdateBookLTRToPoint(new Vector3(x, y, 0));
            yield return new WaitForSeconds(frameTime);
            x += dx;
        }
        ControledBook.ReleasePage();
    }
}