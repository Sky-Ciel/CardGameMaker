using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardInfoPopUpScript : MonoBehaviour
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

    // カード情報を設定する
    public void SetCardInfo(Card cardData)
    {
        card = cardData;
        cardNameText.text = card.cardName;
        cardCostText.text = card.cost.ToString();

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
