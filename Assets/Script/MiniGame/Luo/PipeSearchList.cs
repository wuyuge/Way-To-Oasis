using System.Collections.Generic;
using UnityEngine;

public class PipeSearchList : MonoBehaviour
{
    private Queue<GameObject> _pipeList = new Queue<GameObject>();
    
    public void AddPipe(GameObject pipe)
    {
        _pipeList.Enqueue(pipe);
    }
    
    public GameObject GetPipe()
    {
        return _pipeList.Dequeue();
    }
    
    public void ResetList()
    {
        _pipeList.Clear();
    }
    
    public bool PipeIn(GameObject pipe)
    {
        return _pipeList.Contains(pipe);
    }
}
