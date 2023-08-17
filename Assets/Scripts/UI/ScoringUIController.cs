using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoringUIController : MonoBehaviour
{
    [Header("Background")]
    [SerializeField] Image background;
    [SerializeField] Sprite[] backgroundImages;

    [Header("Scoring Screen")]
    [SerializeField] Canvas scoringScreenCanvas;
    [SerializeField] Text playerScoreText;
    [SerializeField] Button buttonMainMenu;
    [SerializeField] Transform highScoreLeaderboardContainer;

    [Header("High Score Screen")]
    [SerializeField] Canvas newHighScoreScreenCanvas;
    [SerializeField] Button buttonCancel;
    [SerializeField] Button buttonSubmit;
    [SerializeField] InputField playerNameInputField;

    private void Start() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        ShowrandomBackground();

        if (ScoreManager.Instance.hasNewHighScore) {
            ShowNewHighScoreScreen();
        }
        else {
            ShowScoringScreen();
        }

        ButtonPressedBehavior.buttonFunctionTable.Add(
            buttonMainMenu.gameObject.name, OnButtonMainMenuClicked);
        ButtonPressedBehavior.buttonFunctionTable.Add(
            buttonSubmit.gameObject.name, OnButtonsubmitClicked);
        ButtonPressedBehavior.buttonFunctionTable.Add(
            buttonCancel.gameObject.name, HideNewHighScoreScreen);
        GameManager.GameState = GameState.Scoring;
    }


    private void OnDisable() {
        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }

    void ShowrandomBackground() {
        background.sprite = backgroundImages[Random.Range(0, backgroundImages.Length)];
    }

    void ShowNewHighScoreScreen()
    {
        newHighScoreScreenCanvas.enabled = true;
        UIInput.Instance.SelectUI(buttonCancel);
    }

    void ShowScoringScreen() {
        scoringScreenCanvas.enabled = true;
        playerScoreText.text = ScoreManager.Instance.Score.ToString();
        UIInput.Instance.SelectUI(buttonMainMenu);
        // Update high score leaderboard UI
        UpdateHighScoreLeaderboard();
    }

    private void HideNewHighScoreScreen() {
        newHighScoreScreenCanvas.enabled = false;
        ScoreManager.Instance.SavePlayerScoreData();
        ShowrandomBackground();
        ShowScoringScreen();
    }

    void UpdateHighScoreLeaderboard() {
        var playerScoreList = ScoreManager.Instance.LoadPlayerScoreData().list;

        for (int  i = 0; i < highScoreLeaderboardContainer.childCount; i++) {
            var child = highScoreLeaderboardContainer.GetChild(i);

            child.Find("Rank").GetComponent<Text>().text = (i + 1).ToString();
            child.Find("Score").GetComponent<Text>().text = playerScoreList[i].score.ToString();
            child.Find("Name").GetComponent<Text>().text = playerScoreList[i].playerName.ToString();
        }
    }

    void OnButtonMainMenuClicked() {
        scoringScreenCanvas.enabled = false;
        SceneLoader.Instance.LoadMenuScene();
    }

    void OnButtonsubmitClicked() {
        if (!string.IsNullOrEmpty(playerNameInputField.text)) {
            ScoreManager.Instance.setPlayerName(playerNameInputField.text);
        }

        HideNewHighScoreScreen();
    }
}
