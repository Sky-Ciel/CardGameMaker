using UnityEngine;
using UnityEngine.SceneManagement;

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

    // デッキ生成シーンに遷移する
    public void GoToDeckBuilding()
    {
        SceneManager.LoadScene("DeckBuildingScene");
    }

    // バトル準備シーンに遷移する
    public void GoToBattlePreparation()
    {
        SceneManager.LoadScene("BattlePreparationScene");
    }
}
