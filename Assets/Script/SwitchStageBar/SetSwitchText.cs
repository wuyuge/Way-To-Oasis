using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SetSwitchText : MonoBehaviour
{
    public Progress progress;
    public TextMeshProUGUI showText;
    public Manager language;
    private void OnEnable()
    {
        
        showText.text = "";
        if (progress.start)
        {
            showText.text = language.isEn ? "Allocate supplies" : "分配负重阶段";
        }
        else if (progress.talk)
        {
            showText.text = language.isEn ? "Conversation" : "交谈阶段";
        }
        else if (progress.food)
        {
            showText.text = language.isEn ? "Allocate food" : "分配食物阶段";
        }
    }
}
