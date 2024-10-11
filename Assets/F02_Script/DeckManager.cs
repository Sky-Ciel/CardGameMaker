using UnityEngine;
using System.Collections.Generic;

namespace DeckManager{

    public class DeckManager : MonoBehaviour
    {
        public List<Deck> allDecks = new List<Deck>();  // すべてのデッキリスト

        // 新しいデッキを追加するメソッド
        public void AddNewDeck(string deckName, List<Card> cards)
        {
            Deck newDeck = ScriptableObject.CreateInstance<Deck>();
            newDeck.deckName = deckName;
            newDeck.deckCards = new List<Card>(cards);  // カードリストをコピー
            allDecks.Add(newDeck);
        }

        // 何番目のデッキかを指定してデッキを取得する
        public Deck GetDeckByIndex(int index)
        {
            if (index >= 0 && index < allDecks.Count)
            {
                return allDecks[index];
            }
            return null;  // 範囲外ならnullを返す
        }

        // デッキを保存する（ScriptableObjectとしてアセット化する）
        public void SaveDecks()
        {
            for (int i = 0; i < allDecks.Count; i++)
            {
                string path = $"Assets/Decks/Deck_{i}.asset";
                UnityEditor.AssetDatabase.CreateAsset(allDecks[i], path);
            }
            UnityEditor.AssetDatabase.SaveAssets();
        }

        // デッキを読み込む
        public void LoadDecks()
        {
            allDecks.Clear();
            int index = 0;
            Deck loadedDeck;
            
            while ((loadedDeck = UnityEditor.AssetDatabase.LoadAssetAtPath<Deck>($"Assets/Decks/Deck_{index}.asset")) != null)
            {
                allDecks.Add(loadedDeck);
                index++;
            }
        }
    }

}