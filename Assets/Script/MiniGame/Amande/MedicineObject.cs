using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MedicineObject", menuName = "创建数据/新建阿曼德材料")]
public class MedicineObject : ScriptableObject
{
    public enum Medicine
    {
        惰性粉末,副产物b12,T51A,棘草提取液,光合原料
    }
    
    public Medicine medicineName;
    
}

public static class MedicineManager
{
    public static List<MedicineObject> Container = new List<MedicineObject>();
    public static MedicineSender Sender;
    public static MedicineComposer Composer;
}
