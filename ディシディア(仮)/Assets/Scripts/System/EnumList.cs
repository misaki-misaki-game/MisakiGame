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
    E_None = default // なにもしていない
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
    E_Guard = 3 // 防御状態
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