using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AllDeckSelectPrefab : MonoBehaviour
{
    public Deck thisDeck;
    public int thisNum;

    public TextMeshProUGUI deckNameText;

    public CanvasGroup popList;

    public DeckDisplayManager DDM;

    public CardDisplayScript c;

    // Update is called once per frame
    
    void Start(){
        if(thisDeck.KeyCard != null){
            c.SetCardInfo(thisDeck.KeyCard);
        }
    }

    void Update()
    {
        deckNameText.text = thisDeck.deckName;
    }

    public void OnDeckButtonClicked()
    {
        Debug.Log($"デッキ '{thisDeck.deckName}' が選択されました。");
        // 必要に応じて、デッキ編集画面への遷移やデッキ情報の表示などを行う
        DeckEditorManager.isEdit = true;
        DeckEditorManager.EditNumber = thisNum;

        //SceneManager.LoadScene("DeckBuild");
        DDM.popUpCardMenuIn(thisDeck, thisNum);
    }
}
