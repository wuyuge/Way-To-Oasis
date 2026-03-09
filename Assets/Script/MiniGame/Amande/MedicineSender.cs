using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MedicineSender : MonoBehaviour
{
    [SerializeField]
    private MedicineObject medicineObject;

    public int clearTime;
    public MiniAmandeTalk amandeTalk;
    public Manager fail, heavyFail;
    
    private void Awake()
    {
        MedicineManager.Sender = this;
    }


    private void OnEnable()
    {
        clearTime = 0;
    }

    public void SendMedicine()
    {
        if (medicineObject != null)
        {
            MedicineManager.Container.Add(medicineObject);
            medicineObject = null;
        }

        if (MedicineManager.Container.Count > 1)
        {
            MedicineManager.Composer.Compose();
        }
    }


    public void SetMedicine(MedicineObject medicine)
    {
        medicineObject = medicine;
    }
    
    public void Clear()
    {
        if (MedicineManager.Container.Count > 0)
        {
            clearTime++;
            if (clearTime > 3)
            {
                amandeTalk.SetText(heavyFail.TxtLine[Random.Range(0, heavyFail.TxtLine.Count)]);
                AchievementManager.UnlockAchievement("ACH_ENVIRONMENT_CONTAMINATION");
            }
            else
            {
                amandeTalk.SetText(fail.TxtLine[Random.Range(0, fail.TxtLine.Count)]);
            }
        }
        
        MedicineManager.Container.Clear();
    }
    
}
