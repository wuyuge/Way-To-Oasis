using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterImageManager : MonoBehaviour
{
    
    public List<GameObject> RightImage = new List<GameObject>();
    public List<GameObject> LeftImage = new List<GameObject>();
    public GameObject RightBar,LeftBar;
    private int LastPosition;//0为左 1为右
    public Animator anim;

    public void SetImage(string name,int express = 0)//先判断是否已经显示，如果已经显示则把对应图像高亮如果没有先置入右侧
    {
        bool haveImage = false;
        foreach (GameObject g in RightImage)
        {
            if (g.activeSelf)
            {
                haveImage = true;
                break;
            }
        }
        if (!haveImage)
        {
            
            anim.SetTrigger("Up");
        }
        switch (name)
        {
            case "阿曼德":

                if (CheckRight(0))
                {
                    ShowImage(0, RightImage);
                    SetGray(LeftImage);
                    LastPosition = 1;
                    RightBar.SetActive(true);
                    LeftBar.SetActive(false);
                    
                    
                }
                else if (CheckLeft(0))
                {
                    ShowImage(0, LeftImage);
                    SetGray(RightImage);
                    LastPosition = 0;
                    RightBar.SetActive(false);
                    LeftBar.SetActive(true);
                }
                else
                {
                    if(LastPosition == 0)
                    {
                        SetGameObj(RightImage);
                        ShowImage(0, RightImage);
                        SetGray(LeftImage);
                        LastPosition = 1;
                        RightBar.SetActive(true);
                        LeftBar.SetActive(false);

                    }
                    else if(LastPosition == 1)
                    {
                        SetGameObj(LeftImage);
                        ShowImage(0, LeftImage);
                        SetGray(RightImage);
                        LastPosition = 0;
                        RightBar.SetActive(false);
                        LeftBar.SetActive(true);
                    }
                }
                break;
            case "艾米莉":

                if (CheckRight(1))
                {
                    ShowImage(1, RightImage);
                    SetGray(LeftImage);
                    LastPosition = 1;
                    RightBar.SetActive(true);
                    LeftBar.SetActive(false);

                }
                else if (CheckLeft(1))
                {
                    ShowImage(1, LeftImage);
                    SetGray(RightImage);
                    LastPosition = 0;
                    RightBar.SetActive(false);
                    LeftBar.SetActive(true);
                }
                else
                {
                    if (LastPosition == 0)
                    {
                        SetGameObj(RightImage);
                        ShowImage(1, RightImage);
                        SetGray(LeftImage);
                        LastPosition = 1;
                        RightBar.SetActive(true);
                        LeftBar.SetActive(false);
                    }
                    else if (LastPosition == 1)
                    {
                        SetGameObj(LeftImage);
                        ShowImage(1, LeftImage);
                        SetGray(RightImage);
                        LastPosition = 0;
                        RightBar.SetActive(false);
                        LeftBar.SetActive(true);
                    }
                }
                break;
            case "莱文":

                if (CheckRight(2))
                {
                    ShowImage(2, RightImage);
                    SetGray(LeftImage);
                    LastPosition = 1;
                    RightBar.SetActive(true);
                    LeftBar.SetActive(false);

                }
                else if (CheckLeft(2))
                {
                    ShowImage(2, LeftImage);
                    SetGray(RightImage);
                    LastPosition = 0;
                    RightBar.SetActive(false);
                    LeftBar.SetActive(true);
                }
                else
                {
                    if (LastPosition == 0)
                    {
                        SetGameObj(RightImage);
                        ShowImage(2, RightImage);
                        SetGray(LeftImage);
                        LastPosition = 1;
                        RightBar.SetActive(true);
                        LeftBar.SetActive(false);

                    }
                    else if (LastPosition == 1)
                    {
                        SetGameObj(LeftImage);
                        ShowImage(2, LeftImage);
                        SetGray(RightImage);
                        LastPosition = 0;
                        RightBar.SetActive(false);
                        LeftBar.SetActive(true);
                    }
                }
                break;
            case "博金森":

                if (CheckRight(3))
                {
                    ShowImage(3, RightImage);
                    SetGray(LeftImage);
                    LastPosition = 1;
                    RightBar.SetActive(true);
                    LeftBar.SetActive(false);

                }
                else if (CheckLeft(3))
                {
                    ShowImage(3, LeftImage);
                    SetGray(RightImage);
                    LastPosition = 0;
                    RightBar.SetActive(false);
                    LeftBar.SetActive(true);
                }
                else
                {
                    if (LastPosition == 0)
                    {
                        SetGameObj(RightImage);
                        ShowImage(3, RightImage);
                        SetGray(LeftImage);
                        LastPosition = 1;
                        RightBar.SetActive(true);
                        LeftBar.SetActive(false);

                    }
                    else if (LastPosition == 1)
                    {
                        SetGameObj(LeftImage);
                        ShowImage(3, LeftImage);
                        SetGray(RightImage);
                        LastPosition = 0;
                        RightBar.SetActive(false);
                        LeftBar.SetActive(true);
                    }
                }
                break;
            case "洛尔坎":

                if (CheckRight(4))
                {
                    ShowImage(4, RightImage);
                    SetGray(LeftImage);
                    LastPosition = 1;
                    RightBar.SetActive(true);
                    LeftBar.SetActive(false);

                }
                else if (CheckLeft(4))
                {
                    ShowImage(4, LeftImage);
                    SetGray(RightImage);
                    LastPosition = 0;
                    RightBar.SetActive(false);
                    LeftBar.SetActive(true);
                }
                else
                {
                    if (LastPosition == 0)
                    {
                        SetGameObj(RightImage);
                        ShowImage(4, RightImage);
                        SetGray(LeftImage);
                        LastPosition = 1;
                        RightBar.SetActive(true);
                        LeftBar.SetActive(false);
                    }
                    else if (LastPosition == 1)
                    {
                        SetGameObj(LeftImage);
                        ShowImage(4, LeftImage);
                        SetGray(RightImage);
                        LastPosition = 0;
                        RightBar.SetActive(false);
                        LeftBar.SetActive(true);
                    }
                }
                break;
        }

        if (RightBar.activeSelf)
        {
            anim.SetBool("Right",true);
            anim.SetBool("Left",false);
        }

        if (LeftBar.activeSelf)
        {
            anim.SetBool("Right",false);
            anim.SetBool("Left",true);
        }

    }


    void ShowImage(int index,List<GameObject> list)
    {
        list[index].SetActive(true);
        list[index].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    

    private void SetGameObj(List<GameObject> list)
    {
        foreach(GameObject g in list)
        {
            g.SetActive(false);
        }
    }


    private void SetGray(List<GameObject> list)
    {
        foreach (GameObject g in list)
        {
            if (g.activeSelf)
            {
                g.GetComponent<Image>().color = new Color32(140,140,140,255);
            }
        }

    }

    /// <summary>
    /// 检查右侧图像列表中指定索引位置的图像是否处于激活状态。
    /// </summary>
    /// <param name="CurIndex">要检查的图像在RightImage列表中的索引。</param>
    /// <returns>如果指定索引位置的图像是激活状态，则返回true；否则，若无任何图像处于激活状态或指定索引位置的图像未激活，则返回true。</returns>
    private bool CheckRight(int CurIndex)
    {
        int index = -1;
        foreach (GameObject g in RightImage)
        {
            index++;
            if (g.activeSelf)
            {
                if(index == CurIndex)
                {
                    return true;
                }
                return false;
            }
        }

        return true;


    }

    /// <summary>
    /// 检查左侧图像列表中指定索引位置的图像是否处于激活状态。
    /// </summary>
    /// <param name="CurIndex">要检查的图像在LeftImage列表中的索引。</param>
    /// <returns>如果指定索引位置的图像是激活状态，则返回true；否则返回false。若无任何图像处于激活状态，也返回true。</returns>
    private bool CheckLeft(int CurIndex)
    {
        int index = -1;
        foreach (GameObject g in LeftImage)
        {
            index++;
            if (g.activeSelf)
            {
                if (index == CurIndex)
                {
                    return true;
                }
                return false;
            }
        }

        return true;


    }
    

    public void ResetTrigger()
    {
        anim.ResetTrigger("Down");
        anim.ResetTrigger("Up");
    }


    public void CloseImage()
    {
        foreach(GameObject g in RightImage)
        {
            if(g.activeSelf && !g.GetComponent<Animator>().GetBool("close"))
                g.GetComponent<Animator>().SetTrigger("close");
        }
        foreach(GameObject g in LeftImage)
        {
            if (g.activeSelf && !g.GetComponent<Animator>().GetBool("close"))
                g.GetComponent<Animator>().SetTrigger("close");
        }
        if(!anim.GetBool("Down"))
            anim.SetTrigger("Down");
        Invoke("SetGameObj",0.5f);
    }
    private void SetGameObj()
    {
        foreach (GameObject g in RightImage)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in LeftImage)
        {
            g.SetActive(false);
        }
    }



   
    


}
