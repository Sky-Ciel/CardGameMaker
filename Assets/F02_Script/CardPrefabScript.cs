using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CardPrefabScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Card card;  // このプレハブが保持するカード情報
    public Image cardImage;  // カードの画像
    public TextMeshProUGUI cardNameText;  // カード名表示用テキスト

    private Transform parentBeforeDrag;  // ドラッグ前の親オブジェクト
    private CanvasGroup canvasGroup;
    private GameObject draggingCard;  // ドラッグ時のコピーオブジェクト

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // カード情報を設定する
    public void SetCardInfo(Card cardData)
    {
        card = cardData;
        cardNameText.text = card.cardName;
        cardImage.sprite = card.illustration;
    }

    // ドラッグを開始したとき
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentBeforeDrag = transform.parent;
        
        // ドラッグ用のコピーを作成
        draggingCard = Instantiate(gameObject, transform.root);
        draggingCard.GetComponent<CanvasGroup>().blocksRaycasts = false;  // Raycast を無効化

        // 見た目を少し変更してわかりやすくする（オプション）
        draggingCard.GetComponent<Image>().color = new Color(1, 1, 1, 0.7f);
    }

    // ドラッグ中の処理
    public void OnDrag(PointerEventData eventData)
    {
        // ドラッグ中のカードの位置をマウスに追従
        draggingCard.transform.position = Input.mousePosition;
    }

    // ドラッグ終了時
    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(draggingCard);  // ドラッグ用のコピーを削除
    }
}
