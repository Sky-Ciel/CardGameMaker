using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class TitleScreen : MonoBehaviour
{
    public GameSettings gameSettings;
    public CardManager cardManager;

    [Header("------ フェード ------")]
    public CanvasGroup fadeElement; // フェードさせる要素
    public GameObject fade;

    [Header("------ プログラムボタン ------")]
    public RectTransform window; // アニメーションさせるウィンドウ
    public Button programButton; // アニメーションさせるボタン

    [Header("------ デッキ制作ボタン ------")]
    public RectTransform window_d; // アニメーションさせるウィンドウ
    public Button programButton_d; // アニメーションさせるボタン

    [Header("------ 設定表示ボタン ------")]
    public RectTransform window_set; // アニメーションさせるウィンドウ

    [Header("------ 全画面フェード ------")]
    public CanvasGroup AllFadeCanva; // フェードさせる要素
    public GameObject AllFade;

    void Start()
    {
        Debug.Log("Hello World");

        // ウィンドウの初期位置を保存（上の画面外から始めるためにYを大きくする）
        window.anchoredPosition = new Vector2(0, 1100); // 画面上部にウィンドウを移動
        window_d.anchoredPosition = new Vector2(0, 1100); // 画面上部にウィンドウを移動
        window_set.anchoredPosition = new Vector2(0, 1100); // 画面上部にウィンドウを移動

        AnimateButton();
        fade.SetActive(false);

        fadeIn();
    }

    void fadeIn(){
        AllFadeCanva.alpha = 1f;
        AllFade.SetActive(true);
        AllFadeCanva.DOFade(0f, 0.5f).OnComplete(() => {
            AllFade.SetActive(false);
        });
    }
    void fadeOut(string scene){
        AllFadeCanva.alpha = 0f;
        AllFade.SetActive(true);
        AllFadeCanva.DOFade(1f, 0.5f).OnComplete(() => {
            SceneManager.LoadScene(scene);
        });
    }

    void Update(){
        programButton.interactable = gameSettings.isPath && cardManager.isPath && cardManager.isIMGPath;
    }

    //----------------- 処理 -----------------
    public void Programing(bool yes){
        if(yes){
            // ゲーム設定の読み込み
            gameSettings.LoadGameSettings();
            Debug.Log("ゲーム設定を読み込みました。");

            // カードの読み込み
            cardManager.LoadCards();
            Debug.Log("カード情報を読み込みました。");
        }

        window.DOAnchorPos(new Vector2(0,1100), 0.7f).SetEase(Ease.OutQuad); // 1秒で降りる
        fade.SetActive(false);
    }

    // デッキ生成シーンに遷移する
    public void GoToDeckBuilding(bool yes)
    {
        if(yes){
            fadeOut("DeckSelect");
        }else{
            window_d.DOAnchorPos(new Vector2(0,1100), 0.7f).SetEase(Ease.OutQuad); // 1秒で降りる
            fade.SetActive(false);
        }
    }

    // バトル準備シーンに遷移する
    public void GoToBattlePreparation()
    {
        SceneManager.LoadScene("BattlePreparationScene");
    }

    //----------------- アニメーション -----------------

    //　プログラム反映確認
    public void OnProgramButtonClick()
    {
        // ウィンドウが上からふわっと降りてくるアニメーション
        window.DOAnchorPos(new Vector2(0,0), 0.7f).SetEase(Ease.OutQuad); // 1秒で降りる
        fade.SetActive(true);
        // フェードアニメーション
        fadeElement.DOFade(1f, 1f); // 1秒かけてフェードイン
    }

    //　デッキ制作画面へ行きたい時
    public void OnDeckButtonClick()
    {
        // ウィンドウが上からふわっと降りてくるアニメーション
        window_d.DOAnchorPos(new Vector2(0,0), 0.7f).SetEase(Ease.OutQuad); // 1秒で降りる
        fade.SetActive(true);
        // フェードアニメーション
        fadeElement.DOFade(1f, 1f); // 1秒かけてフェードイン
    }

    //　設定確認画面
    public void OnSettingButtonClick(bool open)
    {
        if(open){
            // ウィンドウが上からふわっと降りてくるアニメーション
            window_set.DOAnchorPos(new Vector2(0,0), 0.7f).SetEase(Ease.OutQuad); // 1秒で降りる
            fade.SetActive(true);
            // フェードアニメーション
            fadeElement.DOFade(1f, 1f); // 1秒かけてフェードイン
        }else{
            window_set.DOAnchorPos(new Vector2(0,1100), 0.7f).SetEase(Ease.OutQuad); // 1秒で降りる
            fade.SetActive(false);
        }
    }

    // ボタンのふわふわアニメーション
    void AnimateButton()
    {
        // ボタンがふわふわと拡大・縮小するアニメーション
        programButton.transform.DOScale(1.1f, 0.6f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        programButton_d.transform.DOScale(1.1f, 0.6f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}
