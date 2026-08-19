using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Select : MonoBehaviour
{
    [FormerlySerializedAs("obj")] public Toggle toggle;
    private Image _image;
    public Toggle linkToggle;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }
    
    public void OnValueChanged(bool value)
    {
        if (value)
        {
            _image.color = new Color32(200, 200, 200,255);
            
        }
        else
        {
            _image.color = Color.white;
        }

        if (linkToggle.isOn)
        {
            linkToggle.isOn = false;
            toggle.isOn = value;
        }
    }



}
