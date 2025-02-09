using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Element")]
    [SerializeField] private CanvasGroup menuCG;
    [SerializeField] private CanvasGroup gameCG;
    [SerializeField] private CanvasGroup levelCompleteCG;
    [SerializeField] private CanvasGroup settingCG;


    [Header("Menu")]
    [SerializeField] private TextMeshProUGUI menuCurrentLevel;

    [Header("Game")]
    [SerializeField] private TextMeshProUGUI rewardBottleCount;
    [SerializeField] private TextMeshProUGUI undoCount;
    [SerializeField] private TextMeshProUGUI currentLevel;
    

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ShowMenu();
        HideGame();
        HideLevelComplete();
        
        currentLevel.text = GameManager.instance.GetLevel().ToString();
        
        GameManager.onGameStateChanged += GameStateChangedCallBack; 
    }

    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= GameStateChangedCallBack;
    }

    private void GameStateChangedCallBack(GameState gameState)
    {
        switch(gameState)
        {
            case GameState.Menu:
                ShowMenu();
                HideGame();
                break;
            case GameState.Game:
                ShowGame();
                HideMenu();
                HideLevelComplete();
                HideSetting();
                break;
            case GameState.LevelComplete:
                ShowLevelComplete();
                HideGame();
                break;
            case GameState.Setting:
                ShowSetting();
                HideMenu();
                break;
        }
    }

    #region Show CG

    private void ShowCG(CanvasGroup cg)
    {
        cg.alpha = 1.0f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    private void HideCG(CanvasGroup cg)
    {
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }


    private void ShowMenu()
    {
        ShowCG(menuCG);
    }

    private void HideMenu()
    {
        HideCG(menuCG);
    }

    private void ShowGame()
    {
        ShowCG(gameCG);
    }

    private void HideGame()
    {
        HideCG(gameCG);
    }

    private void ShowLevelComplete()
    {
        ShowCG(levelCompleteCG);
    }

    private void HideLevelComplete()
    {
        HideCG(levelCompleteCG);
    }

    public  void ShowSetting()
    {
        ShowCG(settingCG);
    }

    public void HideSetting()
    {
        HideCG(settingCG);
    }

    #endregion

}
