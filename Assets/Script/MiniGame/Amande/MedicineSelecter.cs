using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicineSelector : MonoBehaviour
{
    public MedicineObject medicineObject;

    public void Select()
    {
        MedicineManager.Medicine = medicineObject;
    }
}
