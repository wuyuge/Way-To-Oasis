using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class StartIntroText : MonoBehaviour
{
    public GameObject next;
    public Manager textContainer;
    public TextMeshProUGUI text;
    public StartImportLine startImport;
    public float showSpeed = 0.05f;
    [FormerlySerializedAs("audio")] public AudioSource audioSource;
    public float closeSecond = 0.35f;
    private int _curLine = 0;
    private bool _isShowing = false;
    private Coroutine _coroutine; 
    [SerializeField]
    private Animator anim;

    public bool canShow;
    public Manager language;

    private void Start()
    {
        if (anim == null)
        {
            try
            {
                anim = GetComponent<Animator>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    /*private void OnEnable()
    {
        if
       _coroutine = StartCoroutine(ShowText());
    }*/


    void Update()
    {
        if (Input.anyKeyDown && canShow)
        {
            if (_isShowing)
            {
                ShowAllText();
                _isShowing = false;
            }
            else
            {
                _curLine++;
                _coroutine = StartCoroutine(ShowText());
            }
        }
    }

    private IEnumerator ShowText()
    {
        text.text = "";
        if (_curLine < textContainer.data.Count)
        {
            var inFlag = false;
            var flag = "";
            var s = language.isEn? textContainer.data[_curLine].en : textContainer.data[_curLine].cn;
            foreach (var value in s)
            {
                if (value == '<')
                {
                    flag = string.Empty;
                    inFlag = true;
                    flag += value;
                    continue;
                }

                if (value == '>')
                {
                    inFlag = false;
                    flag += value;
                    text.text += flag;
                    continue;
                }

                if (inFlag)
                {
                    flag += value;
                    continue;
                }
                text.text += value;
                _isShowing = true;
                audioSource.Play();
                yield return new WaitForSeconds(showSpeed);
            }
        }
        else
        {
            if (next != null)
            {
                next.SetActive(true);
                anim.SetBool("end",true);
                
            }
            else
            {
                startImport.InputPlayerName();
                anim.SetBool("end",true);
            
                yield break;
            }
            
        }
        _isShowing = false;
        
    }

    private void ShowAllText()
    {
        StopCoroutine(_coroutine);
        var s = language.isEn? textContainer.data[_curLine].en : textContainer.data[_curLine].cn;
        text.text = s;
    }
    
    private void OnDisable()
    {
        if (_coroutine is null) return;
        StopCoroutine(_coroutine);
    }

    private void CloseObject()
    {
        gameObject.SetActive(false);
    }

    public void SetCanShow()
    {
        canShow = true;
    }
}
