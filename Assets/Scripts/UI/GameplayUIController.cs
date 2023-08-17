using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour
{
    [Header("Player Input")]
    [SerializeField] PlayerInput playerInput;
    
    [Header("Audio Data")]
    [SerializeField] AudioData pauseSFX;
    [SerializeField] AudioData unpauseSFX;

    [Header("canvas")]
    [SerializeField] Canvas hUDCanvas;
    [SerializeField] Canvas menusCanvas;

    [Header("Bottons")]
    [SerializeField] Button resumeButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button mainMenuButton;

    private void OnEnable() {
        playerInput.onPause += Pause;
        playerInput.onUnpause += Unpause;

        // 这样会导致按钮一按下立刻执行功能，动画被跳过
        // resumeButton.onClick.AddListener(OnResumeButtonClick);
        // optionsButton.onClick.AddListener(OnOptionsButtonClick);
        // mainMenuButton.onClick.AddListener(OnMainMenuButtonClick);
        
        // 由动画器完成按钮功能
        ButtonPressedBehavior.buttonFunctionTable.Add(
            resumeButton.gameObject.name, OnResumeButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(
            optionsButton.gameObject.name, OnOptionsButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(
            mainMenuButton.gameObject.name, OnMainMenuButtonClick);
    }

    private void OnDisable() {
        playerInput.onPause -= Pause;
        playerInput.onUnpause -= Unpause;

        // 由于该字典是静态的，不会自动清空，因此需要手动清空
        ButtonPressedBehavior.buttonFunctionTable.Clear();
        // resumeButton.onClick.RemoveAllListeners();
        // optionsButton.onClick.RemoveAllListeners();
        // mainMenuButton.onClick.RemoveAllListeners();
    }

    void Pause() {
        // 这样会造成子弹时间无法暂停的bug，因为子弹时间是一个连续改变时间刻度的携程
        // Time.timeScale = 0f;  
        TimeController.Instance.Pause();
        
        hUDCanvas.enabled = false;
        menusCanvas.enabled = true;
        GameManager.GameState = GameState.Paused;
        
        playerInput.EnablePauseMenuInput();
        playerInput.Switch2DynamicUpdateMode();
        UIInput.Instance.SelectUI(resumeButton);
        AudioManager.Instance.PlaySFX(pauseSFX);
    }

    void Unpause() {
        // 先选中恢复按钮再播放选中动画，播放完成后会自动执行对应的Click函数，无需显式地调用
        // resumeButton.Select();
        UIInput.Instance.SelectUI(resumeButton);
        resumeButton.animator.SetTrigger("Pressed");
        AudioManager.Instance.PlaySFX(unpauseSFX);
        // GameManager.GameState = GameState.Playing;
        // OnResumeButtonClick();
    }

    void OnResumeButtonClick() {
        // Time.timeScale = 1f;
        TimeController.Instance.Unpause();
        
        hUDCanvas.enabled = true;
        menusCanvas.enabled = false;
        GameManager.GameState = GameState.Playing;

        playerInput.Switch2FixedUpdateMode();
        playerInput.EnableGameplayInput();
    }

    void OnOptionsButtonClick() {
        UIInput.Instance.SelectUI(optionsButton);
        playerInput.EnablePauseMenuInput();
    }

    void OnMainMenuButtonClick() {
        SceneLoader.Instance.LoadMenuScene();
    }
}
