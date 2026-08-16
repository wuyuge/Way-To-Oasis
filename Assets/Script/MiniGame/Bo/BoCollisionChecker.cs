using System.Collections.Generic;
using UnityEngine;

public class BoCollisionChecker : MonoBehaviour
{
    [System.Serializable]
    public class ConnectData
    {
        public RectTransform rectTransform;
        public RectTransform connectPosition;
        public float distance;
        public bool connected;
        [Range(0,5)]public int index;
    }

    public List<ConnectData> connectPieces = new List<ConnectData>();

    private BoPiece _ownerPiece;
    public RectTransform SelfRt { get; private set; }

    private void Awake()
    {
        _ownerPiece = GetComponent<BoPiece>();
        SelfRt = GetComponent<RectTransform>();
    }

    private void Update()
    {
        CheckConnectDistance();
    }

    /// <summary>
    /// 检测吸附距离，满足条件执行拼接
    /// </summary>
    private void CheckConnectDistance()
    {
        if (_ownerPiece == null || SelfRt == null) return;

        foreach (var data in connectPieces)
        {
            if (data.rectTransform == null) continue;
            if (data.connected) continue;

            BoPiece targetPiece = data.rectTransform.GetComponent<BoPiece>();

            // 【修改】同组合内碎片直接跳过，不再用 enabled 判断
            if (targetPiece != null && _ownerPiece.GetRootPiece() == targetPiece.GetRootPiece())
                continue;

            if (CanConnect(SelfRt, data.rectTransform, data.distance))
            {
                // 【修改】判断"目标所在组合正在被拖动"，而不是"鼠标正好在目标碎片上"
                if (Input.GetMouseButtonUp(0) && IsTargetGroupBeingDragged(targetPiece))
                {
                    DoConnect(data);
                }
            }
        }
    }

    // 【新增】判断当前拖动的组合是不是目标碎片所在的组合
    private bool IsTargetGroupBeingDragged(BoPiece targetPiece)
    {
        if (targetPiece == null || BoGlobalData.CurrentPiece == null) return false;

        BoPiece currentPiece = BoGlobalData.CurrentPiece.GetComponent<BoPiece>();
        if (currentPiece == null) return false;

        return currentPiece.GetRootPiece() == targetPiece.GetRootPiece();
    }

    /// <summary>
    /// 使用世界坐标计算两点距离
    /// </summary>
    public bool CanConnect(RectTransform self, RectTransform target, float checkDistance)
    {
        Vector2 posSelf = self.position;
        Vector2 posTarget = target.position;

        float dx = posSelf.x - posTarget.x;
        float dy = posSelf.y - posTarget.y;
        float dist = Mathf.Sqrt(dx * dx + dy * dy);

        return dist < checkDistance;
    }

    /// <summary>
    /// 完整拼接逻辑（支持组合间合并）
    /// </summary>
    private void DoConnect(ConnectData data)
    {
        if (data.connected)
        {
            Debug.LogWarning($"目标已经连接，禁止重复更换父对象");
            return;
        }

        RectTransform targetRt = data.rectTransform;
        if (targetRt == null) return;

        // 【新增】找到目标碎片所在组合的根
        BoPiece targetPiece = targetRt.GetComponent<BoPiece>();
        BoPiece targetRootPiece = targetPiece != null ? targetPiece.GetRootPiece() : null;
        RectTransform targetRootRt = targetRootPiece != null ? targetRootPiece.RectTransform : targetRt;

        // 【新增】记录目标碎片连接前的世界坐标
        Vector3 targetWorldPos = targetRt.position;

        // 【修改】把目标组合的根设为当前碎片的子（保持世界坐标不变）
        targetRootRt.SetParent(SelfRt, true);

        // 【新增】计算连接点世界坐标，偏移整个目标组合使目标碎片对齐到连接点
        Vector3 connectWorldPos = data.connectPosition.position;
        Vector3 offset = connectWorldPos - targetWorldPos;
        targetRootRt.position += offset;

        // 【修改】设置目标根的渲染顺序
        targetRootRt.SetSiblingIndex(data.index);

        // 【修改】禁用目标根的拖拽能力
        if (targetRootPiece != null)
        {
            targetRootPiece.TryDisable();
        }

        data.connected = true;
        Debug.Log($"拼接完成，目标组合已合并，渲染层级设置为:{data.index}");
    }
}