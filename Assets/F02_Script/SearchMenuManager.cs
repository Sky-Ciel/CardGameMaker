using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using DG.Tweening;  // DOTweenを利用したアニメーション

public class SearchMenuManager : MonoBehaviour
{
    [Header("------ 検索メニューのパーツ ------")]
    [Tooltip("検索メニューのポップアップ")]
    public GameObject searchMenu;  // 検索メニューのポップアップ

    [Tooltip("カード名の入力フィールド")]
    public TMP_InputField nameInputField;  // カード名の入力

    [Tooltip("種族名の入力フィールド")]
    public TMP_InputField raceInputField;  // 種族名の入力

    [Tooltip("種族検索ボックスのコンテナ")]
    public GameObject raceSearchContainer;  // 種族検索フィールド（非表示にするためのコンテナ）

    [Tooltip("カードタイプの選択ドロップダウン")]
    public TMP_Dropdown typeDropdown;  // カードタイプの選択（ユニット、マジック、トラップ）

    [Tooltip("コスト選択スクロールビュー")]
    public ScrollRect costScrollView;  // コスト選択用のスクロールビュー

    [Tooltip("コストボタンのプレハブ")]
    public GameObject costButtonPrefab;  // コストボタンのプレハブ

    [Tooltip("コストボタンを配置するコンテンツの親オブジェクト")]
    public Transform costContent;  // コストボタンを配置するコンテナ

    [Tooltip("検索実行ボタン")]
    public Button searchButton;  // 検索実行ボタン

    [Tooltip("検索メニューを閉じるボタン")]
    public Button closeButton;  // 検索メニューを閉じるボタン

    [Tooltip("検索結果を表示するコンテンツの親オブジェクト")]
    public Transform cardContent;  // 検索結果を表示する場所

    [Tooltip("ゲーム設定を参照")]
    GS gs;  // ゲーム設定

    [Tooltip("カードの表示用プレハブ")]
    public GameObject cardPrefab;  // 検索結果に表示するカードプレハブ

    private List<Card> availableCards;  // 検索対象の全カードリスト
    private List<int> selectedCosts = new List<int>();  // 選択されたコストリスト
    private int maxCost;  // カードリストから取得した最大コスト

    void Start()
    {
        if (gs == null)
        {
            gs = ScriptableObject.CreateInstance<GS>();
        }
        gs = PlayerPrefsUtility.LoadScriptableObject<GS>("GameSetting");

        string json = PlayerPrefs.GetString("CardData");
        availableCards = JsonConvert.DeserializeObject<List<Card>>(json);

        // 検索ボタンと閉じるボタンのリスナーを設定
        searchButton.onClick.AddListener(PerformSearch);
        closeButton.onClick.AddListener(CloseSearchMenu);

        // ゲーム設定から種族が使用されない場合は、種族検索フィールドを非表示にする
        if (gs.NoRace)
        {
            raceSearchContainer.SetActive(false);
        }

        // カードリストに基づいて最大コストを取得し、コストボタンを生成
        maxCost = GetMaxCardCost();
        GenerateCostButtons();

        searchMenu.SetActive(false);
    }

    // 検索メニューを開いた際、カードリストを渡して初期化
    public void OpenSearchMenu()
    {
        searchMenu.SetActive(true);

        // メニューのふわっとしたフェードインアニメーション
        CanvasGroup canvasGroup = searchMenu.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = searchMenu.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.5f);  // 0.5秒でフェードイン
        searchMenu.transform.localScale = Vector3.zero;  // メニューのスケールを最小化
        searchMenu.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);  // ふわっとしたスケールアップ
    }

    // 検索メニューを閉じる処理
    public void CloseSearchMenu()
    {
        // メニューのフェードアウトと縮小アニメーション
        CanvasGroup canvasGroup = searchMenu.GetComponent<CanvasGroup>();
        canvasGroup.DOFade(0, 0.5f).OnComplete(() => searchMenu.SetActive(false));  // フェードアウト後に非表示
        searchMenu.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);  // スケールダウン
    }

    // 検索処理を実行する
    public void PerformSearch()
    {
        string nameQuery = nameInputField.text.ToLower();
        string raceQuery = raceInputField.text.ToLower();
        CardType? typeQuery = (CardType?)typeDropdown.value;

        // 選択されたコストリストを取得
        List<int> selectedCosts = this.selectedCosts;

        // 部分一致による検索ロジック
        var filteredCards = availableCards.Where(card =>
            (string.IsNullOrEmpty(nameQuery) || card.cardName.ToLower().Contains(nameQuery)) &&
            (string.IsNullOrEmpty(raceQuery) || card.Race.ToLower().Contains(raceQuery)) &&
            (typeQuery == null || card.cardType == typeQuery) &&
            (selectedCosts.Count == 0 || selectedCosts.Contains(card.cost))).ToList();

        // フィルタリングされたカードを表示
        DisplayFilteredCards(filteredCards);
        CloseSearchMenu();
    }

    // フィルタリングされたカードを表示
    private void DisplayFilteredCards(List<Card> filteredCards)
    {
        // 既存のカードをクリア
        foreach (Transform child in cardContent)
        {
            Destroy(child.gameObject);
        }

        // フィルタされたカードを表示
        foreach (var card in filteredCards)
        {
            GameObject cardObject = Instantiate(cardPrefab, cardContent);
            CardPrefabScript cardScript = cardObject.GetComponent<CardPrefabScript>();
            cardScript.SetCardInfo(card);
        }
    }

    // 最大コストを取得する
    private int GetMaxCardCost()
    {
        if (availableCards != null && availableCards.Count > 0)
        {
            return availableCards.Max(card => card.cost);
        }
        return 0;
    }

    // コストボタンを生成する
    private void GenerateCostButtons()
    {
        for (int i = 0; i <= maxCost; i++)
        {
            GameObject costButton = Instantiate(costButtonPrefab, costContent);
            TextMeshProUGUI buttonText = costButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = i.ToString();

            Button button = costButton.GetComponent<Button>();
            int costValue = i;
            button.onClick.AddListener(() => OnCostButtonClick(costButton, costValue));
        }
    }

    // コストボタンがクリックされた時の処理
    private void OnCostButtonClick(GameObject button, int cost)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (selectedCosts.Contains(cost))
        {
            // 既に選択されている場合は解除
            selectedCosts.Remove(cost);
            buttonImage.color = Color.white;  // ボタンをOFFの色に
        }
        else
        {
            // 新しく選択された場合
            selectedCosts.Add(cost);
            buttonImage.color = Color.green;  // ボタンをONの色に
        }

        // コスト選択に基づいて再度検索を実行
        PerformSearch();
    }
}
