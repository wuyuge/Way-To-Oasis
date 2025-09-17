using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public GameObject Menu;

    public TalkSystem MainTalk, LearnTalk;
    public Material Error;
    public Animator Black;

    private void Awake()
    {
        Error.SetFloat("_GlitchIntensity", 0);
        Error.SetFloat("_NoiseIntensity", 0);
        Error.SetFloat("_ColorShift", 0);
        Error.SetFloat("_GlitchSpeed", 0);
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
        MainTalk.on = false;
        LearnTalk.on = false;

    }

    public void BackToGame()
    {
        Menu.SetActive(false);
        Time.timeScale = 1;
        MainTalk.on = true;
        LearnTalk.on = true;
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

}
