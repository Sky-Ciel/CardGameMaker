using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
public class Card : ScriptableObject
{
    public string cardName; // カード名
    public int Atk; //攻撃
    public int Def; //守備
    public int Rank; //ランク
    public int Rarity; //レア度
    public string Race; // 種族名
    public Element element;
    public CardType cardType; // カードの種類（列挙型）
    public string illustrationPath; // カードイラストのパス
    public int cost; // コスト
    public CardEffect effect; // 効果

    // 保存しないプロパティ
    //[System.NonSerialized]
    public Sprite illustration;  // Sprite はシリアライズしない

    public string GenerateEffectText()
    {
        List<string> effectTexts = new List<string>();

        if (effect != null)
        {
            // Heal効果
            if (effect.healEffects != null && effect.healEffects.Count > 0)
            {
                foreach (var healEffect in effect.healEffects)
                {
                    effectTexts.Add($"{GetTriggerText(healEffect.trigger)}、ライフを{healEffect.heal}回復する。");
                }
            }

            // Draw効果
            if (effect.drawEffects != null && effect.drawEffects.Count > 0)
            {
                foreach (var drawEffect in effect.drawEffects)
                {
                    effectTexts.Add($"{GetTriggerText(drawEffect.trigger)}、カードを{drawEffect.draw}枚引く。");
                }
            }

            // BuffAtk効果
            if (effect.buffAtkEffects != null && effect.buffAtkEffects.Count > 0)
            {
                foreach (var buffAtkEffect in effect.buffAtkEffects)
                {
                    effectTexts.Add($"{GetTriggerText(buffAtkEffect.trigger)}、{buffAtkEffect.target}体のユニットの攻撃力を{buffAtkEffect.value}上げる。");
                }
            }

            // Remove効果
            if (effect.removeEffects != null && effect.removeEffects.Count > 0)
            {
                foreach (var removeEffect in effect.removeEffects)
                {
                    string targetConditionText = GetTargetConditionText(removeEffect.target_condition);
                    effectTexts.Add($"{GetTriggerText(removeEffect.trigger)}、{targetConditionText}から{removeEffect.DesTarget}体を除外する。");
                }
            }

            // TemporaryBuff効果
            if (effect.temporaryBuffEffects != null && effect.temporaryBuffEffects.Count > 0)
            {
                foreach (var tempBuffEffect in effect.temporaryBuffEffects)
                {
                    string buffTypeText = tempBuffEffect.buff_type == "atk" ? "攻撃力" : "守備力";
                    effectTexts.Add($"{GetTriggerText(tempBuffEffect.trigger)}、{tempBuffEffect.target}体のユニットの{buffTypeText}を{tempBuffEffect.duration}ターンの間、{tempBuffEffect.value}上げる。");
                }
            }
        }

        return string.Join(" ", effectTexts);
    }

    private string GetTriggerText(TriggerEvent trigger)
    {
        switch (trigger)
        {
            case TriggerEvent.Play:
                return "このカードをプレイした時";
            case TriggerEvent.Attack:
                return "このカードが攻撃する時";
            case TriggerEvent.End:
                return "ターン終了時";
            default:
                return "";
        }
    }

    private string GetTargetConditionText(TargetCondition condition)
    {
        List<string> conditionTexts = new List<string>();

        switch (condition.location)
        {
            case LocationType.OwnField:
                conditionTexts.Add("自分のフィールド");
                break;
            case LocationType.OpponentField:
                conditionTexts.Add("相手のフィールド");
                break;
            case LocationType.AllField:
                conditionTexts.Add("全てのフィールド");
                break;
        }

        if (condition.type != CardType.Unit)
        {
            conditionTexts.Add($"{condition.type}カード");
        }

        if (condition.atk_less_than > 0)
        {
            conditionTexts.Add($"攻撃力が{condition.atk_less_than}未満");
        }

        if (condition.atk_greater_than > 0)
        {
            conditionTexts.Add($"攻撃力が{condition.atk_greater_than}より大きい");
        }

        if (condition.def_less_than > 0)
        {
            conditionTexts.Add($"守備力が{condition.def_less_than}未満");
        }

        if (condition.def_greater_than > 0)
        {
            conditionTexts.Add($"守備力が{condition.def_greater_than}より大きい");
        }

        return string.Join("の", conditionTexts);
    }
}

public enum Element
{
    Fire, //赤枠
    Water, //青枠
    Earth, //茶枠
    Air, //黄緑枠
    Light, //黄色枠
    Dark //黒紫枠
}

public enum TriggerEvent
{
    Play,   // プレイ時に発動
    Attack, // 攻撃時に発動
    End     // ターン終了時に発動
}

public enum CardType
{
    Unit,
    Magic,
    Trap
}

[System.Serializable]
public class CardEffect
{
    public List<HealEffect> healEffects; // Heal 効果のリスト
    public List<DrawEffect> drawEffects; // Draw 効果のリスト
    public List<BuffAtkEffect> buffAtkEffects; // BuffAtk 効果のリスト
    public List<RemoveEffect> removeEffects; // Remove 効果のリスト
    public List<TemporaryBuffEffect> temporaryBuffEffects; // Temporary Buff 効果のリスト
}

[System.Serializable]
public class HealEffect
{
    public int heal; // 回復量
    public TriggerEvent trigger; // トリガーイベント
}

[System.Serializable]
public class DrawEffect
{
    public int draw; // ドロー枚数
    public TriggerEvent trigger; // トリガーイベント
}

[System.Serializable]
public class BuffAtkEffect
{
    public int value; // 攻撃力の増加量
    public int target; // 対象ユニット数
    public TriggerEvent trigger; // トリガーイベント
}

[System.Serializable]
public class RemoveEffect
{
    public int DesTarget; // 除外するカード数
    public TargetCondition target_condition; // ターゲット条件
    public TriggerEvent trigger; // トリガーイベント
}

[System.Serializable]
public class TemporaryBuffEffect
{
    public string buff_type; // 強化するタイプ（例：攻撃力、守備力）
    public int value; // 強化量
    public int duration; // 効果が持続するターン数
    public int target; // 対象ユニット数
    public TriggerEvent trigger; // トリガーイベント
}

[System.Serializable]
public class TargetCondition
{
    public LocationType location; // フィールドの場所を表す列挙型
    public CardType type;         // カードタイプ（列挙型を使用）
    public int atk_less_than;     // 攻撃力が指定値未満
    public int atk_greater_than;  // 攻撃力が指定値より大きい
    public int def_less_than;     // 守備力が指定値未満
    public int def_greater_than;  // 守備力が指定値より大きい
}


public enum LocationType
{
    OwnField,         // 自分のフィールド
    OpponentField,    // 相手のフィールド
    AllField          // 全てのフィールド
}