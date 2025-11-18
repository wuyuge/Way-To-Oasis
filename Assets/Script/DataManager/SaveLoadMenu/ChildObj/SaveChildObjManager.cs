using UnityEngine;

public class SaveChildObjManager : MonoBehaviour
{

    public void CoverFile()
    {
        gameObject.transform.parent.parent.gameObject.GetComponent<SaveFileButton>().CoverFile();
    }

    public void Cancel()
    {
        gameObject.transform.parent.parent.gameObject.GetComponent<SaveFileButton>().Cancel();
    }

    public void SaveFile()
    {
        gameObject.transform.parent.parent.gameObject.GetComponent<SaveFileButton>().SaveFile();
    }

}
