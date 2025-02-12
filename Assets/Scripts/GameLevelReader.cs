using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelReader : MonoBehaviour
{
    public Game game;
    public static GameLevelReader instance;
    public static event Action<int> OnLevelLoaded;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        GameManager.onGameStateChanged += GameStateChangedCallBack;
    }

    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= GameStateChangedCallBack;
    }

    private void GameStateChangedCallBack(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Game:
                Debug.Log("load level" + GameManager.instance.currentLevel);
                LoadLevelFromResources(GameManager.instance.currentLevel); // Load level hiện tại
                break;
        }
    }

    // Hàm load dữ liệu từ Resources
    public void LoadLevelFromResources(int levelNumber)
    {
    
        string fileName = $"Levels/level_{levelNumber}"; // Định dạng tệp trong folder Resources/Level
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);

        if (textAsset == null)
        {
            Debug.LogError($"Could not load level data from Resources/{fileName}. Make sure the file exists.");
            return;
        }

        LoadLevel(textAsset);

    }
    int bottleCount = 0;
    int ballPerBottle = 0;
    int currentLevelNumber = 0;

    public void LoadLevel(TextAsset textAsset)
    {
        string[] lines = textAsset.text.Split(new string[] { "\n", "\r" }, System.StringSplitOptions.RemoveEmptyEntries);

        List<int[]> bottleArrays = new List<int[]>();

        // Đọc số level hiện tại
        currentLevelNumber = int.Parse(lines[0]);

        for (int i = 1; i < lines.Length; i++) // Bắt đầu từ dòng 1 (bỏ qua dòng level hiện tại)
        {
            string line = lines[i];

            if (i == 1) // Dòng thứ hai (index 1) chứa bottleCount và ballPerBottle
            {
                string[] lineSplits = line.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                bottleCount = int.Parse(lineSplits[0]);
                ballPerBottle = int.Parse(lineSplits[1]);
            }
            else
            {
                int[] convertArray = new int[ballPerBottle];
                for (int j = 0; j < convertArray.Length; j++)
                {
                    convertArray[j] = CharacterToInt(line[j]);
                }

                bottleArrays.Add(convertArray);
            }
        }

        game.LoadLevel(bottleArrays);
        OnLevelLoaded?.Invoke(currentLevelNumber);

    }


    public int GetBottleCount()
    {
        return bottleCount;
    }

    public int GetBallPerBottle()
    {
        return ballPerBottle;
    }
    public int GetCurrentLevel()
    {
        return currentLevelNumber;
    }


    private int CharacterToInt(char c)
    {
        switch (c)
        {
            default: return 0;
            case '0': return 0;
            case '1': return 1;
            case '2': return 2;
            case '3': return 3;
            case '4': return 4;
            case '5': return 5;
            case '6': return 6;
            case '7': return 7;
            case '8': return 8;
            case '9': return 9;
            case 'A': return 10;
            case 'B': return 11;
            case 'C': return 12;
            case 'D': return 13;
            case 'E': return 14;
            case 'F': return 15;
        }
    }
}
