using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ConversationHistory : MonoBehaviour
{
    public Sprite[] characterSprites = new Sprite[5];
    public GameObject playerContainer,characterContainer,asideContainer;
    public TextMeshProUGUI pText;
    public TextMeshProUGUI cText;
    public TextMeshProUGUI aText;
    public Image characterImage;
    public Image imageBack;

    /// <summary>
    /// 设置对话历史记录，根据是否为玩家发言以及是否为旁白调整显示内容。
    /// </summary>
    /// <param name="history">包含对话信息的历史对象。</param>
    /// <param name="isAside">指示该条目是否为旁白，默认值为false。</param>
    public void SetHistory(TextHistory history)
    {
        if (history.IsASide)
        {
            asideContainer.SetActive(true);
            aText.text = history.Text;
            return;
        }
        playerContainer.SetActive(history.IsPlayer);
        characterContainer.SetActive(!history.IsPlayer);
        asideContainer.SetActive(false);
        if (history.IsPlayer)
        {
            pText.text = history.Text;
            return;
        }
        cText.text = history.Text;
        SetImage(history.CharacterName);
        
        
    }

    /// <summary>
    /// 根据角色名称设置对应的角色图片。
    /// </summary>
    /// <param name="characterName">要设置的角色名称。</param>
    private void SetImage(string characterName)
    {
        switch (characterName)
        {
            case "艾米莉":
                characterImage.sprite = characterSprites[0];
                imageBack.color = new Color32(238, 192, 144, 255);
                break;
            case "洛尔坎":
                characterImage.sprite = characterSprites[1];
                imageBack.color = new Color32(220, 149, 131, 255);
                break;
            case "博金森":
                characterImage.sprite = characterSprites[2];
                imageBack.color = new Color32(227, 171, 96, 255);
                break;
            case "阿曼德":
                characterImage.sprite = characterSprites[3];
                imageBack.color = new Color32(222, 183, 204, 255);
                break;
            case "莱文":
                characterImage.sprite = characterSprites[4];
                imageBack.color = new Color32(163, 196, 172, 255);
                break;
            default:
                Debug.LogError($"历史对话设定角色图片错误 传入参数{characterName}");
                break;
                
        }
    }

    /// <summary>
    /// 重置对话历史，清空玩家和角色的文本以及角色图片。
    /// </summary>
    public void ResetThis()
    {
        pText.text = string.Empty;
        cText.text = string.Empty;
        characterImage.sprite = null;
        aText.text = string.Empty;
    }
    
}
