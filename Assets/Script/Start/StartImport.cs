using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartImport : MonoBehaviour
{

    public void SetAnim()
    {
        transform.parent.gameObject.GetComponent<Animator>().SetTrigger("Close");
    }

}
