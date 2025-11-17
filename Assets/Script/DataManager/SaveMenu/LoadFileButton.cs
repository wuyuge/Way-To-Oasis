using UnityEngine;

public class LoadFileButton : MonoBehaviour
{
    private SaveManager _saveManager;
    public int saveIndex;
    

    private void Awake()
    {
        _saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
    }


    public void Load()
    {
        //获取挂载这个脚本的对应存档
        
        _saveManager.LoadData(saveIndex);
    }






}
