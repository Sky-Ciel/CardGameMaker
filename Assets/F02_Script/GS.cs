using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameSetting", menuName = "GameSetting")]
public class GS : ScriptableObject
{
    public int lifePoints;
    public int deckSize;
    public string[] turnProgress;
    public int fieldLimit;
    public bool noTargetSelection;
    public bool freeSummon;
    public int maxCopiesPerCard; // 同名カードの上限枚数
}
