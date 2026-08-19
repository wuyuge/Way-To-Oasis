using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopCharacterManager : MonoBehaviour
{
    [System.Serializable]
    public class ShopCharacter
    {
        public Button button;
        public Character character;
        public string name;
    }
    
    
    public List<ShopCharacter> characterList = new List<ShopCharacter>();
    public Manager DeadName;
    public bool kill;
    
    public void SelectBody()//ªª ¨ÃÂ
    {
        Init();
        kill = false;
        foreach (var value in characterList)
        {
            if (DeadName.TxtLine.Any(x => x.Contains(value.name)))
            {
                value.button.interactable = true;
            }
        }
    }

    public void KillSB()//…±»À
    {
        Init();
        kill = true;
        foreach (var value in characterList)
        {
            if (value.character is null)
            {
                continue;
            }
            if(!value.character.Dead) value.button.interactable = true;
        }
    }

    private void Init()
    {
        foreach (var value in characterList)
        {
            value.button.interactable = false;
        }
    }
    
}





   
