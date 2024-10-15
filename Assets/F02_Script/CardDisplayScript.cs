using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class CardDisplayScript : MonoBehaviour
{
    [Header("------ カード本体のパーツ ------")]
    public Card card;  // このプレハブが保持するカード情報
    public Image cardImage;  // カードの画像
    public TextMeshProUGUI cardNameText;  // カード名表示用テキスト
    public TextMeshProUGUI cardTextText;  // 効果表示用テキスト
    public TextMeshProUGUI cardCostText;  // コスト表示用テキスト
    public TextMeshProUGUI cardRaceText;  // 種族表示用テキスト
    public TextMeshProUGUI cardStatesText;  // 攻守表示用テキスト

    [Header("------ フレーム情報 ------")]
    public Image flameImage;  // カードの画像
    public Sprite[] flames;
    public Sprite flames_magi; //魔法カードのフレーム
    public Sprite flames_trap; //罠カードのフレーム

    public Sprite defaultCardImage;

    void Update()
    {
        SetCardInfo(card);
    }

    // カード情報を設定する
    public void SetCardInfo(Card cardData)
    {
        card = cardData;
        cardNameText.text = card.cardName;
        cardCostText.text = card.cost.ToString();

        if (File.Exists(card.illustrationPath))
        {
            byte[] fileData = System.IO.File.ReadAllBytes(card.illustrationPath);  // 画像ファイルをバイト配列として読み込み
            Debug.Log($"card.illustrationPath: {card.illustrationPath}を読み込み");

            Texture2D texture = new Texture2D(2, 2);  // Texture2Dのインスタンスを作成
            if (texture.LoadImage(fileData))  // 画像データをテクスチャに読み込む
            {
                card.illustration = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                card.illustration = defaultCardImage;  // デフォルト画像を使用
            }
        }
        else
        {
            card.illustration = defaultCardImage;  // デフォルト画像を使用
        }

        cardImage.sprite = card.illustration;

        cardRaceText.text = "";
        cardStatesText.text = "";

        cardTextText.text = card.GenerateEffectText();
        
        // カードの種類による分岐
        if(card.cardType == CardType.Magic){
            flameImage.sprite = flames_magi;
        }else if(card.cardType == CardType.Trap){
            flameImage.sprite = flames_trap;
        }else{
            cardStatesText.text = "ATK: " +card.Atk.ToString() + " / " + "DEF: " +card.Def.ToString();

            if(card.Race != "" && card.Race != null){
                cardRaceText.text = card.Race;
            }else{
                cardRaceText.text = card.cardType.ToString();
            }
            
            switch(card.element){
                case Element.Fire:
                    flameImage.sprite = flames[0];
                    break;
                case Element.Water:
                    flameImage.sprite = flames[1];
                    break;
                case Element.Earth:
                    flameImage.sprite = flames[2];
                    break;
                case Element.Air:
                    flameImage.sprite = flames[3];
                    break;
                case Element.Light:
                    flameImage.sprite = flames[4];
                    break;
                case Element.Dark:
                    flameImage.sprite = flames[5];
                    break;
                default:
                    flameImage.sprite = flames[0];
                    break;
            }
        }
    }
}
