using UnityEngine;

public class MiniThinkBarPosition : MonoBehaviour
{
    public Transform target;


    private void LateUpdate()
    {
        gameObject.transform.position = target.position;
    }
}
