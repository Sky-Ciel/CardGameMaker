using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.IO;

public class DeckCardPrefabScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("------ カード本体のパーツ ------")]
    public Card card;  // このプレハブが保持するカード情報
    public Image cardImage;  // カードの画像
    public TextMeshProUGUI cardNameText;  // カード名表示用テキスト
    public TextMeshProUGUI cardTextText;  // 効果表示用テキスト
    public TextMeshProUGUI cardCostText;  // コスト表示用テキスト
    public TextMeshProUGUI cardRaceText;  // 種族表示用テキスト
    public TextMeshProUGUI cardStatesText;  // 攻守表示用テキスト

    public Sprite defaultCardImage;

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

    [Header("------ ポップアップ情報ウィンドウ ------")]
    public GameObject popupPrefab;  // ポップアップのプレハブ
    private GameObject currentPopup;  // 現在表示中のポップアップ
    [Header("------ アニメーションセッティング ------")]
    [SerializeField] private float popupAnimationDuration = 0.5f;
    [SerializeField] private Ease popupEaseType = Ease.OutBack;
    [Header("------ フェード ------")]
    public GameObject fade;
    private GameObject currentFade;

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

        if (File.Exists(card.illustrationPath)){
            byte[] fileData = System.IO.File.ReadAllBytes(card.illustrationPath);  // 画像ファイルをバイト配列として読み込み
            Texture2D texture = new Texture2D(2, 2);
            if(texture.LoadImage(fileData)){
                card.illustration = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                cardImage.sprite = card.illustration;
            }else{
                cardImage.sprite = defaultCardImage;
            }
        }else{
            cardImage.sprite = defaultCardImage;
        }

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

    // ------------------ ポップアップウィンドウ ------------------
    public void OnPointerClick(PointerEventData eventData)
    {
        OpenPopup();
    }

    private void OpenPopup()
    {
        Canvas mainCanvas = FindObjectOfType<Canvas>();

        currentFade = Instantiate(fade);
        currentFade.transform.SetParent(mainCanvas.transform, false); // Canvasに親を設定
        currentFade.transform.localPosition = Vector3.zero;
        currentFade.SetActive(true);

        // Create the popup
        currentPopup = Instantiate(popupPrefab);
        currentPopup.transform.SetParent(mainCanvas.transform, false); // Canvasに親を設定
        currentPopup.transform.localPosition = Vector3.zero;
        currentPopup.SetActive(true);
        
        // Set the popup's initial state
        RectTransform popupRect = currentPopup.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = currentPopup.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = currentPopup.AddComponent<CanvasGroup>();

        CanvasGroup canvasGroup_f = currentFade.GetComponent<CanvasGroup>();
        if (canvasGroup_f == null) canvasGroup_f = currentFade.AddComponent<CanvasGroup>();

        // Set initial values for animation
        canvasGroup.alpha = 0f;
        popupRect.localScale = Vector3.zero;

        canvasGroup_f.alpha = 0f;

        // Animate the popup
        canvasGroup_f.DOFade(1f, popupAnimationDuration);
        canvasGroup.DOFade(1f, popupAnimationDuration);
        popupRect.DOScale(Vector3.one, popupAnimationDuration).SetEase(popupEaseType);

        // Set the popup info
        SetPopupInfo(currentPopup);
    }
    public void ClosePopup()
    {
        if (currentPopup != null)
        {
            CanvasGroup canvasGroup = currentPopup.GetComponent<CanvasGroup>();
            RectTransform popupRect = currentPopup.GetComponent<RectTransform>();
            CanvasGroup canvasGroup_f = currentFade.GetComponent<CanvasGroup>();

            // Animate closing
            canvasGroup.DOFade(0f, popupAnimationDuration);

            canvasGroup_f.DOFade(0f, popupAnimationDuration).OnComplete(() => {
                currentFade.SetActive(false);
                Destroy(currentFade);
                currentFade = null;
            });

            popupRect.DOScale(Vector3.zero, popupAnimationDuration).SetEase(popupEaseType).OnComplete(() => {
                currentPopup.SetActive(false);
                Destroy(currentPopup);
                currentPopup = null;
            });
        }
    }

    private void SetPopupInfo(GameObject popup)
    {
        // ポップアップ内の各要素を取得
        TextMeshProUGUI nameText = popup.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI typeText = popup.transform.Find("TypeText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI raceText = popup.transform.Find("RaceText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI costText = popup.transform.Find("CostText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI atkDefText = popup.transform.Find("AtkDefText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI effectText = popup.transform.Find("EffectText").GetComponent<TextMeshProUGUI>();
        CardInfoPopUpScript infoCard = popup.transform.Find("InfoCard").GetComponent<CardInfoPopUpScript>();

        infoCard.SetCardInfo(card);
        Button closeButton = popup.transform.Find("CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(ClosePopup);

        // カード情報を設定
        nameText.text = $"名前: {card.cardName}";
        typeText.text = $"カードタイプ: {card.cardType}";
        costText.text = $"コスト: {card.cost}";
        if(card.cardType == CardType.Unit){
            atkDefText.text = $"攻撃力: {card.Atk} / 守備力: {card.Def}";
            raceText.text = $"種族: {card.Race}";
        }else{
            atkDefText.text = "";
            raceText.text = "";
        }
        effectText.text = $"{card.GenerateEffectText()}";
    }
}
