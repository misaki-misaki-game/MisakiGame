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
        void ReceiveBraveDamage(float damage);

        /// <summary>
        /// HP値へのダメージを受け取る関数
        /// </summary>
        void ReceiveHPDamage(float damage);

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
        void BraveHitReaction();

        /// <summary>
        /// HP攻撃を受けた際のリアクション関数
        /// </summary>
        void HPHitReaction();

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