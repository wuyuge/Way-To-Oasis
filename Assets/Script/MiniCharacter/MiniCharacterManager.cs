using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 管理迷你角色的激活、禁用与位置分配
public class MiniCharacterManager : MonoBehaviour
{
    // 可序列化类：关联角色的数据、游戏对象与对话栏
    [System.Serializable]
    public class MiniCharacter
    {
        public string characterName; // 重命名以提高清晰度（避免与系统关键字"name"冲突）
        public GameObject characterObject; // 角色实体对象
        public GameObject characterTalkBar; // 角色对话栏对象
        public float CharacterPositionY = 0f;
    }

    [Header("角色设置")]
    public List<MiniCharacter> miniCharacterList = new List<MiniCharacter>(); // 迷你角色列表（命名更统一）

    [Header("位置设置")]
    public List<RectTransform> positionList = new List<RectTransform>(); // 用于存放存活角色的位置列表
    private List<RectTransform> usedPositions = new List<RectTransform>(); // 跟踪已占用的位置（防止角色重叠）

    [Header("死亡数据来源")]
    public Manager deadNameManager; // 存储死亡角色名称的管理器
    public Manager usedBodyManager; // 另一个存储死亡角色的管理器（与你原始逻辑一致）

    private int previousDeadCount = 0; // 记录上一帧的死亡角色数量（用于检测死亡状态变化）


    private void Awake()
    {

        // 初始化：默认隐藏所有角色的对话栏
        foreach (MiniCharacter character in miniCharacterList)
        {
            if (character.characterTalkBar != null) // 增加空值判断，避免空引用错误
            {
                character.characterTalkBar.SetActive(false);
            }
            character.CharacterPositionY = character.characterObject.transform.position.y;
        }
        previousDeadCount = 0;
        usedPositions.Clear(); // 初始化已占用位置列表
    }


    // 每帧更新
    void Update()
    {
        // 检测两个管理器中是否有新角色死亡，有则重新分配位置
        if (CheckNewDeathOccurred(deadNameManager))
        {
            ResetUsedPositions(); // 重置已占用位置记录
            SwitchCharacterPositions(); // 为存活角色重新分配位置
        }
        else if (CheckNewDeathOccurred(usedBodyManager))
        {
            ResetUsedPositions(); // 重置已占用位置记录
            SwitchCharacterPositions(); // 为存活角色重新分配位置
        }
    }

    /// <summary>
    /// 检测指定管理器中是否有新角色死亡
    /// </summary>
    /// <param name="deathDataManager">存储死亡数据的管理器</param>
    /// <returns>有新死亡返回true，否则返回false</returns>
    bool CheckNewDeathOccurred(Manager deathDataManager)
    {
        int currentDeadCount = 0;
        List<string> currentDeadList = new List<string>(); // 存储当前帧检测到的死亡角色名称

        // 若管理器的文本列表不为空，才进行死亡角色检测
        if (deathDataManager != null && deathDataManager.TxtLine != null && deathDataManager.TxtLine.Count > 0)
        {
            foreach (string textLine in deathDataManager.TxtLine)
            {
                // 跳过包含"Leader"（领导者）的文本行（与你原始逻辑一致）
                if (textLine.Contains("Leader"))
                {
                    continue;
                }

                // 遍历所有角色，判断当前文本行是否包含该角色名称（即角色是否死亡）
                foreach (MiniCharacter character in miniCharacterList)
                {
                    if (!string.IsNullOrEmpty(character.characterName) && textLine.Contains(character.characterName))
                    {
                        // 避免重复添加同一死亡角色
                        if (!currentDeadList.Contains(character.characterName))
                        {
                            currentDeadList.Add(character.characterName);
                            currentDeadCount++;
                        }
                    }
                }
            }
        }

        // 若当前死亡数量与上一帧不同，说明有新角色死亡，更新记录并返回true
        if (currentDeadCount != previousDeadCount)
        {
            previousDeadCount = currentDeadCount;
            // 禁用所有死亡角色的实体与对话栏
            DisableDeadCharacters(currentDeadList);
            return true;
        }

        return false;
    }


    /// <summary>
    /// 禁用所有死亡角色的实体对象与对话栏
    /// </summary>
    /// <param name="deadCharacterNames">死亡角色名称列表</param>
    void DisableDeadCharacters(List<string> deadCharacterNames)
    {
        foreach (MiniCharacter character in miniCharacterList)
        {
            if (deadCharacterNames.Contains(character.characterName))
            {
                if (character.characterObject != null)
                {
                    character.characterObject.SetActive(false);
                }
                if (character.characterTalkBar != null)
                {
                    character.characterTalkBar.SetActive(false);
                }
            }
            
        }
    }


    /// <summary>
    /// 为所有存活角色重新分配位置
    /// </summary>
    void SwitchCharacterPositions()
    {
        foreach (MiniCharacter character in miniCharacterList)
        {
            // 只给激活状态的存活角色分配位置
            if (character.characterObject != null && character.characterObject.activeSelf)
            {
                RectTransform availablePos = GetAvailablePosition(); // 获取一个可用位置
                if (availablePos != null)
                {
                    // 将角色位置设置为可用位置的世界坐标（适配UI RectTransform）
                    character.characterObject.transform.position = new Vector3(availablePos.position.x,character.CharacterPositionY);
                    // 若需要让角色与位置的层级保持一致，可添加以下代码：
                    // character.characterObject.transform.SetParent(availablePos.parent, false);
                }
                else
                {
                    Debug.LogWarning($"没有足够的可用位置分配给角色：{character.characterName}");
                }
            }
        }
    }


    /// <summary>
    /// 获取一个未被占用的位置
    /// </summary>
    /// <returns>可用的位置RectTransform，若无则返回null</returns>
    RectTransform GetAvailablePosition()
    {
        // 遍历所有预设位置，找到第一个未被占用的位置
        foreach (RectTransform pos in positionList)
        {
            if (pos != null && !usedPositions.Contains(pos))
            {
                usedPositions.Add(pos); // 标记该位置为已占用
                return pos;
            }
        }

        // 若所有位置都被占用，打印警告（方便调试）
        Debug.LogWarning("预设的位置数量不足，无法分配新位置！");
        return null;
    }


    /// <summary>
    /// 重置已占用位置的记录（重新分配位置前调用）
    /// </summary>
    void ResetUsedPositions()
    {
        usedPositions.Clear();
    }
}