
# カードゲーム設定ガイド

## 目次
1. [GameSetting.txt](#gamesettingtxt)
2. [CardSetting.txt](#cardsettingtxt)

## GameSetting.txt

このファイルでは、ゲームの基本設定を行います。

### 設定項目一覧

1. **ライフの初期値**
   ```
   inLife: 4000
   ```
   - デフォルト: 4000
   - 説明: プレイヤーの初期ライフポイント

2. **デッキの下限枚数**
   ```
   miDeck: 40
   ```
   - デフォルト: 40
   - 説明: デッキの最低枚数
  
3. **同名カードの上限枚数**
   ```
   maxCP: 3
   ```
   - デフォルト: 3
   - 説明: 同じデッキに入れられる同名カードの上限枚数

4. **ターン進行**
   ```
   TurnProgress: [StandbyPhase, DrawPhase, ChargePhase, MainPhase, BattlePhase, EndPhase]
   ```
   - 説明: ゲームの各ターンにおけるフェイズの順番

5. **フィールドの上限枚数**
   ```
   FieldLimit: 5
   ```
   - デフォルト: 5
   - 説明: フィールドに出せるカードの最大数

6. **追加オプション設定 (任意)**
   ```
   NoTargetSelection: T
   FreeSummon: T
   NoRace: T
   ```
   - 説明: ゲーム進行や挙動に関するオプション

### サンプル設定

```
inLife: 4000
miDeck: 40
maxCP: 3
TurnProgress: [StandbyPhase, DrawPhase, ChargePhase, MainPhase, BattlePhase, EndPhase]
FieldLimit: 5
NoTargetSelection: T
FreeSummon: T
```

## CardSetting.txt

このファイルでは、カードの詳細設定を行います。

### 設定項目一覧

- name: カード名
- type: カードの種類 (unit, magic, trap など)
- race: カードの種族
- illust: カードのイラスト
- element: カードの属性
- cost: カードをプレイするためのコスト
- rank: カードのランク
- rarity: カードのレア度
- atk: カードの攻撃力
- def: カードの守備力
- effect: カードの効果
- DesTarget: ターゲット指定

### 効果の種類

1. heal: プレイヤーのライフを回復
2. draw: デッキからカードを引く
3. buff_atk: ユニットの攻撃力を強化
4. remove: フィールド上のカードを除外
5. temporary_buff: 指定ターン数の間、ユニットを強化

### 対象に取れるエリア

- OwnField: 自分のフィールド
- OpponentField: 相手のフィールド
- AllField: 全てのフィールド

### 利用可能なElement

- Fire: 火属性
- Water: 水属性
- Earth: 地属性
- Air: 空属性
- Light: 光属性
- Dark: 闇属性

### サンプル設定

```json
{
    "cards": [
        {
            "name": "戦士",
            "type": "Unit",
            "illust": "warrior.png",
            "cost": 3,
            "atk": 1500,
            "def": 1000,
            "rank": 1,
            "rarity": 3,
            "race": "ヒューマン",
            "element": "Water",
            "effect": {
                "remove": {
                    "DesTarget": 1,
                    "target_condition": {
                        "location": "all_field",
                        "atk_less_than": 2000,
                        "type": "Unit"
                    },
                    "trigger": "play"
                }
            }
        }
    ]
}
```

注意事項：
- 設定項目は必要に応じて省略可能です。省略した場合、デフォルトの値が使用されます。
- 各設定は順番に関係なく記述できますが、同じ設定項目を重複して記述しないでください。
