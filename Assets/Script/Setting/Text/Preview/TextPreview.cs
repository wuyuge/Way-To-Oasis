using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TextPreview : MonoBehaviour
{
    private TextMeshProUGUI Text;
    private bool IsShowing,Break;
    public string StartText;
    public string en;
    public string cn;
    public int CurrentSpeed;
    public Manager language;
    private void OnEnable()
    {
        Text = GetComponent<TextMeshProUGUI>();
        
        StartText = language.isEn ? en : cn;
        Text.text = StartText;
    }


    public async Task ResetSpeed(int Speed)
    {
        CurrentSpeed = Speed;
        
        if(!IsShowing)
        {
            Text.text = string.Empty;
            IsShowing = true;
            foreach (char s in StartText)
            {

                Text.text += s;
                if (Break)
                {
                    Text.text = string.Empty;
                    Break = false;
                    IsShowing = false;
                    return;

                }

                await Task.Delay(Speed);
            }
            IsShowing = false;
        }
        else
        {
            Break = true;
        }
            
        
 



    }


    private void Update()
    {
        
        if(!IsShowing && !Break && Text.text == string.Empty)
        {
            _ = ResetSpeed(CurrentSpeed);
        }
        

    }


    public void ResetSize(float Value)
    {
        

        Text.fontSize = Value;




    }





}
