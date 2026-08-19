using UnityEngine;

public class FullMode : MonoBehaviour
{
    private bool _fullModeOn;
    private CgManager _cgManager;

    private void Awake()
    {
        _cgManager = gameObject.transform.parent.gameObject.GetComponent<CgManager>();
    }

    private void OnEnable()
    {
        _fullModeOn = false;
    }

    public void Click()
    {
         _cgManager.SetFullMode(!_fullModeOn);
         _fullModeOn = !_fullModeOn;
    }

    public void CloseCg()
    {
        _cgManager.HideCg();
    }
    
}
