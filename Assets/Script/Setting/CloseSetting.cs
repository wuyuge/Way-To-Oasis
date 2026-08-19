using UnityEngine;
/// <summary>
/// 仅用于挂载在按钮上点击时关闭界面
/// </summary>
public class CloseSetting : MonoBehaviour
{
    public GameObject linkGameObject;
    
    public void Close()
    {
        linkGameObject.SetActive(false);
    }
    
    
    
}
