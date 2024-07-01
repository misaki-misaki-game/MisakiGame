/// --------変数一覧-------- ///

#region public変数
/// -------public変数------- ///

// アニメーションの状態
public enum AnimState
{
    E_Idle = 0, // 待機中
    E_Move = 1, // 移動中
    E_Attack = 2, // 攻撃中
    E_HitReaction = 3, // 被ダメージ中
    E_Dead = 4, // 戦闘不能中
    E_None = default // なにもしていない
}

// 攻撃の種類
public enum AttackState
{
    E_None=0,
    E_BraveAttack = 1,
    E_HPAttack = 2
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