using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public Card[] cardArray; // ScriptableObject の配列で管理
    public Sprite defaultCardImage; // デフォルト画像

    public void LoadCards()
    {
        #if UNITY_EDITOR
        TextAsset cardSettingText = Resources.Load<TextAsset>("CardSetting");
        if (cardSettingText != null)
        {
            ParseCardSettings(cardSettingText.text);
        }
        else
        {
            Debug.LogError("CardSetting.txt not found in Resources.");
        }
        #else
        string filePath = Path.Combine(Application.dataPath, "../CardSetting.txt");
        if (File.Exists(filePath))
        {
            string fileContent = File.ReadAllText(filePath);
            ParseCardSettings(fileContent);
        }
        else
        {
            Debug.LogError("CardSetting.txt not found in application folder.");
        }
        #endif
    }

    private void ParseCardSettings(string content)
    {
        try
        {
            CardDataArray cardDataArray = JsonConvert.DeserializeObject<CardDataArray>(content);

            cardArray = new Card[cardDataArray.cards.Length];

            for (int i = 0; i < cardDataArray.cards.Length; i++)
            {
                CardData data = cardDataArray.cards[i];
                Card card = ScriptableObject.CreateInstance<Card>();

                card.cardName = data.name;
                card.cardType = (CardType)System.Enum.Parse(typeof(CardType), data.type, true); // 列挙型にパース
                card.cost = data.cost;

                // 効果のパース
                card.effect = new CardEffect();
                
                // Heal 効果が存在する場合
                if (data.effect.heal != null)
                {
                    card.effect.healEffects = new List<HealEffect> { data.effect.heal };
                }

                // Draw 効果が存在する場合
                if (data.effect.draw != null)
                {
                    card.effect.drawEffects = new List<DrawEffect> { data.effect.draw };
                }

                // BuffAtk 効果が存在する場合
                if (data.effect.buff_atk != null)
                {
                    card.effect.buffAtkEffects = new List<BuffAtkEffect> { data.effect.buff_atk };
                }

                // Remove 効果が存在する場合
                if (data.effect.remove != null)
                {
                    card.effect.removeEffects = new List<RemoveEffect> { data.effect.remove };

                    // Remove 効果内の TargetCondition のパース
                    card.effect.removeEffects[0].target_condition.location = 
                        (LocationType)System.Enum.Parse(typeof(LocationType), data.effect.remove.target_condition.location, true);

                    card.effect.removeEffects[0].target_condition.type = 
                        (CardType)System.Enum.Parse(typeof(CardType), data.effect.remove.target_condition.type, true);
                }

                // Temporary Buff 効果が存在する場合
                if (data.effect.temporary_buff != null)
                {
                    card.effect.temporaryBuffEffects = new List<TemporaryBuffEffect> { data.effect.temporary_buff };
                }

                // 画像の読み込み
                #if UNITY_EDITOR
                card.illustration = Resources.Load<Sprite>("Image/" + data.illust);
                if (card.illustration == null)
                {
                    // 画像が存在しない場合はデフォルト画像を使用
                    card.illustration = defaultCardImage;
                }
                #else
                string imagePath = Path.Combine(Application.dataPath, "../Image/", data.illust);
                if (File.Exists(imagePath))
                {
                    byte[] imageBytes = File.ReadAllBytes(imagePath);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageBytes);
                    card.illustration = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
                else
                {
                    // 画像が存在しない場合はデフォルト画像を使用
                    card.illustration = defaultCardImage;
                }
                #endif

                // カードを配列に追加
                cardArray[i] = card;
            }

            Debug.Log("Cards loaded successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error parsing card settings: {e.Message}");
        }
    }
}

[System.Serializable]
public class CardData
{
    public string name;
    public string type;
    public string illust;
    public int cost;
    public CardEffectData effect; // 効果を保持するクラス
}

[System.Serializable]
public class CardDataArray
{
    public CardData[] cards;
}

[System.Serializable]
public class CardEffectData
{
    public HealEffect heal;
    public DrawEffect draw;
    public BuffAtkEffect buff_atk;
    public RemoveEffect remove;
    public TemporaryBuffEffect temporary_buff;
}
