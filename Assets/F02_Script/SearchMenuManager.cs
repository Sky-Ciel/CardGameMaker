using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using DG.Tweening;

public class SearchMenuManager : MonoBehaviour
{
    [Header("------ 検索メニューのパーツ ------")]
    [Tooltip("検索メニューのポップアップ")]
    public GameObject searchMenu;

    [Tooltip("カード名の入力フィールド")]
    public TMP_InputField nameInputField;

    [Tooltip("種族名の入力フィールド")]
    public TMP_InputField raceInputField;

    [Tooltip("種族検索ボックスのコンテナ")]
    public GameObject raceSearchContainer;

    [Tooltip("カードタイプの選択ドロップダウン")]
    public TMP_Dropdown typeDropdown;

    [Tooltip("コスト選択スクロールビュー")]
    public ScrollRect costScrollView;

    [Tooltip("コストボタンのプレハブ")]
    public GameObject costButtonPrefab;

    [Tooltip("コストボタンを配置するコンテンツの親オブジェクト")]
    public Transform costContent;

    [Tooltip("検索実行ボタン")]
    public Button searchButton;

    [Tooltip("検索メニューを閉じるボタン")]
    public Button closeButton;

    [Tooltip("検索結果を表示するコンテンツの親オブジェクト")]
    public Transform cardContent;

    [Tooltip("ゲーム設定を参照")]
    GS gs;

    [Tooltip("カードの表示用プレハブ")]
    public GameObject cardPrefab;

    [Header("------ 検索条件をリセットするボタン ------")]
    public Button resetButton;

    [Header("------ フェード ------")]
    public GameObject fade;
    public CanvasGroup fadeC;

    private List<Card> availableCards;
    private List<int> selectedCosts = new List<int>();
    private int maxCost;

    void Start()
    {
        if (gs == null)
        {
            gs = ScriptableObject.CreateInstance<GS>();
        }
        gs = PlayerPrefsUtility.LoadScriptableObject<GS>("GameSetting");

        string json = PlayerPrefs.GetString("CardData");
        availableCards = JsonConvert.DeserializeObject<List<Card>>(json);

        searchButton.onClick.AddListener(PerformSearch);
        closeButton.onClick.AddListener(CloseSearchMenu);
        resetButton.onClick.AddListener(ResetSearchFilters);

        if (gs.NoRace)
        {
            raceSearchContainer.SetActive(false);
        }

        maxCost = GetMaxCardCost();
        GenerateCostButtons();

        InitializeTypeDropdown();

        searchMenu.SetActive(false);
        fade.SetActive(false);
    }

    private void InitializeTypeDropdown()
    {
        typeDropdown.ClearOptions();
        List<string> options = new List<string> { "全てのタイプ" };
        options.AddRange(System.Enum.GetNames(typeof(CardType)));
        typeDropdown.AddOptions(options);
    }

    public void OpenSearchMenu()
    {
        searchMenu.SetActive(true);
        fade.SetActive(true);

        CanvasGroup canvasGroup = searchMenu.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = searchMenu.AddComponent<CanvasGroup>();

        fadeC.alpha = 0;
        fadeC.DOFade(1, 0.5f);

        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.5f);
        searchMenu.transform.localScale = Vector3.zero;
        searchMenu.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void CloseSearchMenu()
    {
        CanvasGroup canvasGroup = searchMenu.GetComponent<CanvasGroup>();

        fadeC.DOFade(0, 0.5f).OnComplete(() => fade.SetActive(false));
        
        canvasGroup.DOFade(0, 0.5f).OnComplete(() => searchMenu.SetActive(false));
        searchMenu.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
    }

    public void PerformSearch()
    {
        string nameQuery = nameInputField.text.ToLower();
        string raceQuery = raceInputField.text.ToLower();
        CardType? typeQuery = typeDropdown.value == 0 ? null : (CardType?)(typeDropdown.value - 1);

        List<int> selectedCosts = this.selectedCosts;

        var filteredCards = availableCards.Where(card =>
            (string.IsNullOrEmpty(nameQuery) || card.cardName.ToLower().Contains(nameQuery)) &&
            (string.IsNullOrEmpty(raceQuery) || card.Race.ToLower().Contains(raceQuery)) &&
            (typeQuery == null || card.cardType == typeQuery) &&
            (selectedCosts.Count == 0 || selectedCosts.Contains(card.cost))).ToList();

        DisplayFilteredCards(filteredCards);
        CloseSearchMenu();
    }

    private void DisplayFilteredCards(List<Card> filteredCards)
    {
        foreach (Transform child in cardContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var card in filteredCards)
        {
            GameObject cardObject = Instantiate(cardPrefab, cardContent);
            CardPrefabScript cardScript = cardObject.GetComponent<CardPrefabScript>();
            cardScript.SetCardInfo(card);
        }
    }

    private int GetMaxCardCost()
    {
        if (availableCards != null && availableCards.Count > 0)
        {
            return availableCards.Max(card => card.cost);
        }
        return 0;
    }

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

    private void OnCostButtonClick(GameObject button, int cost)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (selectedCosts.Contains(cost))
        {
            selectedCosts.Remove(cost);
            buttonImage.color = Color.black;
        }
        else
        {
            selectedCosts.Add(cost);
            buttonImage.color = Color.yellow;
        }

        //PerformSearch();
    }

    public void ResetSearchFilters()
    {
        nameInputField.text = "";
        raceInputField.text = "";
        typeDropdown.value = 0;
        selectedCosts.Clear();

        foreach (Transform child in costContent)
        {
            child.GetComponent<Image>().color = Color.black;
        }

        //PerformSearch();
    }
}