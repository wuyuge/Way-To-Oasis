using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicineComposer : MonoBehaviour
{
    public enum MedicineType
    {
        标准型速效污染阻隔剂,洛尔坎需要的防水镀剂,PRCT7型程序性细胞转换浓缩液,α型通用镇定剂
    }
    
    [System.Serializable]
    public class Formula
    {
        public MedicineType medicineType;//什么药的配方
        public int needNum;//配方需要多少个材料
        public List<MedicineObject> needMedicine;//需要药的列表
    }
    public List<Formula> formulas;

    public void Compose()
    {
        // 边界校验：避免空指针异常
        if (formulas == null || formulas.Count == 0)
        {
            Debug.LogWarning("未配置任何合成配方！");
            return;
        }
        if (MedicineManager.Container == null || MedicineManager.Container.Count == 0)
        {
            Debug.LogWarning("材料容器为空，无法合成！");
            return;
        }

        // 打印当前材料列表
        string currentMaterials = GetMaterialsListString(MedicineManager.Container);
        Debug.Log($"📋 当前材料容器中的材料列表：{currentMaterials}");

        // 标记是否合成成功
        bool isComposeSuccess = false;

        foreach (var originalFormula in formulas)
        {
            // 第一步：校验材料数量是否匹配
            if (originalFormula.needNum != MedicineManager.Container.Count)
            {
                continue;
            }

            bool complete = true;
            // 关键：创建配方材料列表的副本（只复制列表内容，不影响原始数据）
            List<MedicineObject> tempNeedMedicine = new List<MedicineObject>(originalFormula.needMedicine);

            foreach (var obj in MedicineManager.Container)
            {
                // 检查当前材料是否在副本列表中
                if (!tempNeedMedicine.Contains(obj))
                {
                    complete = false;
                    break;
                }
                // 从副本中移除已匹配的材料（不影响原始配方）
                tempNeedMedicine.Remove(obj);
            }

            // 额外校验：副本列表为空才说明材料完全匹配（数量+种类）
            if (complete && tempNeedMedicine.Count == 0)
            {

                MedicineManager.Container.Clear();
                isComposeSuccess = true;
                break;
            }
        }

        // 遍历完所有配方都未匹配成功，提示合成失败并清空材料
        if (!isComposeSuccess)
        {
            Debug.LogError($"❌ 合成失败！当前材料组合【{currentMaterials}】无法合成任何药品，材料已清空。");
            MedicineManager.Container.Clear(); 
        }
    }

    /// <summary>
    /// 将MedicineObject列表转换为易读的字符串（方便打印和查看）
    /// </summary>
    /// <param name="materials">材料列表</param>
    /// <returns>格式化的材料字符串，如“材料A、材料B、材料C”</returns>
    private string GetMaterialsListString(List<MedicineObject> materials)
    {
        if (materials == null || materials.Count == 0)
        {
            return "无";
        }

        List<string> materialNames = new List<string>();
        foreach (var mat in materials)
        {
            string matName = mat != null ? mat.name : "未知材料";
            materialNames.Add(matName);
        }

        // 拼接成“材料1、材料2、材料3”的格式
        return string.Join("、", materialNames);
    }
}