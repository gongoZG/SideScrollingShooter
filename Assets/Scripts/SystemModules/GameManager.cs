using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    public static GameState GameState {
        get => Instance.gameState; 
        set => Instance.gameState = value;
    }
    public static System.Action onGameOver;
    [SerializeField] GameState gameState = GameState.Playing;

}

public enum GameState {
    Playing,
    Paused,
    GameOver,
    Scoring,
}
