using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public GameObject Menu;

    public TalkSystem MainTalk, LearnTalk;


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




}
