using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameSetting", menuName = "GameSetting")]
public class GS : ScriptableObject
{
    public int lifePoints; //初期ライフ値
    public int deckSize; //デッキ枚数下限
    public string[] turnProgress; //ターン進行
    public int fieldLimit; //フィールドの上限枚数
    public bool noTargetSelection; //攻撃時にターゲットを選べなくする
    public bool freeSummon; //コストの概念のないゲームにする
    public bool NoRace; //種族の概念のないゲームにする
    public int maxCopiesPerCard; // 同名カードの上限枚数
}
