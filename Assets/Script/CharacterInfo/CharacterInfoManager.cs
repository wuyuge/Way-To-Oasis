using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class CharacterInfoManager : MonoBehaviour
{


    [System.Serializable]
    public class CharacterInfo
    {

        public string Name;
        public GameObject Info;
        public GameObject Image;


    }

    public List<CharacterInfo> CharacterInfos = new List<CharacterInfo>();


    public void ShowInfo(string Name)
    {

        foreach(CharacterInfo Info in CharacterInfos)
        {
            if(Info.Name == Name)
            {
                Info.Info.SetActive(true);
                Info.Image.SetActive(true);

            }
        }



    }

    public void CloseInfo()
    {
        foreach (CharacterInfo Info in CharacterInfos)
        {
            if (Info.Info.activeSelf)
            {
                Info.Info.GetComponent<Animator>().SetTrigger("Close");
                Info.Image.GetComponent<Animator>().SetTrigger("close");
            }
        }
    }




}
