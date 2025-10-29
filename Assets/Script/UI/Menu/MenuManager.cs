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
    private SettingDataManager SettingManager;


    private void Awake()
    {
        if(Setmaterial)
        {
            Error.SetFloat("_GlitchIntensity", 0);
            Error.SetFloat("_NoiseIntensity", 0);
            Error.SetFloat("_ColorShift", 0);
            Error.SetFloat("_GlitchSpeed", 0);
        }
        SettingManager = GameObject.Find("SaveManager").GetComponent<SettingDataManager>();
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
        GameObject.Find("AudioManager").GetComponent<AudioManager>().AudioPlayer("Click");
        Menu.SetActive(true);
        Time.timeScale = 0;
        MainTalk.GetComponent<TalkSystem>().on = false;
        LearnTalk.GetComponent<TalkSystem>().on = false;

    }

    public void BackToGame()
    {
        GameObject.Find("AudioManager").GetComponent<AudioManager>().AudioPlayer("Click");
        Menu.transform.Find("General").gameObject.SetActive(false);
        Menu.transform.Find("Text").gameObject.SetActive(false);
        Menu.transform.Find("Video").gameObject.SetActive(false);
        Menu.transform.Find("Audio").gameObject.SetActive(false);
        Menu.SetActive(false);
        SaveSetting();
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
        //GameObject.Find("AudioManager").GetComponent<AudioManager>().AudioPlayer("Click");
        SceneManager.LoadScene("Main");



    }

    public void BackToStart()
    {
        
        Time.timeScale = 1;
        SceneManager.LoadScene("Start");
        GameObject AudioManager = GameObject.Find("AudioManager");
        if(AudioManager != null)
        {
            AudioManager.GetComponent<AudioManager>().AudioPlayer("Click");
        }
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


    
    void SaveSetting()
    {
        SettingManager.Save();
    }


    

}
