using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CostDistributionGraph : MonoBehaviour
{
    public List<Image> costBars;  // コストごとのバーのリスト（0から11+まで）
    public List<Color> barColors;  // コストごとのバーの色リスト

    private int[] costCounts = new int[12];  // コストごとの枚数カウント

    // デッキ内のコスト分布を更新するメソッド
    public void UpdateCostDistribution(List<Card> deck)
    {
        // コストごとのカウントを初期化
        for (int i = 0; i < costCounts.Length; i++)
        {
            costCounts[i] = 0;
        }

        // カードのコストをカウント
        foreach (var card in deck)
        {
            int costIndex = Mathf.Min(card.cost, 11);  // 11以上は11+として扱う
            costCounts[costIndex]++;
        }

        // 最大のカード枚数を取得
        float maxCount = Mathf.Max(costCounts);

        // カード枚数が0の場合、全てのバーの高さを0にする
        if (maxCount == 0)
        {
            // 全てのバーの高さを0に設定
            for (int i = 0; i < costBars.Count; i++)
            {
                costBars[i].fillAmount = 0;
                costBars[i].color = barColors[i];  // 色を設定
            }
        }
        else
        {
            // 各コストバーの長さと色を更新（最大のカード枚数を1として相対的に調整）
            for (int i = 0; i < costBars.Count; i++)
            {
                // 高さは最大値を1として、相対的にスケーリング
                float normalizedHeight = (float)costCounts[i] / maxCount;
                costBars[i].fillAmount = normalizedHeight;
                costBars[i].color = barColors[i];  // 色を設定
            }
        }
    }
}
