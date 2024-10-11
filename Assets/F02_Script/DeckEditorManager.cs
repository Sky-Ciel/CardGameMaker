using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class DeckEditorManager : MonoBehaviour
{
    [Header("------ カード一覧 ------")]
    public ScrollRect cardScrollView;  // 上段のカードスクロールビュー
    public GameObject cardPrefab;      // カードを表示するPrefab
    public Transform cardContent;      // カードを配置するコンテナ（GridLayoutGroupがアタッチされている）
    
    [Header("------ デッキ ------")]
    public Transform deckContent;      // デッキ（下段）のスクロールビューコンテナ

    public List<Card> availableCards;  // 全てのカードリスト

    [Header("------ フェード ------")]
    public CanvasGroup fadeElement; // フェードさせる要素
    public GameObject fade;

    [Header("------ 編集終了ボタン ------")]
    public RectTransform window_back; // アニメーションさせるウィンドウ

    [Header("------ ポップアップ カード詳細 ------")]
    public GameObject popupInfo;

    [Header("------ デッキリスト ------")]
    public RectTransform menuPanel;  // メニューウィンドウのパネル
    public float tweenDuration = 0.5f;  // メニューの動きの速度
    private bool isMenuOpen = false;  // メニューが開いているかのフラグ

    void Start()
    {
        fade.SetActive(false);

        LoadAvailableCards();  // カード情報をロード
        DisplayCards();        // カードをスクロールビューに表示
    }

    // 全てのカードを表示する関数
    private void DisplayCards()
    {
        // 既存のカードをクリア
        foreach (Transform child in cardContent)
        {
            Destroy(child.gameObject);
        }

        // カードPrefabを生成し、カード情報を表示
        foreach (var card in availableCards)
        {
            GameObject cardObject = Instantiate(cardPrefab, cardContent);

            // CardPrefabScript にカード情報をセットする
            CardPrefabScript cardScript = cardObject.GetComponent<CardPrefabScript>();
            cardScript.SetCardInfo(card);
        }

        popupInfo.SetActive(false);
    }

    // 使用可能なカードをロードする関数（サンプル）
    private void LoadAvailableCards()
    {
        // PlayerPrefs から保存されたカードリストをロード
        string json = PlayerPrefs.GetString("CardData");
        availableCards = JsonConvert.DeserializeObject<List<Card>>(json);

        // カードリストが空の場合はデフォルトカードをロード
        if (availableCards == null || availableCards.Count == 0)
        {
            // 仮のカードリストをロード (初回起動用)
            availableCards = new List<Card>
            {
                new Card { cardName = "Fireball", illustration = Resources.Load<Sprite>("Images/fireball") },
                new Card { cardName = "Warrior", illustration = Resources.Load<Sprite>("Images/warrior") },
                new Card { cardName = "Shield", illustration = Resources.Load<Sprite>("Images/shield") }
            };
        }
    }

    //----------------- アニメーション -----------------
    public void OnBackWindow()
    {
        // ウィンドウが上からふわっと降りてくるアニメーション
        window_back.DOAnchorPos(new Vector2(0,0), 0.7f).SetEase(Ease.OutQuad);
        fade.SetActive(true);
        // フェードアニメーション
        fadeElement.DOFade(1f, 1f); // 1秒かけてフェードイン
    }
    public void OffBackWindow()
    {
        window_back.DOAnchorPos(new Vector2(0,1100), 0.7f).SetEase(Ease.OutQuad);
        fade.SetActive(false);
    }

    public void GoTitle(bool yes)
    {
        window_back.DOAnchorPos(new Vector2(0,1100), 0.7f).SetEase(Ease.OutQuad);
        SceneManager.LoadScene("Title");
    }

    // メニューを開閉するメソッド
    public void ToggleMenu()
    {
        if (isMenuOpen)
        {
            // メニューを閉じる (右側に隠す)
            menuPanel.DOAnchorPos(new Vector2(1270, 0), tweenDuration).SetEase(Ease.InOutQuad);
        }
        else
        {
            // メニューを開く (左側に表示)
            menuPanel.DOAnchorPos(new Vector2(645f, 0), tweenDuration).SetEase(Ease.InOutQuad);
        }
        isMenuOpen = !isMenuOpen;  // 状態を反転
    }
}
