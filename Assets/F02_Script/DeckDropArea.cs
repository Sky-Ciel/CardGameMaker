using UnityEngine;
using UnityEngine.EventSystems;

public class DeckDropArea : MonoBehaviour, IDropHandler
{
    public Transform deckContent;  // デッキエリアのコンテナ
    public GameObject cardPrefab;  // デッキに追加するカードのプレハブ

    // ドロップされた時の処理
    public void OnDrop(PointerEventData eventData)
    {
        // ドラッグしているカードオブジェクトを取得
        GameObject droppedCard = eventData.pointerDrag;
        if (droppedCard != null)
        {
            CardPrefabScript cardScript = droppedCard.GetComponent<CardPrefabScript>();

            if (cardScript != null)
            {
                // デッキに追加するための新しいカードオブジェクトを生成
                GameObject newCard = Instantiate(cardPrefab, deckContent);
                
                // 新しいカードの情報をセット
                CardPrefabScript newCardScript = newCard.GetComponent<CardPrefabScript>();
                newCardScript.SetCardInfo(cardScript.card);

                // ログやデバッグ用にカードの追加を表示
                Debug.Log("Card added to deck: " + cardScript.card.cardName);
            }
        }
    }
}
