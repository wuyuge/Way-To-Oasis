using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day0SwicthMain : MonoBehaviour
{
    public TalkSystem TalkSystem;

    public void Switch()
    {
        TalkSystem.MainCanvas.SetActive(true);
        TalkSystem.charabar.SetActive(false);
        TalkSystem.transform.parent.gameObject.SetActive(false);
        TalkSystem.line = 0;
    }



}
