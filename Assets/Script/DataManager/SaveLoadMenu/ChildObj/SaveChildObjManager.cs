using UnityEngine;

public class SaveChildObjManager : MonoBehaviour
{

    public void CoverFile(GameObject o)
    {
        o.GetComponent<SaveFileButton>().CoverFile();
    }

    public void Cancel(GameObject o)
    {
        o.GetComponent<SaveFileButton>().Cancel();
    }
    
    public void SaveFile(GameObject o)
    {
        o.GetComponent<SaveFileButton>().SaveFile();
    }

}
