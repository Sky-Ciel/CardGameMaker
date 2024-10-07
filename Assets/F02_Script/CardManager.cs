using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public Sprite defaultCardImage; // デフォルト画像
    public List<Card> cardList = new List<Card>();

    void Start()
    {
        LoadCards();  // カードのロードを呼び出し
    }

    public void LoadCards()
    {
        cardList = new List<Card>();

        #if UNITY_EDITOR
        TextAsset cardSettingText = Resources.Load<TextAsset>("CardSetting");
        if (cardSettingText != null)
        {
            ParseCardSettings(cardSettingText.text);
        }
        else
        {
            Debug.LogError("CardSetting.json not found in Resources.");
        }
        #else
        string filePath = Path.Combine(Application.dataPath, "../CardSetting.json");
        if (File.Exists(filePath))
        {
            string fileContent = File.ReadAllText(filePath);
            ParseCardSettings(fileContent);
        }
        else
        {
            Debug.LogError("CardSetting.json not found in application folder.");
        }
        #endif
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

                // 画像の読み込み
                #if UNITY_EDITOR
                newCard.illustration = Resources.Load<Sprite>("Image/" + cardSetting.illust);
                if (newCard.illustration == null)
                {
                    newCard.illustration = defaultCardImage;  // デフォルト画像を使用
                }
                #else
                string imagePath = Path.Combine(Application.dataPath, "../Image/", cardSetting.illust);
                if (File.Exists(imagePath))
                {
                    byte[] imageBytes = File.ReadAllBytes(imagePath);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageBytes);
                    newCard.illustration = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
                else
                {
                    newCard.illustration = defaultCardImage;  // デフォルト画像を使用
                }
                #endif

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
        foreach (var card in cardList)
        {
            // Sprite のパスを保存する
            if (card.illustration != null)
            {
                card.illustrationPath = card.illustration.name;  // Resources フォルダにある場合はファイル名
            }
        }

        string json = JsonConvert.SerializeObject(cardList);  // カードリストをJSONに変換
        PlayerPrefs.SetString("CardData", json);  // PlayerPrefs に保存
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
