using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicineSender : MonoBehaviour
{
    public MedicineObject medicineObject;
    
    public void SendMedicine()
    {
        MedicineManager.Container.Add(medicineObject);
    }
    
}
