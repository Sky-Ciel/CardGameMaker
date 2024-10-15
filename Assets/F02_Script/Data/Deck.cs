using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDeck", menuName = "Deck")]
public class Deck : ScriptableObject
{
    public string deckName;  // デッキ名
    public List<Card> deckCards;  // カードリスト
    public Card KeyCard; // キーカード
}
