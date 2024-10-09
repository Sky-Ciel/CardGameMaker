using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;

public class GameSettings : MonoBehaviour
{
    GS gs;

    public TMP_Text setting;

    void Start()
    {
        // gs が null かどうか確認し、null ならインスタンス化する
        if (gs == null)
        {
            gs = ScriptableObject.CreateInstance<GS>();
        }

        LoadGS();
    }

    void UpdateSettingText(){
        string ss = "";
        ss += $"初期ライフ: {gs.lifePoints}\n";
        ss += $"デッキ最低枚数: {gs.deckSize}\n";
        ss += $"同名カード: {gs.maxCopiesPerCard}枚まで\n";
        ss += $"<ターン進行>\n";
        foreach(var step in gs.turnProgress){
            ss += $"    -{step}\n";
        }
        ss += $"フィールドの上限枚数: {gs.fieldLimit}\n";
        var m = gs.noTargetSelection ? "不可" : "可";
        ss += $"攻撃時のターゲット選択: {m}\n";
        m = gs.freeSummon ? "なし" : "あり";
        ss += $"コスト制: {m}\n";
        m = gs.NoRace ? "なし" : "あり";
        ss += $"種族の区別: {m}\n";
    
        setting.text = ss;
    }

    // GameSettings を保存する関数
    public void SaveGS()
    {
        if (gs != null)
        {
            PlayerPrefsUtility.SaveScriptableObject("GameSetting", gs);
            PlayerPrefs.Save();
            Debug.Log("Game settings saved.");
        }
        else
        {
            Debug.LogError("GameSetting (gs) is null. Cannot save settings.");
        }
        UpdateSettingText();
    }

    // GameSettings をロードする関数
    public void LoadGS()
    {
        gs = PlayerPrefsUtility.LoadScriptableObject<GS>("GameSetting");
        
        // インスタンスがロードされなかった場合、初期化する
        if (gs == null)
        {
            gs = ScriptableObject.CreateInstance<GS>();
            Debug.Log("Game settings initialized as new.");
        }
        else
        {
            Debug.Log("Game settings loaded from PlayerPrefs.");
        }
        UpdateSettingText();
    }

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
                    gs.lifePoints = int.Parse(lifeString);
                }
                else if (line.Contains("miDeck"))
                {
                    string deckString = line.Split(':')[1].Trim();
                    gs.deckSize = int.Parse(deckString);
                }
                else if (line.Contains("TurnProgress"))
                {
                    gs.turnProgress = line.Split(':')[1].Trim(new char[] { '[', ']', ' ' }).Split(',');
                }
                else if (line.Contains("FieldLimit"))
                {
                    string fieldLimitString = line.Split(':')[1].Trim();
                    gs.fieldLimit = int.Parse(fieldLimitString);
                }
                else if (line.Contains("NoTargetSelection"))
                {
                    gs.noTargetSelection = line.Contains("T");
                }
                else if (line.Contains("FreeSummon"))
                {
                    gs.freeSummon = line.Contains("T");
                }
                else if (line.Contains("maxCopiesPerCard"))
                {
                    gs.maxCopiesPerCard = int.Parse(line.Split(':')[1].Trim());
                }
                else if (line.Contains("NoRace"))
                {
                    gs.NoRace = line.Contains("T");
                }
            }
            catch (FormatException e)
            {
                Debug.LogError($"Failed to parse line: {line}. Error: {e.Message}");
            }
        }

        Debug.Log("Game settings loaded successfully.");
        SaveGS();
    }
}
