using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public static class PlayerPrefsUtility
{
    // リストを保存
    public static void SaveList<T>(string key, List<T> list)
    {
        string json = JsonConvert.SerializeObject(list);  // リストを JSON に変換
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    // リストを読み込む
    public static List<T> LoadList<T>(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            return JsonConvert.DeserializeObject<List<T>>(json);  // JSON からリストにデシリアライズ
        }
        return new List<T>();  // データが存在しない場合は空のリストを返す
    }

    // ScriptableObject を保存 (IDなどを保存して再生成)
    public static void SaveScriptableObject<T>(string key, T scriptableObject) where T : ScriptableObject
    {
        string json = JsonUtility.ToJson(scriptableObject);  // ScriptableObject を JSON に変換
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    // ScriptableObject を読み込む
    public static T LoadScriptableObject<T>(string key) where T : ScriptableObject
    {
        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            T scriptableObject = ScriptableObject.CreateInstance<T>();  // 新しいインスタンスを作成
            JsonUtility.FromJsonOverwrite(json, scriptableObject);  // JSON からデータを上書き
            return scriptableObject;
        }
        return null;
    }
}
