using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public GameObject Menu;







    public void OpenMenu()
    {
        Menu.SetActive(true);
       
        Time.timeScale = 0;
        
        
    }

    public void BackToGame()
    {
        Menu.SetActive(false);
        Time.timeScale = 1;
    }

    public void OpenSetting()
    {
        Menu.SetActive(true);
    }

    public void StartGame()
    {

        SceneManager.LoadScene("Main");



    }





    public void QuitGame()
    {
        Application.Quit();
    }




}
