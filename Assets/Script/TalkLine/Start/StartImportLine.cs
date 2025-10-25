using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartImportLine : MonoBehaviour
{
    public Manager TextLine;
    public int DelayTime,BlockDelayTime,EndDelay;
    public TextMeshProUGUI TextUI;
    private char SpecialChar = '¨€';
    private int BlockTime;
    public GameObject Tips;
    public bool SwitchScence = false;
    public string Scence;
    public GameObject PlayerNameBar;
    private bool Import;
    public AudioSource Type;


    public async Task StartImport()
    {
        int index = 0;
        Import = true;
        foreach (string s in TextLine.TxtLine)
        {

            foreach (char c in s)
            {
                if (TextUI.text[TextUI.text.Length - 1] == '¨€')
                {
                    TextUI.text = TextUI.text.Remove(TextUI.text.Length - 1);
                }
                
                TextUI.text += c;
                BlockTime = Random.Range(0, 2);
                for (int i = 0; i < BlockTime; i++)
                {
                    if (TextUI.text[TextUI.text.Length - 1] == '¨€') TextUI.text = TextUI.text.Remove(TextUI.text.Length - 1);
                    await Task.Delay(BlockDelayTime);
                    Type.Play();
                    TextUI.text += SpecialChar;
                    await Task.Delay(BlockDelayTime);
                }
                await Task.Delay(DelayTime);
                Type.Play();

            }
            if (TextUI.text[TextUI.text.Length - 1] == '¨€')
            {
                TextUI.text = TextUI.text.Remove(TextUI.text.Length - 1);
            }
            if(s != TextLine.TxtLine[TextLine.TxtLine.Count - 1])
            TextUI.text += "\n";
            index++;

        }
        if(DelayTime != 0)
        {
            Tips.SetActive(true);
            SwitchScence = true;
        }
        while (true)
        {
            if (TextUI.text[TextUI.text.Length - 1] == '¨€') TextUI.text = TextUI.text.Remove(TextUI.text.Length - 1);
            await Task.Delay(EndDelay);
            TextUI.text += SpecialChar;
            Type.Play();
            await Task.Delay(EndDelay);
            Type.Stop();
        }
        

    }


    private void Update()
    {
        if (Input.anyKey && Import)
        {
            if (SwitchScence)
            {
                SceneManager.LoadScene(Scence);
            }
            else
            {
                Tips.SetActive(true);
                DelayTime = 0;
                BlockDelayTime = 0;
                Invoke("SetStage", 0.2f) ;
            }
        }
    }


    void SetStage()
    {
        SwitchScence = true;
    }



    public void InputPlayerName()
    {

        PlayerNameBar.SetActive(true);



    }





}
