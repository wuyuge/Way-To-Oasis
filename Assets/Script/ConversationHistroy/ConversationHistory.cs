using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ConversationHistory : MonoBehaviour
{
    public Sprite[] characterSprites = new Sprite[5];
    public GameObject playerContainer,characterContainer;
    public TextMeshProUGUI pText;
    public TextMeshProUGUI cText;
    public Image characterImage;
    public Image imageBack;

    /// <summary>
    /// 根据提供的对话历史设置玩家或角色的文本和图片。
    /// </summary>
    /// <param name="history">包含对话历史信息的对象，包括是否为玩家、对话文本以及角色名称。</param>
    public void SetHistory(TextHistory history)
    {
        playerContainer.SetActive(history.IsPlayer);
        characterContainer.SetActive(!history.IsPlayer);
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
                imageBack.color = new Color32(244, 198, 100, 255);
                break;
            case "洛尔坎":
                characterImage.sprite = characterSprites[1];
                imageBack.color = new Color32(253, 143, 141, 255);
                break;
            case "博金森":
                characterImage.sprite = characterSprites[2];
                imageBack.color = new Color32(241, 178, 79, 255);
                break;
            case "阿曼德":
                characterImage.sprite = characterSprites[3];
                imageBack.color = new Color32(216, 190, 254, 255);
                break;
            case "莱文":
                characterImage.sprite = characterSprites[4];
                imageBack.color = new Color32(148, 205, 185, 255);
                break;
            default:
                Debug.LogError("历史对话设定角色图片错误");
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
    }
    
}
