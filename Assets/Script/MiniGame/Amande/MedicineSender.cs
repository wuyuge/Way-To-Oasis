using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MedicineSender : MonoBehaviour
{
    [SerializeField]
    private MedicineObject medicineObject;

    public int clearTime;
    public MiniAmandeTalk amandeTalk;
    public Manager fail, heavyFail;
    public MedicineComposer composer;
    public Animator liquid;
    public bool compete;
    private int _containNum;
    public AudioSource aS;
    public AudioClip clip1, clip2;

    private void Awake()
    {
        try
        {
            composer = GetComponent<MedicineComposer>();
        }
        catch (Exception e)
        {
            Debug.LogError($"阿曼德合成器获取失败 {e}");
        }
    }

    private void OnEnable()
    {
        clearTime = 0;
        _containNum = 0;
    }

    private void PlaySound()
    {
        var i = Random.Range(0, 2);
        aS.clip = i == 0 ? clip1 : clip2;
        aS.Play();
    }

    public void SendMedicine()
    {
        if (compete)
        {
            return;
        }
        _containNum++;
        if (medicineObject != null)
        {
            composer.container.Add(medicineObject);
            PlaySound();
            medicineObject = null;
        }

        if (composer.container.Count > 1)
        {
            if (composer.Compose())
            {
                amandeTalk.ShowSuccess();
                compete = true;
            }
        }
        liquid.SetInteger("contain",_containNum);
    }
    

    public void SetMedicine()
    {
        medicineObject = MedicineManager.Medicine;
    }
    
    public void Clear()
    {
        if (compete)
        {
            return;
        }
        
        if (composer.container.Count > 0)
        {
            clearTime++;
            if (clearTime > 3)
            {
                amandeTalk.SetFail(2);
                AchievementManager.UnlockAchievement("ACH_ENVIRONMENT_CONTAMINATION");
            }
            else
            {
                amandeTalk.SetFail(1);
            }
        }

        _containNum = 0;
        liquid.SetTrigger("clear");
        liquid.SetInteger("contain",_containNum);
        composer.container.Clear();
    }
    
}
