using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DeckCardPrefabScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("------ カード本体のパーツ ------")]
    public Card card;  // このプレハブが保持するカード情報
    public Image cardImage;  // カードの画像
    public TextMeshProUGUI cardNameText;  // カード名表示用テキスト
    public TextMeshProUGUI cardTextText;  // 効果表示用テキスト
    public TextMeshProUGUI cardCostText;  // コスト表示用テキスト
    public TextMeshProUGUI cardRaceText;  // 種族表示用テキスト
    public TextMeshProUGUI cardStatesText;  // 攻守表示用テキスト

    private CanvasGroup canvasGroup;
    private Vector3 startPosition;
    private Transform parentBeforeDrag;
    public DeckDropArea deckDropArea;  // デッキに対する参照を保持

    GS gs; //ゲーム設定

    [Header("------ フレーム情報 ------")]
    public Image flameImage;  // カードの画像
    public Sprite[] flames;
    public Sprite flames_magi; //魔法カードのフレーム
    public Sprite flames_trap; //罠カードのフレーム

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // カード情報を設定する
    public void SetCardInfo(Card cardData, DeckDropArea deckArea)
    {
        deckDropArea = deckArea;  // デッキへの参照を保存

        card = cardData;
        cardNameText.text = card.cardName;
        cardCostText.text = card.cost.ToString();

        cardImage.sprite = card.illustration;

        cardRaceText.text = "";
        cardStatesText.text = "";
        
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

    // ドラッグを開始したとき
    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        parentBeforeDrag = transform.parent;
        transform.SetParent(transform.root);  // 親を一時的に変更
        canvasGroup.blocksRaycasts = false;   // Raycast を無効化
    }

    // ドラッグ中の処理
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;  // マウスに追従
    }

    // ドラッグ終了時
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(parentBeforeDrag.GetComponent<RectTransform>(), Input.mousePosition))
        {
            // デッキ外にドロップされた場合、デッキリストからカードを削除
            deckDropArea.RemoveCardFromDeck(card);
            Destroy(gameObject);  // ゲームオブジェクト自体も削除
        }
        else
        {
            transform.SetParent(parentBeforeDrag);  // 元の位置に戻す
            transform.position = startPosition;
        }
        canvasGroup.blocksRaycasts = true;  // Raycast を再有効化
    }
}
