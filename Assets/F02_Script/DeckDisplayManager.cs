using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class DeckDisplayManager : MonoBehaviour
{
    public Transform contentParent;  // ScrollView の Content
    public GameObject deckButtonPrefab;  // デッキを表示するためのボタンプレファブ

    private List<Deck> allDecks = new List<Deck>();  // 保存されたデッキリスト

    public CanvasGroup c_fade;
    public GameObject fade;

    void Start()
    {
        DeckEditorManager.isEdit = false;
        DeckEditorManager.EditNumber = 0;

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
        // 保存されたデッキリストを PlayerPrefs からロード
        allDecks = PlayerPrefsUtility.LoadList<Deck>("AllDecks");

        // デッキが存在しない場合の対処
        if (allDecks == null || allDecks.Count == 0)
        {
            Debug.LogWarning("デッキが存在しません。");
            return;
        }

        int num = 0;
        // デッキリストを UI に並べる
        foreach (var deck in allDecks)
        {
            GameObject newDeckButton = Instantiate(deckButtonPrefab, contentParent);
            AllDeckSelectPrefab a = newDeckButton.GetComponent<AllDeckSelectPrefab>();
            a.thisDeck = deck;
            a.thisNum = num;

            /*newDeckButton.GetComponentInChildren<TextMeshProUGUI>().text = deck.deckName;
            // デッキボタンを押したときの動作
            newDeckButton.GetComponent<Button>().onClick.AddListener(() => OnDeckButtonClicked(deck, num));*/
            num += 1;
        }
    }

    // デッキボタンが押されたときの処理
    public void OnDeckButtonClicked(Deck deck, int num)
    {
        Debug.Log($"デッキ '{deck.deckName}' が選択されました。");
        // 必要に応じて、デッキ編集画面への遷移やデッキ情報の表示などを行う
        DeckEditorManager.isEdit = true;
        DeckEditorManager.EditNumber = num-1;

        SceneManager.LoadScene("DeckBuild");
    }

    // デッキボタンが押されたときの処理
    public void OnDeckNewDeckBuild()
    {
        Debug.Log($"新規デッキを制作します。");
        // 必要に応じて、デッキ編集画面への遷移やデッキ情報の表示などを行う
        DeckEditorManager.isEdit = false;
        DeckEditorManager.EditNumber = 0;

        SceneManager.LoadScene("DeckBuild");
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
