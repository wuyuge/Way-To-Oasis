using System;
using UnityEngine;


public class LuoGameLoader : MonoBehaviour
{
    public LuoLevelFile levelFile;
    public GameObject replaceBox;
    
    
    public void LoadLevel(bool reset = false)
    {
        foreach (Transform obj in replaceBox.transform)
        {
            Destroy(obj.gameObject);
        }

        foreach (var items in levelFile.replaceItems)
        {
            Instantiate(items, replaceBox.transform);
        }
        
        for (int i = 0; i < transform.childCount; i++)
        {
            var typeList = levelFile.pipeTypeList[i];
            if (typeList.isStartPoint)
            {
                LuoGameStartPoint.SetStartPointIndex(i);
            }

            PipeManager tempManager = transform.GetChild(i).gameObject.GetComponent<PipeManager>();
            tempManager.UpdateItemList();
            tempManager.SetOpen(typeList.type,typeList.state,typeList.isStartPoint,
                typeList.startIsVertical,typeList.isDestination,typeList.destinationIsVertical);
            if (reset)
            {
                ResetAnimator(tempManager); 
            }
            
        }
        PipeManager manager = transform.GetChild(0).gameObject.GetComponent<PipeManager>();
        manager.ResetConnection();


    }


    private void OnEnable()
    {
        LoadLevel();
    }

    public void Restart()
    {
        LoadLevel(true);
    }


    private void ResetAnimator(PipeManager manager)
    {
        manager.anim.SetTrigger("Reset");
    }
    
}

public static class LuoGameStartPoint
{
    private static int _startPointIndex;

    public static void SetStartPointIndex(int index)
    {
        _startPointIndex = index;
    }
    
    public static int GetStartPointIndex()
    {
        return _startPointIndex;
    }
    
}


