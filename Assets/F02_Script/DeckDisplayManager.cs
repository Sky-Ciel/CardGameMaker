using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;
using TMPro;

public class DeckDisplayManager : MonoBehaviour
{
    public Transform contentParent;  // ScrollView の Content
    [SerializeField] List<GameObject> deckButtonPrefabList;  // デッキを新規作成
    public GameObject deckButtonPrefab;  // デッキを表示するためのボタンプレファブ

    private List<Deck> allDecks = new List<Deck>();  // 保存されたデッキリスト

    public CanvasGroup c_fade;
    public GameObject fade;

    [Header("------ デッキメニュー関連 ------")]
    public GameObject popCardMenu;
    public CanvasGroup popCardMenu_can;
    public GameObject popCardMenuFade;
    public CanvasGroup popCardMenuFade_can;

    public DeckDisplayManager thisDDM; //自身

    Deck SelectingDeck;
    int SelectingNum;

    [Header("------ デッキメニュー(パーツ) ------")]
    public TextMeshProUGUI deckName_menu;
    public CardDisplayScript keycard_menu;

    [Header("------ デッキリスト関連 ------")]
    public GameObject popDeckList;
    public CanvasGroup popDeckList_can;
    public GameObject popDeckListFade;
    public CanvasGroup popDeckListFade_can;
    public RectTransform AllMenu;
    public CanvasGroup AllMenu_fade;

    [Header("------ デッキリスト(パーツ) ------")]
    public TextMeshProUGUI deckName_list;
    public DeckListDisplay deckList_list;
    public CostDistributionGraph costGraph_list;
    public Transform deckContent;  // デッキエリアのコンテナ
    public GameObject deckCardPrefab;  // デッキ用のカードプレハブ

    [Header("------ 削除確認 ------")]
    public GameObject deleteCheck;
    public CanvasGroup deleteCheck_can;
    public GameObject deleteCheckFade;
    public CanvasGroup deleteCheckFade_can;

    [Header("------ コピー確認 ------")]
    public GameObject copyCheck;
    public CanvasGroup copyCheck_can;
    public GameObject copyCheckFade;
    public CanvasGroup copyCheckFade_can;

    void Start()
    {
        DeckEditorManager.isEdit = false;
        DeckEditorManager.EditNumber = 0;

        popCardMenu.SetActive(false);
        popCardMenuFade.SetActive(false);
        popDeckList.SetActive(false);
        popDeckListFade.SetActive(false);
        
        deleteCheck.SetActive(false);
        deleteCheckFade.SetActive(false);

        LoadDecks();  // デッキをロードして表示する

        c_fade.alpha = 1f;
        fade.SetActive(true);
        c_fade.DOFade(0f, 0.5f).OnComplete(() => {
            fade.SetActive(false);
        });
    }

    // デッキをロードしてUIに表示するメソッド
    public void LoadDecks()
    {
        foreach (var D in deckButtonPrefabList)
        {
            Destroy(D);
        }

        // 保存されたデッキリストを PlayerPrefs からロード
        allDecks = PlayerPrefsUtility.LoadList<Deck>("AllDecks");

        // デッキが存在しない場合の対処
        if (allDecks == null || allDecks.Count == 0)
        {
            Debug.LogWarning("デッキが存在しません。");
            return;
        }

        deckButtonPrefabList = new List<GameObject>();

        int num = 0;
        // デッキリストを UI に並べる
        foreach (var deck in allDecks)
        {
            GameObject newDeckButton = Instantiate(deckButtonPrefab, contentParent);
            AllDeckSelectPrefab a = newDeckButton.GetComponent<AllDeckSelectPrefab>();
            a.thisDeck = deck;
            a.thisNum = num;
            a.DDM = thisDDM;

            deckButtonPrefabList.Add(newDeckButton);
            num += 1;
        }
    }

    // カードメニュー(編集・リスト表示を選択)
    public void popUpCardMenuIn(Deck d, int n)
    {
        deckName_menu.text = d.deckName;
        keycard_menu.SetCardInfo(d.KeyCard);

        popCardMenu.SetActive(true);
        popCardMenuFade.SetActive(true);

        popCardMenu_can.alpha = 0f;
        popCardMenuFade_can.alpha = 0f;

        popCardMenuFade_can.DOFade(1f, 0.5f);
        popCardMenu_can.DOFade(1f, 0.5f);

        SelectingDeck = d;
        SelectingNum = n;
    }
    public void popUpCardMenuOut(){
        popCardMenu.SetActive(true);
        popCardMenuFade.SetActive(true);

        popCardMenu_can.alpha = 1f;
        popCardMenuFade_can.alpha = 1f;


        popCardMenu_can.DOFade(0f, 0.5f);
        popCardMenuFade_can.DOFade(0f, 0.5f).OnComplete(() => {
            popCardMenu.SetActive(false);
            popCardMenuFade.SetActive(false);
        });
    }

    // 削除確認画面 - 削除
    public void deleteCheckIn()
    {
        deleteCheck.SetActive(true);
        deleteCheckFade.SetActive(true);

        deleteCheck_can.alpha = 0f;
        deleteCheckFade_can.alpha = 0f;

        deleteCheck_can.DOFade(1f, 0.5f);
        deleteCheckFade_can.DOFade(1f, 0.5f);
    }
    public void deleteCheckOut(bool yes){
        // デッキを削除する
        if(yes){
            DeckEditorManager.isEdit = false;
            DeckEditorManager.EditNumber = 0;
            allDecks.RemoveAt(SelectingNum);
            PlayerPrefsUtility.SaveList<Deck>("AllDecks", allDecks);
            LoadDecks();
            popUpCardMenuOut();
            popUpDeckList_Close();
        }

        deleteCheck_can.alpha = 1f;
        deleteCheckFade_can.alpha = 1f;


        deleteCheck_can.DOFade(0f, 0.5f);
        deleteCheckFade_can.DOFade(0f, 0.5f).OnComplete(() => {
            deleteCheck.SetActive(false);
            deleteCheckFade.SetActive(false);
        });
    }

    // コピー確認画面 - コピー
    public void copyCheckIn()
    {
        copyCheck.SetActive(true);
        copyCheckFade.SetActive(true);

        copyCheck_can.alpha = 0f;
        copyCheckFade_can.alpha = 0f;

        copyCheck_can.DOFade(1f, 0.5f);
        copyCheckFade_can.DOFade(1f, 0.5f);
    }
    public void copyCheckOut(bool yes){
        // デッキをコピーする
        if(yes){
            DeckEditorManager.isEdit = false;
            DeckEditorManager.EditNumber = 0;
            Deck newDeck = new Deck();
            newDeck.deckName = SelectingDeck.deckName + "のコピー";
            newDeck.deckCards = SelectingDeck.deckCards;
            newDeck.KeyCard = SelectingDeck.KeyCard;
            allDecks.Add(newDeck);
            PlayerPrefsUtility.SaveList<Deck>("AllDecks", allDecks);
            LoadDecks();
            popUpCardMenuOut();
            popUpDeckList_Close();
        }

        copyCheck_can.alpha = 1f;
        copyCheckFade_can.alpha = 1f;


        copyCheck_can.DOFade(0f, 0.5f);
        copyCheckFade_can.DOFade(0f, 0.5f).OnComplete(() => {
            copyCheck.SetActive(false);
            copyCheckFade.SetActive(false);
        });
    }

    // デッキリスト
    public void popUpDeckList(){
        SetDeckListView();
        
        popDeckList.SetActive(true);
        popDeckListFade.SetActive(true);

        popDeckList_can.alpha = 0f;
        popDeckListFade_can.alpha = 0f;
        AllMenu_fade.alpha = 1f;

        AllMenu.localScale = Vector3.zero;

        // Animate the popup
        AllMenu_fade.DOFade(0f, 0.5f);
        AllMenu.DOScale(Vector3.one, 0.5f);
        popDeckList_can.DOFade(1f, 0.5f);
        popDeckListFade_can.DOFade(1f, 0.5f);
    }
    // デッキリスト
    public void popUpDeckList_Close(){
        popDeckList_can.alpha = 1f;
        popDeckListFade_can.alpha = 1f;
        AllMenu_fade.alpha = 0f;

        AllMenu.localScale = Vector3.zero;

        // Animate the popup
        popDeckList_can.DOFade(0f, 0.5f);
        popDeckListFade_can.DOFade(0f, 0.5f);

        AllMenu_fade.DOFade(1f, 0.5f);
        AllMenu.DOScale(Vector3.one, 0.5f).OnComplete(() => {
            popDeckList.SetActive(false);
            popDeckListFade.SetActive(false);
        });
    }

    // デッキリスト表示
    void SetDeckListView(){
        deckName_list.text = SelectingDeck.deckName;
        deckList_list.UpdateDeckList(SelectingDeck.deckCards);
        costGraph_list.UpdateCostDistribution(SelectingDeck.deckCards);

        // ソート後にデッキ内の表示を更新
        for (int i = 0; i < deckContent.childCount; i++)
        {
            Destroy(deckContent.GetChild(i).gameObject);  // 既存のカードオブジェクトを削除
        }

        // デッキに追加
        for(int i=0; i<SelectingDeck.deckCards.Count; i++){
            GameObject newCard = Instantiate(deckCardPrefab, deckContent);
            SelectListViewCard newCardScript = newCard.GetComponent<SelectListViewCard>();
            newCardScript.SetCardInfo(SelectingDeck.deckCards[i]);  // デッキエリアを参照に追加
        }
    }

    // デッキ内のカードを自動でソートする
    private void SortDeck()
    {
        // カードをユニット、マジック、トラップの順でソート
        SelectingDeck.deckCards.Sort((card1, card2) =>
        {
            if (card1.cardType == card2.cardType)
            {
                return string.Compare(card1.cardName, card2.cardName, StringComparison.Ordinal);
            }
            else if (card1.cardType == CardType.Unit)
            {
                return -1;
            }
            else if (card1.cardType == CardType.Magic && card2.cardType == CardType.Trap)
            {
                return -1;
            }
            return 1;
        });

        // ソート後にデッキ内の表示を更新
        for (int i = 0; i < deckContent.childCount; i++)
        {
            Destroy(deckContent.GetChild(i).gameObject);  // 既存のカードオブジェクトを削除
        }
        foreach (var card in SelectingDeck.deckCards)
        {
            GameObject newCard = Instantiate(deckCardPrefab, deckContent);
            SelectListViewCard newCardScript = newCard.GetComponent<SelectListViewCard>();
            newCardScript.SetCardInfo(card);  // デッキエリアを参照に追加
        }
    }

    // デッキボタンが押されたときの処理
    public void OnDeckNewDeckBuild()
    {
        Debug.Log($"新規デッキを制作します。");
        // 必要に応じて、デッキ編集画面への遷移やデッキ情報の表示などを行う
        DeckEditorManager.isEdit = false;
        DeckEditorManager.EditNumber = 0;

        c_fade.alpha = 0f;
        fade.SetActive(true);
        c_fade.DOFade(1f, 0.5f).OnComplete(() => {
            SceneManager.LoadScene("DeckBuild");
        });
    }

    public void startBuilding(){
        c_fade.alpha = 0f;
        fade.SetActive(true);
        c_fade.DOFade(1f, 0.5f).OnComplete(() => {
            SceneManager.LoadScene("DeckBuild");
        });
    }

    public void GoTitle()
    {
        DeckEditorManager.isEdit = false;
        DeckEditorManager.EditNumber = 0;

        c_fade.alpha = 0f;
        fade.SetActive(true);
        c_fade.DOFade(1f, 0.5f).OnComplete(() => {
            SceneManager.LoadScene("Title");
        });
    }
}
