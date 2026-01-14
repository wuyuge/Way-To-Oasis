using System.Collections.Generic;
using UnityEngine;

public class HistoryManager : MonoBehaviour
{
    public List<GameObject> contents =  new List<GameObject>();
    public GameObject conversationPrefab;
    
    public void Refresh()
    {
        if (contents.Count == 0)
        {
            return;
        }
        foreach (var g in contents)
        {
            g.GetComponent<ConversationHistory>().ResetThis();
            g.SetActive(false);
        }
    }


    public void SetHistory(TextHistory history)
    {
        GameObject tempObj;
        if (conversationPrefab == null)
        {
            Debug.LogError("conversationPrefab is null");
            return;
        }
        if (contents.Count == 0)
        {
            tempObj = Instantiate(conversationPrefab, transform);
            contents.Add(tempObj);
            tempObj.GetComponent<ConversationHistory>().SetHistory(history);
            return;
        }

        foreach (var g in contents)
        {
            if (!g.activeSelf)
            {
                g.SetActive(true);
                g.GetComponent<ConversationHistory>().SetHistory(history);
                return;
            }
        }
        
        tempObj = Instantiate(conversationPrefab, transform);
        contents.Add(tempObj);
        tempObj.GetComponent<ConversationHistory>().SetHistory(history);
    }

}

public class TextHistory
{
    public bool IsPlayer;
    public string Text;
    public string CharacterName;
    public bool IsASide;
}
