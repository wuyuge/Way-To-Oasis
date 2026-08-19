using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LuoItemBox : MonoBehaviour
{
    public bool setting = false;
    private RectTransform rectTransform;
    public int index;
    private Image _image;
    private RectTransform curObj;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        if (!setting)
        {
            foreach (var value in LuoGlobalData.ItemList)
            {
                if (UiCollider.IsCollision(value, rectTransform) )
                {
                    setting = true;
                    curObj = value;
                }
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                LuoGlobalData.LevelLoader.Spawn(curObj.gameObject.GetComponent<LuoItem>().type,index,false);
                _image.enabled = false;
                Destroy(curObj.gameObject);
                enabled = false;
            }
            if (!UiCollider.IsCollision(curObj, rectTransform) )
            {
                setting = false;
                
            }
        }
    }

    public void Set(int index)
    {
        this.index = index;
    }
    
    
    
}
