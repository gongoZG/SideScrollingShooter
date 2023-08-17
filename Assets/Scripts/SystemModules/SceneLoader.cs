using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : PersistentSingleton<SceneLoader>
{
    [SerializeField] Image transitionImage;
    [SerializeField] float fadeTime = 3.5f;
    const string MAIN = "Main";
    const string MENU = "Main Menu";
    const string SCORING = "Scoring";
    Color color;

    void Load(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadCoroutine(string sceneName) {
        var loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        loadingOperation.allowSceneActivation = false;

        transitionImage.gameObject.SetActive(true);  // 黑色转场
        
        // Fade Out
        while (color.a < 1f) {
            color.a = Mathf.Clamp01(color.a + Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }

        // 当场景加载进度为 0.9， 即加载好场景等待被启用时才启用新的场景
        yield return new WaitUntil(() => loadingOperation.progress >= 0.9f);
        // Load(sceneName);
        loadingOperation.allowSceneActivation = true;  // Activate New Scene

        // Fade In
        while (color.a > 0f) {
            color.a = Mathf.Clamp01(color.a - Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }
        transitionImage.gameObject.SetActive(false);
    }

    public void LoadMainScene() {
        // Load(MAIN);
        StopAllCoroutines();  // 先停止其他的加载携程，防止频繁地切换场景导致的问题
        StartCoroutine(LoadCoroutine(MAIN));
    }

    public void LoadMenuScene() {
        StopAllCoroutines();
        StartCoroutine(LoadCoroutine(MENU));
    }

    public void LoadScoringScene() {
        StopAllCoroutines();
        StartCoroutine(LoadCoroutine(SCORING));
    }

}
