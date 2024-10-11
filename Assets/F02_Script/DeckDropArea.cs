using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;

public class DeckDropArea : MonoBehaviour, IDropHandler
{
    public Transform deckContent;  // デッキエリアのコンテナ
    public GameObject deckCardPrefab;  // デッキ用のカードプレハブ
    public int maxCopiesPerCard;  // 同じカードをデッキに何枚入れられるか（GameSettings から取得）
    GS gs;  // ゲーム設定
    public GameObject alertPrefab;  // アラートを表示するためのプレハブ
    public Transform alertContainer;  // アラート表示場所（Canvas内）

    private List<Card> deckCards = new List<Card>();  // デッキ内のカードリスト
    Deck deck;

    DeckDropArea thisScript;

    public DeckListDisplay deckListDisplay;  // デッキリスト表示
    public CostDistributionGraph costDistributionGraph;  // コスト分布グラフ表示

    void Start()
    {
        thisScript = GetComponent<DeckDropArea>();

        if (gs == null)
        {
            gs = ScriptableObject.CreateInstance<GS>();
        }
        gs = PlayerPrefsUtility.LoadScriptableObject<GS>("GameSetting");

        maxCopiesPerCard = gs.maxCopiesPerCard;  // GameSettings から maxCopiesPerCard を取得

        UpdateLists();
    }

    void UpdateLists(){
        // デッキリストとコスト分布グラフを更新
        deckListDisplay.UpdateDeckList(deckCards);
        costDistributionGraph.UpdateCostDistribution(deckCards);
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedCard = eventData.pointerDrag;
        if (droppedCard != null)
        {
            CardPrefabScript cardScript = droppedCard.GetComponent<CardPrefabScript>();

            if (cardScript != null)
            {
                int cardCount = CountCardInDeck(cardScript.card);
                if (cardCount >= maxCopiesPerCard)
                {
                    ShowAlert();
                    return;
                }

                // デッキに追加
                GameObject newCard = Instantiate(deckCardPrefab, deckContent);
                DeckCardPrefabScript newCardScript = newCard.GetComponent<DeckCardPrefabScript>();
                newCardScript.SetCardInfo(cardScript.card, this);  // デッキエリアを参照に追加
                deckCards.Add(cardScript.card);

                SortDeck();  // デッキ内のカードをソート

                UpdateLists();
            }
        }
    }

    // デッキからカードを削除するメソッド
    public void RemoveCardFromDeck(Card card)
    {
        deckCards.Remove(card);  // デッキリストからカードを削除
        Debug.Log("Card removed from deck: " + card.cardName);

        UpdateLists();
    }

    private int CountCardInDeck(Card card)
    {
        int count = 0;
        foreach (var deckCard in deckCards)
        {
            if (deckCard.cardName == card.cardName)
            {
                count++;
            }
        }
        return count;
    }

    // アラートをフェードイン＆アウトさせる
    private void ShowAlert()
    {
        GameObject alert = Instantiate(alertPrefab, alertContainer);

        // フェードイン＆アウトのアニメーションを追加 (DOTweenなどを使用)
        CanvasGroup canvasGroup = alert.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.5f).OnComplete(() =>
        {
            // 2秒後にフェードアウトして消える
            canvasGroup.DOFade(0, 0.5f).SetDelay(1f).OnComplete(() =>
            {
                Destroy(alert);
            });
        });
    }

    // デッキ内のカードを自動でソートする
    private void SortDeck()
    {
        // カードをユニット、マジック、トラップの順でソート
        deckCards.Sort((card1, card2) =>
        {
            if (card1.cardType == card2.cardType)
            {
                return string.Compare(card1.cardName, card2.cardName, StringComparison.Ordinal);
            }
            else if (card1.cardType == CardType.Unit)
            {
                return -1;
            }
            else if (card1.cardType == CardType.Magic && card2.cardType == CardType.Trap)
            {
                return -1;
            }
            return 1;
        });

        // ソート後にデッキ内の表示を更新
        for (int i = 0; i < deckContent.childCount; i++)
        {
            Destroy(deckContent.GetChild(i).gameObject);  // 既存のカードオブジェクトを削除
        }

        foreach (var card in deckCards)
        {
            GameObject newCard = Instantiate(deckCardPrefab, deckContent);
            DeckCardPrefabScript newCardScript = newCard.GetComponent<DeckCardPrefabScript>();
            newCardScript.SetCardInfo(card, thisScript);
        }
    }
}
