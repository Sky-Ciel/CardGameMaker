using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public GameSettings gameSettings;
    public CardManager cardManager;

    void Start()
    {
        Debug.Log("Hello World");

        // ゲーム設定の読み込み
        gameSettings.LoadGameSettings();
        Debug.Log("ゲーム設定を読み込みました。");

        // カードの読み込み
        cardManager.LoadCards();
        Debug.Log("カード情報を読み込みました。");
    }
}
