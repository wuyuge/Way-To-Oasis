using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class LoadOnStart : MonoBehaviour
{

    public GameObject loadMenu;
    private bool _saveDataIsExist;
    
    /// <summary>
    /// 加载时检测是否存在存档可以读档
    /// </summary>
    void Start()
    {
        //遍历检测文件夹中是否存在存档文件
        for (int i = 0; i < 6; i++)
        {
            string fileName = SaveConstants.SaveFileNameTemplate.Replace("{Field}", i.ToString());
            string filePath = Path.Combine(SaveConstants.SaveFolderPath, fileName);
            if (File.Exists(filePath))
            {
                _saveDataIsExist = true;
                break;
            }
            _saveDataIsExist = false;
        }

        if (!_saveDataIsExist)
        {
            gameObject.GetComponent<Button>().interactable = false;
        }
        else
        {
            gameObject.GetComponent<Button>().interactable = true;
        }
    }


    public void OpenLoadMenu()
    {
        loadMenu.SetActive(true);
    }
    
    
    
    
}
