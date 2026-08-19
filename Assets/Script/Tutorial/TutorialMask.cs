using UnityEngine;
using UnityEngine.UI;

public class TutorialMask : MonoBehaviour
{
    public Image mask;

    private void Awake()
    {
        mask = GetComponent<Image>();
    }

    private void Update()
    {
        mask.enabled = GlobalData.ShowText.CanShowText;
    }
}
