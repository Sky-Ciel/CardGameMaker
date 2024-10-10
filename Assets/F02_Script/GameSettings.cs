using System;
using System.IO;
using System.Collections.Generic; 
using UnityEngine;
using UnityEditor;
using TMPro;

public class GameSettings : MonoBehaviour
{
    GS gs;

    public TMP_Text setting;

    [Header("------ パス入力 ------")]
    string path;
    public TMP_Text pathText;
    public bool isPath;
    public TMP_InputField inputField_path; 
    public string[] allowedExtensions = { ".txt", ".json" };

    void Start()
    {
        // gs が null かどうか確認し、null ならインスタンス化する
        if (gs == null)
        {
            gs = ScriptableObject.CreateInstance<GS>();
        }

        LoadGS();
        path = ""; //パスのリセット
    }
    void Update(){
        OnConfirmButtonClick();
    }
    
    public void OnConfirmButtonClick()
    {
        // InputFieldに入力されたパスを取得
        path = inputField_path.text;

        // ファイルパスが空でないかチェック
        if (!string.IsNullOrEmpty(path))
        {
            // 入力されたパスの拡張子を取得
            string fileExtension = System.IO.Path.GetExtension(path).ToLower();

            // 許可された拡張子かどうかを確認
            if (IsExtensionAllowed(fileExtension))
            {
                // ファイルパスを20文字以内に短縮して表示
                if (path.Length > 20)
                {
                    pathText.text = "ファイル: ..." + path.Substring(path.Length - 20);
                }
                else
                {
                    pathText.text = "ファイル: " + path;
                }
                isPath = true;
            }
            else
            {
                // 拡張子が一致しない場合の警告を表示
                pathText.text = "ファイル形式が一致しません。";
                isPath = false;
            }
        }
        else
        {
            // パスが入力されていない場合の警告
            pathText.text = "ファイルパスを入力してください。";
            isPath = false;
        }
    }

    // 許可された拡張子かどうかをチェックする関数
    private bool IsExtensionAllowed(string extension)
    {
        foreach (string allowedExtension in allowedExtensions)
        {
            if (extension == allowedExtension)
            {
                return true;
            }
        }
        return false;
    }

    void UpdateSettingText()
    {
        string ss = "";
        ss += $"初期ライフ: {gs.lifePoints}\n";
        ss += $"デッキ最低枚数: {gs.deckSize}\n";
        ss += $"同名カード: {gs.maxCopiesPerCard}枚まで\n";
        ss += $"<ターン進行>\n";
        foreach(var step in gs.turnProgress)
        {
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
        string fileContent = File.ReadAllText(path);
        ParseGameSettings(fileContent);

        /*
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
        */
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
                string[] parts = line.Split(':');
                if (parts.Length != 2) continue;

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                switch (key)
                {
                    case "inLife":
                        gs.lifePoints = int.Parse(value);
                        break;
                    case "miDeck":
                        gs.deckSize = int.Parse(value);
                        break;
                    case "TurnProgress":
                        ParseTurnProgress(value);
                        break;
                    case "FieldLimit":
                        gs.fieldLimit = int.Parse(value);
                        break;
                    case "NoTargetSelection":
                        gs.noTargetSelection = value.Contains("T");
                        break;
                    case "FreeSummon":
                        gs.freeSummon = value.Contains("T");
                        break;
                    case "maxCopiesPerCard":
                        gs.maxCopiesPerCard = int.Parse(value);
                        break;
                    case "NoRace":
                        gs.NoRace = value.Contains("T");
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse line: {line}. Error: {e.Message}");
            }
        }

        Debug.Log("Game settings loaded successfully.");
        SaveGS();
    }

    private void ParseTurnProgress(string value)
    {
        string[] turn = value.Trim(new char[] { '[', ']', ' ' }).Split(',');
        gs.turnProgress = new List<TurnProgress>();
        foreach (var t in turn)
        {
            if (Enum.TryParse(t.Trim(), out TurnProgress progress))
            {
                gs.turnProgress.Add(progress);  // Add メソッドを使用
            }
            else
            {
                Debug.LogWarning($"Invalid TurnProgress value: {t}");
            }
        }
    }
}