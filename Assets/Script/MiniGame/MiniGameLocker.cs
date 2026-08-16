using UnityEngine;

public class MiniGameLocker : MonoBehaviour
{
    private void OnEnable()
    {
        GlobalData.OnMiniGame = true;
    }

    private void OnDisable()
    {
        GlobalData.OnMiniGame = false;
    }
}
