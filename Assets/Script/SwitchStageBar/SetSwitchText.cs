using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SetSwitchText : MonoBehaviour
{
    public Progress progress;
    public TextMeshProUGUI showText;
    private void OnEnable()
    {
        
        showText.text = "";
        if (progress.start)
        {
            showText.text = "分配负重阶段";
        }
        else if (progress.talk)
        {
            showText.text = "交谈阶段";
        }
        else if (progress.food)
        {
            showText.text = "分配食物阶段";
        }
    }
}
