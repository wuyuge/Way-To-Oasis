using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cinemachine;

public class DeveloperDebuger : MonoBehaviour
{
    public GameObject DeveloperPanel;
    public Progress progress;
    [Header("尸体管理")]
    public Manager DeadName;
    public Manager UesdBody, FinalBody, HaveBody;
    [Header("食物管理")]
    public Manager FinalFood;
    public Manager HaveFood;
    [Header("对话框动画管理")]
    public Animator TalkBar;
    [Header("角色框管理")]
    public Animator CharaBar;
    [Header("引导遮罩管理")]
    public GameObject Mask;

    private Camera mainCamera;
    public Achievement achievementList;
    
    private CinemachineImpulseSource _impulseSource;

    private void Awake()
    {
        AchievementManager.UnlockAchievement("ACH_NEW_LEADER");
    }

    void Start()
    {
        mainCamera = Camera.main;
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            if(DeveloperPanel != null) DeveloperPanel.SetActive(!DeveloperPanel.activeSelf);
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (var value in achievementList.achievements)
            {
                AchievementManager.ClearAchievement(value);
            }
            
        }
    }

    public void ResetBody()
    {
        DeadName.TxtLine.Clear();
        DeadName.TxtLine.Add("Leader");
        UesdBody.TxtLine.Clear();
        if (progress.food)
        {
            FinalBody.Weight = 1;
            HaveBody.Weight = 0;
            progress.DownBar.GetComponent<DownBar>().BodyText.GetComponent<TextMeshProUGUI>().text = FinalBody.Weight.ToString();
        }
        else
        {
            FinalBody.Weight = 0;
            HaveBody.Weight = 1;
            progress.DownBar.GetComponent<DownBar>().BodyText.GetComponent<TextMeshProUGUI>().text = HaveBody.Weight.ToString();
        }
    }

    public void ResetFood()
    {
        if (progress.food)
        {
            FinalFood.Weight = 6;
            progress.DownBar.GetComponent<DownBar>().FoodText.GetComponent<TextMeshProUGUI>().text = FinalFood.Weight.ToString();
        }
        else
        {
            HaveFood.Weight = 6;
            progress.DownBar.GetComponent<DownBar>().FoodText.GetComponent<TextMeshProUGUI>().text = HaveFood.Weight.ToString();
        }
        
        
    }

    public void UpTalkBar()
    {
        TalkBar.SetTrigger("Up");
    }

    public void DownTalkBar()
    {
        TalkBar.SetTrigger("Down");
    }

    public void UpCharaBar()
    {
        CharaBar.SetTrigger("Up");
    }
    
    public void DownCharaBar()
    {
        CharaBar.SetTrigger("Down");
    }

    public void Day0Skip()
    {
        progress.can_skip = true;
    }
    
    public void BackTalk()
    {
        progress.TalkBar.GetComponent<TalkSystem>().line--;
        _ = progress.TalkBar.GetComponent<TalkSystem>().ShowText();
    }

    public void CloseMask()
    {
        Mask.SetActive(false);
    }



}
