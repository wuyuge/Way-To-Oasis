using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 迷你角色管理器：负责角色激活/禁用、死亡状态检测和位置自动补位
/// </summary>
public class MiniCharacterManager : MonoBehaviour
{
    [System.Serializable]
    public class MiniCharacter
    {
        [Tooltip("角色唯一名称（用于死亡检测匹配）")]
        public string characterName;
        [Tooltip("角色实体对象（控制显示/隐藏）")]
        public GameObject characterObject;
        [Tooltip("角色对应的对话栏")]
        public GameObject characterTalkBar;

        public GameObject thinkBar;
        [Tooltip("角色Y轴固定位置（X轴由位置列表决定）")]
        public float fixedYPosition = 0f;

        public RectTransform SitPosition;
    }

    [Header("角色配置")]
    [Tooltip("所有迷你角色的列表")]
    public List<MiniCharacter> miniCharacters = new List<MiniCharacter>();

    [Header("位置配置")]
    [Tooltip("角色可占用的位置列表（按顺序分配）")]
    public List<RectTransform> positionSlots = new List<RectTransform>();

    [Header("死亡数据源")]
    [Tooltip("存储死亡角色名称的管理器1")]
    public Manager deadNameManager;
    [Tooltip("存储死亡角色名称的管理器2")]
    public Manager usedBodyManager;

    [Header("调试选项")]
    [Tooltip("是否启用调试日志")]
    public bool enableDebugLogs = false;

    // 死亡状态跟踪
    private HashSet<string> deadCharacters = new HashSet<string>(); // 当前死亡角色集合
    private int previousDeadCount1 = 0; // 管理器1上一帧死亡数量
    private int previousDeadCount2 = 0; // 管理器2上一帧死亡数量

    // 缓存组件
    private Animator _animator;
    private List<RectTransform> _availablePositions = new List<RectTransform>(); // 可用位置缓存

    private Animator LightAnim;


    public GameObject CampLight;

    public AudioSource WalkingSound;

    public GameObject CampFire;

    public bool isWalking;

    private void Awake()
    {
        InitializeComponents();
        InitializeCharacters();
    }

    private void Start()
    {
        // 初始分配一次位置
        UpdateCharacterPositions();
        LightAnim = CampLight.GetComponent<Animator>();
    }

    private void Update()
    {
        CheckDeathStatusChanges();
    }

    /// <summary>
    /// 初始化组件和缓存
    /// </summary>
    private void InitializeComponents()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogWarning("当前对象上未找到Animator组件，动画相关功能将失效");
        }

        // 过滤位置列表中的空引用
        _availablePositions.Clear();
        foreach (var pos in positionSlots)
        {
            if (pos != null)
            {
                _availablePositions.Add(pos);
            }
            else
            {
                Debug.LogWarning("位置列表中存在空引用，已自动过滤");
            }
        }
    }

    /// <summary>
    /// 初始化角色状态
    /// </summary>
    private void InitializeCharacters()
    {
        foreach (var character in miniCharacters)
        {
            if (character.characterObject == null)
            {
                Debug.LogWarning($"角色 {character.characterName} 未指定实体对象");
                continue;
            }

            // 初始隐藏对话栏
            if (character.characterTalkBar != null)
            {
                character.characterTalkBar.SetActive(false);
            }

            if (character.thinkBar is not null)
            {
                character.thinkBar.SetActive(false);
            }

            // 初始激活所有角色（死亡检测会自动禁用死亡角色）
            character.characterObject.SetActive(true);
        }
    }

    /// <summary>
    /// 检测两个管理器的死亡状态变化
    /// </summary>
    private void CheckDeathStatusChanges()
    {
        bool hasChange1 = CheckManagerDeathChanges(deadNameManager, ref previousDeadCount1);
        bool hasChange2 = CheckManagerDeathChanges(usedBodyManager, ref previousDeadCount2);

        if (hasChange1 || hasChange2)
        {
            if (enableDebugLogs)
                Debug.Log($"检测到死亡状态变化，当前死亡角色数：{deadCharacters.Count}");

            UpdateCharacterPositions(); // 重新分配位置
        }
    }

    /// <summary>
    /// 检测单个管理器的死亡角色变化
    /// </summary>
    private bool CheckManagerDeathChanges(Manager manager, ref int previousCount)
    {
        if (manager == null || manager.TxtLine == null)
            return false;

        HashSet<string> currentDead = new HashSet<string>();

        // 提取当前管理器中的死亡角色
        foreach (var line in manager.TxtLine)
        {
            string cleanLine = line.Trim();
            if (cleanLine.Contains("Leader")) // 忽略领导者
                continue;

            // 精确匹配角色名称
            foreach (var character in miniCharacters)
            {
                if (cleanLine.Contains(character.characterName))
                {
                    currentDead.Add(character.characterName);
                }
            }
        }

        // 检测是否有变化
        if (currentDead.Count != previousCount)
        {
            previousCount = currentDead.Count;
            // 更新全局死亡集合（合并新死亡角色）
            foreach (var dead in currentDead)
            {
                deadCharacters.Add(dead);
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// 更新所有角色的位置（死亡角色禁用，存活角色补位）
    /// </summary>
    private void UpdateCharacterPositions()
    {
        // 1. 处理死亡角色（禁用）
        foreach (var character in miniCharacters)
        {
            if (character.characterObject == null)
                continue;

            bool isDead = deadCharacters.Contains(character.characterName);
            character.characterObject.SetActive(!isDead);

            // 同步隐藏对话栏
            if (character.characterTalkBar != null)
            {
                character.characterTalkBar.SetActive(false);
            }
            
            if (character.thinkBar is not null)
            {
                character.thinkBar.SetActive(false);
            }
        }

        // 2. 收集存活角色
        List<MiniCharacter> aliveCharacters = new List<MiniCharacter>();
        foreach (var character in miniCharacters)
        {
            if (character.characterObject != null &&
                character.characterObject.activeSelf &&
                !deadCharacters.Contains(character.characterName))
            {
                aliveCharacters.Add(character);
            }
        }

        // 3. 检查位置是否充足
        if (aliveCharacters.Count > _availablePositions.Count)
        {
            Debug.LogError($"存活角色数量（{aliveCharacters.Count}）超过可用位置数量（{_availablePositions.Count}），部分角色将无法显示");
        }

        // 4. 为存活角色分配位置（按顺序补位）
        for (int i = 0; i < aliveCharacters.Count; i++)
        {
            if (i >= _availablePositions.Count)
                break; // 位置不足时停止分配

            var character = aliveCharacters[i];
            var targetPos = _availablePositions[i];

            // 设置位置（使用世界坐标，若为UI建议用anchoredPosition）
            character.characterObject.transform.position = new Vector3(
                targetPos.position.x,
                character.fixedYPosition,
                character.characterObject.transform.position.z
            );

            if (enableDebugLogs)
                Debug.Log($"角色 {character.characterName} 已移动到位置 {i}");
        }
    }

    #region 动画控制方法
    public void ShowMiniCharacter()
    {
        if (_animator != null)
            _animator.SetTrigger("Show");
    }

    public void CloseMiniCharacter()
    {
        if (_animator != null)
            _animator.SetTrigger("Close");
        LightAnim.SetTrigger("Close");
    }

    public void SetSit()
    {
        if (_animator != null)
            _animator.enabled = false;

        foreach (var character in miniCharacters)
        {
            if (character.characterObject == null)
                continue;

            var anim = character.characterObject.GetComponent<Animator>();
            if (anim != null)
                anim.SetTrigger("Sit");

            character.characterObject.GetComponent<RectTransform>().position = character.SitPosition.position;

        }

        CampFire.SetActive(true);


        isWalking = false;
        CampLight.SetActive(true);

        Invoke(nameof(EnableAnimator), 1f);
    }

    public void SetStand()
    {
        if (_animator != null)
            _animator.enabled = false;

        foreach (var character in miniCharacters)
        {
            if (character.characterObject == null)
                continue;

            var anim = character.characterObject.GetComponent<Animator>();
            if (anim != null)
                anim.SetTrigger("Stand");
        }
        UpdateCharacterPositions();
        CampLight.SetActive(false);
        Invoke(nameof(EnableAnimator), 1f);
        if(WalkingSound.isPlaying)         
        {
            WalkingSound.Stop();
        }
        isWalking = false;
        CampFire.SetActive(false);
    }

    public void SetWalk()
    {
        if (_animator != null)
            _animator.enabled = false;
        WalkingSound.Play();
        foreach (var character in miniCharacters)
        {
            if (character.characterObject == null)
                continue;

            var anim = character.characterObject.GetComponent<Animator>();
            if (anim.GetBool("Stand"))
            {
                anim.ResetTrigger("Stand");
            }
            
            anim.SetTrigger("Walk");
        }

        isWalking = true;
        CampFire.SetActive(false);
        Invoke(nameof(EnableAnimator), 1f);
    }



    private void EnableAnimator()
    {
        if (_animator != null)
            _animator.enabled = true;
    }
    #endregion


    public void OffLight()
    {
        LightAnim.SetTrigger("Close");
    }

}