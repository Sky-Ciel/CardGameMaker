using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class DeckListDisplay : MonoBehaviour
{
    public Transform listParent;  // 全カードリストのコンテンツ部分
    public GameObject cardEntryPrefab; // カード表示用のプレハブ
    public GameObject sectionHeaderPrefab; // 見出しのプレハブ（ユニット、マジック、トラップ）
    public TMP_Text unitCountText;  // ユニット枚数表示
    public TMP_Text magicCountText; // マジック枚数表示
    public TMP_Text trapCountText;  // トラップ枚数表示
    public TMP_Text sumCountText;  // 合計枚数表示

    private List<Card> currentDeck = new List<Card>();

    // デッキリストを更新するメソッド
    public void UpdateDeckList(List<Card> deck)
    {
        currentDeck = deck;

        // リストをクリア
        foreach (Transform child in listParent)
        {
            Destroy(child.gameObject);
        }

        int unitCount = 0, magicCount = 0, trapCount = 0;

        // カードを種類ごとにグループ化して表示（カード名 x枚数）
        var unitCards = deck.Where(card => card.cardType == CardType.Unit).GroupBy(card => card.cardName);
        var magicCards = deck.Where(card => card.cardType == CardType.Magic).GroupBy(card => card.cardName);
        var trapCards = deck.Where(card => card.cardType == CardType.Trap).GroupBy(card => card.cardName);

        // ユニット見出しを追加
        if(unitCards.Count()>0){
            GameObject unitHeader = Instantiate(sectionHeaderPrefab, listParent);
            TMP_Text unitHeaderText = unitHeader.GetComponent<TMP_Text>();
            unitHeaderText.text = "--- ユニット ---";
        }

        // ユニットカードを表示
        foreach (var group in unitCards)
        {
            GameObject cardEntry = Instantiate(cardEntryPrefab, listParent);
            TMP_Text cardText = cardEntry.GetComponent<TMP_Text>();
            cardText.text = $"{group.Key} x{group.Count()}";  // カード名 x枚数
            unitCount += group.Count();
        }

        // マジック見出しを追加
        if(magicCards.Count()>0){
            GameObject magicHeader = Instantiate(sectionHeaderPrefab, listParent);
            TMP_Text magicHeaderText = magicHeader.GetComponent<TMP_Text>();
            magicHeaderText.text = "--- マジック ---";
        }

        // マジックカードを表示
        foreach (var group in magicCards)
        {
            GameObject cardEntry = Instantiate(cardEntryPrefab, listParent);
            TMP_Text cardText = cardEntry.GetComponent<TMP_Text>();
            cardText.text = $"{group.Key} x{group.Count()}";  // カード名 x枚数
            magicCount += group.Count();
        }

        // トラップ見出しを追加
        if(trapCards.Count()>0){
            GameObject trapHeader = Instantiate(sectionHeaderPrefab, listParent);
            TMP_Text trapHeaderText = trapHeader.GetComponent<TMP_Text>();
            trapHeaderText.text = "--- トラップ ---";
        }

        // トラップカードを表示
        foreach (var group in trapCards)
        {
            GameObject cardEntry = Instantiate(cardEntryPrefab, listParent);
            TMP_Text cardText = cardEntry.GetComponent<TMP_Text>();
            cardText.text = $"{group.Key} x{group.Count()}";  // カード名 x枚数
            trapCount += group.Count();
        }

        // 枚数を更新
        unitCountText.text = $"ユニット: {unitCount}";
        magicCountText.text = $"マジック: {magicCount}";
        trapCountText.text = $"トラップ: {trapCount}";
        sumCountText.text = $"合計: {unitCount+magicCount+trapCount}";
    }
}
