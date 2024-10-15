/// --------変数一覧-------- ///

#region public変数
/// -------public変数------- ///

// アニメーションの状態
public enum AnimState
{
    E_Idle = 0, // 待機中
    E_Move = 1, // 移動中
    E_Attack = 2, // ブレイブ攻撃中
    E_HitReaction = 3, // 被ダメージ中
    E_Dead = 4, // 戦闘不能中
    E_Guard = 5, // 防御中
    E_Dodge = 6, // 回避中
}

// 攻撃の種類
public enum AttackState
{
    E_None = 0, // 攻撃していない
    E_BraveAttack = 1, // ブレイブ攻撃時
    E_HPAttack = 2 // HP攻撃時
}

// ブレイブの状態
public enum BraveState
{
    E_Default = 0, // 通常状態
    E_Regenerate = 1, // リジェネ状態
    E_Break = 2 // ブレイク状態
}

// 被ダメージの状態
public enum DamageState
{
    E_Default = 0, // 通常状態
    E_SuperArmor = 1, // スーパーアーマー状態
    E_Invincible = 2, // 無敵状態
    E_Guard = 3, // 防御状態
    E_Dodge = 4 // 回避状態
}

// エフェクトの名称
public enum EffectName
{
    braveDamageEffect = 0,
    hpDamageEffect = 1,
    hpShockWaveEffect = 2,
    hpFireBreath = 3,
    hpBomb1 = 4,
    hpBomb2 = 5,
    hpMeteor = 6,
    summon = 7,
    death = 8
}

// パーティクルシステムのトリガー対象
public enum ParticleSystemTrigger
{
    E_PlayerWepon = 0,
    E_EnemyWepon = 1
}

// ゲームの状態
public enum GameState
{
    E_Title = 0,
    E_InGame = 1
}

// BGMのリスト
public enum BGMList
{
    E_TitleBGM = 0,
    E_OpeningBGM = 1,
    E_InGameBGM = 2,
    E_VictoryBGM = 3,
    E_DefeatBGM = 4
}

// SEのリスト
public enum SEList
{
    E_FootstepsSE = 0,
    E_SlashSE = 1,
    E_GetHitSE = 2,
    E_DragonBiteSE = 3,
    E_DragonstepsSE = 4,
    E_DragonFlameSE = 5,
    E_DragonScreamSE = 6,
    E_DragonWingSE = 7,
    E_DragonGetHitVoiceSE = 8,
    E_BeginChanceTime = 9,
    E_EndChanceTime = 10,
    E_SmallDragonstepsSE = 11,
    E_SmallDragonScreamSE = 12,
    E_BornSE = 13
}

// プールするオブジェクトタイプ
public enum PoolType
{
    E_Effect = 0,
    E_DamageText = 1
}

/// -------public変数------- ///
#endregion

#region protected変数
/// -----protected変数------ ///



/// -----protected変数------ ///
#endregion

#region private変数
/// ------private変数------- ///



/// ------private変数------- ///
#endregion

#region プロパティ
/// -------プロパティ------- ///



/// -------プロパティ------- ///
#endregion

/// --------変数一覧-------- ///