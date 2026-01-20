using System;
using UnityEngine;


public class LuoGameLoader : MonoBehaviour
{
    public LuoLevelFile levelFile;
    
    
    public void LoadLevel()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var typeList = levelFile.pipeTypeList[i];
            if (typeList.isStartPoint)
            {
                LuoGameStartPoint.SetStartPointIndex(i);
            }
            transform.GetChild(i).gameObject.GetComponent<PipeManager>().SetOpen(typeList.type,typeList.state,typeList.isStartPoint,
                typeList.startIsVertical,typeList.isDestination,typeList.destinationIsVertical);
        }
    }


    private void Start()
    {
        LoadLevel();
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


