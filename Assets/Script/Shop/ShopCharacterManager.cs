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
        foreach (string s in DeadName.TxtLine)
        {
            switch (s)
            {
                case "°¢ÂüµÂ":
                    CharacterList[3].GetComponent<Button>().enabled = true;
                    break;
                case "Âå¶û¿²":
                    CharacterList[1].GetComponent<Button>().enabled = true;
                    break;
                case "²©½ðÉ­":
                    CharacterList[2].GetComponent<Button>().enabled = true;
                    break;
                case "À³ÎÄ":
                    CharacterList[4].GetComponent<Button>().enabled = true;
                    break;
                case "°¬Ã×Àò":
                    CharacterList[0].GetComponent<Button>().enabled = true;

                    break;

            }

        }
    }

    public void KillSB()
    {
        kill = true;
        SetState(true);
        foreach (string s in DeadName.TxtLine)
        {
            switch (s)
            {
                case "°¢ÂüµÂ":
                    CharacterList[3].GetComponent<Button>().enabled = false;
                    break;
                case "Âå¶û¿²":
                    CharacterList[1].GetComponent<Button>().enabled = false;
                    break;
                case "²©½ðÉ­":
                    CharacterList[2].GetComponent<Button>().enabled = false;
                    break;
                case "À³ÎÄ":
                    CharacterList[4].GetComponent<Button>().enabled = false;
                    break;
                case "°¬Ã×Àò":
                    CharacterList[0].GetComponent<Button>().enabled = false;

                    break;

            }

        }
    }


}





   
