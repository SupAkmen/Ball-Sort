using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Menu,
    Game,
    LevelComplete,
    Setting

}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int currentLevel = 1;

    public GameGraphic game;

    [Header("Settings")]
    public GameState gameState;


    [Header("Event")]
    public static Action<GameState> onGameStateChanged;
    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        LoadData();
    }

    private void Start()
    {
        game = FindObjectOfType<GameGraphic>();
    }

    public void SetGameState(GameState gameState)
    {
        this.gameState = gameState;
        onGameStateChanged?.Invoke(gameState);
    }

    public void BackButtonCallBack()
    {
        SetGameState(GameState.Menu);
    }

    public void NextButtonCallBack()
    {
        ResetLevel();
        currentLevel++;
        SaveData();
        SetGameState(GameState.Game);

    }

    public void ResetLevel()
    {
        game.ClearBottleGraphics();
    }

    public void PlayButtonCallBack()
    {
        currentLevel = 1;
        SetGameState(GameState.Game);
    }

    public int GetLevel()
    {
        return currentLevel;
    }


    private void LoadData()
    {
        currentLevel = PlayerPrefs.GetInt("currentLevel", 1);
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("currentLevel", currentLevel);
    }
}
