using System.Collections;
using UnityEngine;
namespace Misaki
{
    public interface IBattle
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        /// <summary>
        /// ブレイブ値へのダメージを受け取る関数
        /// </summary>
        /// <param name="damage">ブレイブダメージ値</param>
        /// <param name="direction">攻撃された方向</param>
        /// <returns>防御が成功しているかどうか</returns>
        bool ReceiveBraveDamage(float damage, Vector3 direction);

        /// <summary>
        /// HP値へのダメージを受け取る関数
        /// </summary>
        /// <param name="brave">HPダメージ値</param>
        /// <param name="direction">攻撃された方向</param>
        void ReceiveHPDamage(float brave, Vector3 direction);

        /// <summary>
        /// 生まれる関数
        /// </summary>
        void Born();

        /// <summary>
        /// 死ぬ関数
        /// </summary>
        void Dead();

        /// <summary>
        /// ブレイブ攻撃する関数
        /// </summary>
        void BraveAttack();

        /// <summary>
        /// HP攻撃をする関数
        /// </summary>
        void HPAttack();

        /// <summary>
        /// 回避関数
        /// </summary>
        void Dodge();

        /// <summary>
        /// 移動関数
        /// </summary>
        void Move();

        /// <summary>
        /// 防御関数
        /// </summary>
        void Guard();

        /// <summary>
        /// ブレイブ攻撃を受けた際のリアクション関数
        /// </summary>
        IEnumerator BraveHitReaction();

        /// <summary>
        /// HP攻撃を受けた際のリアクション関数
        /// </summary>
        IEnumerator HPHitReaction();

        /// <summary>
        /// ブレイブ攻撃開始時の関数
        /// </summary>
        /// <param name="motionValue">攻撃モーション値</param>
        void BeginBraveAttack(float motionValue);

        /// <summary>
        /// HP攻撃開始時の関数
        /// </summary>
        void BiginHPAttack();

        /// <summary>
        /// 攻撃終了時の関数
        /// </summary>
        void EndAttack();

        /// <summary>
        /// アニメーション終了時の関数
        /// </summary>
        void EndAnim();

        /// <summary>
        /// 自分のHP攻撃が当たった時の関数
        /// </summary>
        void HitHPAttack();

        /// <summary>
        /// ブレイブを徐々に回復する関数
        /// </summary>
        void RegenerateBrave();

        /// <summary>
        /// ノックバック開始関数
        /// </summary>
        void BiginKnockBack();

        /// <summary>
        /// ノックバック終了関数
        /// </summary>
        void EndKnockBack();

        /// <summary>
        /// ダメージエフェクトを発生させる許可を出す関数
        /// </summary>
        void CanDamageEffect();

        /// <summary>
        /// 無敵時間開始関数
        /// </summary>
        void BiginInvincible();

        /// <summary>
        /// スーパーアーマー開始関数
        /// </summary>
        void BiginSuperArmor();

        /// <summary>
        /// 被ダメージの状態をリセットする関数
        /// </summary>
        void ResetDamageState();

        /// <summary>
        /// 防御を受けた際のリアクション関数
        /// </summary>
        void GuardReaction();

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///



        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///



        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
}