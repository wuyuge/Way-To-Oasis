using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdvancedPreloader : MonoBehaviour
{
    public string targetScene;
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    private AsyncOperation loadOperation;
    private Scene currentScene; // 记录当前场景引用

    void Start()
    {
        // 保存当前场景的引用（确保获取到正确的激活场景）
        //currentScene = SceneManager.GetActiveScene();
        //StartCoroutine(PreloadScene());
        SceneManager.LoadScene(targetScene);
    }

    //IEnumerator PreloadScene()
    //{
    //    // 异步加载目标场景（Additive模式）
    //    loadOperation = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
    //    loadOperation.allowSceneActivation = false;

    //    while (!loadOperation.isDone)
    //    {
    //        float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
    //        progressBar.value = progress;
    //        progressText.text = $"加载中... {Mathf.Round(progress * 100)}%";

    //        // 当加载进度达到90%（可激活状态）
    //        if (loadOperation.progress >= 0.9f)
    //        {
    //            progressText.text = "点击任意键进入游戏";
    //            if (Input.anyKeyDown)
    //            {
    //                // 允许新场景激活
    //                loadOperation.allowSceneActivation = true;
    //                // 等待新场景完全激活
    //                yield return new WaitUntil(() => loadOperation.isDone);
    //                // 激活新场景（确保成为当前活动场景）
    //                SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene));
    //                // 卸载原场景（使用保存的引用，避免场景名变化导致的问题）
    //                yield return SceneManager.UnloadSceneAsync(currentScene);
    //                Time.timeScale = 1f; // 确保时间流逝恢复正常
    //                // 清理未使用的资源（可选，但推荐）
    //                Resources.UnloadUnusedAssets();
    //            }
    //        }

    //        yield return null;
    //    }
    //}
}