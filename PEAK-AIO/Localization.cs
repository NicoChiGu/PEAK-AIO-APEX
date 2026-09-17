using System.Collections.Generic;

public enum Language
{
    English,
    SimplifiedChinese,
    Japanese,
    Korean,
    Italian,
    TraditionalChinese
}

public static class Localization
{
    public static Language CurrentLanguage = Language.English;

    private static readonly Dictionary<string, Dictionary<Language, string>> Strings = new Dictionary<string, Dictionary<Language, string>>
    {
        // Sidebar
        { "tab.player", new Dictionary<Language, string> {
            { Language.English, "PLAYER" },
            { Language.SimplifiedChinese, "玩家" },
            { Language.TraditionalChinese, "玩家" },
            { Language.Japanese, "プレイヤー" },
            { Language.Korean, "플레이어" },
            { Language.Italian, "GIOCATORE" }
        }},
        { "tab.items", new Dictionary<Language, string> {
            { Language.English, "ITEMS" },
            { Language.SimplifiedChinese, "物品" },
            { Language.TraditionalChinese, "物品" },
            { Language.Japanese, "アイテム" },
            { Language.Korean, "아이템" },
            { Language.Italian, "OGGETTI" }
        }},
        { "tab.lobby", new Dictionary<Language, string> {
            { Language.English, "LOBBY" },
            { Language.SimplifiedChinese, "大厅" },
            { Language.TraditionalChinese, "大廳" },
            { Language.Japanese, "ロビー" },
            { Language.Korean, "로비" },
            { Language.Italian, "LOBBY" }
        }},
        { "tab.world", new Dictionary<Language, string> {
            { Language.English, "WORLD" },
            { Language.SimplifiedChinese, "世界" },
            { Language.TraditionalChinese, "世界" },
            { Language.Japanese, "ワールド" },
            { Language.Korean, "월드" },
            { Language.Italian, "MONDO" }
        }},
        { "tab.creatures", new Dictionary<Language, string> {
            { Language.English, "CREATURES" },
            { Language.SimplifiedChinese, "生物" },
            { Language.TraditionalChinese, "生物" },
            { Language.Japanese, "クリーチャー" },
            { Language.Korean, "생물" },
            { Language.Italian, "CREATURE" }
        }},
        { "tab.achievements", new Dictionary<Language, string> {
            { Language.English, "ACHIEVE" },
            { Language.SimplifiedChinese, "成就" },
            { Language.TraditionalChinese, "成就" },
            { Language.Japanese, "実績" },
            { Language.Korean, "도전 과제" },
            { Language.Italian, "OBIETTIVI" }
        }},
        { "tab.about", new Dictionary<Language, string> {
            { Language.English, "ABOUT" },
            { Language.SimplifiedChinese, "关于" },
            { Language.TraditionalChinese, "關於" },
            { Language.Japanese, "情報" },
            { Language.Korean, "정보" },
            { Language.Italian, "INFO" }
        }},
        { "tab.language", new Dictionary<Language, string> {
            { Language.English, "LANG" },
            { Language.SimplifiedChinese, "语言" },
            { Language.TraditionalChinese, "語言" },
            { Language.Japanese, "言語" },
            { Language.Korean, "언어" },
            { Language.Italian, "LINGUA" }
        }},

         { "tab.debug", new Dictionary<Language, string> {
            { Language.English, "Debug" },
            { Language.SimplifiedChinese, "Debug" },
            { Language.TraditionalChinese, "Debug" },
            { Language.Japanese, "Debug" },
            { Language.Korean, "Debug" },
            { Language.Italian, "Debug" }
        }},

        // Player tab - Self Mods
        { "player.selfmods", new Dictionary<Language, string> {
            { Language.English, "Self Mods" },
            { Language.SimplifiedChinese, "自身模组" },
            { Language.TraditionalChinese, "自身模組" },
            { Language.Japanese, "セルフMOD" },
            { Language.Korean, "셀프 모드" },
            { Language.Italian, "Mod Personali" }
        }},
        { "player.infinite_stamina", new Dictionary<Language, string> {
            { Language.English, "Infinite Stamina" },
            { Language.SimplifiedChinese, "无限体力" },
            { Language.TraditionalChinese, "無限體力" },
            { Language.Japanese, "無限スタミナ" },
            { Language.Korean, "무한 스태미나" },
            { Language.Italian, "Stamina Infinita" }
        }},
        { "player.freeze_afflictions", new Dictionary<Language, string> {
            { Language.English, "Freeze Afflictions" },
            { Language.SimplifiedChinese, "冻结状态异常" },
            { Language.TraditionalChinese, "凍結狀態異常" },
            { Language.Japanese, "状態異常凍結" },
            { Language.Korean, "상태이상 동결" },
            { Language.Italian, "Blocca Afflizioni" }
        }},
        { "player.no_weight", new Dictionary<Language, string> {
            { Language.English, "No Weight" },
            { Language.SimplifiedChinese, "无重量" },
            { Language.TraditionalChinese, "無重量" },
            { Language.Japanese, "重量無効" },
            { Language.Korean, "무게 무시" },
            { Language.Italian, "Nessun Peso" }
        }},
        { "player.change_speed", new Dictionary<Language, string> {
            { Language.English, "Change Speed" },
            { Language.SimplifiedChinese, "修改速度" },
            { Language.TraditionalChinese, "修改速度" },
            { Language.Japanese, "速度変更" },
            { Language.Korean, "속도 변경" },
            { Language.Italian, "Modifica Velocità" }
        }},
        { "player.change_jump", new Dictionary<Language, string> {
            { Language.English, "Change Jump" },
            { Language.SimplifiedChinese, "修改跳跃" },
            { Language.TraditionalChinese, "修改跳躍" },
            { Language.Japanese, "ジャンプ変更" },
            { Language.Korean, "점프 변경" },
            { Language.Italian, "Modifica Salto" }
        }},
        { "player.change_climb", new Dictionary<Language, string> {
            { Language.English, "Change Climb" },
            { Language.SimplifiedChinese, "修改攀爬" },
            { Language.TraditionalChinese, "修改攀爬" },
            { Language.Japanese, "登攀速度変更" },
            { Language.Korean, "등반 변경" },
            { Language.Italian, "Modifica Arrampicata" }
        }},
        { "player.change_vine_climb", new Dictionary<Language, string> {
            { Language.English, "Change Vine Climb" },
            { Language.SimplifiedChinese, "修改藤蔓攀爬" },
            { Language.TraditionalChinese, "修改藤蔓攀爬" },
            { Language.Japanese, "ツル登攀変更" },
            { Language.Korean, "덩굴 등반 변경" },
            { Language.Italian, "Modifica Arrampicata su Liane" }
        }},
        { "player.change_rope_climb", new Dictionary<Language, string> {
            { Language.English, "Change Rope Climb" },
            { Language.SimplifiedChinese, "修改绳索攀爬" },
            { Language.TraditionalChinese, "修改繩索攀爬" },
            { Language.Japanese, "ロープ登攀変更" },
            { Language.Korean, "로프 등반 변경" },
            { Language.Italian, "Modifica Arrampicata su Corda" }
        }},
        { "player.teleport_to_ping", new Dictionary<Language, string> {
            { Language.English, "Teleport to Ping" },
            { Language.SimplifiedChinese, "传送到标记点" },
            { Language.TraditionalChinese, "傳送到標記點" },
            { Language.Japanese, "ピンに移動" },
            { Language.Korean, "핑으로 텔레포트" },
            { Language.Italian, "Teletrasporto al Ping" }
        }},
        { "player.fly_mode", new Dictionary<Language, string> {
            { Language.English, "Fly Mode" },
            { Language.SimplifiedChinese, "飞行模式" },
            { Language.TraditionalChinese, "飛行模式" },
            { Language.Japanese, "飛行モード" },
            { Language.Korean, "비행 모드" },
            { Language.Italian, "Modalità Volo" }
        }},
        { "player.no_fall_dmg", new Dictionary<Language, string> {
            { Language.English, "No Fall Dmg" },
            { Language.SimplifiedChinese, "无坠落伤害" },
            { Language.TraditionalChinese, "無墜落傷害" },
            { Language.Japanese, "落下ダメージ無効" },
            { Language.Korean, "낙하 피해 없음" },
            { Language.Italian, "Nessun Danno da Caduta" }
        }},

        // Player tab - Teleport
        { "player.teleport", new Dictionary<Language, string> {
            { Language.English, "Teleport" },
            { Language.SimplifiedChinese, "传送" },
            { Language.TraditionalChinese, "傳送" },
            { Language.Japanese, "テレポート" },
            { Language.Korean, "텔레포트" },
            { Language.Italian, "Teletrasporto" }
        }},
        { "player.teleport_to_coords", new Dictionary<Language, string> {
            { Language.English, "Teleport to coords" },
            { Language.SimplifiedChinese, "传送到坐标" },
            { Language.TraditionalChinese, "傳送到座標" },
            { Language.Japanese, "座標にテレポート" },
            { Language.Korean, "좌표로 텔레포트" },
            { Language.Italian, "Teletrasporto alle coordinate" }
        }},

        // Player tab - Details
        { "player.details", new Dictionary<Language, string> {
            { Language.English, "Details" },
            { Language.SimplifiedChinese, "详细设置" },
            { Language.TraditionalChinese, "詳細設定" },
            { Language.Japanese, "詳細設定" },
            { Language.Korean, "상세 설정" },
            { Language.Italian, "Dettagli" }
        }},
        { "player.jump_mult", new Dictionary<Language, string> {
            { Language.English, "Jump Mult: %.2f" },
            { Language.SimplifiedChinese, "跳跃倍率: %.2f" },
            { Language.TraditionalChinese, "跳躍倍率: %.2f" },
            { Language.Japanese, "ジャンプ倍率: %.2f" },
            { Language.Korean, "점프 배율: %.2f" },
            { Language.Italian, "Moltiplicatore Salto: %.2f" }
        }},
        { "player.move_speed", new Dictionary<Language, string> {
            { Language.English, "Move Speed: %.2f" },
            { Language.SimplifiedChinese, "移动速度: %.2f" },
            { Language.TraditionalChinese, "移動速度: %.2f" },
            { Language.Japanese, "移動速度: %.2f" },
            { Language.Korean, "이동 속도: %.2f" },
            { Language.Italian, "Velocità Movimento: %.2f" }
        }},
        { "player.climb_speed", new Dictionary<Language, string> {
            { Language.English, "Climb Speed: %.2f" },
            { Language.SimplifiedChinese, "攀爬速度: %.2f" },
            { Language.TraditionalChinese, "攀爬速度: %.2f" },
            { Language.Japanese, "登攀速度: %.2f" },
            { Language.Korean, "등반 속도: %.2f" },
            { Language.Italian, "Velocità Arrampicata: %.2f" }
        }},
        { "player.vine_speed", new Dictionary<Language, string> {
            { Language.English, "Vine Speed: %.2f" },
            { Language.SimplifiedChinese, "藤蔓速度: %.2f" },
            { Language.TraditionalChinese, "藤蔓速度: %.2f" },
            { Language.Japanese, "ツル速度: %.2f" },
            { Language.Korean, "덩굴 속도: %.2f" },
            { Language.Italian, "Velocità Liane: %.2f" }
        }},
        { "player.rope_speed", new Dictionary<Language, string> {
            { Language.English, "Rope Speed: %.2f" },
            { Language.SimplifiedChinese, "绳索速度: %.2f" },
            { Language.TraditionalChinese, "繩索速度: %.2f" },
            { Language.Japanese, "ロープ速度: %.2f" },
            { Language.Korean, "로프 속도: %.2f" },
            { Language.Italian, "Velocità Corda: %.2f" }
        }},
        { "player.fly_speed", new Dictionary<Language, string> {
            { Language.English, "Fly Speed: %.2f" },
            { Language.SimplifiedChinese, "飞行速度: %.2f" },
            { Language.TraditionalChinese, "飛行速度: %.2f" },
            { Language.Japanese, "飛行速度: %.2f" },
            { Language.Korean, "비행 속도: %.2f" },
            { Language.Italian, "Velocità Volo: %.2f" }
        }},
        { "player.fly_acceleration", new Dictionary<Language, string> {
            { Language.English, "Fly Acceleration: %.2f" },
            { Language.SimplifiedChinese, "飞行加速度: %.2f" },
            { Language.TraditionalChinese, "飛行加速度: %.2f" },
            { Language.Japanese, "飛行加速度: %.2f" },
            { Language.Korean, "비행 가속도: %.2f" },
            { Language.Italian, "Accelerazione Volo: %.2f" }
        }},
        { "player.spawn_backpack", new Dictionary<Language, string> {
            { Language.English, "Spawn Backpack" },
            { Language.SimplifiedChinese, "生成背包" },
            { Language.TraditionalChinese, "生成背包" },
            { Language.Japanese, "生成バックパック" },
            { Language.Korean, "생성 가방" },
            { Language.Italian, "Generare Zaino" }
        }},

        // Tooltips - Player
        { "tip.infinite_stamina", new Dictionary<Language, string> {
            { Language.English, "Prevents stamina from decreasing, allowing unlimited sprinting and actions." },
            { Language.SimplifiedChinese, "防止体力下降，允许无限冲刺和执行动作。" },
            { Language.TraditionalChinese, "防止體力下降，允許無限衝刺和執行動作。" },
            { Language.Japanese, "スタミナの減少を防ぎ、無制限のダッシュとアクションを可能にします。" },
            { Language.Korean, "스태미나 감소를 방지하여 무제한 달리기와 행동이 가능합니다." },
            { Language.Italian, "Impedisce alla stamina di diminuire, permettendo scatti e azioni illimitate." }
        }},
        { "tip.freeze_afflictions", new Dictionary<Language, string> {
            { Language.English, "Prevents your statuses from changing." },
            { Language.SimplifiedChinese, "防止你的状态发生变化。" },
            { Language.TraditionalChinese, "防止你的狀態發生變化。" },
            { Language.Japanese, "ステータスの変化を防ぎます。" },
            { Language.Korean, "상태 변화를 방지합니다." },
            { Language.Italian, "Impedisce ai tuoi stati di cambiare." }
        }},
        { "tip.no_weight", new Dictionary<Language, string> {
            { Language.English, "Disables weight penalties from carried items and backpack." },
            { Language.SimplifiedChinese, "禁用携带物品和背包的重量惩罚。" },
            { Language.TraditionalChinese, "禁用攜帶物品和背包的重量懲罰。" },
            { Language.Japanese, "所持アイテムやバックパックの重量ペナルティを無効にします。" },
            { Language.Korean, "소지품 및 배낭의 무게 패널티를 비활성화합니다." },
            { Language.Italian, "Disabilita le penalità di peso dagli oggetti trasportati e dallo zaino." }
        }},
        { "tip.change_speed", new Dictionary<Language, string> {
            { Language.English, "Overrides your character's movement speed with a custom multiplier." },
            { Language.SimplifiedChinese, "使用自定义倍率覆盖角色的移动速度。" },
            { Language.TraditionalChinese, "使用自訂倍率覆蓋角色的移動速度。" },
            { Language.Japanese, "キャラクターの移動速度をカスタム倍率で上書きします。" },
            { Language.Korean, "캐릭터의 이동 속도를 사용자 정의 배율로 변경합니다." },
            { Language.Italian, "Sostituisce la velocità di movimento del personaggio con un moltiplicatore personalizzato." }
        }},
        { "tip.change_jump", new Dictionary<Language, string> {
            { Language.English, "Modifies jump height, allowing higher or lower jumps depending on your settings." },
            { Language.SimplifiedChinese, "修改跳跃高度，根据设置允许更高或更低的跳跃。" },
            { Language.TraditionalChinese, "修改跳躍高度，根據設定允許更高或更低的跳躍。" },
            { Language.Japanese, "ジャンプの高さを変更し、設定に応じて高くまたは低くジャンプできます。" },
            { Language.Korean, "점프 높이를 수정하여 설정에 따라 더 높거나 낮게 점프할 수 있습니다." },
            { Language.Italian, "Modifica l'altezza del salto, permettendo salti più alti o più bassi in base alle impostazioni." }
        }},
        { "tip.change_climb", new Dictionary<Language, string> {
            { Language.English, "Adjusts the speed at which you climb ladders and surfaces." },
            { Language.SimplifiedChinese, "调整攀爬梯子和表面的速度。" },
            { Language.TraditionalChinese, "調整攀爬梯子和表面的速度。" },
            { Language.Japanese, "はしごや壁を登る速度を調整します。" },
            { Language.Korean, "사다리와 표면을 오르는 속도를 조정합니다." },
            { Language.Italian, "Regola la velocità con cui ti arrampichi su scale e superfici." }
        }},
        { "tip.change_vine_climb", new Dictionary<Language, string> {
            { Language.English, "Changes climbing speed specifically for vines." },
            { Language.SimplifiedChinese, "专门修改藤蔓的攀爬速度。" },
            { Language.TraditionalChinese, "專門修改藤蔓的攀爬速度。" },
            { Language.Japanese, "ツルの登攀速度を変更します。" },
            { Language.Korean, "덩굴 등반 속도를 변경합니다." },
            { Language.Italian, "Modifica la velocità di arrampicata per le liane." }
        }},
        { "tip.change_rope_climb", new Dictionary<Language, string> {
            { Language.English, "Modifies climbing speed when using ropes or rope-based obstacles." },
            { Language.SimplifiedChinese, "修改使用绳索或绳索障碍物时的攀爬速度。" },
            { Language.TraditionalChinese, "修改使用繩索或繩索障礙物時的攀爬速度。" },
            { Language.Japanese, "ロープやロープ系障害物での登攀速度を変更します。" },
            { Language.Korean, "로프 또는 로프 장애물 사용 시 등반 속도를 수정합니다." },
            { Language.Italian, "Modifica la velocità di arrampicata quando si usano corde o ostacoli basati su corde." }
        }},
        { "tip.teleport_to_ping", new Dictionary<Language, string> {
            { Language.English, "Teleports your character to the pinged location on the map." },
            { Language.SimplifiedChinese, "将角色传送到地图上的标记位置。" },
            { Language.TraditionalChinese, "將角色傳送到地圖上的標記位置。" },
            { Language.Japanese, "マップ上のピンの位置にキャラクターをテレポートします。" },
            { Language.Korean, "캐릭터를 맵에서 핑한 위치로 텔레포트합니다." },
            { Language.Italian, "Teletrasporta il tuo personaggio alla posizione del ping sulla mappa." }
        }},
        { "tip.fly_mode", new Dictionary<Language, string> {
            { Language.English, "Allows free movement in all directions while ignoring gravity." },
            { Language.SimplifiedChinese, "允许忽略重力在所有方向自由移动。" },
            { Language.TraditionalChinese, "允許忽略重力在所有方向自由移動。" },
            { Language.Japanese, "重力を無視して全方向に自由移動できます。" },
            { Language.Korean, "중력을 무시하고 모든 방향으로 자유 이동이 가능합니다." },
            { Language.Italian, "Permette il movimento libero in tutte le direzioni ignorando la gravità." }
        }},

        // Items tab
        { "items.slot", new Dictionary<Language, string> {
            { Language.English, "Slot" },
            { Language.SimplifiedChinese, "槽位" },
            { Language.TraditionalChinese, "槽位" },
            { Language.Japanese, "スロット" },
            { Language.Korean, "슬롯" },
            { Language.Italian, "Slot" }
        }},
        { "items.item_n", new Dictionary<Language, string> {
            { Language.English, "Item {0}:" },
            { Language.SimplifiedChinese, "物品 {0}:" },
            { Language.TraditionalChinese, "物品 {0}:" },
            { Language.Japanese, "アイテム {0}:" },
            { Language.Korean, "아이템 {0}:" },
            { Language.Italian, "Oggetto {0}:" }
        }},
        { "items.none", new Dictionary<Language, string> {
            { Language.English, "None" },
            { Language.SimplifiedChinese, "无" },
            { Language.TraditionalChinese, "無" },
            { Language.Japanese, "なし" },
            { Language.Korean, "없음" },
            { Language.Italian, "Nessuno" }
        }},
        { "items.search", new Dictionary<Language, string> {
            { Language.English, "Search items..." },
            { Language.SimplifiedChinese, "搜索物品..." },
            { Language.TraditionalChinese, "搜尋物品..." },
            { Language.Japanese, "アイテム検索..." },
            { Language.Korean, "아이템 검색..." },
            { Language.Italian, "Cerca oggetti..." }
        }},
        { "items.charge_format", new Dictionary<Language, string> {
            { Language.English, "Charge: %.1f" },
            { Language.SimplifiedChinese, "充能: %.1f" },
            { Language.TraditionalChinese, "充能: %.1f" },
            { Language.Japanese, "チャージ: %.1f" },
            { Language.Korean, "충전: %.1f" },
            { Language.Italian, "Carica: %.1f" }
        }},
        { "items.recharge", new Dictionary<Language, string> {
            { Language.English, "Recharge" },
            { Language.SimplifiedChinese, "充能" },
            { Language.TraditionalChinese, "充能" },
            { Language.Japanese, "チャージ" },
            { Language.Korean, "충전" },
            { Language.Italian, "Ricarica" }
        }},
        { "items.refresh", new Dictionary<Language, string> {
            { Language.English, "Refresh Item List" },
            { Language.SimplifiedChinese, "刷新物品列表" },
            { Language.TraditionalChinese, "重新整理物品列表" },
            { Language.Japanese, "アイテムリスト更新" },
            { Language.Korean, "아이템 목록 새로고침" },
            { Language.Italian, "Aggiorna Lista Oggetti" }
        }},

        { "items.equip_item", new Dictionary<Language, string> {
            { Language.English, "Equip to Slot" },
            { Language.SimplifiedChinese, "装备至该槽位" },
            { Language.TraditionalChinese, "裝備至該槽位" },
            { Language.Japanese, "スロットに装備" },
            { Language.Korean, "슬롯에 장착" },
            { Language.Italian, "Equipaggia nello slot" }
        }},
        { "tip.item_search", new Dictionary<Language, string> {
            { Language.English, "Search and assign any available item to this slot." },
            { Language.SimplifiedChinese, "搜索并分配任何可用物品到此槽位。" },
            { Language.TraditionalChinese, "搜尋並分配任何可用物品到此槽位。" },
            { Language.Japanese, "利用可能なアイテムを検索してこのスロットに割り当てます。" },
            { Language.Korean, "사용 가능한 아이템을 검색하여 이 슬롯에 할당합니다." },
            { Language.Italian, "Cerca e assegna qualsiasi oggetto disponibile a questo slot." }
        }},
        { "tip.recharge", new Dictionary<Language, string> {
            { Language.English, "Set how much to recharge the item's charges when clicking 'Recharge'." },
            { Language.SimplifiedChinese, "设置点击「充能」时为物品充能的数量。" },
            { Language.TraditionalChinese, "設定點擊「充能」時為物品充能的數量。" },
            { Language.Japanese, "「チャージ」クリック時のチャージ量を設定します。" },
            { Language.Korean, "'충전' 클릭 시 아이템 충전량을 설정합니다." },
            { Language.Italian, "Imposta quanta carica aggiungere all'oggetto quando clicchi 'Ricarica'." }
        }},
        { "tip.refresh_items", new Dictionary<Language, string> {
            { Language.English, "Reloads the list of available items in case something was missed or updated." },
            { Language.SimplifiedChinese, "重新加载可用物品列表，以防遗漏或更新。" },
            { Language.TraditionalChinese, "重新載入可用物品列表，以防遺漏或更新。" },
            { Language.Japanese, "見落としや更新に備え、利用可能なアイテムリストを再読み込みします。" },
            { Language.Korean, "누락되거나 업데이트된 경우 사용 가능한 아이템 목록을 다시 불러옵니다." },
            { Language.Italian, "Ricarica l'elenco degli oggetti disponibili nel caso qualcosa sia stato perso o aggiornato." }
        }},
        { "items.loaded_count", new Dictionary<Language, string> {
            { Language.English, "Loaded Items: {0}" },
            { Language.SimplifiedChinese, "已加载物品数: {0}" },
            { Language.TraditionalChinese, "已加載物品數: {0}" },
            { Language.Japanese, "ロード済みアイテム: {0}" },
            { Language.Korean, "로드된 아이템: {0}" },
            { Language.Italian, "Oggetti Caricati: {0}" }
        }},
        { "items.current", new Dictionary<Language, string> {
            { Language.English, "Current" },
            { Language.SimplifiedChinese, "当前" },
            { Language.TraditionalChinese, "當前" },
            { Language.Japanese, "現在" },
            { Language.Korean, "현재" },
            { Language.Italian, "Attuale" }
        }},
        { "items.spawn_item", new Dictionary<Language, string> {
            { Language.English, "Spawn in World" },
            { Language.SimplifiedChinese, "生成到世界" },
            { Language.TraditionalChinese, "生成到世界" },
            { Language.Japanese, "ワールドに出現" },
            { Language.Korean, "월드에 소환" },
            { Language.Italian, "Genera nel Mondo" }
        }},
        { "items.none_available", new Dictionary<Language, string> {
            { Language.English, "No items available" },
            { Language.SimplifiedChinese, "无可用物品" },
            { Language.TraditionalChinese, "無可用物品" },
            { Language.Japanese, "利用可能なアイテムなし" },
            { Language.Korean, "사용 가능한 아이템 없음" },
            { Language.Italian, "Nessun oggetto disponibile" }
        }},
        { "items.no_matches", new Dictionary<Language, string> {
            { Language.English, "No matches" },
            { Language.SimplifiedChinese, "无匹配项" },
            { Language.TraditionalChinese, "無相符項" },
            { Language.Japanese, "一致なし" },
            { Language.Korean, "일치 항목 없음" },
            { Language.Italian, "Nessuna corrispondenza" }
        }},
        { "items.empty_notice", new Dictionary<Language, string> {
            { Language.English, "No item data detected. If you are in the main menu, load into a game level first, or click 'Refresh Item List'." },
            { Language.SimplifiedChinese, "当前未检测到物品数据。若处于主菜单，请先进入游戏关卡，或点击「刷新物品列表」。" },
            { Language.TraditionalChinese, "當前未檢測到物品數據。若處於主菜單，請先進入遊戲關卡，或點擊「重新整理物品列表」。" },
            { Language.Japanese, "アイテムデータが見つかりません。メインメニューの場合はゲームに参加するか、「アイテムリスト更新」を押してください。" },
            { Language.Korean, "아이템 데이터를 감지할 수 없습니다. 메인 메뉴라면 게임에 진입하거나 '새로고침'을 클릭하세요." },
            { Language.Italian, "Nessun dato di oggetti rilevato. Se sei nel menu principale, avvia una partita o clicca 'Aggiorna Lista Oggetti'." }
        }},
        { "items.empty_tip", new Dictionary<Language, string> {
            { Language.English, "Item assets will be automatically loaded into the database when game level initializes." },
            { Language.SimplifiedChinese, "游戏关卡初始化时，物品数据库将自动收集所有游戏预制体。" },
            { Language.TraditionalChinese, "遊戲關卡初始化時，物品數據庫將自動收集所有遊戲預製體。" },
            { Language.Japanese, "ゲームレベル開始時にアイテムデータベースが自動的にロードされます。" },
            { Language.Korean, "게임 레벨이 시작되면 아이템 데이터베이스가 자동으로 로드됩니다." },
            { Language.Italian, "I dati degli oggetti verranno caricati automaticamente all'avvio della partita." }
        }},

        // Lobby tab
        { "lobby.players", new Dictionary<Language, string> {
            { Language.English, "Lobby Players" },
            { Language.SimplifiedChinese, "大厅玩家" },
            { Language.TraditionalChinese, "大廳玩家" },
            { Language.Japanese, "ロビープレイヤー" },
            { Language.Korean, "로비 플레이어" },
            { Language.Italian, "Giocatori nella Lobby" }
        }},
        { "lobby.select_player", new Dictionary<Language, string> {
            { Language.English, "Select Player" },
            { Language.SimplifiedChinese, "选择玩家" },
            { Language.TraditionalChinese, "選擇玩家" },
            { Language.Japanese, "プレイヤー選択" },
            { Language.Korean, "플레이어 선택" },
            { Language.Italian, "Seleziona Giocatore" }
        }},
        { "lobby.all_players", new Dictionary<Language, string> {
            { Language.English, "All Players" },
            { Language.SimplifiedChinese, "所有玩家" },
            { Language.TraditionalChinese, "所有玩家" },
            { Language.Japanese, "全プレイヤー" },
            { Language.Korean, "모든 플레이어" },
            { Language.Italian, "Tutti i Giocatori" }
        }},
        { "lobby.revive_all", new Dictionary<Language, string> {
            { Language.English, "Revive All" },
            { Language.SimplifiedChinese, "复活全部" },
            { Language.TraditionalChinese, "復活全部" },
            { Language.Japanese, "全員復活" },
            { Language.Korean, "전원 부활" },
            { Language.Italian, "Rianima Tutti" }
        }},
        { "lobby.kill_all", new Dictionary<Language, string> {
            { Language.English, "Kill All" },
            { Language.SimplifiedChinese, "击杀全部" },
            { Language.TraditionalChinese, "擊殺全部" },
            { Language.Japanese, "全員キル" },
            { Language.Korean, "전원 처치" },
            { Language.Italian, "Uccidi Tutti" }
        }},
        { "lobby.exclude_self", new Dictionary<Language, string> {
            { Language.English, "Exclude Self from Kill All" },
            { Language.SimplifiedChinese, "击杀全部时排除自己" },
            { Language.TraditionalChinese, "擊殺全部時排除自己" },
            { Language.Japanese, "全員キルから自分を除外" },
            { Language.Korean, "전원 처치에서 자신 제외" },
            { Language.Italian, "Escludi Te Stesso da Uccidi Tutti" }
        }},
        { "lobby.warp_all_to_me", new Dictionary<Language, string> {
            { Language.English, "Warp All To Me" },
            { Language.SimplifiedChinese, "将所有人传送到我身边" },
            { Language.TraditionalChinese, "將所有人傳送到我身邊" },
            { Language.Japanese, "全員を自分の元へ" },
            { Language.Korean, "전원 내게 워프" },
            { Language.Italian, "Teletrasporta Tutti da Me" }
        }},
        { "lobby.refresh_players", new Dictionary<Language, string> {
            { Language.English, "Refresh Players List" },
            { Language.SimplifiedChinese, "刷新玩家列表" },
            { Language.TraditionalChinese, "重新整理玩家列表" },
            { Language.Japanese, "プレイヤーリスト更新" },
            { Language.Korean, "플레이어 목록 새로고침" },
            { Language.Italian, "Aggiorna Lista Giocatori" }
        }},
        { "tip.refresh_players", new Dictionary<Language, string> {
            { Language.English, "Manually reloads the list of players in case it didn't update automatically." },
            { Language.SimplifiedChinese, "手动重新加载玩家列表，以防未自动更新。" },
            { Language.TraditionalChinese, "手動重新載入玩家列表，以防未自動更新。" },
            { Language.Japanese, "自動更新されなかった場合に手動でプレイヤーリストを再読み込みします。" },
            { Language.Korean, "자동 업데이트되지 않은 경우 수동으로 플레이어 목록을 다시 불러옵니다." },
            { Language.Italian, "Ricarica manualmente l'elenco dei giocatori nel caso non si sia aggiornato automaticamente." }
        }},
        { "lobby.actions", new Dictionary<Language, string> {
            { Language.English, "Actions" },
            { Language.SimplifiedChinese, "操作" },
            { Language.TraditionalChinese, "操作" },
            { Language.Japanese, "アクション" },
            { Language.Korean, "행동" },
            { Language.Italian, "Azioni" }
        }},
        { "lobby.revive", new Dictionary<Language, string> {
            { Language.English, "Revive" },
            { Language.SimplifiedChinese, "复活" },
            { Language.TraditionalChinese, "復活" },
            { Language.Japanese, "復活" },
            { Language.Korean, "부활" },
            { Language.Italian, "Rianima" }
        }},
        { "lobby.kill", new Dictionary<Language, string> {
            { Language.English, "Kill" },
            { Language.SimplifiedChinese, "击杀" },
            { Language.TraditionalChinese, "擊殺" },
            { Language.Japanese, "キル" },
            { Language.Korean, "처치" },
            { Language.Italian, "Uccidi" }
        }},
        { "lobby.warp_to", new Dictionary<Language, string> {
            { Language.English, "Warp To" },
            { Language.SimplifiedChinese, "传送至" },
            { Language.TraditionalChinese, "傳送至" },
            { Language.Japanese, "ワープ" },
            { Language.Korean, "워프" },
            { Language.Italian, "Teletrasporta a" }
        }},
        { "lobby.warp_to_me", new Dictionary<Language, string> {
            { Language.English, "Warp To Me" },
            { Language.SimplifiedChinese, "传送到我身边" },
            { Language.TraditionalChinese, "傳送到我身邊" },
            { Language.Japanese, "自分の元へ" },
            { Language.Korean, "내게 워프" },
            { Language.Italian, "Teletrasporta da Me" }
        }},
        { "lobby.special_actions", new Dictionary<Language, string> {
            { Language.English, "Special Actions" },
            { Language.SimplifiedChinese, "特殊操作" },
            { Language.TraditionalChinese, "特殊操作" },
            { Language.Japanese, "特殊アクション" },
            { Language.Korean, "특수 행동" },
            { Language.Italian, "Azioni Speciali" }
        }},
        { "lobby.spawn_scoutmaster", new Dictionary<Language, string> {
            { Language.English, "Spawn Scoutmaster" },
            { Language.SimplifiedChinese, "生成Scoutmaster" },
            { Language.TraditionalChinese, "生成Scoutmaster" },
            { Language.Japanese, "スカウトマスター召喚" },
            { Language.Korean, "스카우트마스터 소환" },
            { Language.Italian, "Genera Scoutmaster" }
        }},
        { "tip.spawn_scoutmaster", new Dictionary<Language, string> {
            { Language.English, "Spawns a Scoutmaster near the selected player. Only works for host. Forces aggro." },
            { Language.SimplifiedChinese, "在选定玩家附近生成Scoutmaster。仅限房主使用。强制仇恨。" },
            { Language.TraditionalChinese, "在選定玩家附近生成Scoutmaster。僅限房主使用。強制仇恨。" },
            { Language.Japanese, "選択したプレイヤーの近くにスカウトマスターを召喚します。ホスト専用。アグロ強制。" },
            { Language.Korean, "선택한 플레이어 근처에 스카우트마스터를 소환합니다. 호스트 전용. 어그로 강제." },
            { Language.Italian, "Genera uno Scoutmaster vicino al giocatore selezionato. Funziona solo per l'host. Forza l'aggro." }
        }},
        { "lobby.no_player_selected", new Dictionary<Language, string> {
            { Language.English, "No player selected." },
            { Language.SimplifiedChinese, "未选择玩家。" },
            { Language.TraditionalChinese, "未選擇玩家。" },
            { Language.Japanese, "プレイヤーが選択されていません。" },
            { Language.Korean, "플레이어가 선택되지 않았습니다." },
            { Language.Italian, "Nessun giocatore selezionato." }
        }},

        // World tab
        { "world.containers", new Dictionary<Language, string> {
            { Language.English, "Containers" },
            { Language.SimplifiedChinese, "容器" },
            { Language.TraditionalChinese, "容器" },
            { Language.Japanese, "コンテナ" },
            { Language.Korean, "컨테이너" },
            { Language.Italian, "Contenitori" }
        }},
        { "world.select_container", new Dictionary<Language, string> {
            { Language.English, "Select Container" },
            { Language.SimplifiedChinese, "选择容器" },
            { Language.TraditionalChinese, "選擇容器" },
            { Language.Japanese, "コンテナ選択" },
            { Language.Korean, "컨테이너 선택" },
            { Language.Italian, "Seleziona Contenitore" }
        }},
        { "world.no_containers", new Dictionary<Language, string> {
            { Language.English, "No containers found." },
            { Language.SimplifiedChinese, "未找到容器。" },
            { Language.TraditionalChinese, "未找到容器。" },
            { Language.Japanese, "コンテナが見つかりません。" },
            { Language.Korean, "컨테이너를 찾을 수 없습니다." },
            { Language.Italian, "Nessun contenitore trovato." }
        }},
        { "world.refresh_luggage", new Dictionary<Language, string> {
            { Language.English, "Refresh Luggage List" },
            { Language.SimplifiedChinese, "刷新行李列表" },
            { Language.TraditionalChinese, "重新整理行李列表" },
            { Language.Japanese, "荷物リスト更新" },
            { Language.Korean, "수하물 목록 새로고침" },
            { Language.Italian, "Aggiorna Lista Bagagli" }
        }},
        { "tip.refresh_luggage", new Dictionary<Language, string> {
            { Language.English, "Reloads the list of luggage within 300m of your position." },
            { Language.SimplifiedChinese, "重新加载你位置300米内的行李列表。" },
            { Language.TraditionalChinese, "重新載入你位置300公尺內的行李列表。" },
            { Language.Japanese, "現在地から300m以内の荷物リストを再読み込みします。" },
            { Language.Korean, "현재 위치에서 300m 이내의 수하물 목록을 다시 불러옵니다." },
            { Language.Italian, "Ricarica l'elenco dei bagagli entro 300m dalla tua posizione." }
        }},
        { "world.all_nearby", new Dictionary<Language, string> {
            { Language.English, "All Nearby Containers" },
            { Language.SimplifiedChinese, "附近所有容器" },
            { Language.TraditionalChinese, "附近所有容器" },
            { Language.Japanese, "近くの全コンテナ" },
            { Language.Korean, "주변 모든 컨테이너" },
            { Language.Italian, "Tutti i Contenitori Vicini" }
        }},
        { "world.open_all_nearby", new Dictionary<Language, string> {
            { Language.English, "Open All Nearby" },
            { Language.SimplifiedChinese, "打开附近所有" },
            { Language.TraditionalChinese, "打開附近所有" },
            { Language.Japanese, "近くを全て開く" },
            { Language.Korean, "주변 모두 열기" },
            { Language.Italian, "Apri Tutti i Vicini" }
        }},
        { "world.warp_to_luggage", new Dictionary<Language, string> {
            { Language.English, "Warp To Luggage" },
            { Language.SimplifiedChinese, "传送到行李" },
            { Language.TraditionalChinese, "傳送到行李" },
            { Language.Japanese, "荷物にワープ" },
            { Language.Korean, "수하물로 워프" },
            { Language.Italian, "Teletrasporta al Bagaglio" }
        }},
        { "world.open_luggage", new Dictionary<Language, string> {
            { Language.English, "Open Luggage" },
            { Language.SimplifiedChinese, "打开行李" },
            { Language.TraditionalChinese, "打開行李" },
            { Language.Japanese, "荷物を開く" },
            { Language.Korean, "수하물 열기" },
            { Language.Italian, "Apri Bagaglio" }
        }},
        { "world.no_luggage_selected", new Dictionary<Language, string> {
            { Language.English, "No luggage selected." },
            { Language.SimplifiedChinese, "未选择行李。" },
            { Language.TraditionalChinese, "未選擇行李。" },
            { Language.Japanese, "荷物が選択されていません。" },
            { Language.Korean, "수하물이 선택되지 않았습니다." },
            { Language.Italian, "Nessun bagaglio selezionato." }
        }},

        // About tab
        { "about.title", new Dictionary<Language, string> {
            { Language.English, "PEAK AIO Mod" },
            { Language.SimplifiedChinese, "PEAK AIO 模组" },
            { Language.TraditionalChinese, "PEAK AIO 模組" },
            { Language.Japanese, "PEAK AIO MOD" },
            { Language.Korean, "PEAK AIO 모드" },
            { Language.Italian, "PEAK AIO Mod" }
        }},
        { "about.version", new Dictionary<Language, string> {
            { Language.English, "Version: 1.1.1" },
            { Language.SimplifiedChinese, "版本: 1.1.1" },
            { Language.TraditionalChinese, "版本: 1.1.1" },
            { Language.Japanese, "バージョン: 1.1.1" },
            { Language.Korean, "버전: 1.1.1" },
            { Language.Italian, "Versione: 1.1.1" }
        }},
        { "about.author", new Dictionary<Language, string> {
            { Language.English, "Author: K1rito (Inspired by OniGremlin)" },
            { Language.SimplifiedChinese, "作者: K1rito (启发于OniGremlin)" },
            { Language.TraditionalChinese, "作者: K1rito (啟發於OniGremlin)" },
            { Language.Japanese, "作者: K1rito (OniGremlin からインスパイア)" },
            { Language.Korean, "제작자: K1rito (OniGremlin 에서 영감)" },
            { Language.Italian, "Autore: K1rito (Ispirato da OniGremlin)" }
        }},
        { "about.description", new Dictionary<Language, string> {
            { Language.English, "PEAK AIO is a quality-of-life and utility mod designed for the game PEAK. It brings together a wide range of player enhancements, inventory tools, world manipulation, and lobby control features in one sleek ImGui-powered interface." },
            { Language.SimplifiedChinese, "PEAK AIO 是为游戏 PEAK 设计的生活质量和实用模组。它将广泛的玩家增强、物品工具、世界操控和大厅控制功能集成在一个简洁的 ImGui 界面中。" },
            { Language.TraditionalChinese, "PEAK AIO 是為遊戲 PEAK 設計的生活品質和實用模組。它將廣泛的玩家增強、物品工具、世界操控和大廳控制功能整合在一個簡潔的 ImGui 介面中。" },
            { Language.Japanese, "PEAK AIO はゲーム PEAK 向けに設計されたQOL・ユーティリティMODです。プレイヤー強化、インベントリツール、ワールド操作、ロビー制御機能を、洗練されたImGuiインターフェースにまとめています。" },
            { Language.Korean, "PEAK AIO는 게임 PEAK를 위해 설계된 편의성 및 유틸리티 모드입니다. 플레이어 강화, 인벤토리 도구, 월드 조작, 로비 제어 기능을 세련된 ImGui 인터페이스에 통합했습니다." },
            { Language.Italian, "PEAK AIO è una mod di qualità di vita e utilità progettata per il gioco PEAK. Riunisce una vasta gamma di miglioramenti per il giocatore, strumenti per l'inventario, manipolazione del mondo e funzionalità di controllo della lobby in un'elegante interfaccia basata su ImGui." }
        }},
        { "about.key_features", new Dictionary<Language, string> {
            { Language.English, "Key Features:" },
            { Language.SimplifiedChinese, "主要功能:" },
            { Language.TraditionalChinese, "主要功能:" },
            { Language.Japanese, "主な機能:" },
            { Language.Korean, "주요 기능:" },
            { Language.Italian, "Caratteristiche Principali:" }
        }},
        { "about.feature1", new Dictionary<Language, string> {
            { Language.English, "Infinite stamina and affliction immunity" },
            { Language.SimplifiedChinese, "无限体力和状态异常免疫" },
            { Language.TraditionalChinese, "無限體力和狀態異常免疫" },
            { Language.Japanese, "無限スタミナと状態異常免疫" },
            { Language.Korean, "무한 스태미나 및 상태이상 면역" },
            { Language.Italian, "Stamina infinita e immunità alle afflizioni" }
        }},
        { "about.feature2", new Dictionary<Language, string> {
            { Language.English, "Adjustable movement: speed, jump, and climb mods" },
            { Language.SimplifiedChinese, "可调节移动: 速度、跳跃和攀爬模组" },
            { Language.TraditionalChinese, "可調節移動: 速度、跳躍和攀爬模組" },
            { Language.Japanese, "速度・ジャンプ・登攀の調整可能なMOD" },
            { Language.Korean, "조정 가능한 이동: 속도, 점프, 등반 모드" },
            { Language.Italian, "Movimento regolabile: mod di velocità, salto e arrampicata" }
        }},
        { "about.feature3", new Dictionary<Language, string> {
            { Language.English, "Real-time inventory editing and recharge" },
            { Language.SimplifiedChinese, "实时物品编辑和充能" },
            { Language.TraditionalChinese, "即時物品編輯和充能" },
            { Language.Japanese, "リアルタイムインベントリ編集とチャージ" },
            { Language.Korean, "실시간 인벤토리 편집 및 충전" },
            { Language.Italian, "Modifica e ricarica dell'inventario in tempo reale" }
        }},
        { "about.feature4", new Dictionary<Language, string> {
            { Language.English, "Player-to-player warp, revive, and kill tools" },
            { Language.SimplifiedChinese, "玩家间传送、复活和击杀工具" },
            { Language.TraditionalChinese, "玩家間傳送、復活和擊殺工具" },
            { Language.Japanese, "プレイヤー間ワープ・復活・キルツール" },
            { Language.Korean, "플레이어 간 워프, 부활, 처치 도구" },
            { Language.Italian, "Strumenti di teletrasporto, rianimazione e uccisione tra giocatori" }
        }},
        { "about.feature5", new Dictionary<Language, string> {
            { Language.English, "Custom teleportation and ping-based movement" },
            { Language.SimplifiedChinese, "自定义传送和基于标记的移动" },
            { Language.TraditionalChinese, "自訂傳送和基於標記的移動" },
            { Language.Japanese, "カスタムテレポートとピングベース移動" },
            { Language.Korean, "사용자 정의 텔레포트 및 핑 기반 이동" },
            { Language.Italian, "Teletrasporto personalizzato e movimento basato su ping" }
        }},
        { "about.feature6", new Dictionary<Language, string> {
            { Language.English, "Stylized UI with tabbed interface" },
            { Language.SimplifiedChinese, "风格化的标签页界面" },
            { Language.TraditionalChinese, "具有分頁介面的風格化UI" },
            { Language.Japanese, "タブ付きスタイリッシュUI" },
            { Language.Korean, "탭 인터페이스를 갖춘 스타일리시 UI" },
            { Language.Italian, "Interfaccia stilizzata con schede" }
        }},
        { "about.thanks", new Dictionary<Language, string> {
            { Language.English, "Special Thanks:" },
            { Language.SimplifiedChinese, "特别感谢:" },
            { Language.TraditionalChinese, "特別感謝:" },
            { Language.Japanese, "スペシャルサンクス:" },
            { Language.Korean, "특별 감사:" },
            { Language.Italian, "Ringraziamenti Speciali:" }
        }},
        { "about.thanks1", new Dictionary<Language, string> {
            { Language.English, "Penswer for insight, and guidance" },
            { Language.SimplifiedChinese, "Penswer 提供的见解和指导" },
            { Language.TraditionalChinese, "Penswer 提供的見解和指導" },
            { Language.Japanese, "Penswer 氏の洞察とガイダンス" },
            { Language.Korean, "Penswer의 통찰과 안내" },
            { Language.Italian, "Penswer per intuizione e guida" }
        }},
        { "about.thanks2", new Dictionary<Language, string> {
            { Language.English, "BepInEx team for the modding framework" },
            { Language.SimplifiedChinese, "BepInEx 团队提供的模组框架" },
            { Language.TraditionalChinese, "BepInEx 團隊提供的模組框架" },
            { Language.Japanese, "BepInEx チームのMODフレームワーク" },
            { Language.Korean, "BepInEx 팀의 모딩 프레임워크" },
            { Language.Italian, "Team BepInEx per il framework di modding" }
        }},
        { "about.thanks3", new Dictionary<Language, string> {
            { Language.English, "DearImGuiInjection for seamless UI integration" },
            { Language.SimplifiedChinese, "DearImGuiInjection 提供的无缝UI集成" },
            { Language.TraditionalChinese, "DearImGuiInjection 提供的無縫UI整合" },
            { Language.Japanese, "DearImGuiInjection のシームレスなUI統合" },
            { Language.Korean, "DearImGuiInjection의 원활한 UI 통합" },
            { Language.Italian, "DearImGuiInjection per l'integrazione UI senza soluzione di continuità" }
        }},
        { "about.thanks4", new Dictionary<Language, string> {
            { Language.English, "HarmonyX for runtime patching support" },
            { Language.SimplifiedChinese, "HarmonyX 提供的运行时补丁支持" },
            { Language.TraditionalChinese, "HarmonyX 提供的執行時修補支援" },
            { Language.Japanese, "HarmonyX のランタイムパッチサポート" },
            { Language.Korean, "HarmonyX의 런타임 패치 지원" },
            { Language.Italian, "HarmonyX per il supporto al patching runtime" }
        }},
        { "about.disclaimer", new Dictionary<Language, string> {
            { Language.English, "This mod is provided as-is for educational and personal use. Not affiliated with or endorsed by the developers of PEAK. Use responsibly." },
            { Language.SimplifiedChinese, "此模组按原样提供，仅供教育和个人使用。与PEAK的开发者无关，也未获得其认可。请负责任地使用。" },
            { Language.TraditionalChinese, "此模組按原樣提供，僅供教育和個人使用。與PEAK的開發者無關，也未獲得其認可。請負責任地使用。" },
            { Language.Japanese, "このMODは教育および個人使用を目的として現状のまま提供されます。PEAKの開発者とは無関係であり、推奨もされていません。責任を持ってご使用ください。" },
            { Language.Korean, "이 모드는 교육 및 개인 용도로 있는 그대로 제공됩니다. PEAK 개발자와 관련이 없으며 승인을 받지 않았습니다. 책임감 있게 사용하세요." },
            { Language.Italian, "Questa mod è fornita così com'è per uso educativo e personale. Non affiliata o approvata dagli sviluppatori di PEAK. Usare responsabilmente." }
        }},

        // Language tab
        { "lang.title", new Dictionary<Language, string> {
            { Language.English, "Language Settings" },
            { Language.SimplifiedChinese, "语言设置" },
            { Language.TraditionalChinese, "語言設定" },
            { Language.Japanese, "言語設定" },
            { Language.Korean, "언어 설정" },
            { Language.Italian, "Impostazioni Lingua" }
        }},
        { "lang.select", new Dictionary<Language, string> {
            { Language.English, "Select Language:" },
            { Language.SimplifiedChinese, "选择语言:" },
            { Language.TraditionalChinese, "選擇語言:" },
            { Language.Japanese, "言語を選択:" },
            { Language.Korean, "언어 선택:" },
            { Language.Italian, "Seleziona Lingua:" }
        }},
        { "lang.current", new Dictionary<Language, string> {
            { Language.English, "Current Language: English" },
            { Language.SimplifiedChinese, "当前语言: 简体中文" },
            { Language.TraditionalChinese, "當前語言: 繁體中文" },
            { Language.Japanese, "現在の言語: 日本語" },
            { Language.Korean, "현재 언어: 한국어" },
            { Language.Italian, "Lingua Corrente: Italiano" }
        }},

        // New feature keys: Map / Segment Jump
        { "world.segment_teleport", new Dictionary<Language, string> {
            { Language.English, "Map / Segment Jump" },
            { Language.SimplifiedChinese, "地图 / 区域跳转" },
            { Language.TraditionalChinese, "地圖 / 區域跳轉" },
            { Language.Japanese, "マップ / セグメントワープ" },
            { Language.Korean, "지도 / 세그먼트 이동" },
            { Language.Italian, "Mappa / Salto Segmento" }
        }},
        { "world.current_segment", new Dictionary<Language, string> {
            { Language.English, "Current Segment:" },
            { Language.SimplifiedChinese, "当前区域:" },
            { Language.TraditionalChinese, "當前區域:" },
            { Language.Japanese, "現在のセグメント:" },
            { Language.Korean, "현재 세그먼트:" },
            { Language.Italian, "Segmento Corrente:" }
        }},
        { "world.jump_to_segment", new Dictionary<Language, string> {
            { Language.English, "Jump to Segment" },
            { Language.SimplifiedChinese, "跳转到区域" },
            { Language.TraditionalChinese, "跳轉到區域" },
            { Language.Japanese, "セグメントに移動" },
            { Language.Korean, "세그먼트로 이동" },
            { Language.Italian, "Vai al Segmento" }
        }},
        { "world.segment_beach", new Dictionary<Language, string> {
            { Language.English, "Shore (Beach)" },
            { Language.SimplifiedChinese, "海岸 (Beach)" },
            { Language.TraditionalChinese, "海岸 (Beach)" },
            { Language.Japanese, "海岸 (Beach)" },
            { Language.Korean, "해안 (Beach)" },
            { Language.Italian, "Spiaggia (Beach)" }
        }},
        { "world.segment_tropics", new Dictionary<Language, string> {
            { Language.English, "Tropics" },
            { Language.SimplifiedChinese, "热带 (Tropics)" },
            { Language.TraditionalChinese, "熱帶 (Tropics)" },
            { Language.Japanese, "熱帯 (Tropics)" },
            { Language.Korean, "열대 (Tropics)" },
            { Language.Italian, "Tropici (Tropics)" }
        }},
        { "world.segment_alpine", new Dictionary<Language, string> {
            { Language.English, "Alpine" },
            { Language.SimplifiedChinese, "高山 (Alpine)" },
            { Language.TraditionalChinese, "高山 (Alpine)" },
            { Language.Japanese, "高山 (Alpine)" },
            { Language.Korean, "고산 (Alpine)" },
            { Language.Italian, "Alpino (Alpine)" }
        }},
        { "world.segment_caldera", new Dictionary<Language, string> {
            { Language.English, "Volcano (Caldera)" },
            { Language.SimplifiedChinese, "火山 (Volcano)" },
            { Language.TraditionalChinese, "火山 (Volcano)" },
            { Language.Japanese, "火山 (Volcano)" },
            { Language.Korean, "화산 (Volcano)" },
            { Language.Italian, "Vulcano (Caldera)" }
        }},
        { "world.segment_thekiln", new Dictionary<Language, string> {
            { Language.English, "The Kiln" },
            { Language.SimplifiedChinese, "熔炉 (The Kiln)" },
            { Language.TraditionalChinese, "熔爐 (The Kiln)" },
            { Language.Japanese, "窯 (The Kiln)" },
            { Language.Korean, "가마 (The Kiln)" },
            { Language.Italian, "La Fornace (The Kiln)" }
        }},
        { "world.segment_peak", new Dictionary<Language, string> {
            { Language.English, "The Peak" },
            { Language.SimplifiedChinese, "顶峰 (The Peak)" },
            { Language.TraditionalChinese, "頂峰 (The Peak)" },
            { Language.Japanese, "頂上 (The Peak)" },
            { Language.Korean, "정상 (The Peak)" },
            { Language.Italian, "La Vetta (The Peak)" }
        }},
        { "world.segment_mesa", new Dictionary<Language, string> {
            { Language.English, "Mesa" },
            { Language.SimplifiedChinese, "方山 (Mesa)" },
            { Language.TraditionalChinese, "方山 (Mesa)" },
            { Language.Japanese, "メサ (Mesa)" },
            { Language.Korean, "메사 (Mesa)" },
            { Language.Italian, "Mesa (Mesa)" }
        }},
        { "world.segment_roots", new Dictionary<Language, string> {
            { Language.English, "Roots" },
            { Language.SimplifiedChinese, "森蕈 (Roots)" },
            { Language.TraditionalChinese, "森蕈 (Roots)" },
            { Language.Japanese, "森蕈 (Roots)" },
            { Language.Korean, "버섯숲 (Roots)" },
            { Language.Italian, "Radici (Roots)" }
        }},
        { "world.segment_swamp", new Dictionary<Language, string> {
            { Language.English, "Swamp" },
            { Language.SimplifiedChinese, "雾沼 (Swamp)" },
            { Language.TraditionalChinese, "霧沼 (Swamp)" },
            { Language.Japanese, "湿原 (Swamp)" },
            { Language.Korean, "늪지대 (Swamp)" },
            { Language.Italian, "Palude (Swamp)" }
        }},
        { "world.segment_citadel", new Dictionary<Language, string> {
            { Language.English, "The Citadel" },
            { Language.SimplifiedChinese, "城塞 (The Citadel)" },
            { Language.TraditionalChinese, "城塞 (The Citadel)" },
            { Language.Japanese, "城塞 (The Citadel)" },
            { Language.Korean, "성채 (The Citadel)" },
            { Language.Italian, "La Cittadella (The Citadel)" }
        }},
        { "world.segment_void", new Dictionary<Language, string> {
            { Language.English, "Void" },
            { Language.SimplifiedChinese, "虚空 (Void)" },
            { Language.TraditionalChinese, "虛空 (Void)" },
            { Language.Japanese, "虚空 (Void)" },
            { Language.Korean, "공허 (Void)" },
            { Language.Italian, "Vuoto (Void)" }
        }},
        { "world.daily_route_info", new Dictionary<Language, string> {
            { Language.English, "Today's Island Flight Route" },
            { Language.SimplifiedChinese, "今日每日岛屿 (登机航线)" },
            { Language.TraditionalChinese, "今日每日島嶼 (登機航線)" },
            { Language.Japanese, "本日の島ルート" },
            { Language.Korean, "오늘의 섬 등반 항로" },
            { Language.Italian, "Rotta dell'Isola di Oggi" }
        }},
        { "world.next_rotation_info", new Dictionary<Language, string> {
            { Language.English, "Next Island Rotation Preview" },
            { Language.SimplifiedChinese, "明日地图轮换预告" },
            { Language.TraditionalChinese, "明日地圖輪換預告" },
            { Language.Japanese, "明日の島マップ予告" },
            { Language.Korean, "내일 섬 로테이션 예고" },
            { Language.Italian, "Prossima Mappa dell'Isola" }
        }},
        { "world.rotation_timer", new Dictionary<Language, string> {
            { Language.English, "Next Rotation In:" },
            { Language.SimplifiedChinese, "距下次地图换日:" },
            { Language.TraditionalChinese, "距下次地圖換日:" },
            { Language.Japanese, "次回マップ更新まで:" },
            { Language.Korean, "다음 맵 교체까지:" },
            { Language.Italian, "Prossima Rotazione Tra:" }
        }},
        { "world.airport_status", new Dictionary<Language, string> {
            { Language.English, "Airport Lobby (Before Flight)" },
            { Language.SimplifiedChinese, "机场大厅 (登机前)" },
            { Language.TraditionalChinese, "機場大廳 (登機前)" },
            { Language.Japanese, "空港ロビー (搭乗前)" },
            { Language.Korean, "공항 로비 (탑승 전)" },
            { Language.Italian, "Lobby Aeroporto (Prima del Volo)" }
        }},
        { "world.next_level_target", new Dictionary<Language, string> {
            { Language.English, "Next Area:" },
            { Language.SimplifiedChinese, "下一关:" },
            { Language.TraditionalChinese, "下一關:" },
            { Language.Japanese, "次のエリア:" },
            { Language.Korean, "다음 구역:" },
            { Language.Italian, "Prossima Area:" }
        }},
        { "world.return_airport", new Dictionary<Language, string> {
            { Language.English, "Return to Airport" },
            { Language.SimplifiedChinese, "返回机场大厅" },
            { Language.TraditionalChinese, "返回機場大廳" },
            { Language.Japanese, "空港ロビーに戻る" },
            { Language.Korean, "공항 로비로 귀환" },
            { Language.Italian, "Torna all'Aeroporto" }
        }},
        { "world.custom_map_tag", new Dictionary<Language, string> {
            { Language.English, "Custom Map" },
            { Language.SimplifiedChinese, "自定义地图" },
            { Language.TraditionalChinese, "自定義地圖" },
            { Language.Japanese, "カスタムマップ" },
            { Language.Korean, "커스텀 맵" },
            { Language.Italian, "Mappa Personalizzata" }
        }},
        { "world.custom_route_info", new Dictionary<Language, string> {
            { Language.English, "Custom Flight Route" },
            { Language.SimplifiedChinese, "自定义登岛航线" },
            { Language.TraditionalChinese, "自定義登島航線" },
            { Language.Japanese, "カスタム搭乗航路" },
            { Language.Korean, "커스텀 탑승 항로" },
            { Language.Italian, "Rotta Personalizzata" }
        }},
        { "world.loading_announced", new Dictionary<Language, string> {
            { Language.English, "Target Island Confirmed (Departing Soon)" },
            { Language.SimplifiedChinese, "已确认登岛航线 (即将出发)" },
            { Language.TraditionalChinese, "已確認登島航線 (即將出發)" },
            { Language.Japanese, "目的地確定 (まもなく搭乗)" },
            { Language.Korean, "목적지 확정 (곧 출발)" },
            { Language.Italian, "Destinazione Confermata (In Partenza)" }
        }},
        { "world.waiting_for_host", new Dictionary<Language, string> {
            { Language.English, "Waiting for Host to select island at check-in kiosk..." },
            { Language.SimplifiedChinese, "等待房主在前台柜台选定航线..." },
            { Language.TraditionalChinese, "等待房主在前台櫃台選定航線..." },
            { Language.Japanese, "ホストの搭乗手続きを待機中..." },
            { Language.Korean, "호스트의 탑승 수속 대기 중..." },
            { Language.Italian, "In attesa che l'host confermi il volo..." }
        }},
        { "world.custom_scene_active", new Dictionary<Language, string> {
            { Language.English, "Current Scene: {0} (External Custom Map)" },
            { Language.SimplifiedChinese, "当前场景: {0} (外部自定义场景)" },
            { Language.TraditionalChinese, "當前場景: {0} (外部自定義場景)" },
            { Language.Japanese, "現在のシーン: {0} (外部カスタムマップ)" },
            { Language.Korean, "현재 씬: {0} (外部 커스텀 맵)" },
            { Language.Italian, "Scena Attuale: {0} (Mappa Personalizzata)" }
        }},
        { "world.playlist_queue", new Dictionary<Language, string> {
            { Language.English, "Playlist Queue" },
            { Language.SimplifiedChinese, "播放列表队列" },
            { Language.TraditionalChinese, "播放列表隊列" },
            { Language.Japanese, "プレイリストキュー" },
            { Language.Korean, "플레이리스트 큐" },
            { Language.Italian, "Coda Playlist" }
        }},
        { "world.playlist_overview", new Dictionary<Language, string> {
            { Language.English, "Playlist Journey Overview" },
            { Language.SimplifiedChinese, "航程播放列表全景" },
            { Language.TraditionalChinese, "航程播放列表全景" },
            { Language.Japanese, "プレイリスト航路全体図" },
            { Language.Korean, "플레이리스트 항로 개요" },
            { Language.Italian, "Panoramica Itinerario Playlist" }
        }},
        { "world.route_flow_title", new Dictionary<Language, string> {
            { Language.English, "Flight Route Flow (Ribbon)" },
            { Language.SimplifiedChinese, "航线流程条" },
            { Language.TraditionalChinese, "航線流程條" },
            { Language.Japanese, "ルートフローリボン" },
            { Language.Korean, "항로 플로우 리본" },
            { Language.Italian, "Nastro di Flusso della Rotta" }
        }},
        { "world.stage_tag", new Dictionary<Language, string> {
            { Language.English, "Stop {0}/{1}" },
            { Language.SimplifiedChinese, "第 {0}/{1} 站" },
            { Language.TraditionalChinese, "第 {0}/{1} 站" },
            { Language.Japanese, "第 {0}/{1} ステージ" },
            { Language.Korean, "제 {0}/{1} 코스" },
            { Language.Italian, "Tappa {0}/{1}" }
        }},
        { "world.status_active", new Dictionary<Language, string> {
            { Language.English, "ACTIVE" },
            { Language.SimplifiedChinese, "进行中" },
            { Language.TraditionalChinese, "進行中" },
            { Language.Japanese, "進行中" },
            { Language.Korean, "진행중" },
            { Language.Italian, "ATTIVO" }
        }},
        { "world.status_completed", new Dictionary<Language, string> {
            { Language.English, "CLEARED" },
            { Language.SimplifiedChinese, "已通关" },
            { Language.TraditionalChinese, "已通關" },
            { Language.Japanese, "クリア" },
            { Language.Korean, "클리어" },
            { Language.Italian, "COMPLETATO" }
        }},
        { "world.status_upcoming", new Dictionary<Language, string> {
            { Language.English, "UPCOMING" },
            { Language.SimplifiedChinese, "待启航" },
            { Language.TraditionalChinese, "待啟航" },
            { Language.Japanese, "待機中" },
            { Language.Korean, "출발 대기" },
            { Language.Italian, "IN ARRIVO" }
        }},
        { "world.airport_preview_tip", new Dictionary<Language, string> {
            { Language.English, "Flight route planned. Real-time segment & campfire tracking activates on island." },
            { Language.SimplifiedChinese, "航班航线已规划完毕，登岛后将实时追踪节点与营火进度。" },
            { Language.TraditionalChinese, "航班航線已規劃完畢，登島後將實時追蹤節點與營火進度。" },
            { Language.Japanese, "航路計画完了。島に到着後、各区間と焚き火の進捗をリアルタイム追跡します。" },
            { Language.Korean, "항로 계획 완료. 섬 도착 후 구역 및 모닥불 진행 상황을 실시간 추적합니다." },
            { Language.Italian, "Rotta pianificata. Il monitoraggio in tempo reale si attiverà sull'isola." }
        }},
        { "world.airport_teleport_hidden_hint", new Dictionary<Language, string> {
            { Language.English, "Campfire teleportation and region jump controls will be available after entering the island and confirming the route." },
            { Language.SimplifiedChinese, "进入海岛并确认路线后将开放营火传送与区域跳转功能。" },
            { Language.TraditionalChinese, "進入海島並確認路線後將開放營火傳送與區域跳轉功能。" },
            { Language.Japanese, "島に入りルートが確定すると、キャンプファイヤー転送とエリアジャンプが利用可能になります。" },
            { Language.Korean, "섬에 진입하여 경로가 확정되면 모닥불 텔레포트 및 구역 이동 기능이 활성화됩니다." },
            { Language.Italian, "Il teletrasporto ai falò e il salto di zona saranno disponibili dopo l'accesso all'isola e la conferma del percorso." }
        }},
        { "world.current_node_tag", new Dictionary<Language, string> {
            { Language.English, "HERE" },
            { Language.SimplifiedChinese, "当前" },
            { Language.TraditionalChinese, "當前" },
            { Language.Japanese, "現在地" },
            { Language.Korean, "현재 위치" },
            { Language.Italian, "QUI" }
        }},
        { "world.route_header", new Dictionary<Language, string> {
            { Language.English, "Mountain Route (Full Route)" },
            { Language.SimplifiedChinese, "登山完整路线" },
            { Language.TraditionalChinese, "登山完整路線" },
            { Language.Japanese, "登山ルート全体" },
            { Language.Korean, "전체 등반 경로" },
            { Language.Italian, "Percorso Completo della Montagna" }
        }},
        { "world.teleport_next_campfire", new Dictionary<Language, string> {
            { Language.English, "Teleport to Campfire Before Next Area" },
            { Language.SimplifiedChinese, "传送到下一个区域前的篝火" },
            { Language.TraditionalChinese, "傳送到下一個區域前的營火" },
            { Language.Japanese, "次のエリア手前の焚き火にテレポート" },
            { Language.Korean, "다음 구역 앞 모닥불로 순간이동" },
            { Language.Italian, "Teletrasporta al Falò dell'Area Successiva" }
        }},
        { "world.teleport_campfire", new Dictionary<Language, string> {
            { Language.English, "Teleport to End Campfire" },
            { Language.SimplifiedChinese, "传送到终点篝火" },
            { Language.TraditionalChinese, "傳送到終點營火" },
            { Language.Japanese, "終点の焚き火にテレポート" },
            { Language.Korean, "종점 모닥불로 순간이동" },
            { Language.Italian, "Teletrasporta al Falò Finale" }
        }},
        { "world.jump_to_start", new Dictionary<Language, string> {
            { Language.English, "Jump to Start" },
            { Language.SimplifiedChinese, "跳转到起点" },
            { Language.TraditionalChinese, "跳轉到起點" },
            { Language.Japanese, "開始地点へ移動" },
            { Language.Korean, "시작 지점으로 이동" },
            { Language.Italian, "Vai all'Inizio" }
        }},
        { "world.current_altitude", new Dictionary<Language, string> {
            { Language.English, "Altitude:" },
            { Language.SimplifiedChinese, "当前海拔:" },
            { Language.TraditionalChinese, "當前海拔:" },
            { Language.Japanese, "現在の標高:" },
            { Language.Korean, "현재 고도:" },
            { Language.Italian, "Altitudine Corrente:" }
        }},
        { "world.level_label", new Dictionary<Language, string> {
            { Language.English, "Level {0}" },
            { Language.SimplifiedChinese, "第 {0} 区域" },
            { Language.TraditionalChinese, "第 {0} 區域" },
            { Language.Japanese, "第 {0} エリア" },
            { Language.Korean, "제 {0} 구역" },
            { Language.Italian, "Livello {0}" }
        }},
        { "world.at_peak", new Dictionary<Language, string> {
            { Language.English, "Already at The Peak" },
            { Language.SimplifiedChinese, "已到达顶峰" },
            { Language.TraditionalChinese, "已到達頂峰" },
            { Language.Japanese, "すでに頂上に到達" },
            { Language.Korean, "이미 정상에 도달함" },
            { Language.Italian, "Già alla Vetta" }
        }},
        { "world.summon_helicopter", new Dictionary<Language, string> {
            { Language.English, "Summon Helicopter" },
            { Language.SimplifiedChinese, "召唤直升机" },
            { Language.TraditionalChinese, "召喚直升機" },
            { Language.Japanese, "ヘリコプターを呼ぶ" },
            { Language.Korean, "헬리콥터 호출" },
            { Language.Italian, "Chiama Elicottero" }
        }},
        { "world.teleport_to_peak", new Dictionary<Language, string> {
            { Language.English, "Jump to Peak" },
            { Language.SimplifiedChinese, "跳转到顶峰" },
            { Language.TraditionalChinese, "跳轉到頂峰" },
            { Language.Japanese, "頂上へ移動" },
            { Language.Korean, "정상으로 이동" },
            { Language.Italian, "Vai alla Vetta" }
        }},
        { "world.next_area_label", new Dictionary<Language, string> {
            { Language.English, "Next Area:" },
            { Language.SimplifiedChinese, "下一区域:" },
            { Language.TraditionalChinese, "下一區域:" },
            { Language.Japanese, "次のエリア:" },
            { Language.Korean, "다음 구역:" },
            { Language.Italian, "Area Successiva:" }
        }},
        { "world.at_campfire_tag", new Dictionary<Language, string> {
            { Language.English, "At Campfire" },
            { Language.SimplifiedChinese, "营地" },
            { Language.TraditionalChinese, "營地" },
            { Language.Japanese, "焚き火" },
            { Language.Korean, "모닥불" },
            { Language.Italian, "Al Falò" }
        }},
        { "world.current_tag", new Dictionary<Language, string> {
            { Language.English, "Current" },
            { Language.SimplifiedChinese, "当前" },
            { Language.TraditionalChinese, "目前" },
            { Language.Japanese, "現在地" },
            { Language.Korean, "현재" },
            { Language.Italian, "Attuale" }
        }},
        { "world.teleport_kiln_safe", new Dictionary<Language, string> {
            { Language.English, "Teleport to Kiln Point" },
            { Language.SimplifiedChinese, "传送至熔炉安全点" },
            { Language.TraditionalChinese, "傳送至熔爐安全點" },
            { Language.Japanese, "熔炉安全地点へ" },
            { Language.Korean, "용광로 안전 지점으로" },
            { Language.Italian, "Teletrasporto al Forno" }
        }},
        { "world.teleport_citadel_safe", new Dictionary<Language, string> {
            { Language.English, "Teleport to Citadel Point" },
            { Language.SimplifiedChinese, "传送至城塞安全点" },
            { Language.TraditionalChinese, "傳送至城塞安全點" },
            { Language.Japanese, "城塞安全地点へ" },
            { Language.Korean, "성채 안전 지점으로" },
            { Language.Italian, "Teletrasporto alla Cittadella" }
        }},
        { "world.light_campfire", new Dictionary<Language, string> {
            { Language.English, "Light Campfire (To Level {0})" },
            { Language.SimplifiedChinese, "点燃篝火 (开启第 {0} 区域)" },
            { Language.TraditionalChinese, "點燃營火 (開啟第 {0} 區域)" },
            { Language.Japanese, "焚き火を点火 (第 {0} エリアへ)" },
            { Language.Korean, "모닥불 점화 (제 {0} 구역 진입)" },
            { Language.Italian, "Accendi Falò (Verso Livello {0})" }
        }},
        { "world.light_action", new Dictionary<Language, string> {
            { Language.English, "Light" },
            { Language.SimplifiedChinese, "点燃" },
            { Language.TraditionalChinese, "點燃" },
            { Language.Japanese, "点火" },
            { Language.Korean, "점화" },
            { Language.Italian, "Accendi" }
        }},
        { "world.teleport_next_area_campfire", new Dictionary<Language, string> {
            { Language.English, "Teleport to Next Area's Campfire (Level {0})" },
            { Language.SimplifiedChinese, "传送到下一个区域的终点篝火 (第 {0} 区域)" },
            { Language.TraditionalChinese, "傳送到下一個區域的終點營火 (第 {0} 區域)" },
            { Language.Japanese, "次のエリアの焚き火へ (第 {0} エリア)" },
            { Language.Korean, "다음 구역 종점 모닥불로 (제 {0} 구역)" },
            { Language.Italian, "Al Falò dell'Area Successiva (Livello {0})" }
        }},

        // Player afflictions
        { "player.clear_afflictions", new Dictionary<Language, string> {
            { Language.English, "Clear All Afflictions" },
            { Language.SimplifiedChinese, "清除所有异常状态" },
            { Language.TraditionalChinese, "清除所有異常狀態" },
            { Language.Japanese, "全状態異常を解除" },
            { Language.Korean, "모든 상태이상 치료" },
            { Language.Italian, "Rimuovi Tutte le Afflizioni" }
        }},
        { "tip.clear_afflictions", new Dictionary<Language, string> {
            { Language.English, "Removes injury, poison, cold, curse, thorns, spores, web, and other afflictions" },
            { Language.SimplifiedChinese, "清除受伤、中毒、寒冷、诅咒、荆棘、孢子、蛛网等所有异常状态" },
            { Language.TraditionalChinese, "清除受傷、中毒、寒冷、詛咒、荊棘、孢子、蛛網等所有異常狀態" },
            { Language.Japanese, "怪我、毒、寒さ、呪い、トゲ、胞子、クモの巣などの状態異常を全解除" },
            { Language.Korean, "부상, 독, 추위, 저주, 가시, 포자, 거미줄 등 모든 상태이상 제거" },
            { Language.Italian, "Rimuove ferite, veleno, freddo, maledizione, spine, spore, ragnatele e altre afflizioni" }
        }},

        // Items spawn
        { "tip.spawn_item", new Dictionary<Language, string> {
            { Language.English, "Spawns the selected item into the world in front of the player" },
            { Language.SimplifiedChinese, "在玩家前方将选中的物品生成到游戏世界中" },
            { Language.TraditionalChinese, "在玩家前方將選中的物品生成到遊戲世界中" },
            { Language.Japanese, "選択したアイテムをプレイヤーの前方に生成します" },
            { Language.Korean, "선택한 아이템을 플레이어 앞 월드에 소환합니다" },
            { Language.Italian, "Genera l'oggetto selezionato nel mondo davanti al giocatore" }
        }},

        // Slot 4 & Backpacks
        { "items.slot4", new Dictionary<Language, string> {
            { Language.English, "Slot 4 (Backpack)" },
            { Language.SimplifiedChinese, "槽位 4 (背包)" },
            { Language.TraditionalChinese, "槽位 4 (背包)" },
            { Language.Japanese, "スロット 4 (バックパック)" },
            { Language.Korean, "슬롯 4 (배낭)" },
            { Language.Italian, "Slot 4 (Zaino)" }
        }},
        { "items.drop_backpack", new Dictionary<Language, string> {
            { Language.English, "Drop Backpack" },
            { Language.SimplifiedChinese, "丢下背包" },
            { Language.TraditionalChinese, "丟下背包" },
            { Language.Japanese, "バックパックを落とす" },
            { Language.Korean, "배낭 버리기" },
            { Language.Italian, "Lascia Zaino" }
        }},
        { "items.tip_backpack_slot", new Dictionary<Language, string> {
            { Language.English, "Only backpack items (Jetpack, Fannypack, Rocketpack, Backpack). Drops current backpack before equipping new." },
            { Language.SimplifiedChinese, "仅限背包类物品(喷气/滑稽/火箭/普通背包)。刷出前自动丢下当前背包。" },
            { Language.TraditionalChinese, "僅限背包類物品(噴氣/滑稽/火箭/普通背包)。刷出前自動丟下當前背包。" },
            { Language.Japanese, "バックパック専用（ジェットパック、ファニーパック等）。装備前に現在の物を落とします。" },
            { Language.Korean, "배낭류 전용(제트팩, 힙색, 로켓팩 등). 새 장비 장착 전 기존 배낭을 떨어뜨립니다." },
            { Language.Italian, "Solo zaini (Jetpack, Marsupio, Rocketpack, Zaino). Rilascia lo zaino attuale prima di equipaggiare." }
        }},

        // Lobby Give Items
        { "lobby.give_items", new Dictionary<Language, string> {
            { Language.English, "Give Items" },
            { Language.SimplifiedChinese, "给予物品" },
            { Language.TraditionalChinese, "給予物品" },
            { Language.Japanese, "アイテム付与" },
            { Language.Korean, "아이템 주기" },
            { Language.Italian, "Dai Oggetti" }
        }},
        { "lobby.give_selected_item", new Dictionary<Language, string> {
            { Language.English, "Give Item" },
            { Language.SimplifiedChinese, "给该玩家物品" },
            { Language.TraditionalChinese, "給該玩家物品" },
            { Language.Japanese, "プレイヤーに付与" },
            { Language.Korean, "플레이어에게 주기" },
            { Language.Italian, "Dai al Giocatore" }
        }},
        { "lobby.give_all_item", new Dictionary<Language, string> {
            { Language.English, "Give to All" },
            { Language.SimplifiedChinese, "全员发放此物" },
            { Language.TraditionalChinese, "全員發放此物" },
            { Language.Japanese, "全員に付与" },
            { Language.Korean, "모두에게 주기" },
            { Language.Italian, "Dai a Tutti" }
        }},
        { "lobby.give_backpack", new Dictionary<Language, string> {
            { Language.English, "Give Backpack" },
            { Language.SimplifiedChinese, "发普通背包" },
            { Language.TraditionalChinese, "發普通背包" },
            { Language.Japanese, "バックパック付与" },
            { Language.Korean, "배낭 지급" },
            { Language.Italian, "Dai Zaino" }
        }},
        { "lobby.give_jetpack", new Dictionary<Language, string> {
            { Language.English, "Give Jetpack" },
            { Language.SimplifiedChinese, "发喷气背包" },
            { Language.TraditionalChinese, "發噴氣背包" },
            { Language.Japanese, "ジェットパック付与" },
            { Language.Korean, "제트팩 지급" },
            { Language.Italian, "Dai Jetpack" }
        }},
        { "lobby.give_rocketpack", new Dictionary<Language, string> {
            { Language.English, "Give Rocketpack" },
            { Language.SimplifiedChinese, "发火箭背包" },
            { Language.TraditionalChinese, "發火箭背包" },
            { Language.Japanese, "ロケットパック付与" },
            { Language.Korean, "로켓팩 지급" },
            { Language.Italian, "Dai Rocketpack" }
        }},
        { "lobby.give_fannypack", new Dictionary<Language, string> {
            { Language.English, "Give Fannypack" },
            { Language.SimplifiedChinese, "发滑稽背包" },
            { Language.TraditionalChinese, "發滑稽背包" },
            { Language.Japanese, "ファニーパック付与" },
            { Language.Korean, "힙색 지급" },
            { Language.Italian, "Dai Marsupio" }
        }},

        // Revive & Restore
        { "lobby.revive_restore", new Dictionary<Language, string> {
            { Language.English, "Revive (Restore Items)" },
            { Language.SimplifiedChinese, "复活(恢复物品)" },
            { Language.TraditionalChinese, "復活(恢復物品)" },
            { Language.Japanese, "復活(アイテム復元)" },
            { Language.Korean, "부활(아이템 복구)" },
            { Language.Italian, "Rianima (Ripristina Oggetti)" }
        }},
        { "lobby.revive_all_restore", new Dictionary<Language, string> {
            { Language.English, "Revive All (Restore)" },
            { Language.SimplifiedChinese, "全员复活(恢复物品)" },
            { Language.TraditionalChinese, "全員復活(恢復物品)" },
            { Language.Japanese, "全員復活(復元)" },
            { Language.Korean, "전원 부활(복구)" },
            { Language.Italian, "Rianima Tutti (Ripristina)" }
        }},
        { "tip.revive_restore", new Dictionary<Language, string> {
            { Language.English, "Revives player and restores recorded inventory slots and backpack items (single-use anti-duplication lock)" },
            { Language.SimplifiedChinese, "复活玩家并恢复其死前记录的所有槽位及背包内物品（单次消耗防刷）" },
            { Language.TraditionalChinese, "復活玩家並恢復其死前記錄的所有槽位及背包內物品（單次消耗防刷）" },
            { Language.Japanese, "プレイヤーを復活させ、記録されたスロットとバックパック内のアイテムを復元します（重複付与防止）" },
            { Language.Korean, "플레이어를 부활시키고 사망 전 기록된 슬롯 및 배낭 아이템을 복구합니다(중복 복구 방지)" },
            { Language.Italian, "Rianima il giocatore e ripristina slot e zaino registrati (blocco anti-duplicazione monouso)" }
        }},

        // Lobby Extra Controls
        { "lobby.give_to_slot4", new Dictionary<Language, string> {
            { Language.English, "Equip to Slot 4 (Backpack)" },
            { Language.SimplifiedChinese, "直接装备到4号背包栏" },
            { Language.TraditionalChinese, "直接裝備到4號背包欄" },
            { Language.Japanese, "スロット4(バックパック)に装備" },
            { Language.Korean, "슬롯 4(배낭)에 즉시 장착" },
            { Language.Italian, "Equipaggia Slot 4 (Zaino)" }
        }},
        { "lobby.spawn_in_front", new Dictionary<Language, string> {
            { Language.English, "Spawn on Ground in Front" },
            { Language.SimplifiedChinese, "生成掉落在面前地面" },
            { Language.TraditionalChinese, "生成掉落在面前地面" },
            { Language.Japanese, "目の前の地面にドロップ" },
            { Language.Korean, "앞 바닥에 드롭 생성" },
            { Language.Italian, "Genera a terra davanti" }
        }},
        { "lobby.equip_backpack", new Dictionary<Language, string> {
            { Language.English, "Equip Backpack" },
            { Language.SimplifiedChinese, "穿戴普通背包" },
            { Language.TraditionalChinese, "穿戴普通背包" },
            { Language.Japanese, "バックパック装備" },
            { Language.Korean, "배낭 장착" },
            { Language.Italian, "Equipaggia Zaino" }
        }},
        { "lobby.equip_jetpack", new Dictionary<Language, string> {
            { Language.English, "Equip Jetpack" },
            { Language.SimplifiedChinese, "穿戴喷气背包" },
            { Language.TraditionalChinese, "穿戴噴氣背包" },
            { Language.Japanese, "ジェットパック装備" },
            { Language.Korean, "제트팩 장착" },
            { Language.Italian, "Equipaggia Jetpack" }
        }},
        { "lobby.equip_rocketpack", new Dictionary<Language, string> {
            { Language.English, "Equip Rocketpack" },
            { Language.SimplifiedChinese, "穿戴火箭背包" },
            { Language.TraditionalChinese, "穿戴火箭背包" },
            { Language.Japanese, "ロケットパック装備" },
            { Language.Korean, "로켓팩 장착" },
            { Language.Italian, "Equipaggia Rocketpack" }
        }},
        { "lobby.equip_fannypack", new Dictionary<Language, string> {
            { Language.English, "Equip Fannypack" },
            { Language.SimplifiedChinese, "穿戴滑稽背包" },
            { Language.TraditionalChinese, "穿戴滑稽背包" },
            { Language.Japanese, "ファニーパック装備" },
            { Language.Korean, "힙색 장착" },
            { Language.Italian, "Equipaggia Marsupio" }
        }},
        { "lobby.select_item_first", new Dictionary<Language, string> {
            { Language.English, "Select an item in right list first" },
            { Language.SimplifiedChinese, "请先在右侧列表选中物品" },
            { Language.TraditionalChinese, "請先在右側列表選中物品" },
            { Language.Japanese, "先に右リストでアイテムを選択" },
            { Language.Korean, "먼저 오른쪽 목록에서 아이템 선택" },
            { Language.Italian, "Seleziona prima l'oggetto a destra" }
        }},
        { "world.refresh_route", new Dictionary<Language, string> {
            { Language.English, "Refresh Route" },
            { Language.SimplifiedChinese, "刷新地图路线" },
            { Language.TraditionalChinese, "刷新地圖路線" },
            { Language.Japanese, "ルート再読込" },
            { Language.Korean, "경로 새로고침" },
            { Language.Italian, "Aggiorna Percorso" }
        }},
        { "error.title", new Dictionary<Language, string> {
            { Language.English, "Error Alert" },
            { Language.SimplifiedChinese, "错误提示" },
            { Language.TraditionalChinese, "錯誤提示" },
            { Language.Japanese, "エラー警告" },
            { Language.Korean, "오류 알림" },
            { Language.Italian, "Avviso Errore" }
        }},
        { "error.auto_close", new Dictionary<Language, string> {
            { Language.English, "Auto-close" },
            { Language.SimplifiedChinese, "自动关闭" },
            { Language.TraditionalChinese, "自動關閉" },
            { Language.Japanese, "自動消去" },
            { Language.Korean, "자동 닫힘" },
            { Language.Italian, "Chiusura automatica" }
        }},
        { "creatures.title", new Dictionary<Language, string> {
            { Language.English, "CREATURE SPAWNER" },
            { Language.SimplifiedChinese, "生物生成器" },
            { Language.TraditionalChinese, "生物生成器" },
            { Language.Japanese, "クリーチャースポナー" },
            { Language.Korean, "생물 생성기" },
            { Language.Italian, "GENERATORE CREATURE" }
        }},
        { "creatures.desc", new Dictionary<Language, string> {
            { Language.English, "Spawn native monsters, boss entities, and wildlife with custom distance & aggro overrides." },
            { Language.SimplifiedChinese, "生成游戏原生怪物、Boss实体及野生动物，支持自定义生成距离与强制仇恨锁定。" },
            { Language.TraditionalChinese, "生成遊戲原生怪物、Boss實體及野生動物，支援自訂生成距離與強制仇恨鎖定。" },
            { Language.Japanese, "原生モンスター、ボス、野生動物を生成します。距離とターゲットのカスタム指定に対応。" },
            { Language.Korean, "게임 내 몬스터, 보스 및 야생 동물을 생성합니다. 거리 및 어그로 대상 커스텀 지원." },
            { Language.Italian, "Genera mostri, boss e fauna nativa con distanza personalizzata e override dell'aggro." }
        }},
        { "creatures.host_required", new Dictionary<Language, string> {
            { Language.English, "Host Only: You must be the MasterClient to spawn networked room entities." },
            { Language.SimplifiedChinese, "仅限房主：生成联网房间生物必须具备主机/房主权限。" },
            { Language.TraditionalChinese, "僅限房主：生成聯網房間生物必須具備主機/房主權限。" },
            { Language.Japanese, "ホスト専用：ルーム生物の同期生成にはホスト権限が必要です。" },
            { Language.Korean, "호스트 전용: 동기화된 룸 생물을 생성하려면 방장 권한이 필요합니다." },
            { Language.Italian, "Solo Host: Devi essere il MasterClient per generare entità di rete." }
        }},
        { "creatures.host_badge_ready", new Dictionary<Language, string> {
            { Language.English, "● Host Authority Ready (Full Spawning Enabled)" },
            { Language.SimplifiedChinese, "● 房主权限就绪（全功能生物生成已解锁）" },
            { Language.TraditionalChinese, "● 房主權限就緒（全功能生物生成已解鎖）" },
            { Language.Japanese, "● ホスト権限確認済（全生物の生成可能）" },
            { Language.Korean, "● 호스트 권한 활성화됨 (모든 생물 생성 가능)" },
            { Language.Italian, "● Autorità Host Attiva (Generazione Completa Abilitata)" }
        }},
        { "creatures.host_badge_warning", new Dictionary<Language, string> {
            { Language.English, "▲ Client Mode (Room Objects like Scoutmaster require Host)" },
            { Language.SimplifiedChinese, "▲ 客机模式（生成教官等房间对象需要房主权限）" },
            { Language.TraditionalChinese, "▲ 客機模式（生成教官等房間物件需要房主權限）" },
            { Language.Japanese, "▲ クライアントモード（スカウトマスター等の生成にはホストが必要）" },
            { Language.Korean, "▲ 클라이언트 모드 (스카우트마스터 등 룸 오브젝트는 호스트 필요)" },
            { Language.Italian, "▲ Modalità Client (Gli oggetti di stanza richiedono l'Host)" }
        }},
        { "creatures.select_type", new Dictionary<Language, string> {
            { Language.English, "Select Creature Type" },
            { Language.SimplifiedChinese, "选择生物类型" },
            { Language.TraditionalChinese, "選擇生物類型" },
            { Language.Japanese, "クリーチャータイプ選択" },
            { Language.Korean, "생물 유형 선택" },
            { Language.Italian, "Seleziona Tipo Creatura" }
        }},
        { "creatures.type.scoutmaster", new Dictionary<Language, string> {
            { Language.English, "Scoutmaster (Boss / Big Ghost)" },
            { Language.SimplifiedChinese, "教官 (Scoutmaster / 大幽灵)" },
            { Language.TraditionalChinese, "教官 (Scoutmaster / 大幽靈)" },
            { Language.Japanese, "スカウトマスター (教官/大幽霊)" },
            { Language.Korean, "스카우트마스터 (교관/대형 유령)" },
            { Language.Italian, "Scoutmaster (Boss / Grande Spettro)" }
        }},
        { "creatures.type.scoutmaster_desc", new Dictionary<Language, string> {
            { Language.English, "The menacing shadow boss. Stalks, teleports, grabs players and violently hurls them off cliffs." },
            { Language.SimplifiedChinese, "令人胆寒的黑影向导Boss。能瞬移追踪、抓取队员并残暴地抛下万丈悬崖。" },
            { Language.TraditionalChinese, "令人膽寒的黑影嚮導Boss。能瞬移追蹤、抓取隊員並殘暴地拋下萬丈懸崖。" },
            { Language.Japanese, "不気味な黒い影のボス。テレポート追跡し、プレイヤーを掴んで崖下へ投げ飛ばします。" },
            { Language.Korean, "공포스러운 그림자 보스. 순간이동으로 추적하고 플레이어를 붙잡아 절벽 아래로 내던집니다." },
            { Language.Italian, "L'inquietante boss ombra. Insegue, teletrasporta, afferra i giocatori e li scaglia dai dirupi." }
        }},
        { "creatures.type.bigghost", new Dictionary<Language, string> {
            { Language.English, "Player Ghost (Specter)" },
            { Language.SimplifiedChinese, "玩家灵体 (Player Ghost)" },
            { Language.TraditionalChinese, "玩家靈體 (Player Ghost)" },
            { Language.Japanese, "プレイヤーゴースト (霊体)" },
            { Language.Korean, "플레이어 고스트 (영체)" },
            { Language.Italian, "Fantasma Giocatore (Spettro)" }
        }},
        { "creatures.type.bigghost_desc", new Dictionary<Language, string> {
            { Language.English, "Ethereal floating specter. Supports custom size scaling (up to 5x Big Ghost) and target following." },
            { Language.SimplifiedChinese, "半透明漂浮幽灵。支持自定义体型倍率缩放（可达5倍大幽灵）并跟随目标。" },
            { Language.TraditionalChinese, "半透明漂浮幽靈。支援自訂體型倍率縮放（可達5倍大幽靈）並跟隨目標。" },
            { Language.Japanese, "浮遊する霊体。最大5倍までのサイズ拡大とターゲット追尾をサポート。" },
            { Language.Korean, "떠다니는 반투명 영체. 최대 5배 거대 유령 크기 조절 및 대상 추적 지원." },
            { Language.Italian, "Spettro etereo fluttuante. Supporta la scala dimensionale (fino a 5x) e l'inseguimento del bersaglio." }
        }},
        { "creatures.type.zombie", new Dictionary<Language, string> {
            { Language.English, "Mushroom Zombie" },
            { Language.SimplifiedChinese, "真菌丧尸 (Mushroom Zombie)" },
            { Language.TraditionalChinese, "真菌喪屍 (Mushroom Zombie)" },
            { Language.Japanese, "キノコゾンビ" },
            { Language.Korean, "버섯 좀비" },
            { Language.Italian, "Zombie Fungino" }
        }},
        { "creatures.type.zombie_desc", new Dictionary<Language, string> {
            { Language.English, "Fungal-infected crawler that wakes from slumber, lunges, bites and spreads spore madness." },
            { Language.SimplifiedChinese, "真菌寄生丧尸。能苏醒突袭、飞扑撕咬攀登者并传播孢子感染。" },
            { Language.TraditionalChinese, "真菌寄生喪屍。能甦醒突襲、飛撲撕咬攀登者並傳播孢子感染。" },
            { Language.Japanese, "キノコに寄生されたゾンビ。跳躍して噛みつき、胞子感染を広げます。" },
            { Language.Korean, "버섯에 기생당한 좀비. 도약하여 생존자를 물어뜯고 포자를 퍼뜨립니다." },
            { Language.Italian, "Zombie parassitato dai funghi. Balza, morde i sopravvissuti e diffonde spore." }
        }},
        { "creatures.type.scorpion", new Dictionary<Language, string> {
            { Language.English, "Scorpion (Mob & Item)" },
            { Language.SimplifiedChinese, "荒漠毒蝎 (Scorpion)" },
            { Language.TraditionalChinese, "荒漠毒蠍 (Scorpion)" },
            { Language.Japanese, "砂漠のサソリ" },
            { Language.Korean, "사막 전갈" },
            { Language.Italian, "Scorpione del Deserto" }
        }},
        { "creatures.type.scorpion_desc", new Dictionary<Language, string> {
            { Language.English, "Deadly desert arachnid. Attacks players, inflicts poison, but can also be caught, held, or cooked and eaten!" },
            { Language.SimplifiedChinese, "致命荒漠毒虫。会攻击施加剧毒，但也可被玩家捡入背包、手持或在篝火上烤熟食用！" },
            { Language.TraditionalChinese, "致命荒漠毒蟲。會攻擊施加劇毒，但也可被玩家撿入背包、手持或在篝火上烤熟食用！" },
            { Language.Japanese, "危険な毒サソリ。攻撃して毒を与えますが、拾って調理し食べることも可能！" },
            { Language.Korean, "치명적인 사막 전갈. 중독 공격을 가하지만, 가방에 줍거나 모닥불에 구워 먹을 수도 있습니다!" },
            { Language.Italian, "Aracnide velenoso del deserto. Avvelena i giocatori, ma può essere raccolto, cotto e mangiato!" }
        }},
        { "creatures.type.beetle", new Dictionary<Language, string> {
            { Language.English, "Giant Beetle (Mob & Item)" },
            { Language.SimplifiedChinese, "巨型甲虫 (Giant Beetle)" },
            { Language.TraditionalChinese, "巨型甲蟲 (Giant Beetle)" },
            { Language.Japanese, "巨大カブトムシ" },
            { Language.Korean, "거대 딱정벌레" },
            { Language.Italian, "Scarabeo Gigante" }
        }},
        { "creatures.type.beetle_desc", new Dictionary<Language, string> {
            { Language.English, "Heavily armored insect that rams into players with massive knockback force." },
            { Language.SimplifiedChinese, "重装重壳甲虫。会以巨大冲击力将玩家击飞出数米远。" },
            { Language.TraditionalChinese, "重裝重殼甲蟲。會以巨大衝擊力將玩家擊飛出數米遠。" },
            { Language.Japanese, "重装甲の昆虫。強力なノックバック衝撃でプレイヤーを吹き飛ばします。" },
            { Language.Korean, "단단한 갑각을 지닌 곤충. 거대한 넉백 힘으로 플레이어를 날려버립니다." },
            { Language.Italian, "Insetto corazzato che carica i giocatori respingendoli con forza devastante." }
        }},
        { "creatures.type.bees", new Dictionary<Language, string> {
            { Language.English, "Killer Bee Swarm (Beehive)" },
            { Language.SimplifiedChinese, "杀人蜂群 (Killer Bees)" },
            { Language.TraditionalChinese, "殺人蜂群 (Killer Bees)" },
            { Language.Japanese, "キラービーの群れ" },
            { Language.Korean, "살인벌 떼" },
            { Language.Italian, "Sciame di Vespe Assassine" }
        }},
        { "creatures.type.bees_desc", new Dictionary<Language, string> {
            { Language.English, "Spawns an active wild beehive that unleashes a raging swarm buzzing after all survivors." },
            { Language.SimplifiedChinese, "生成野外蜂巢并释放暴怒蜂群，嗡鸣追击周围所有攀登者。" },
            { Language.TraditionalChinese, "生成野外蜂巢並釋放暴怒蜂群，嗡鳴追擊周圍所有攀登者。" },
            { Language.Japanese, "野生の蜂の巣を生成し、激怒した蜂の群れがサバイバーを追跡します。" },
            { Language.Korean, "야생 벌집을 생성하고 성난 벌 떼를 풀어 주변의 모든 생존자를 공격합니다." },
            { Language.Italian, "Genera un alveare selvatico che rilascia uno sciame infuriato contro i superstiti." }
        }},
        { "creatures.spawn_anchor", new Dictionary<Language, string> {
            { Language.English, "Spawn Reference Anchor" },
            { Language.SimplifiedChinese, "生成基准位置" },
            { Language.TraditionalChinese, "生成基準位置" },
            { Language.Japanese, "生成の基準位置" },
            { Language.Korean, "생성 기준 위치" },
            { Language.Italian, "Punto di Riferimento" }
        }},
        { "creatures.anchor_self", new Dictionary<Language, string> {
            { Language.English, "In Front of Self" },
            { Language.SimplifiedChinese, "自己身前" },
            { Language.TraditionalChinese, "自己身前" },
            { Language.Japanese, "自分の前方" },
            { Language.Korean, "자신의 앞" },
            { Language.Italian, "Davanti a Sé" }
        }},
        { "creatures.anchor_player", new Dictionary<Language, string> {
            { Language.English, "Near Selected Player" },
            { Language.SimplifiedChinese, "选中队友身前" },
            { Language.TraditionalChinese, "選中隊友身前" },
            { Language.Japanese, "選択した仲間の前方" },
            { Language.Korean, "선택한 팀원 앞" },
            { Language.Italian, "Vicino al Giocatore" }
        }},
        { "creatures.anchor_crosshair", new Dictionary<Language, string> {
            { Language.English, "Crosshair Aim Point" },
            { Language.SimplifiedChinese, "准星视线落点" },
            { Language.TraditionalChinese, "準星視線落點" },
            { Language.Japanese, "照準の着地点" },
            { Language.Korean, "조준선 지면 착탄점" },
            { Language.Italian, "Punto di Mira / Mirino" }
        }},
        { "creatures.spawn_distance", new Dictionary<Language, string> {
            { Language.English, "Spawn Distance: {0:F1}m" },
            { Language.SimplifiedChinese, "生成距离: {0:F1} 米" },
            { Language.TraditionalChinese, "生成距離: {0:F1} 米" },
            { Language.Japanese, "生成距離: {0:F1}m" },
            { Language.Korean, "생성 거리: {0:F1}m" },
            { Language.Italian, "Distanza Generazione: {0:F1}m" }
        }},
        { "creatures.ghost_scale", new Dictionary<Language, string> {
            { Language.English, "Ghost Scale: {0:F1}x" },
            { Language.SimplifiedChinese, "幽灵体型倍率: {0:F1} 倍" },
            { Language.TraditionalChinese, "幽靈體型倍率: {0:F1} 倍" },
            { Language.Japanese, "ゴースト拡大倍率: {0:F1}x" },
            { Language.Korean, "유령 크기 배율: {0:F1}배" },
            { Language.Italian, "Scala Spettro: {0:F1}x" }
        }},
        { "creatures.aggro_target", new Dictionary<Language, string> {
            { Language.English, "Target & Aggro Binding" },
            { Language.SimplifiedChinese, "仇恨目标绑定" },
            { Language.TraditionalChinese, "仇恨目標綁定" },
            { Language.Japanese, "ターゲット / 敵対指定" },
            { Language.Korean, "어그로 대상 지정" },
            { Language.Italian, "Bersaglio Aggro" }
        }},
        { "creatures.target_self", new Dictionary<Language, string> {
            { Language.English, "Lock Onto Myself" },
            { Language.SimplifiedChinese, "锁定自己" },
            { Language.TraditionalChinese, "鎖定自己" },
            { Language.Japanese, "自分を狙わせる" },
            { Language.Korean, "자신에게 어그로 고정" },
            { Language.Italian, "Aggro su Me Stesso" }
        }},
        { "creatures.target_selected", new Dictionary<Language, string> {
            { Language.English, "Lock Onto Selected Player" },
            { Language.SimplifiedChinese, "锁定选中的队友" },
            { Language.TraditionalChinese, "鎖定選中的隊友" },
            { Language.Japanese, "選択した仲間を狙わせる" },
            { Language.Korean, "선택한 팀원에게 어그로 고정" },
            { Language.Italian, "Aggro su Giocatore Selezionato" }
        }},
        { "creatures.target_none", new Dictionary<Language, string> {
            { Language.English, "Free AI Roam" },
            { Language.SimplifiedChinese, "自由漫游寻路" },
            { Language.TraditionalChinese, "自由漫遊尋路" },
            { Language.Japanese, "自由徘徊" },
            { Language.Korean, "자유 배회" },
            { Language.Italian, "Vagabondaggio Libero" }
        }},
        { "creatures.btn_spawn", new Dictionary<Language, string> {
            { Language.English, "Spawn Creature" },
            { Language.SimplifiedChinese, "立即生成生物" },
            { Language.TraditionalChinese, "立即生成生物" },
            { Language.Japanese, "クリーチャーを生成" },
            { Language.Korean, "생물 즉시 생성" },
            { Language.Italian, "Genera Creatura Ora" }
        }},
        { "creatures.success", new Dictionary<Language, string> {
            { Language.English, "Spawned {0} at safe ground ({1:F1}m away)!" },
            { Language.SimplifiedChinese, "已在安全地面成功生成 {0}（距离 {1:F1} 米）！" },
            { Language.TraditionalChinese, "已在安全地面成功生成 {0}（距離 {1:F1} 米）！" },
            { Language.Japanese, "安全な地面に {0} を生成しました (距離 {1:F1}m)！" },
            { Language.Korean, "안전한 지면에 {0} 생성 완료 ({1:F1}m 거리)!" },
            { Language.Italian, "Generato {0} su terreno sicuro a {1:F1}m!" }
        }},
        { "creatures.cooldown", new Dictionary<Language, string> {
            { Language.English, "Please wait a moment before spawning another creature..." },
            { Language.SimplifiedChinese, "刷怪冷却中，请稍后再试..." },
            { Language.TraditionalChinese, "刷怪冷卻中，請稍後再試..." },
            { Language.Japanese, "連続生成クールダウン中です。少しお待ちください..." },
            { Language.Korean, "연속 생성 쿨다운 중입니다. 잠시 후 다시 시도하세요..." },
            { Language.Italian, "Attendi un istante prima di generare un'altra creatura..." }
        }},
        { "creatures.no_ground", new Dictionary<Language, string> {
            { Language.English, "Failed to find solid ground at target position. Try moving closer or adjusting distance." },
            { Language.SimplifiedChinese, "目标位置未检测到坚实地面，无法生成生物。请调整距离或移动位置。" },
            { Language.TraditionalChinese, "目標位置未檢測到堅實地面，無法生成生物。請調整距離或移動位置。" },
            { Language.Japanese, "目標地点に安定した地面が見つかりません。距離を調整してください。" },
            { Language.Korean, "목표 위치에서 안정적인 지면을 찾지 못했습니다. 거리를 조절해 보세요." },
            { Language.Italian, "Impossibile trovare terreno solido nella posizione bersaglio. Regola la distanza." }
        }},
        // Item Attributes & Enchantments
        { "items.attributes_title", new Dictionary<Language, string> {
            { Language.English, "ITEM ATTRIBUTES & ENCHANTMENT" },
            { Language.SimplifiedChinese, "物品属性定制与附魔" },
            { Language.TraditionalChinese, "物品屬性定制與附魔" },
            { Language.Japanese, "アイテム属性カスタム＆エンチャント" },
            { Language.Korean, "아이템 속성 커스텀 및 마법 부여" },
            { Language.Italian, "ATTRIBUTI E INCANTAMENTO OGGETTI" }
        }},
        { "items.mushroom_customization", new Dictionary<Language, string> {
            { Language.English, "Mushroom Spawn Customization" },
            { Language.SimplifiedChinese, "蘑菇生成属性定制" },
            { Language.TraditionalChinese, "蘑菇生成屬性定制" },
            { Language.Japanese, "キノコ生成属性カスタム" },
            { Language.Korean, "버섯 생성 속성 설정" },
            { Language.Italian, "Personalizzazione Funghi Generati" }
        }},
        { "items.mushroom_mode_vanilla", new Dictionary<Language, string> {
            { Language.English, "Default (Vanilla)" },
            { Language.SimplifiedChinese, "原版随机" },
            { Language.TraditionalChinese, "原版隨機" },
            { Language.Japanese, "デフォルト（ランダム）" },
            { Language.Korean, "기본값 (랜덤)" },
            { Language.Italian, "Predefinito (Casuale)" }
        }},
        { "items.mushroom_mode_purified", new Dictionary<Language, string> {
            { Language.English, "Purified (Good Only)" },
            { Language.SimplifiedChinese, "纯良无毒 (仅良性效果)" },
            { Language.TraditionalChinese, "純良無毒 (僅良性效果)" },
            { Language.Japanese, "無毒化（有益効果のみ）" },
            { Language.Korean, "정화됨 (유익한 효과만)" },
            { Language.Italian, "Purificato (Solo Benefici)" }
        }},
        { "items.mushroom_mode_toxic", new Dictionary<Language, string> {
            { Language.English, "Toxic (Bad Only)" },
            { Language.SimplifiedChinese, "剧毒恶搞 (仅恶性效果)" },
            { Language.TraditionalChinese, "劇毒惡搞 (僅惡性效果)" },
            { Language.Japanese, "劇毒化（有害効果のみ）" },
            { Language.Korean, "맹독 (유해한 효과만)" },
            { Language.Italian, "Tossico (Solo Malus)" }
        }},
        { "items.mushroom_mode_specific", new Dictionary<Language, string> {
            { Language.English, "Specific Effect:" },
            { Language.SimplifiedChinese, "指定效果:" },
            { Language.TraditionalChinese, "指定效果:" },
            { Language.Japanese, "指定効果:" },
            { Language.Korean, "지정 효과:" },
            { Language.Italian, "Effetto Specifico:" }
        }},
        { "items.mushroom_effect_0", new Dictionary<Language, string> {
            { Language.English, "Luminous Glow (Body Light)" },
            { Language.SimplifiedChinese, "0: 夜光荧光 (身体照明)" },
            { Language.TraditionalChinese, "0: 夜光熒光 (身體照明)" },
            { Language.Japanese, "0: 発光（体温照明）" },
            { Language.Korean, "0: 발광 (신체 조명)" },
            { Language.Italian, "0: Bagliore Luminoso" }
        }},
        { "items.mushroom_effect_1", new Dictionary<Language, string> {
            { Language.English, "Super Speed (Run & Climb)" },
            { Language.SimplifiedChinese, "1: 超绝神速 (移动与攀爬)" },
            { Language.TraditionalChinese, "1: 超絕神速 (移動與攀爬)" },
            { Language.Japanese, "1: 神速（移動＆登山）" },
            { Language.Korean, "1: 초고속 (이동 및 등반)" },
            { Language.Italian, "1: Super Velocità" }
        }},
        { "items.mushroom_effect_2", new Dictionary<Language, string> {
            { Language.English, "Moon Low Gravity (High Jump)" },
            { Language.SimplifiedChinese, "2: 月球低重力 (大跳跃)" },
            { Language.TraditionalChinese, "2: 月球低重力 (大跳躍)" },
            { Language.Japanese, "2: 低重力（大ジャンプ）" },
            { Language.Korean, "2: 저중력 (슈퍼 점프)" },
            { Language.Italian, "2: Bassa Gravità Lunare" }
        }},
        { "items.mushroom_effect_3", new Dictionary<Language, string> {
            { Language.English, "Golden Invincibility" },
            { Language.SimplifiedChinese, "3: 金身绝对无敌 (免疫一切)" },
            { Language.TraditionalChinese, "3: 金身絕對無敵 (免疫一切)" },
            { Language.Japanese, "3: 完全無敵（黄金身）" },
            { Language.Korean, "3: 절대 무적 (골든 바디)" },
            { Language.Italian, "3: Invincibilità Dorata" }
        }},
        { "items.mushroom_effect_4", new Dictionary<Language, string> {
            { Language.English, "Full Cleanse (Heal All)" },
            { Language.SimplifiedChinese, "4: 完全净化全愈 (状态清空)" },
            { Language.TraditionalChinese, "4: 完全淨化全愈 (狀態清空)" },
            { Language.Japanese, "4: 完全浄化（全回復）" },
            { Language.Korean, "4: 완전 정화 (모든 치유)" },
            { Language.Italian, "4: Purificazione Totale" }
        }},
        { "items.mushroom_effect_5", new Dictionary<Language, string> {
            { Language.English, "Spore Blast (Fart Skyward)" },
            { Language.SimplifiedChinese, "5: 孢子爆炸/屁崩冲天" },
            { Language.TraditionalChinese, "5: 孢子爆炸/屁崩沖天" },
            { Language.Japanese, "5: 胞子爆発（吹き飛び）" },
            { Language.Korean, "5: 포자 폭발 (공중 날림)" },
            { Language.Italian, "5: Esplosione di Spore" }
        }},
        { "items.mushroom_effect_6", new Dictionary<Language, string> {
            { Language.English, "Total Blindness (Black Screen)" },
            { Language.SimplifiedChinese, "6: 全屏致盲黑屏" },
            { Language.TraditionalChinese, "6: 全屏致盲黑屏" },
            { Language.Japanese, "6: 暗闇（全画面盲目）" },
            { Language.Korean, "6: 시야 상실 (암흑)" },
            { Language.Italian, "6: Cecità Totale" }
        }},
        { "items.mushroom_effect_7", new Dictionary<Language, string> {
            { Language.English, "Trip & Fall (Slip Down)" },
            { Language.SimplifiedChinese, "7: 脱力摔倒滑坠" },
            { Language.TraditionalChinese, "7: 脫力摔倒滑墜" },
            { Language.Japanese, "7: 転倒滑落" },
            { Language.Korean, "7: 미끄러짐 및 넘어짐" },
            { Language.Italian, "7: Caduta e Scivolone" }
        }},
        { "items.mushroom_effect_8", new Dictionary<Language, string> {
            { Language.English, "Parasitic Spores Infestation" },
            { Language.SimplifiedChinese, "8: 孢子寄生感染" },
            { Language.TraditionalChinese, "8: 孢子寄生感染" },
            { Language.Japanese, "8: 胞子寄生感染" },
            { Language.Korean, "8: 기생 포자 감염" },
            { Language.Italian, "8: Infestazione da Spore" }
        }},
        { "items.mushroom_effect_9", new Dictionary<Language, string> {
            { Language.English, "Limb Numbness (No Grip)" },
            { Language.SimplifiedChinese, "9: 四肢麻木脱力" },
            { Language.TraditionalChinese, "9: 四肢麻木脫力" },
            { Language.Japanese, "9: 四肢麻痺" },
            { Language.Korean, "9: 사지 마비" },
            { Language.Italian, "9: Intorpidimento Totale" }
        }},
        { "items.blowgun_enchantment", new Dictionary<Language, string> {
            { Language.English, "Blowgun Dart Ammunition" },
            { Language.SimplifiedChinese, "吹箭筒弹药实时附魔" },
            { Language.TraditionalChinese, "吹箭筒彈藥實時附魔" },
            { Language.Japanese, "吹き矢弾薬リアルタイム付与" },
            { Language.Korean, "바람총 탄약 실시간 마법부여" },
            { Language.Italian, "Incantamento Dardi Cerbottana" }
        }},
        { "items.dart_enable", new Dictionary<Language, string> {
            { Language.English, "Enchant Dart Ammunition" },
            { Language.SimplifiedChinese, "启用吹箭筒弹药附魔" },
            { Language.TraditionalChinese, "啟用吹箭筒彈藥附魔" },
            { Language.Japanese, "吹き矢弾薬エンチャント有効化" },
            { Language.Korean, "바람총 탄약 마법부여 활성화" },
            { Language.Italian, "Attiva Incanto Dardi" }
        }},
        { "items.dart_ammo_buffs", new Dictionary<Language, string> {
            { Language.English, "Beneficial Darts:" },
            { Language.SimplifiedChinese, "增益与治疗弹:" },
            { Language.TraditionalChinese, "增益與治療彈:" },
            { Language.Japanese, "強化・治療弾:" },
            { Language.Korean, "강화 및 치유 탄약:" },
            { Language.Italian, "Dardi Benefici e Cure:" }
        }},
        { "items.dart_ammo_debuffs", new Dictionary<Language, string> {
            { Language.English, "Debuff & Prank Darts:" },
            { Language.SimplifiedChinese, "负面与恶搞弹:" },
            { Language.TraditionalChinese, "負面與惡搞彈:" },
            { Language.Japanese, "妨害・悪ふざけ弾:" },
            { Language.Korean, "디버프 및 장난 탄약:" },
            { Language.Italian, "Dardi Malus e Scherzi:" }
        }},
        { "items.dart_invincible", new Dictionary<Language, string> {
            { Language.English, "Invincible" },
            { Language.SimplifiedChinese, "金身无敌" },
            { Language.TraditionalChinese, "金身無敵" },
            { Language.Japanese, "無敵" },
            { Language.Korean, "무적" },
            { Language.Italian, "Invincibile" }
        }},
        { "items.dart_speed", new Dictionary<Language, string> {
            { Language.English, "Speed" },
            { Language.SimplifiedChinese, "超绝神速" },
            { Language.TraditionalChinese, "超絕神速" },
            { Language.Japanese, "神速" },
            { Language.Korean, "신속" },
            { Language.Italian, "Velocità" }
        }},
        { "items.dart_stamina", new Dictionary<Language, string> {
            { Language.English, "Infinite Stamina" },
            { Language.SimplifiedChinese, "无限耐力" },
            { Language.TraditionalChinese, "無限耐力" },
            { Language.Japanese, "無限スタミナ" },
            { Language.Korean, "무한 스태미나" },
            { Language.Italian, "Resistenza Infinita" }
        }},
        { "items.dart_cleanse", new Dictionary<Language, string> {
            { Language.English, "Full Cleanse" },
            { Language.SimplifiedChinese, "全愈净化" },
            { Language.TraditionalChinese, "全愈淨化" },
            { Language.Japanese, "完全浄化" },
            { Language.Korean, "완전 치유" },
            { Language.Italian, "Guarigione Totale" }
        }},
        { "items.dart_lowgrav", new Dictionary<Language, string> {
            { Language.English, "Low Grav" },
            { Language.SimplifiedChinese, "低重力" },
            { Language.TraditionalChinese, "低重力" },
            { Language.Japanese, "低重力" },
            { Language.Korean, "저중력" },
            { Language.Italian, "Bassa Gravità" }
        }},
        { "items.dart_glow", new Dictionary<Language, string> {
            { Language.English, "Glow" },
            { Language.SimplifiedChinese, "夜光发光" },
            { Language.TraditionalChinese, "夜光發光" },
            { Language.Japanese, "発光" },
            { Language.Korean, "발광" },
            { Language.Italian, "Bagliore" }
        }},
        { "items.dart_revive", new Dictionary<Language, string> {
            { Language.English, "Medical Revive" },
            { Language.SimplifiedChinese, "医疗复活" },
            { Language.TraditionalChinese, "醫療復活" },
            { Language.Japanese, "医療蘇生" },
            { Language.Korean, "의료 부활" },
            { Language.Italian, "Rianimazione Medica" }
        }},
        { "items.dart_poison", new Dictionary<Language, string> {
            { Language.English, "Poison" },
            { Language.SimplifiedChinese, "致命剧毒" },
            { Language.TraditionalChinese, "致命劇毒" },
            { Language.Japanese, "劇毒" },
            { Language.Korean, "맹독" },
            { Language.Italian, "Veleno" }
        }},
        { "items.dart_starvation", new Dictionary<Language, string> {
            { Language.English, "Starvation" },
            { Language.SimplifiedChinese, "极度饥饿" },
            { Language.TraditionalChinese, "極度飢餓" },
            { Language.Japanese, "飢餓" },
            { Language.Korean, "극심한 허기" },
            { Language.Italian, "Fame Estrema" }
        }},
        { "items.dart_sleep", new Dictionary<Language, string> {
            { Language.English, "Hypnosis/Sleep" },
            { Language.SimplifiedChinese, "深度昏睡" },
            { Language.TraditionalChinese, "深度昏睡" },
            { Language.Japanese, "催眠・昏睡" },
            { Language.Korean, "수면 및 혼수" },
            { Language.Italian, "Sonnolenza Profonda" }
        }},
        { "items.dart_fall", new Dictionary<Language, string> {
            { Language.English, "Slip & Fall" },
            { Language.SimplifiedChinese, "脱力扑街" },
            { Language.TraditionalChinese, "脫力撲街" },
            { Language.Japanese, "転倒滑落" },
            { Language.Korean, "넘어뜨리기" },
            { Language.Italian, "Caduta a Terra" }
        }},
        { "items.dart_thorns", new Dictionary<Language, string> {
            { Language.English, "Thorns" },
            { Language.SimplifiedChinese, "万刺穿身" },
            { Language.TraditionalChinese, "萬刺穿身" },
            { Language.Japanese, "茨の針" },
            { Language.Korean, "가시 찌르기" },
            { Language.Italian, "Spine Infisse" }
        }},
        { "items.dart_spores", new Dictionary<Language, string> {
            { Language.English, "Spores" },
            { Language.SimplifiedChinese, "孢子寄生" },
            { Language.TraditionalChinese, "孢子寄生" },
            { Language.Japanese, "胞子感染" },
            { Language.Korean, "포자 감염" },
            { Language.Italian, "Spore Fungine" }
        }},
        { "items.dart_blind", new Dictionary<Language, string> {
            { Language.English, "Blindness" },
            { Language.SimplifiedChinese, "全屏致盲" },
            { Language.TraditionalChinese, "全屏致盲" },
            { Language.Japanese, "全盲目" },
            { Language.Korean, "암흑 실명" },
            { Language.Italian, "Cecità" }
        }},
        { "items.dart_numb", new Dictionary<Language, string> {
            { Language.English, "Numbness" },
            { Language.SimplifiedChinese, "肢体麻木" },
            { Language.TraditionalChinese, "肢體麻木" },
            { Language.Japanese, "四肢麻痺" },
            { Language.Korean, "사지 마비" },
            { Language.Italian, "Intorpidimento" }
        }},
        { "items.dart_chaos", new Dictionary<Language, string> {
            { Language.English, "Chaos (Random Every Shot)" },
            { Language.SimplifiedChinese, "混沌随机 (每枪随机一种效果)" },
            { Language.TraditionalChinese, "混沌隨機 (每槍隨機一種效果)" },
            { Language.Japanese, "カオス（毎射ランダム効果）" },
            { Language.Korean, "카오스 (발사마다 무작위 효과)" },
            { Language.Italian, "Caos (Effetto Casuale ad Ogni Colpo)" }
        }},
        { "items.extended_attributes", new Dictionary<Language, string> {
            { Language.English, "Extended Item Attributes" },
            { Language.SimplifiedChinese, "扩展物品属性" },
            { Language.TraditionalChinese, "擴展物品屬性" },
            { Language.Japanese, "拡張アイテム属性" },
            { Language.Korean, "확장 아이템 속성" },
            { Language.Italian, "Attributi Oggetti Estesi" }
        }},
        { "items.food_poison_immunity", new Dictionary<Language, string> {
            { Language.English, "Food Poison Immunity (Spoiled/Toxic)" },
            { Language.SimplifiedChinese, "食物防腐免毒 (免疫变质与毒食物)" },
            { Language.TraditionalChinese, "食物防腐免毒 (免疫變質與毒食物)" },
            { Language.Japanese, "食中毒完全無効（腐敗・有毒食）" },
            { Language.Korean, "식중독 완전 면역 (부패 및 독 음식)" },
            { Language.Italian, "Immunità da Intossicazione Alimentare" }
        }},
        { "items.infinite_tool_charge", new Dictionary<Language, string> {
            { Language.English, "Infinite Torch & Tool Charge" },
            { Language.SimplifiedChinese, "火把与工具无限耐久/充能" },
            { Language.TraditionalChinese, "火把與工具無限耐久/充能" },
            { Language.Japanese, "松明・ツール無限燃料/耐久" },
            { Language.Korean, "횃불 및 도구 무한 내구도/충전" },
            { Language.Italian, "Carica Infinita Torce e Strumenti" }
        }}
,

        // Status Effects & Debuffs
        { "status.injury", new Dictionary<Language, string> {
            { Language.English, "Injury" },
            { Language.SimplifiedChinese, "负伤" },
            { Language.TraditionalChinese, "受傷" },
            { Language.Japanese, "負傷" },
            { Language.Korean, "부상" },
            { Language.Italian, "Ferita" }
        }},
        { "status.hunger", new Dictionary<Language, string> {
            { Language.English, "Hunger" },
            { Language.SimplifiedChinese, "饥饿" },
            { Language.TraditionalChinese, "飢餓" },
            { Language.Japanese, "空腹" },
            { Language.Korean, "굶주림" },
            { Language.Italian, "Fame" }
        }},
        { "status.cold", new Dictionary<Language, string> {
            { Language.English, "Cold" },
            { Language.SimplifiedChinese, "寒冷" },
            { Language.TraditionalChinese, "寒冷" },
            { Language.Japanese, "寒さ" },
            { Language.Korean, "추위" },
            { Language.Italian, "Freddo" }
        }},
        { "status.poison", new Dictionary<Language, string> {
            { Language.English, "Poison" },
            { Language.SimplifiedChinese, "中毒" },
            { Language.TraditionalChinese, "中毒" },
            { Language.Japanese, "毒" },
            { Language.Korean, "독" },
            { Language.Italian, "Veleno" }
        }},
        { "status.crab", new Dictionary<Language, string> {
            { Language.English, "Parasitic Crab" },
            { Language.SimplifiedChinese, "寄生蟹" },
            { Language.TraditionalChinese, "寄生蟹" },
            { Language.Japanese, "カニ寄生" },
            { Language.Korean, "기생 게" },
            { Language.Italian, "Granchio Parassita" }
        }},
        { "status.curse", new Dictionary<Language, string> {
            { Language.English, "Curse" },
            { Language.SimplifiedChinese, "诅咒" },
            { Language.TraditionalChinese, "詛咒" },
            { Language.Japanese, "呪い" },
            { Language.Korean, "저주" },
            { Language.Italian, "Maledizione" }
        }},
        { "status.drowsy", new Dictionary<Language, string> {
            { Language.English, "Drowsiness" },
            { Language.SimplifiedChinese, "嗜睡" },
            { Language.TraditionalChinese, "嗜睡" },
            { Language.Japanese, "眠気" },
            { Language.Korean, "졸음" },
            { Language.Italian, "Sonnolenza" }
        }},
        { "status.weight", new Dictionary<Language, string> {
            { Language.English, "Overweight" },
            { Language.SimplifiedChinese, "超重" },
            { Language.TraditionalChinese, "超重" },
            { Language.Japanese, "重量過多" },
            { Language.Korean, "과체중" },
            { Language.Italian, "Sovraccarico" }
        }},
        { "status.hot", new Dictionary<Language, string> {
            { Language.English, "Overheat" },
            { Language.SimplifiedChinese, "炎热" },
            { Language.TraditionalChinese, "炎熱" },
            { Language.Japanese, "暑さ" },
            { Language.Korean, "더위" },
            { Language.Italian, "Calore" }
        }},
        { "status.thorns", new Dictionary<Language, string> {
            { Language.English, "Thorns" },
            { Language.SimplifiedChinese, "荆棘" },
            { Language.TraditionalChinese, "荊棘" },
            { Language.Japanese, "トゲ" },
            { Language.Korean, "가시" },
            { Language.Italian, "Spine" }
        }},
        { "status.spores", new Dictionary<Language, string> {
            { Language.English, "Spores" },
            { Language.SimplifiedChinese, "孢子" },
            { Language.TraditionalChinese, "孢子" },
            { Language.Japanese, "胞子" },
            { Language.Korean, "포자" },
            { Language.Italian, "Spore" }
        }},
        { "status.web", new Dictionary<Language, string> {
            { Language.English, "Spider Web" },
            { Language.SimplifiedChinese, "蛛网" },
            { Language.TraditionalChinese, "蛛網" },
            { Language.Japanese, "クモの巣" },
            { Language.Korean, "거미줄" },
            { Language.Italian, "Ragnatela" }
        }},
        { "status.panel_title", new Dictionary<Language, string> {
            { Language.English, "Status Effects & Debuffs" },
            { Language.SimplifiedChinese, "状态效果与负面管理" },
            { Language.TraditionalChinese, "狀態效果與負面管理" },
            { Language.Japanese, "ステータス・デバフ管理" },
            { Language.Korean, "상태 효과 및 디버프 관리" },
            { Language.Italian, "Stato ed Effetti Negativi" }
        }},
        { "status.select_effect", new Dictionary<Language, string> {
            { Language.English, "Select Debuff" },
            { Language.SimplifiedChinese, "选择负面效果" },
            { Language.TraditionalChinese, "選擇負面效果" },
            { Language.Japanese, "状態異常を選択" },
            { Language.Korean, "디버프 효과 선택" },
            { Language.Italian, "Seleziona Effetto" }
        }},
        { "status.amount", new Dictionary<Language, string> {
            { Language.English, "Intensity" },
            { Language.SimplifiedChinese, "效果强度" },
            { Language.TraditionalChinese, "效果強度" },
            { Language.Japanese, "効果の強度" },
            { Language.Korean, "효과 강도" },
            { Language.Italian, "Intensità" }
        }},
        { "status.add_effect", new Dictionary<Language, string> {
            { Language.English, "Apply Debuff" },
            { Language.SimplifiedChinese, "施加负面效果" },
            { Language.TraditionalChinese, "施加負面效果" },
            { Language.Japanese, "効果を付与" },
            { Language.Korean, "효과 부여" },
            { Language.Italian, "Applica Effetto" }
        }},
        { "status.subtract_effect", new Dictionary<Language, string> {
            { Language.English, "Reduce Debuff" },
            { Language.SimplifiedChinese, "减轻负面效果" },
            { Language.TraditionalChinese, "減輕負面效果" },
            { Language.Japanese, "効果を軽減" },
            { Language.Korean, "효과 완화" },
            { Language.Italian, "Riduci Effetto" }
        }},
        { "status.clear_all", new Dictionary<Language, string> {
            { Language.English, "Clear All Debuffs" },
            { Language.SimplifiedChinese, "清除所有负面状态" },
            { Language.TraditionalChinese, "清除所有負面狀態" },
            { Language.Japanese, "全デバフ解除" },
            { Language.Korean, "모든 디버프 해제" },
            { Language.Italian, "Rimuovi Tutti i Debuff" }
        }},
        { "status.preset_torture", new Dictionary<Language, string> {
            { Language.English, "Extreme Torture" },
            { Language.SimplifiedChinese, "极限折磨 (恶搞)" },
            { Language.TraditionalChinese, "極限折磨 (惡搞)" },
            { Language.Japanese, "拷問プリセット" },
            { Language.Korean, "고문 프리셋" },
            { Language.Italian, "Tortura Estrema" }
        }},
        { "status.preset_critical", new Dictionary<Language, string> {
            { Language.English, "Near Death" },
            { Language.SimplifiedChinese, "濒死重伤" },
            { Language.TraditionalChinese, "瀕死重傷" },
            { Language.Japanese, "重傷プリセット" },
            { Language.Korean, "빈사 중상" },
            { Language.Italian, "Ferita Mortale" }
        }},
        { "status.preset_purify", new Dictionary<Language, string> {
            { Language.English, "Divine Purify" },
            { Language.SimplifiedChinese, "圣光净化 (全清+满耐力)" },
            { Language.TraditionalChinese, "聖光淨化 (全清+滿耐力)" },
            { Language.Japanese, "聖なる浄化 (全快)" },
            { Language.Korean, "신성한 정화 (완전 회복)" },
            { Language.Italian, "Purificazione Divina" }
        }},
        { "lobby.clear_all_afflictions_all", new Dictionary<Language, string> {
            { Language.English, "Clear All Players' Debuffs" },
            { Language.SimplifiedChinese, "清除全员负面状态" },
            { Language.TraditionalChinese, "清除全員負面狀態" },
            { Language.Japanese, "全員のデバフを解除" },
            { Language.Korean, "모든 플레이어 디버프 해제" },
            { Language.Italian, "Rimuovi Debuff a Tutti" }
        }},
        { "lobby.target_status_mgmt", new Dictionary<Language, string> {
            { Language.English, "Target Player Status Management" },
            { Language.SimplifiedChinese, "目标玩家状态管理" },
            { Language.TraditionalChinese, "目標玩家狀態管理" },
            { Language.Japanese, "対象プレイヤーのステータス管理" },
            { Language.Korean, "대상 플레이어 상태 관리" },
            { Language.Italian, "Gestione Stato Bersaglio" }
        }},
        { "status.non_host_hint", new Dictionary<Language, string> {
            { Language.English, "Note: Non-host can apply debuffs, remove thorns and boost morale." },
            { Language.SimplifiedChinese, "提示：非房主模式可对他人施加负面、拔除荆棘与提振耐力。" },
            { Language.TraditionalChinese, "提示：非房主模式可對他人施加負面、拔除荊棘與提振耐力。" },
            { Language.Japanese, "注意: 非ホスト時はデバフ付与・トゲ除去・スタミナ回復が有効です。" },
            { Language.Korean, "참고: 방장이 아닐 경우 디버프 부여, 가시 제거 및 사기 진작이 적용됩니다." },
            { Language.Italian, "Nota: Da non-host puoi applicare debuff, rimuovere spine e ripristinare vigore." }
        }},
        { "status.toast_applied", new Dictionary<Language, string> {
            { Language.English, "Applied status: {0} ({1:P0}) to {2}" },
            { Language.SimplifiedChinese, "已对 {2} 施加状态: {0} ({1:P0})" },
            { Language.TraditionalChinese, "已對 {2} 施加狀態: {0} ({1:P0})" },
            { Language.Japanese, "{2} に状態付与: {0} ({1:P0})" },
            { Language.Korean, "{2} 에게 상태 부여됨: {0} ({1:P0})" },
            { Language.Italian, "Applicato stato: {0} ({1:P0}) a {2}" }
        }},
        { "status.toast_cleared", new Dictionary<Language, string> {
            { Language.English, "Cleared all afflictions for {0}" },
            { Language.SimplifiedChinese, "已清除 {0} 的所有负面状态" },
            { Language.TraditionalChinese, "已清除 {0} 的所有負面狀態" },
            { Language.Japanese, "{0} の全デバフを解除しました" },
            { Language.Korean, "모든 디버프가 해제되었습니다" },
            { Language.Italian, "Rimossi tutti i debuff da {0}" }
        }},
        { "network.reconnect_toast", new Dictionary<Language, string> {
            { Language.English, "Network interruption detected ({0}). Auto-reconnecting to room..." },
            { Language.SimplifiedChinese, "检测到网络异常中断 ({0})，正在尝试快速自愈重连..." },
            { Language.TraditionalChinese, "檢測到網路異常中斷 ({0})，正在嘗試快速自愈重連..." },
            { Language.Japanese, "ネットワーク切断を検知 ({0})。ルームへ自動再接続中..." },
            { Language.Korean, "네트워크 중단 감지됨 ({0}). 방으로 자동 재접속 시도 중..." },
            { Language.Italian, "Interruzione di rete rilevata ({0}). Riconnessione automatica in corso..." }
        }},
        { "network.antikick_toast", new Dictionary<Language, string> {
            { Language.English, "Host kick attempt intercepted! Kept in room." },
            { Language.SimplifiedChinese, "已自动拦截房主踢人指令！保持在当前房间中。" },
            { Language.TraditionalChinese, "已自動攔截房主踢人指令！保持在當前房間中。" },
            { Language.Japanese, "ホストからのキックを自動遮断しました！ルームを維持します。" },
            { Language.Korean, "호스트의 강퇴 명령을 차단했습니다! 현재 방에 유지됩니다." },
            { Language.Italian, "Tentativo di espulsione dell'host intercettato! Rimasto nella stanza." }
        }},
        { "network.tuning_applied", new Dictionary<Language, string> {
            { Language.English, "High-resilience network tuning applied." },
            { Language.SimplifiedChinese, "高韧性抗抖动网络优化已生效。" },
            { Language.TraditionalChinese, "高韌性抗抖動網路優化已生效。" },
            { Language.Japanese, "高耐久ネットワーク最適化が適用されました。" },
            { Language.Korean, "고내구성 네트워크 최적화가 적용되었습니다." },
            { Language.Italian, "Ottimizzazione di rete ad alta resilienza applicata." }
        }},
        { "lobby.network_title", new Dictionary<Language, string> {
            { Language.English, "NETWORK & PROTECTION" },
            { Language.SimplifiedChinese, "网络优化与防掉线" },
            { Language.TraditionalChinese, "網路優化與防斷線" },
            { Language.Japanese, "ネットワーク最適化・保護" },
            { Language.Korean, "네트워크 최적화 및 보호" },
            { Language.Italian, "RETE E PROTEZIONE" }
        }},
        { "lobby.enable_network_tuning", new Dictionary<Language, string> {
            { Language.English, "Network Tuning (Robust ACK/Resend)" },
            { Language.SimplifiedChinese, "网络底层加固 (防抖动/抗丢包)" },
            { Language.TraditionalChinese, "網路底層加固 (防抖動/抗丟包)" },
            { Language.Japanese, "通信パラメータ最適化 (再送/耐性強化)" },
            { Language.Korean, "네트워크 파라미터 최적화 (재전송 강화)" },
            { Language.Italian, "Ottimizzazione Rete (Resilienza ACK)" }
        }},
        { "lobby.enable_antikick", new Dictionary<Language, string> {
            { Language.English, "Anti-Kick (Block Host Kick)" },
            { Language.SimplifiedChinese, "防房主恶意踢出 (Anti-Kick)" },
            { Language.TraditionalChinese, "防房主惡意踢出 (Anti-Kick)" },
            { Language.Japanese, "キック保護 (ホストキック無効化)" },
            { Language.Korean, "강퇴 방지 (호스트 강퇴 차단)" },
            { Language.Italian, "Anti-Espulsione (Blocca Kick)" }
        }},
        { "lobby.enable_autoreconnect", new Dictionary<Language, string> {
            { Language.English, "Auto-Reconnect & Rejoin Room" },
            { Language.SimplifiedChinese, "断线自动自愈重连 (Auto-Rejoin)" },
            { Language.TraditionalChinese, "斷線自動自愈重連 (Auto-Rejoin)" },
            { Language.Japanese, "切断時自動再接続 (Auto-Rejoin)" },
            { Language.Korean, "연결 끊김 시 자동 재접속" },
            { Language.Italian, "Riconnessione Automatica alla Stanza" }
        }},
        { "items.suffix_toxic", new Dictionary<Language, string> {
            { Language.English, " (Toxic)" },
            { Language.SimplifiedChinese, "(毒)" },
            { Language.TraditionalChinese, "(毒)" },
            { Language.Japanese, "（毒）" },
            { Language.Korean, "(독)" },
            { Language.Italian, " (Tossico)" }
        }},
        { "items.suffix_nontoxic", new Dictionary<Language, string> {
            { Language.English, " (Safe)" },
            { Language.SimplifiedChinese, "(无毒)" },
            { Language.TraditionalChinese, "(無毒)" },
            { Language.Japanese, "（無毒）" },
            { Language.Korean, "(무독)" },
            { Language.Italian, " (Non tossico)" }
        }},

        // Achievements Tab & UI Keys
        { "achievements.title", new Dictionary<Language, string> {
            { Language.English, "Steam Achievements" },
            { Language.SimplifiedChinese, "Steam 成就管理" },
            { Language.TraditionalChinese, "Steam 成就管理" },
            { Language.Japanese, "Steam 実績管理" },
            { Language.Korean, "Steam 도전 과제 관리" },
            { Language.Italian, "Gestione Obiettivi Steam" }
        }},
        { "achievements.desc", new Dictionary<Language, string> {
            { Language.English, "Click an achievement name to complete it individually. Or use async batch completion." },
            { Language.SimplifiedChinese, "点击成就名称即可手动单项完成；支持一键异步安全完成全部成就。" },
            { Language.TraditionalChinese, "點擊成就名稱即可手動單項完成；支援一鍵非同步安全完成全部成就。" },
            { Language.Japanese, "実績名をクリックして個別に解除可能。全実績の非同期一括解除も行えます。" },
            { Language.Korean, "도전 과제 이름을 클릭하여 개별 완료하거나, 비동기 일괄 완료를 사용할 수 있습니다." },
            { Language.Italian, "Fai clic sul nome di un obiettivo per completarlo, oppure usa il completamento asincrono di tutti." }
        }},
        { "achievements.unlock_all", new Dictionary<Language, string> {
            { Language.English, "Unlock All Achievements" },
            { Language.SimplifiedChinese, "一键完成所有成就" },
            { Language.TraditionalChinese, "一鍵完成所有成就" },
            { Language.Japanese, "全実績を一括解除" },
            { Language.Korean, "모든 도전 과제 일괄 완료" },
            { Language.Italian, "Sblocca Tutti gli Obiettivi" }
        }},
        { "achievements.unlocking_progress", new Dictionary<Language, string> {
            { Language.English, "Unlocking ({0}/{1})..." },
            { Language.SimplifiedChinese, "正在异步完成中 ({0}/{1})..." },
            { Language.TraditionalChinese, "正在非同步完成中 ({0}/{1})..." },
            { Language.Japanese, "非同期解除中 ({0}/{1})..." },
            { Language.Korean, "비동기 완료 처리 중 ({0}/{1})..." },
            { Language.Italian, "Sblocco asincrono ({0}/{1})..." }
        }},
        { "achievements.search", new Dictionary<Language, string> {
            { Language.English, "Search achievements..." },
            { Language.SimplifiedChinese, "搜索成就名称..." },
            { Language.TraditionalChinese, "搜尋成就名稱..." },
            { Language.Japanese, "実績名で検索..." },
            { Language.Korean, "도전 과제 검색..." },
            { Language.Italian, "Cerca obiettivi..." }
        }},
        { "achievements.status_unlocked", new Dictionary<Language, string> {
            { Language.English, "UNLOCKED" },
            { Language.SimplifiedChinese, "已解锁" },
            { Language.TraditionalChinese, "已解鎖" },
            { Language.Japanese, "解除済" },
            { Language.Korean, "완료됨" },
            { Language.Italian, "SBLOCCATO" }
        }},
        { "achievements.status_locked", new Dictionary<Language, string> {
            { Language.English, "LOCKED" },
            { Language.SimplifiedChinese, "未解锁" },
            { Language.TraditionalChinese, "未解鎖" },
            { Language.Japanese, "未解除" },
            { Language.Korean, "미완료" },
            { Language.Italian, "BLOCCATO" }
        }},
        { "achievements.btn_unlock", new Dictionary<Language, string> {
            { Language.English, "Complete" },
            { Language.SimplifiedChinese, "完成" },
            { Language.TraditionalChinese, "完成" },
            { Language.Japanese, "解除" },
            { Language.Korean, "완료" },
            { Language.Italian, "Completa" }
        }},
        { "achievements.unlock_success", new Dictionary<Language, string> {
            { Language.English, "Achievement completed: {0}" },
            { Language.SimplifiedChinese, "成就已成功完成: {0}" },
            { Language.TraditionalChinese, "成就已成功完成: {0}" },
            { Language.Japanese, "実績を解除しました: {0}" },
            { Language.Korean, "도전 과제 완료됨: {0}" },
            { Language.Italian, "Obiettivo sbloccato: {0}" }
        }},
        { "achievements.unlock_all_success", new Dictionary<Language, string> {
            { Language.English, "All {0} achievements completed successfully!" },
            { Language.SimplifiedChinese, "全部 {0} 个成就已成功异步完成！" },
            { Language.TraditionalChinese, "全部 {0} 個成就已成功非同步完成！" },
            { Language.Japanese, "全 {0} 個の実績が正常に非同期解除されました！" },
            { Language.Korean, "전체 {0}개 도전 과제가 비동기로 성공적으로 완료되었습니다!" },
            { Language.Italian, "Tutti i {0} obiettivi sono stati sbloccati con successo in modo asincrono!" }
        }},
        { "achievements.not_in_game", new Dictionary<Language, string> {
            { Language.English, "AchievementManager not ready. Please enter game first." },
            { Language.SimplifiedChinese, "成就系统尚未就绪，请先进入游戏场景。" },
            { Language.TraditionalChinese, "成就系統尚未就緒，請先進入遊戲場景。" },
            { Language.Japanese, "実績マネージャーが未初期化です。ゲームに入ってください。" },
            { Language.Korean, "도전 과제 관리자가 준비되지 않았습니다. 게임에 먼저 접속하세요." },
            { Language.Italian, "AchievementManager non pronto. Entra prima in gioco." }
        }},
        { "achievements.count_format", new Dictionary<Language, string> {
            { Language.English, "Progress: {0} / {1}" },
            { Language.SimplifiedChinese, "当前进度: {0} / {1}" },
            { Language.TraditionalChinese, "當前進度: {0} / {1}" },
            { Language.Japanese, "進捗状況: {0} / {1}" },
            { Language.Korean, "진행 현황: {0} / {1}" },
            { Language.Italian, "Progresso: {0} / {1}" }
        }},

        // 61 Localized Steam Achievements
        { "ach.TriedYourBestBadge", new Dictionary<Language, string> {
            { Language.English, "Tried Your Best" },
            { Language.SimplifiedChinese, "尽力而为" },
            { Language.TraditionalChinese, "盡力而為" },
            { Language.Japanese, "ベストを尽くした" },
            { Language.Korean, "최선을 다했다" },
            { Language.Italian, "Hai Fatto del Tuo Meglio" }
        }},
        { "ach.BeachcomberBadge", new Dictionary<Language, string> {
            { Language.English, "Beachcomber" },
            { Language.SimplifiedChinese, "海滩漫步" },
            { Language.TraditionalChinese, "海灘漫步" },
            { Language.Japanese, "ビーチコーマー" },
            { Language.Korean, "해변 방랑자" },
            { Language.Italian, "Passeggiatore da Spiaggia" }
        }},
        { "ach.TrailblazerBadge", new Dictionary<Language, string> {
            { Language.English, "Trailblazer" },
            { Language.SimplifiedChinese, "开路先锋" },
            { Language.TraditionalChinese, "開路先鋒" },
            { Language.Japanese, "先駆者" },
            { Language.Korean, "개척자" },
            { Language.Italian, "Pioniere" }
        }},
        { "ach.AlpinistBadge", new Dictionary<Language, string> {
            { Language.English, "Alpinist" },
            { Language.SimplifiedChinese, "登山家" },
            { Language.TraditionalChinese, "登山家" },
            { Language.Japanese, "アルピニスト" },
            { Language.Korean, "알피니스트" },
            { Language.Italian, "Alpinista" }
        }},
        { "ach.VolcanologyBadge", new Dictionary<Language, string> {
            { Language.English, "Volcanology" },
            { Language.SimplifiedChinese, "火山专家" },
            { Language.TraditionalChinese, "火山專家" },
            { Language.Japanese, "火山学者" },
            { Language.Korean, "화산학자" },
            { Language.Italian, "Vulcanologo" }
        }},
        { "ach.CookingBadge", new Dictionary<Language, string> {
            { Language.English, "Camp Chef" },
            { Language.SimplifiedChinese, "营地大厨" },
            { Language.TraditionalChinese, "營地大廚" },
            { Language.Japanese, "キャンプシェフ" },
            { Language.Korean, "캠프 요리사" },
            { Language.Italian, "Chef da Campo" }
        }},
        { "ach.HappyCamperBadge", new Dictionary<Language, string> {
            { Language.English, "Happy Camper" },
            { Language.SimplifiedChinese, "快乐露营" },
            { Language.TraditionalChinese, "快樂露營" },
            { Language.Japanese, "ハッピーキャンパー" },
            { Language.Korean, "행복한 캠퍼" },
            { Language.Italian, "Campeggiatore Felice" }
        }},
        { "ach.BoulderingBadge", new Dictionary<Language, string> {
            { Language.English, "Bouldering" },
            { Language.SimplifiedChinese, "抱石之星" },
            { Language.TraditionalChinese, "抱石之星" },
            { Language.Japanese, "ボルダリング" },
            { Language.Korean, "볼더링" },
            { Language.Italian, "Bouldering" }
        }},
        { "ach.ToxicologyBadge", new Dictionary<Language, string> {
            { Language.English, "Toxicology" },
            { Language.SimplifiedChinese, "毒理专家" },
            { Language.TraditionalChinese, "毒理專家" },
            { Language.Japanese, "毒物学" },
            { Language.Korean, "독물학" },
            { Language.Italian, "Tossicologia" }
        }},
        { "ach.ForagingBadge", new Dictionary<Language, string> {
            { Language.English, "Foraging" },
            { Language.SimplifiedChinese, "荒野采摘" },
            { Language.TraditionalChinese, "荒野採摘" },
            { Language.Japanese, "採取" },
            { Language.Korean, "채집" },
            { Language.Italian, "Raccolta" }
        }},
        { "ach.EsotericaBadge", new Dictionary<Language, string> {
            { Language.English, "Esoterica" },
            { Language.SimplifiedChinese, "远古秘法" },
            { Language.TraditionalChinese, "遠古秘法" },
            { Language.Japanese, "秘儀" },
            { Language.Korean, "비전 마法" },
            { Language.Italian, "Esoterismo" }
        }},
        { "ach.PeakBadge", new Dictionary<Language, string> {
            { Language.English, "The Peak" },
            { Language.SimplifiedChinese, "登顶勋章" },
            { Language.TraditionalChinese, "登頂勳章" },
            { Language.Japanese, "頂上到達" },
            { Language.Korean, "정상 정복" },
            { Language.Italian, "La Vetta" }
        }},
        { "ach.LoneWolfBadge", new Dictionary<Language, string> {
            { Language.English, "Lone Wolf" },
            { Language.SimplifiedChinese, "孤狼" },
            { Language.TraditionalChinese, "孤狼" },
            { Language.Japanese, "一匹狼" },
            { Language.Korean, "외로운 늑대" },
            { Language.Italian, "Lupo Solitario" }
        }},
        { "ach.ClutchBadge", new Dictionary<Language, string> {
            { Language.English, "Clutch Revive" },
            { Language.SimplifiedChinese, "绝境逢生" },
            { Language.TraditionalChinese, "絕境逢生" },
            { Language.Japanese, "逆転救助" },
            { Language.Korean, "클러치 구출" },
            { Language.Italian, "Salvataggio In Extremis" }
        }},
        { "ach.BalloonBadge", new Dictionary<Language, string> {
            { Language.English, "Light as Air" },
            { Language.SimplifiedChinese, "轻盈如燕" },
            { Language.TraditionalChinese, "輕盈如燕" },
            { Language.Japanese, "羽根のように軽く" },
            { Language.Korean, "가벼운 착지" },
            { Language.Italian, "Leggero come l'Aria" }
        }},
        { "ach.LeaveNoTraceBadge", new Dictionary<Language, string> {
            { Language.English, "Leave No Trace" },
            { Language.SimplifiedChinese, "无痕山林" },
            { Language.TraditionalChinese, "無痕山林" },
            { Language.Japanese, "足跡を残さず" },
            { Language.Korean, "흔적을 남기지 않기" },
            { Language.Italian, "Nessuna Traccia" }
        }},
        { "ach.SpeedClimberBadge", new Dictionary<Language, string> {
            { Language.English, "Speed Climber" },
            { Language.SimplifiedChinese, "极速攀登" },
            { Language.TraditionalChinese, "極速攀登" },
            { Language.Japanese, "スピードクライマー" },
            { Language.Korean, "스피드 클라이머" },
            { Language.Italian, "Scalatore Veloce" }
        }},
        { "ach.BingBongBadge", new Dictionary<Language, string> {
            { Language.English, "Bing Bong" },
            { Language.SimplifiedChinese, "叮咚玩偶" },
            { Language.TraditionalChinese, "叮咚玩偶" },
            { Language.Japanese, "ビンボン" },
            { Language.Korean, "빙봉" },
            { Language.Italian, "Bing Bong" }
        }},
        { "ach.NaturalistBadge", new Dictionary<Language, string> {
            { Language.English, "Naturalist" },
            { Language.SimplifiedChinese, "自然主义" },
            { Language.TraditionalChinese, "自然主義" },
            { Language.Japanese, "ナチュラリスト" },
            { Language.Korean, "자연주의자" },
            { Language.Italian, "Naturalista" }
        }},
        { "ach.GourmandBadge", new Dictionary<Language, string> {
            { Language.English, "Gourmand" },
            { Language.SimplifiedChinese, "顶级老饕" },
            { Language.TraditionalChinese, "頂級老饕" },
            { Language.Japanese, "美食家" },
            { Language.Korean, "미식가" },
            { Language.Italian, "Buongustaio" }
        }},
        { "ach.MycologyBadge", new Dictionary<Language, string> {
            { Language.English, "Mycology" },
            { Language.SimplifiedChinese, "真菌学家" },
            { Language.TraditionalChinese, "真菌學家" },
            { Language.Japanese, "菌類学者" },
            { Language.Korean, "균류학자" },
            { Language.Italian, "Micologia" }
        }},
        { "ach.FirstAidBadge", new Dictionary<Language, string> {
            { Language.English, "First Aid" },
            { Language.SimplifiedChinese, "急救大师" },
            { Language.TraditionalChinese, "急救大師" },
            { Language.Japanese, "応急手当" },
            { Language.Korean, "응급 처치" },
            { Language.Italian, "Primo Soccorso" }
        }},
        { "ach.SurvivalistBadge", new Dictionary<Language, string> {
            { Language.English, "Survivalist" },
            { Language.SimplifiedChinese, "求生大师" },
            { Language.TraditionalChinese, "求生大師" },
            { Language.Japanese, "サバイバリスト" },
            { Language.Korean, "생존 전문가" },
            { Language.Italian, "Sopravvissuto" }
        }},
        { "ach.AnimalSerenadingBadge", new Dictionary<Language, string> {
            { Language.English, "Animal Serenading" },
            { Language.SimplifiedChinese, "动物小夜曲" },
            { Language.TraditionalChinese, "動物小夜曲" },
            { Language.Japanese, "動物へのセレナーデ" },
            { Language.Korean, "동물 세레나데" },
            { Language.Italian, "Serenata agli Animali" }
        }},
        { "ach.ArboristBadge", new Dictionary<Language, string> {
            { Language.English, "Arborist" },
            { Language.SimplifiedChinese, "巨树巡礼" },
            { Language.TraditionalChinese, "巨樹巡禮" },
            { Language.Japanese, "巨木の巡礼" },
            { Language.Korean, "거목 탐방" },
            { Language.Italian, "Arboricoltore" }
        }},
        { "ach.MentorshipBadge", new Dictionary<Language, string> {
            { Language.English, "Mentorship" },
            { Language.SimplifiedChinese, "团长之教" },
            { Language.TraditionalChinese, "團長之教" },
            { Language.Japanese, "指導の教え" },
            { Language.Korean, "멘토의 가르침" },
            { Language.Italian, "Tutorato" }
        }},
        { "ach.KnotTyingBadge", new Dictionary<Language, string> {
            { Language.English, "Knot Tying" },
            { Language.SimplifiedChinese, "结绳高手" },
            { Language.TraditionalChinese, "結繩高手" },
            { Language.Japanese, "ロープ結びの達人" },
            { Language.Korean, "매듭 묶기" },
            { Language.Italian, "Nodi Perfetti" }
        }},
        { "ach.EmergencyPreparednessBadge", new Dictionary<Language, string> {
            { Language.English, "Emergency Preparedness" },
            { Language.SimplifiedChinese, "临危救难" },
            { Language.TraditionalChinese, "臨危救難" },
            { Language.Japanese, "緊急救護" },
            { Language.Korean, "긴급 구호" },
            { Language.Italian, "Pronto Intervento" }
        }},
        { "ach.AscenderBadge", new Dictionary<Language, string> {
            { Language.English, "Ascender" },
            { Language.SimplifiedChinese, "攀登先锋" },
            { Language.TraditionalChinese, "攀登先鋒" },
            { Language.Japanese, "アセンダー" },
            { Language.Korean, "상승자" },
            { Language.Italian, "Ascensore Umano" }
        }},
        { "ach.PlundererBadge", new Dictionary<Language, string> {
            { Language.English, "Plunderer" },
            { Language.SimplifiedChinese, "搜刮达人" },
            { Language.TraditionalChinese, "搜刮達人" },
            { Language.Japanese, "物色家" },
            { Language.Korean, "약탈자" },
            { Language.Italian, "Sciacallo" }
        }},
        { "ach.BookwormBadge", new Dictionary<Language, string> {
            { Language.English, "Bookworm" },
            { Language.SimplifiedChinese, "饱读诗书" },
            { Language.TraditionalChinese, "飽讀詩書" },
            { Language.Japanese, "本の虫" },
            { Language.Korean, "책벌레" },
            { Language.Italian, "Topo di Biblioteca" }
        }},
        { "ach.EnduranceBadge", new Dictionary<Language, string> {
            { Language.English, "Endurance" },
            { Language.SimplifiedChinese, "极限耐力" },
            { Language.TraditionalChinese, "極限耐力" },
            { Language.Japanese, "限界持久力" },
            { Language.Korean, "극한 인내" },
            { Language.Italian, "Resistenza Estrema" }
        }},
        { "ach.Ascent1", new Dictionary<Language, string> {
            { Language.English, "Ascent Level 1" },
            { Language.SimplifiedChinese, "攀登难度 1" },
            { Language.TraditionalChinese, "攀登難度 1" },
            { Language.Japanese, "アセント レベル 1" },
            { Language.Korean, "등반 레벨 1" },
            { Language.Italian, "Livello di Salita 1" }
        }},
        { "ach.Ascent2", new Dictionary<Language, string> {
            { Language.English, "Ascent Level 2" },
            { Language.SimplifiedChinese, "攀登难度 2" },
            { Language.TraditionalChinese, "攀登難度 2" },
            { Language.Japanese, "アセント レベル 2" },
            { Language.Korean, "등반 레벨 2" },
            { Language.Italian, "Livello di Salita 2" }
        }},
        { "ach.Ascent3", new Dictionary<Language, string> {
            { Language.English, "Ascent Level 3" },
            { Language.SimplifiedChinese, "攀登难度 3" },
            { Language.TraditionalChinese, "攀登難度 3" },
            { Language.Japanese, "アセント レベル 3" },
            { Language.Korean, "등반 레벨 3" },
            { Language.Italian, "Livello di Salita 3" }
        }},
        { "ach.Ascent4", new Dictionary<Language, string> {
            { Language.English, "Ascent Level 4" },
            { Language.SimplifiedChinese, "攀登难度 4" },
            { Language.TraditionalChinese, "攀登難度 4" },
            { Language.Japanese, "アセント レベル 4" },
            { Language.Korean, "등반 레벨 4" },
            { Language.Italian, "Livello di Salita 4" }
        }},
        { "ach.Ascent5", new Dictionary<Language, string> {
            { Language.English, "Ascent Level 5" },
            { Language.SimplifiedChinese, "攀登难度 5" },
            { Language.TraditionalChinese, "攀登難度 5" },
            { Language.Japanese, "アセント レベル 5" },
            { Language.Korean, "등반 레벨 5" },
            { Language.Italian, "Livello di Salita 5" }
        }},
        { "ach.Ascent6", new Dictionary<Language, string> {
            { Language.English, "Ascent Level 6" },
            { Language.SimplifiedChinese, "攀登难度 6" },
            { Language.TraditionalChinese, "攀登難度 6" },
            { Language.Japanese, "アセント レベル 6" },
            { Language.Korean, "등반 레벨 6" },
            { Language.Italian, "Livello di Salita 6" }
        }},
        { "ach.Ascent7", new Dictionary<Language, string> {
            { Language.English, "Ascent Level 7" },
            { Language.SimplifiedChinese, "攀登难度 7" },
            { Language.TraditionalChinese, "攀登難度 7" },
            { Language.Japanese, "アセント レベル 7" },
            { Language.Korean, "등반 레벨 7" },
            { Language.Italian, "Livello di Salita 7" }
        }},
        { "ach.ResourcefulnessBadge", new Dictionary<Language, string> {
            { Language.English, "Resourcefulness" },
            { Language.SimplifiedChinese, "荒野求生" },
            { Language.TraditionalChinese, "荒野求生" },
            { Language.Japanese, "臨機応変" },
            { Language.Korean, "임기응변" },
            { Language.Italian, "Ingegno" }
        }},
        { "ach.NomadBadge", new Dictionary<Language, string> {
            { Language.English, "Nomad" },
            { Language.SimplifiedChinese, "游牧探索" },
            { Language.TraditionalChinese, "遊牧探索" },
            { Language.Japanese, "遊牧民" },
            { Language.Korean, "유목민" },
            { Language.Italian, "Nomade" }
        }},
        { "ach.UltimateBadge", new Dictionary<Language, string> {
            { Language.English, "Ultimate Frisbee" },
            { Language.SimplifiedChinese, "极限飞盘" },
            { Language.TraditionalChinese, "極限飛盤" },
            { Language.Japanese, "究極フリスビー" },
            { Language.Korean, "얼티밋 프리스비" },
            { Language.Italian, "Frisbee Estremo" }
        }},
        { "ach.CoolCucumberBadge", new Dictionary<Language, string> {
            { Language.English, "Cool Cucumber" },
            { Language.SimplifiedChinese, "镇定自若" },
            { Language.TraditionalChinese, "鎮定自若" },
            { Language.Japanese, "冷静沈着" },
            { Language.Korean, "침착한 생존" },
            { Language.Italian, "Calma Glaciale" }
        }},
        { "ach.NeedlepointBadge", new Dictionary<Language, string> {
            { Language.English, "Needlepoint" },
            { Language.SimplifiedChinese, "针尖漫步" },
            { Language.TraditionalChinese, "針尖漫步" },
            { Language.Japanese, "サボテンまみれ" },
            { Language.Korean, "가시투성이" },
            { Language.Italian, "Punti di Spillo" }
        }},
        { "ach.AeronauticsBadge", new Dictionary<Language, string> {
            { Language.English, "Aeronautics" },
            { Language.SimplifiedChinese, "航空先驱" },
            { Language.TraditionalChinese, "航空先驅" },
            { Language.Japanese, "飛行先駆者" },
            { Language.Korean, "항공 개척자" },
            { Language.Italian, "Aeronautica" }
        }},
        { "ach.TwentyFourKaratBadge", new Dictionary<Language, string> {
            { Language.English, "24 Karat Sacrifice" },
            { Language.SimplifiedChinese, "纯金祭礼" },
            { Language.TraditionalChinese, "純金祭禮" },
            { Language.Japanese, "純金の生贄" },
            { Language.Korean, "24캐럿 제물" },
            { Language.Italian, "Sacrificio a 24 Carati" }
        }},
        { "ach.DaredevilBadge", new Dictionary<Language, string> {
            { Language.English, "Daredevil Cannon" },
            { Language.SimplifiedChinese, "亡命特技" },
            { Language.TraditionalChinese, "亡命特技" },
            { Language.Japanese, "命知らずの砲撃" },
            { Language.Korean, "무모한 도전" },
            { Language.Italian, "Scavezzacollo" }
        }},
        { "ach.MegaentomologyBadge", new Dictionary<Language, string> {
            { Language.English, "Megaentomology" },
            { Language.SimplifiedChinese, "巨虫脱险" },
            { Language.TraditionalChinese, "巨蟲脫險" },
            { Language.Japanese, "巨大昆虫脱出" },
            { Language.Korean, "거대 곤충 탈출" },
            { Language.Italian, "Mega-entomologia" }
        }},
        { "ach.AstronomyBadge", new Dictionary<Language, string> {
            { Language.English, "Astronomy Gazer" },
            { Language.SimplifiedChinese, "直视烈阳" },
            { Language.TraditionalChinese, "直視烈陽" },
            { Language.Japanese, "太陽直視" },
            { Language.Korean, "태양 관측" },
            { Language.Italian, "Osservatore Solare" }
        }},
        { "ach.BundledUpBadge", new Dictionary<Language, string> {
            { Language.English, "Bundled Up" },
            { Language.SimplifiedChinese, "严阵以待" },
            { Language.TraditionalChinese, "嚴陣以待" },
            { Language.Japanese, "防寒万全" },
            { Language.Korean, "철통 방한" },
            { Language.Italian, "Ben Coperto" }
        }},
        { "ach.ForestryBadge", new Dictionary<Language, string> {
            { Language.English, "Forestry" },
            { Language.SimplifiedChinese, "林业勋章" },
            { Language.TraditionalChinese, "林業勳章" },
            { Language.Japanese, "林業の証" },
            { Language.Korean, "산림 훈장" },
            { Language.Italian, "Silvicoltura" }
        }},
        { "ach.TreadLightlyBadge", new Dictionary<Language, string> {
            { Language.English, "Tread Lightly" },
            { Language.SimplifiedChinese, "轻足疾行" },
            { Language.TraditionalChinese, "輕足疾行" },
            { Language.Japanese, "忍び足" },
            { Language.Korean, "사뿐한 걸음" },
            { Language.Italian, "Passo Felpato" }
        }},
        { "ach.WebSecurityBadge", new Dictionary<Language, string> {
            { Language.English, "Web Security" },
            { Language.SimplifiedChinese, "蛛网脱险" },
            { Language.TraditionalChinese, "蛛網脫險" },
            { Language.Japanese, "クモの巣脱出" },
            { Language.Korean, "거미줄 탈출" },
            { Language.Italian, "Fuga dalla Tela" }
        }},
        { "ach.UndeadEncounterBadge", new Dictionary<Language, string> {
            { Language.English, "Undead Encounter" },
            { Language.SimplifiedChinese, "尸变逢生" },
            { Language.TraditionalChinese, "屍變逢生" },
            { Language.Japanese, "死霊との遭遇" },
            { Language.Korean, "언데드 조우" },
            { Language.Italian, "Incontro Non Morto" }
        }},
        { "ach.AdvancedMycologyBadge", new Dictionary<Language, string> {
            { Language.English, "Advanced Mycology" },
            { Language.SimplifiedChinese, "高阶真菌" },
            { Language.TraditionalChinese, "高階真菌" },
            { Language.Japanese, "高度な菌類学" },
            { Language.Korean, "고급 균류학" },
            { Language.Italian, "Micologia Avanzata" }
        }},
        { "ach.DisasterResponseBadge", new Dictionary<Language, string> {
            { Language.English, "Disaster Response" },
            { Language.SimplifiedChinese, "灾难搜救" },
            { Language.TraditionalChinese, "災難搜救" },
            { Language.Japanese, "災害救助" },
            { Language.Korean, "재난 구조" },
            { Language.Italian, "Soccorso in Calamità" }
        }},
        { "ach.CalciumIntakeBadge", new Dictionary<Language, string> {
            { Language.English, "Calcium Intake" },
            { Language.SimplifiedChinese, "补钙达人" },
            { Language.TraditionalChinese, "補鈣達人" },
            { Language.Japanese, "カルシウム補給" },
            { Language.Korean, "칼슘 섭취" },
            { Language.Italian, "Assunzione di Calcio" }
        }},
        { "ach.CompetitiveEatingBadge", new Dictionary<Language, string> {
            { Language.English, "Competitive Eating" },
            { Language.SimplifiedChinese, "大胃王" },
            { Language.TraditionalChinese, "大胃王" },
            { Language.Japanese, "大食い王" },
            { Language.Korean, "푸드 파이터" },
            { Language.Italian, "Gara di Mangiatrici" }
        }},
        { "ach.AppliedEsotericaBadge", new Dictionary<Language, string> {
            { Language.English, "Applied Esoterica" },
            { Language.SimplifiedChinese, "实用通灵" },
            { Language.TraditionalChinese, "實用通靈" },
            { Language.Japanese, "実用秘術" },
            { Language.Korean, "응용 심령술" },
            { Language.Italian, "Esoterismo Pratico" }
        }},
        { "ach.MycoacrobaticsBadge", new Dictionary<Language, string> {
            { Language.English, "Mycoacrobatics" },
            { Language.SimplifiedChinese, "蘑菇杂技" },
            { Language.TraditionalChinese, "蘑菇雜技" },
            { Language.Japanese, "キノコアクロバット" },
            { Language.Korean, "버섯 곡예" },
            { Language.Italian, "Mico-acrobazia" }
        }},
        { "ach.CryptogastronomyBadge", new Dictionary<Language, string> {
            { Language.English, "Cryptogastronomy" },
            { Language.SimplifiedChinese, "异食美食家" },
            { Language.TraditionalChinese, "異食美食家" },
            { Language.Japanese, "謎の美食家" },
            { Language.Korean, "기이한 미식" },
            { Language.Italian, "Cripto-gastronomia" }
        }}
    };

    public static readonly string[] LanguageNames = new string[] { "English", "简体中文", "日本語", "한국어", "Italiano", "繁體中文" };

    public static string T(string key)
    {
        Dictionary<Language, string> translations;
        if (Strings.TryGetValue(key, out translations))
        {
            string text;
            if (translations.TryGetValue(CurrentLanguage, out text))
                return text;
            string fallback;
            if (translations.TryGetValue(Language.English, out fallback))
                return fallback;
        }
        return key;
    }

    public static string T(string key, params object[] args)
    {
        try
        {
            return string.Format(T(key), args);
        }
        catch
        {
            return T(key);
        }
    }

    public static void SetLanguage(Language lang)
    {
        CurrentLanguage = lang;
    }

    public static void SetLanguage(int index)
    {
        if (index >= 0 && index < LanguageNames.Length)
            CurrentLanguage = (Language)index;
    }

    public static string GetStatusTypeName(CharacterAfflictions.STATUSTYPE type)
    {
        switch (type)
        {
            case CharacterAfflictions.STATUSTYPE.Injury: return T("status.injury");
            case CharacterAfflictions.STATUSTYPE.Hunger: return T("status.hunger");
            case CharacterAfflictions.STATUSTYPE.Cold: return T("status.cold");
            case CharacterAfflictions.STATUSTYPE.Poison: return T("status.poison");
            case CharacterAfflictions.STATUSTYPE.Crab: return T("status.crab");
            case CharacterAfflictions.STATUSTYPE.Curse: return T("status.curse");
            case CharacterAfflictions.STATUSTYPE.Drowsy: return T("status.drowsy");
            case CharacterAfflictions.STATUSTYPE.Weight: return T("status.weight");
            case CharacterAfflictions.STATUSTYPE.Hot: return T("status.hot");
            case CharacterAfflictions.STATUSTYPE.Thorns: return T("status.thorns");
            case CharacterAfflictions.STATUSTYPE.Spores: return T("status.spores");
            case CharacterAfflictions.STATUSTYPE.Web: return T("status.web");
            default: return type.ToString();
        }
    }

    public static string GetAchievementName(ACHIEVEMENTTYPE type)
    {
        string key = "ach." + type.ToString();
        string name = T(key);
        if (name != key) return name;
        return type.ToString();
    }
}
