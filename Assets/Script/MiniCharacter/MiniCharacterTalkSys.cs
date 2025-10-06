using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;

public class MiniCharacterTalkSys : MonoBehaviour
{
    [System.Serializable]
    public class CharacterTalk
    {
        public string characterName;
        public TextMeshProUGUI characterTalkBar;
    }

    public List<CharacterTalk> characterTalks;

    public void ShowText(string Name,string Text)
    {
        //Debug.Log($"接受数据说话人：{Name} 内容：{Text}");
        // 显示对话栏的逻辑

        foreach (CharacterTalk character in characterTalks)
        {

            if (character.characterName == Name)
            {
                character.characterTalkBar.text = Text;
                character.characterTalkBar.transform.parent.gameObject.SetActive(true);

            }
            else
            {
                character.characterTalkBar.text = "";
                character.characterTalkBar.transform.parent.gameObject.SetActive(false);
                continue;
            }


        }



    }


    public void CompleteTalk()
    {
        foreach (CharacterTalk character in characterTalks)
        {

            
            
            
             character.characterTalkBar.text = "";
             character.characterTalkBar.transform.parent.gameObject.GetComponent<Animator>().SetTrigger("Close");
             continue;
            


        }
    }






}
