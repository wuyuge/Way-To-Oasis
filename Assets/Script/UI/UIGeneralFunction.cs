using UnityEngine;

public class UIGeneralFunction : MonoBehaviour
{
    public GameObject orderObject;
    public TalkSystem talkSystem1, talkSystem2;

    public void SetState()
    {
        if (talkSystem1 is not null) talkSystem1.showText.CanShowText = orderObject.activeSelf;
        if (talkSystem2 is not null) talkSystem2.showText.CanShowText = orderObject.activeSelf;
        orderObject.SetActive(!orderObject.activeSelf);
    }
    
}
