using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CgManager : MonoBehaviour
{
    public List<Sprite> cgList;
    public Image cgContainer;
    public GameObject initialTransForm,fullModeTransForm,frame;
    public TalkSysShowText talkSysShowText;
    private Animator _animator;
    private bool _fullMode;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public bool ShowCg(int index)
    {
        if (cgList.Count < index)
        {
            Debug.LogError("索引超出cg列表长度");
            return false;
        }
        cgContainer.sprite = cgList[index];
        return true;
        
    }
    
    public void HideCg()
    {
        _animator.SetTrigger("Close");
        talkSysShowText.CloseCg();
    }

    public void SetFalse()
    {
        gameObject.SetActive(false);
    }

    public void SetFullMode(bool fullMode)
    {
        _animator.SetBool("FullMode", fullMode);
        _fullMode = fullMode;
    }

    public void SwitchTransForm()
    {
        if (!_fullMode)
        {
            frame.transform.SetParent(fullModeTransForm.transform);
            return;
        }
        frame.transform.SetParent(initialTransForm.transform);
    }
    
    
}
