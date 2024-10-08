
# HowTo: CardSetting.txt 設定ガイド

------------------------------ 設定項目一覧 ------------------------------

### name: カード名
### type: カードの種類 (unit, magic, trap など)
### race: カードの種族(利用しなくても良い。その場合はGameSettingでNoRaceをTにしておく。)
### illust: カードのイラスト(png形式が望ましい、使用する画像はImageファイルの中に入れておくこと。) (イラストとして表示されるサイズは185x147。)
### element: カードの属性。利用しなくても良い。ユニットにのみ適用。
### cost: カードをプレイするためのコスト
### rank: カードのランク。利用しなくても良い。
### rarity: カードのレア度。利用しなくても良い。
### atk: カードの攻撃力
### def: カードの守備力
### effect: カードの効果 (複数可)
### DesTarget: n体のターゲットを指定。条件があればオプションで設定
```json
[
    {
        "name": "カード名",
        "type": "カードの種類",
        "race": "カードの種族",
        "illust": "画像名.png",
        "cost": コスト,
        "atk": 攻撃力,
        "def": 守備力,
        "rank": ランク,
        "rarity": レア度,
        "element": "Water",
        "effect": {
            "DesTarget": n,                  # n体のターゲットを指定
            "target_condition": {            # 条件を指定
                "location": "フィールド位置", # 例: "opponent_field", "own_field", "all_field"
                "type": "カードタイプ",       # 例: "unit", "magic", "trap"
                "atk_less_than": 攻撃力,      # 攻撃力が指定値未満
                "atk_greater_than": 攻撃力,   # 攻撃力が指定値より大きい
                "def_less_than": 防御力,      # 防御力が指定値未満
                "def_greater_than": 防御力    # 防御力が指定値より大きい
            },
            "trigger": "発動タイミング"       # 発動タイミングを指定 (例: "play", "attack" など)
        }
    }
]
```
------------------------------ その他の設定可能な効果 ------------------------------

・heal  プレイヤーのライフを回復
記述例

```json
{
    "effect": {
        "Heal": {
            "heal": 5,                     # ライフを5回復
            "trigger": "play"              # プレイ時に発動
        }
    }
}
```

・draw  デッキからカードを引く
記述例

```json
{
    "effect": {
        "Draw": {
            "draw": 2,                     # カードを2枚ドロー
            "trigger": "play"              # プレイ時に発動
        }
    }
}
```

・buff_atk  ユニットの攻撃力を強化
記述例

```json
{
    "effect": {
        "buff_atk": {
            "value": 300,              # 攻撃力を300強化
            "target": 1,               # 1体のユニットを対象
            "trigger": "attack"         # 攻撃時に発動
        }
    }
}
```

・remove  フィールド上のカードを除外
記述例

```json
{
    "effect": {
        "remove": {
            "DesTarget": 1,            # 1体を指定
            "target_condition": {
                "location": "all_field", # すべてのフィールド
                "type": "unit"           # ユニットタイプ
            },
            "trigger": "end"            # ターン終了時に発動
        }
    }
}
```

・temporary_buff  指定ターン数の間、ユニットを強化
記述例

```json
{
    "effect": {
        "temporary_buff": {
            "buff_type": "atk",        # 攻撃力を強化
            "value": 500,              # 500強化
            "duration": 3,             # 3ターンの間
            "target": 1,               # 1体のユニットを対象
            "trigger": "play"          # プレイ時に発動
        }
    }
}
```

------------------------------ 対象に取れるエリア ------------------------------

- OwnField        ・・・ 自分のフィールド
- OpponentField   ・・・ 相手のフィールド
- AllField        ・・・ 全てのフィールド

## TargetConditionの設定項目 ##

- location:     フィールドの場所を表す列挙型
- type:         カードタイプ（列挙型を使用）
- atk_less_than: 攻撃力が指定値未満
- atk_greater_than: 攻撃力が指定値より大きい
- def_less_than: 守備力が指定値未満
- def_greater_than: 守備力が指定値より大きい

------------------------------ 利用可能なElement ------------------------------

- Fire:  火属性, カード枠は赤
- Water:  水属性, カード枠は青
- Earth:  地属性, カード枠は茶色
- Air:    空属性, カード枠は薄い緑
- Light:  光属性, カード枠は黄色
- Dark:   闇属性, カード枠は黒紫

------------------------------ サンプル ------------------------------

以下は、CardSetting.txt のサンプルです。

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

このサンプルでは、名称 "Example Card" のユニットカードが、攻撃力2000、守備力1500を持ち、効果として「召喚時、相手のフィールド上にいる攻撃力2000以下のユニット1体を破壊する」という内容になっています。

※注意事項 
設定項目は必要に応じて省略することができます。省略した場合、デフォルトの値が使用されます。 
各設定は順番に関係なく記述できますが、同じ設定項目を重複して記述しないようにしてください。
