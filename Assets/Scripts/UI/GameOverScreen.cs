using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] PlayerInput input;
    [SerializeField] Canvas hUDCanvas;
    [SerializeField] AudioData confirmGameOverSound; 

    Canvas canvas;
    Animator animator;

    private void Awake() {
        canvas = GetComponent<Canvas>();
        animator = GetComponent<Animator>();

        canvas.enabled = false;
        animator.enabled = false;
    }

    private void OnEnable() {
        GameManager.onGameOver += OnGameOver;
        input.onGameOver += OnConfirmGameOver;
    }

    private void OnDisable() {
        GameManager.onGameOver -= OnGameOver;
        input.onGameOver -= OnConfirmGameOver;
    }

    private void OnGameOver()
    {
        hUDCanvas.enabled = false;  // 关闭HUD
        canvas.enabled = true;
        animator.enabled = true;
        input.DisableAllInputs();  // 游戏结束的瞬间禁用所有输入
    }

    void OnConfirmGameOver() {
        AudioManager.Instance.PlaySFX(confirmGameOverSound);
        input.DisableAllInputs();
        animator.Play("GameOverScreenExit");
        SceneLoader.Instance.LoadScoringScene();
    }

    // 在 Enter 动画播完的时候调用，关键帧
    void EnableGameOverInput() {
        input.EnableGameOverInput();
    }
}
