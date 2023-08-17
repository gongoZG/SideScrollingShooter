using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : PersistentSingleton<ScoreManager>
{
    public int Score => score;  // 等同于 Score { get => score; }
    [SerializeField] Vector3 scoreTextScale = new Vector3(1.2f, 1.2f, 1f);
    int score;
    int currentScore;

#region Score Display
    public void ResetScore() {
        score = 0;
        currentScore = 0;
        ScoreDisplay.UpdateText(score);
    }

    public void AddScore(int scorePoint) {
        currentScore += scorePoint;
        StartCoroutine(AddScoreCoroutine());
    }

    IEnumerator AddScoreCoroutine() {
        ScoreDisplay.ScaleText(scoreTextScale);
        while (score < currentScore) {
            score += 1;
            ScoreDisplay.UpdateText(score);

            yield return null;
        }
        ScoreDisplay.ScaleText(Vector3.one);
    }
#endregion
    
#region High Score System
    [System.Serializable] public class PlayerScore {
        public int score;
        public string playerName;

        public PlayerScore(int score, string playerName) {
            this.score = score;
            this.playerName = playerName;
        }
    }

    [System.Serializable] public class PlayerScoreData {
        public List<PlayerScore> list = new List<PlayerScore>();
    }

    readonly string SaveFileName = "player_score.json";
    string playerName = "default";
    public bool hasNewHighScore => score > LoadPlayerScoreData().list[9].score;
    public void setPlayerName(string newName) => playerName = newName;

    public void SavePlayerScoreData() {
        var playerScoreData = LoadPlayerScoreData();

        playerScoreData.list.Add(new PlayerScore(score, playerName));
        playerScoreData.list.Sort((x, y) => y.score.CompareTo(x.score));  // 降序排列

        SaveSystem.Save(SaveFileName, playerScoreData);
    }

    public PlayerScoreData LoadPlayerScoreData() {
        var playerScoreData = new PlayerScoreData();

        // 已经存在数据就读取，否则创建 10 个新的数据返回
        if (SaveSystem.SaveFileExists(SaveFileName)) {
            playerScoreData = SaveSystem.load<PlayerScoreData>(SaveFileName);
        }
        else {
            while (playerScoreData.list.Count < 10) {
                playerScoreData.list.Add(new PlayerScore(0, playerName));
            }
            SaveSystem.Save(SaveFileName, playerScoreData);
        }

        return playerScoreData;
    }

#endregion

}
