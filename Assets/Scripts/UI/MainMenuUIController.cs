using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] Canvas mainMenuCanvas;

    [Header("Buttons")]
    [SerializeField] Button buttonStart;
    [SerializeField] Button buttonOptions;
    [SerializeField] Button buttonQuit;

    private void OnEnable() {
        // buttonStart.onClick.AddListener(OnStartGameBUttonClick);
        // buttonOptions.onClick.AddListener(OnButtonOptionsClicked);
        // buttonQuit.onClick.AddListener(OnButtonQuitClicked);
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonStart.gameObject.name, OnStartGameBUttonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonOptions.gameObject.name, OnButtonOptionsClicked);
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonQuit.gameObject.name, OnButtonQuitClicked);
    }

    private void OnDisable() {
        // buttonStart.onClick.RemoveAllListeners();
        // buttonOptions.onClick.RemoveAllListeners();
        // buttonQuit.onClick.RemoveAllListeners();
        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }

    private void Start() {
        Time.timeScale = 1f;
        GameManager.GameState = GameState.Playing;
        UIInput.Instance.SelectUI(buttonStart);  // 开始自动选择一个按钮
    }

    void OnStartGameBUttonClick() {
        mainMenuCanvas.enabled = false;  // 按下后将主菜单关闭
        SceneLoader.Instance.LoadMainScene();
    }

    void OnButtonOptionsClicked() {
        UIInput.Instance.SelectUI(buttonOptions);
    }

    void OnButtonQuitClicked() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
