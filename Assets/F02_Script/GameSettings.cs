using System;
using System.IO;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public int lifePoints;
    public int deckSize;
    public string[] turnProgress;
    public int fieldLimit;
    public bool noTargetSelection;
    public bool freeSummon;
    public int maxCopiesPerCard; // 同名カードの上限枚数

    public void LoadGameSettings()
    {
        #if UNITY_EDITOR
        TextAsset gameSettingText = Resources.Load<TextAsset>("GameSetting");
        if (gameSettingText != null)
        {
            ParseGameSettings(gameSettingText.text);
        }
        else
        {
            Debug.LogError("GameSetting.txt not found in Resources.");
        }
        #else
        string filePath = Path.Combine(Application.dataPath, "../GameSetting.txt");
        if (File.Exists(filePath))
        {
            string fileContent = File.ReadAllText(filePath);
            ParseGameSettings(fileContent);
        }
        else
        {
            Debug.LogError("GameSetting.txt not found in application folder.");
        }
        #endif
    }

    // 設定をパースして変数に保存するメソッド
    private void ParseGameSettings(string content)
    {
        string[] lines = content.Split('\n');
        foreach (string line in lines)
        {
            // コメントアウト行 (###) を無視
            if (line.Trim().StartsWith("###"))
            {
                continue; // コメントアウト行をスキップ
            }
            
            try
            {
                if (line.Contains("inLife"))
                {
                    string lifeString = line.Split(':')[1].Trim();
                    lifePoints = int.Parse(lifeString);
                }
                else if (line.Contains("miDeck"))
                {
                    string deckString = line.Split(':')[1].Trim();
                    deckSize = int.Parse(deckString);
                }
                else if (line.Contains("TurnProgress"))
                {
                    turnProgress = line.Split(':')[1].Trim(new char[] { '[', ']', ' ' }).Split(',');
                }
                else if (line.Contains("FieldLimit"))
                {
                    string fieldLimitString = line.Split(':')[1].Trim();
                    fieldLimit = int.Parse(fieldLimitString);
                }
                else if (line.Contains("NoTargetSelection"))
                {
                    noTargetSelection = line.Contains("T");
                }
                else if (line.Contains("FreeSummon"))
                {
                    freeSummon = line.Contains("T");
                }
                else if (line.Contains("maxCopiesPerCard"))
                {
                    maxCopiesPerCard = int.Parse(line.Split(':')[1].Trim());
                }
            }
            catch (FormatException e)
            {
                Debug.LogError($"Failed to parse line: {line}. Error: {e.Message}");
            }
        }

        Debug.Log("Game settings loaded successfully.");
    }
}
