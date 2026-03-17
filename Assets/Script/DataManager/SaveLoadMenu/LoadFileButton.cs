using UnityEngine;
using UnityEngine.UI;

public class LoadFileButton : MonoBehaviour
{
    private SaveManager _saveManager;
    public int saveIndex;
    public GameObject linkObj;
    public Manager autoSaveIsOn;
    public Button deleteButton;

    private void Awake()
    {
        _saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
    }
    
    
    //在开启读档界面时禁止自动存档
    private void OnEnable()
    {
        if (autoSaveIsOn.GeneralBool)
        {
            autoSaveIsOn.GeneralBool = false;
        }
    }


    public void Load()
    {
        //获取挂载这个脚本的对应存档
        autoSaveIsOn.GeneralBool = true;
        _saveManager.LoadData(saveIndex);
    }

    public void UpLinkObj()
    {
        linkObj.SetActive(true);
        gameObject.GetComponent<Button>().enabled = false;
        if(saveIndex != 0) deleteButton.enabled = false;
        foreach (Transform value in gameObject.transform.parent)
        {
            if (value.gameObject != gameObject)
            {
                value.gameObject.GetComponent<LoadFileButton>().Cancel();
            }
        }
    }


    public void Cancel()
    {
        linkObj.SetActive(false);
        gameObject.GetComponent<Button>().enabled = true;
        if(saveIndex != 0) deleteButton.enabled = true;
    }



}
