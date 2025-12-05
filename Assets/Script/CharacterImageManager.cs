using System.Collections;
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


                return;
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
                return;
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
                return;
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
                return;
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
                return;




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

    private bool CheckRight(int CurIndex)
    {
        int index = -1;
        foreach(GameObject g in RightImage)
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
