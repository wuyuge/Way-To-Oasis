using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public GameObject Menu;
    public bool Setmaterial;
    public GameObject MainTalk, LearnTalk;
    public Material Error;
    public Animator Black;

    private void Awake()
    {
        if(Setmaterial)
        {
            Error.SetFloat("_GlitchIntensity", 0);
            Error.SetFloat("_NoiseIntensity", 0);
            Error.SetFloat("_ColorShift", 0);
            Error.SetFloat("_GlitchSpeed", 0);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Menu.activeSelf)
            {
                BackToGame();
            }
            else
            {
                OpenMenu();
            }
        }
    }


    public void SetAnim(string anim)
    {
        GetComponent<Animator>().SetTrigger(anim);
    }


    public void OpenMenu()
    {
        Menu.SetActive(true);
        Time.timeScale = 0;
        MainTalk.GetComponent<TalkSystem>().on = false;
        LearnTalk.GetComponent<TalkSystem>().on = false;

    }

    public void BackToGame()
    {
        Menu.SetActive(false);
        Time.timeScale = 1;
        MainTalk.GetComponent<TalkSystem>().on = true;
        LearnTalk.GetComponent<TalkSystem>().on = true;
        
        
    }

    public void OpenSetting()
    {
        Menu.SetActive(true);
    }

    public void StartGame()
    {

        SceneManager.LoadScene("Main");



    }

    public void BackToStart()
    {
         
        Time.timeScale = 1;
        SceneManager.LoadScene("Start");
    }



    public void QuitGame()
    {
        Application.Quit();
    }

    public void BlackAnim()
    {
        Black.SetTrigger("Dark");
    }

    public void SetMaterial()
    {
        Error.SetFloat("_GlitchIntensity", 0.006f);
        Error.SetFloat("_NoiseIntensity", 0.01f);
        Error.SetFloat("_ColorShift", 0.015f);
        Error.SetFloat("_GlitchSpeed", 0.25f);
    }


    public void SetFPS(int value)
    {
       
        
        switch (value)
        {
            case 0:
                Application.targetFrameRate = 30;
                Debug.Log("设置帧率 30");
                break;
            case 1:
                Application.targetFrameRate = 60;
                Debug.Log("设置帧率 60");
                break;
            case 2:
                Application.targetFrameRate = 120;
                Debug.Log("设置帧率 120");
                break;
            case 3:
                Application.targetFrameRate = 240;
                Debug.Log("设置帧率 240");
                break;
            case 4:
                Application.targetFrameRate = -1;
                Debug.Log("设置帧率 无限");
                break;



        }

        


    }



    

}
