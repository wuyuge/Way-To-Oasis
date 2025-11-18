using UnityEngine;
using UnityEngine.UI;

public class LoadFileButton : MonoBehaviour
{
    private SaveManager _saveManager;
    public int saveIndex;
    public GameObject linkObj;

    private void Awake()
    {
        _saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
    }


    public void Load()
    {
        //获取挂载这个脚本的对应存档
        
        _saveManager.LoadData(saveIndex);
    }

    public void UpLinkObj()
    {
        linkObj.SetActive(true);
        gameObject.GetComponent<Button>().enabled = false;
    }


    public void Cancel()
    {
        linkObj.SetActive(false);
        gameObject.GetComponent<Button>().enabled = true;
    }



}
