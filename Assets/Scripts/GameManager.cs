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

    public GameGraphic gameGraphics;
    public Game game;

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
        gameGraphics = FindObjectOfType<GameGraphic>();
        game = FindObjectOfType<Game>();
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

    public void PlayButtonCallBack()
    {
        LoadData();
        SetGameState(GameState.Game);
    }

    public void RestartButtonCallBack()
    {
        Restart();
        SetGameState(GameState.Game);
    }

    public void Restart()
    {
        gameGraphics.ClearBottleGraphics();
        LoadData();
        
    }    
    public void ResetLevel()
    {
        gameGraphics.ClearBottleGraphics();
    }

    public void UndoMove()
    {
        if (game != null)
        {
            game.UndoMove();
        }
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
