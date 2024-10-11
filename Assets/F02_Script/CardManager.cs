using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class CardManager : MonoBehaviour
{
    public Sprite defaultCardImage; // デフォルト画像
    public List<Card> cardList = new List<Card>();

    [Header("------ カードパス入力 ------")]
    string path;
    public TMP_Text pathText;
    public bool isPath;
    public TMP_InputField inputField_path; 
    public string[] allowedExtensions = { ".txt", ".json" };

    [Header("------ イメージパス入力 ------")]
    public TMP_InputField inputField_img;  // フォルダパスを入力するフィールド
    public TMP_Text pathText_img;          // フォルダパスの表示用
    private string folderPath;
    public bool isIMGPath;

    void Start()
    {
        Load_saveCards();  // カードのロードを呼び出し
        
        path = PlayerPrefs.GetString("CardSET_Path",""); //パスのリセット
        folderPath = PlayerPrefs.GetString("ImageSET_Path",""); //パスのリセット
        inputField_path.text = path;
        inputField_img.text = folderPath;
    }

    void Update(){
        OnConfirmButtonClick();
        OnConfirmFolderClick();
    }
    
    void OnConfirmButtonClick()
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

    // フォルダ選択
    void OnConfirmFolderClick()
    {
        folderPath = inputField_img.text;  // 入力されたフォルダパスを取得

        if (!string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
        {
            // パスが正しい場合
            // ファイルパスを20文字以内に短縮して表示
            if (folderPath.Length > 20)
            {
                pathText_img.text = "フォルダ: ..." + folderPath.Substring(folderPath.Length - 20);
            }
            else
            {
                pathText_img.text = "フォルダ: " + folderPath;
            }
            isIMGPath = true;
        }
        else
        {
            // フォルダが存在しない場合の警告
            pathText_img.text = "指定されたフォルダが見つかりません。";
            isIMGPath = false;
        }
    }

    public void LoadCards()
    {
        cardList = new List<Card>();

        string fileContent = File.ReadAllText(path);
        ParseCardSettings(fileContent);
    }

    private void ParseCardSettings(string content)
    {
        try
        {
            CardSettingDataArray cardSettings = JsonConvert.DeserializeObject<CardSettingDataArray>(content);
            foreach (var cardSetting in cardSettings.cards)
            {
                Card newCard = new Card
                {
                    cardName = !string.IsNullOrEmpty(cardSetting.name) ? cardSetting.name : "Unknown Card",
                    Atk = cardSetting.atk != 0 ? cardSetting.atk : 0,
                    Def = cardSetting.def != 0 ? cardSetting.def : 0,
                    Rank = cardSetting.rank != 0 ? cardSetting.rank : 1,
                    Rarity = cardSetting.rarity != 0 ? cardSetting.rarity : 1,
                    Race = !string.IsNullOrEmpty(cardSetting.race) ? cardSetting.race : "Unknown Race",
                    element = cardSetting.element != null ? cardSetting.element : Element.Earth,
                    cardType = cardSetting.type != null ? cardSetting.type : CardType.Unit,
                    illustrationPath = !string.IsNullOrEmpty(cardSetting.illust) ? cardSetting.illust : "defaultPath",
                    cost = cardSetting.cost != 0 ? cardSetting.cost : 1,
                    effect = new CardEffect() // 初期値を空の CardEffect に
                };

                // 効果の読み込み
                if (cardSetting.effect != null)
                {
                    // Heal 効果が存在する場合
                    if (cardSetting.effect.Heal != null)
                    {
                        newCard.effect.healEffects = new List<HealEffect>
                        {
                            new HealEffect
                            {
                                heal = cardSetting.effect.Heal.heal,  // 回復量を設定
                                trigger = cardSetting.effect.Heal.trigger  // トリガーイベントを設定
                            }
                        };
                    }

                    // Draw 効果が存在する場合
                    if (cardSetting.effect.Draw != null)
                    {
                        //newCard.effect.drawEffects = new List<DrawEffect> { cardSetting.effect.draw };
                        newCard.effect.drawEffects = new List<DrawEffect>
                        {
                            new DrawEffect
                            {
                                draw = cardSetting.effect.Draw.draw,  // 回復量を設定
                                trigger = cardSetting.effect.Draw.trigger  // トリガーイベントを設定
                            }
                        };
                    }

                    // BuffAtk 効果が存在する場合
                    if (cardSetting.effect.buff_atk != null)
                    {
                        newCard.effect.buffAtkEffects = new List<BuffAtkEffect> { cardSetting.effect.buff_atk };
                    }

                    // Remove 効果が存在する場合
                    if (cardSetting.effect.remove != null)
                    {
                        newCard.effect.removeEffects = new List<RemoveEffect> { cardSetting.effect.remove };
                        newCard.effect.removeEffects[0].target_condition.location = cardSetting.effect.remove.target_condition.location;
                        newCard.effect.removeEffects[0].target_condition.type = cardSetting.effect.remove.target_condition.type;
                    }

                    // Temporary Buff 効果が存在する場合
                    if (cardSetting.effect.temporary_buff != null)
                    {
                        newCard.effect.temporaryBuffEffects = new List<TemporaryBuffEffect> { cardSetting.effect.temporary_buff };
                    }
                }

                string imagePath = Path.Combine(folderPath, cardSetting.illust);  // フォルダパスと画像名を結合
                if (File.Exists(imagePath))
                {
                    byte[] fileData = System.IO.File.ReadAllBytes(imagePath);  // 画像ファイルをバイト配列として読み込み
                    Texture2D texture = new Texture2D(2, 2);  // Texture2Dのインスタンスを作成
                    if (texture.LoadImage(fileData))  // 画像データをテクスチャに読み込む
                    {
                        newCard.illustration = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        newCard.illustrationPath = imagePath;
                        //Debug.Log($"Image({newCard.illustrationPath}) を保存しました！");
                    }
                    else
                    {
                        newCard.illustration = defaultCardImage;  // デフォルト画像を使用
                    }
                }
                else
                {
                    newCard.illustration = defaultCardImage;  // デフォルト画像を使用
                }

                //Debug.Log($"newCard.illustrationPath: ({newCard.illustrationPath})");
                
                // カードをリストに追加
                cardList.Add(newCard);
            }

            Debug.Log("Cards loaded successfully.");
            SaveCards();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error parsing card settings: {e.Message}");
        }
    }

    // カードデータを保存
    public void SaveCards()
    {
        /*foreach (var card in cardList)
        {
            // Sprite のパスを保存する
            if (card.illustration != null)
            {
                card.illustrationPath = card.illustration.name;  // Resources フォルダにある場合はファイル名
            }
        }*/

        string json = JsonConvert.SerializeObject(cardList);  // カードリストをJSONに変換
        PlayerPrefs.SetString("CardData", json);  // PlayerPrefs に保存
        PlayerPrefs.SetString("CardSET_Path",path);
        PlayerPrefs.SetString("ImageSET_Path",folderPath);
        PlayerPrefs.Save();
    }

    // カードデータを読み込む
    public void Load_saveCards()
    {
        if (PlayerPrefs.HasKey("CardData"))
        {
            string json = PlayerPrefs.GetString("CardData");
            cardList = JsonConvert.DeserializeObject<List<Card>>(json);

            foreach (var card in cardList)
            {
                // 画像パスから Sprite を再読み込み
                if (!string.IsNullOrEmpty(card.illustrationPath))
                {
                    card.illustration = Resources.Load<Sprite>("Image/" + card.illustrationPath);  // 画像が Resources フォルダにあると仮定
                }
            }

            Debug.Log("Cards loaded successfully");
        }
        else
        {
            Debug.LogWarning("No saved card data found.");
        }
    }
}

// カードの設定を保持するクラス
[System.Serializable]
public class CardSettingData
{
    public string name;  // カード名
    public int atk;  // 攻撃力
    public int def;  // 守備力
    public int rank;  // ランク
    public int rarity;  // レア度
    public string race;  // 種族
    public Element element;  // 属性
    public CardType type;  // カードの種類
    public string illust;  // イラストのパス
    public int cost;  // コスト
    public CardEffectData effect; // 効果を保持するクラス
}

// カードの設定データを持つ配列
[System.Serializable]
public class CardSettingDataArray
{
    public CardSettingData[] cards;  // カード設定のリスト
}

[System.Serializable]
public class CardEffectData
{
    public HealEffect Heal;  // Heal効果
    public DrawEffect Draw;  // Draw効果
    public BuffAtkEffect buff_atk;  // 攻撃力強化効果
    public RemoveEffect remove;  // 除外効果
    public TemporaryBuffEffect temporary_buff;  // 一時的強化効果
}
