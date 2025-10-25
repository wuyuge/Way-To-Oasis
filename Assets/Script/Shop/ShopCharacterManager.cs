using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopCharacterManager : MonoBehaviour
{
    public List<GameObject> CharacterList = new List<GameObject>();
    public Manager DeadName;
    public bool kill;

    private void SetState(bool _isKill)
    {
        if(!_isKill)
        {
            foreach (GameObject g in CharacterList)
            {
                g.GetComponent<Button>().enabled = false;
            }
        }
        else
        {
            foreach (GameObject g in CharacterList)
            {
                g.GetComponent<Button>().enabled = true;
            }
        }
    }



    public void SelectBody()
    {
        kill = false;
        SetState(false);
        foreach (GameObject g in CharacterList)
        {
            g.GetComponent<Image>().color = Color.gray;
            
        }
        foreach (string s in DeadName.TxtLine)
        {
            switch (s)
            {
                case "°¢ÂüµÂ":
                    CharacterList[3].GetComponent<Button>().enabled = true;
                    CharacterList[3].GetComponent<Image>().color = Color.white;
                    break;
                case "Âå¶û¿²":
                    CharacterList[1].GetComponent<Button>().enabled = true;
                    CharacterList[1].GetComponent<Image>().color = Color.white;
                    break;
                case "²©½ðÉ­":
                    CharacterList[2].GetComponent<Button>().enabled = true;
                    CharacterList[2].GetComponent<Image>().color = Color.white;
                    break;
                case "À³ÎÄ":
                    CharacterList[4].GetComponent<Button>().enabled = true;
                    CharacterList[4].GetComponent<Image>().color = Color.white;
                    break;
                case "°¬Ã×Àò":
                    CharacterList[0].GetComponent<Button>().enabled = true;
                    CharacterList[0].GetComponent<Image>().color = Color.white;
                    break;
                case "Leader":
                    CharacterList[5].GetComponent<Button>().enabled = true;
                    CharacterList[5].GetComponent<Image>().color = Color.white;
                    break;

            }

        }
    }

    public void KillSB()
    {
        kill = true;
        SetState(true);
        CharacterList[5].GetComponent<Button>().enabled = false;
        foreach (string s in DeadName.TxtLine)
        {
            switch (s)
            {
                case "°¢ÂüµÂ":
                    CharacterList[3].GetComponent<Button>().enabled = false;
                    CharacterList[3].GetComponent<Image>().color = Color.gray;
                    break;
                case "Âå¶û¿²":
                    CharacterList[1].GetComponent<Button>().enabled = false;
                    CharacterList[1].GetComponent<Image>().color = Color.gray;
                    break;
                case "²©½ðÉ­":
                    CharacterList[2].GetComponent<Button>().enabled = false;
                    CharacterList[2].GetComponent<Image>().color = Color.gray;
                    break;
                case "À³ÎÄ":
                    CharacterList[4].GetComponent<Button>().enabled = false;
                    CharacterList[4].GetComponent<Image>().color = Color.gray;
                    break;
                case "°¬Ã×Àò":
                    CharacterList[0].GetComponent<Button>().enabled = false;
                    CharacterList[0].GetComponent<Image>().color = Color.gray;

                    break;
                case "Leader":
                    CharacterList[5].GetComponent<Button>().enabled = false;
                    CharacterList[5].GetComponent<Image>().color = Color.gray;
                    break;

            }

        }
    }


}





   
