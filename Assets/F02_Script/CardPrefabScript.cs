using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class CardPrefabScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("------ カード本体のパーツ ------")]
    public Card card;  // このプレハブが保持するカード情報
    public Image cardImage;  // カードの画像
    public TextMeshProUGUI cardNameText;  // カード名表示用テキスト
    public TextMeshProUGUI cardTextText;  // 効果表示用テキスト
    public TextMeshProUGUI cardCostText;  // コスト表示用テキスト
    public TextMeshProUGUI cardRaceText;  // 種族表示用テキスト
    public TextMeshProUGUI cardStatesText;  // 攻守表示用テキスト

    private Transform parentBeforeDrag;  // ドラッグ前の親オブジェクト
    private CanvasGroup canvasGroup;
    private GameObject draggingCard;  // ドラッグ時のコピーオブジェクト

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

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (gs == null)
        {
            gs = ScriptableObject.CreateInstance<GS>();
        }
        gs = PlayerPrefsUtility.LoadScriptableObject<GS>("GameSetting");
    }

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

    // ドラッグを開始したとき
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentBeforeDrag = transform.parent;
        
        // ドラッグ用のコピーを作成
        draggingCard = Instantiate(gameObject, transform.root);
        draggingCard.GetComponent<CanvasGroup>().blocksRaycasts = false;  // Raycast を無効化

        // 見た目を少し変更してわかりやすくする（オプション）
        //draggingCard.GetComponent<Image>().color = new Color(1, 1, 1, 0.7f);
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

    public void OnPointerClick(PointerEventData eventData)
    {
        OpenPopup();
    }

    private void OpenPopup()
    {
        // Create the popup
        currentPopup = popupPrefab;
        currentPopup.SetActive(true);
        
        // Set the popup's initial state
        RectTransform popupRect = currentPopup.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = currentPopup.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = currentPopup.AddComponent<CanvasGroup>();

        // Set initial values for animation
        canvasGroup.alpha = 0f;
        popupRect.localScale = Vector3.zero;

        // Animate the popup
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

            // Animate closing
            canvasGroup.DOFade(0f, popupAnimationDuration);
            popupRect.DOScale(Vector3.zero, popupAnimationDuration).SetEase(popupEaseType).OnComplete(() => {
                currentPopup.SetActive(false);
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
