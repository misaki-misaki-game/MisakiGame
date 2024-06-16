using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Misaki
{
    public abstract partial class BaseCharactorScript : DebugSetUp, IBattle
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        /// <summary>
        /// ブレイブ値へダメージを与える関数
        /// </summary>
        public virtual void AddBraveDamage(float damage)
        {
        }

        /// <summary>
        /// HP値へダメージを与える関数
        /// </summary>
        public virtual void AddHPDamage(float brave)
        {
        }

        /// <summary>
        /// 生まれる関数
        /// </summary>
        public virtual void Born()
        {
        }

        /// <summary>
        /// ブレイブ攻撃する関数
        /// </summary>
        public virtual void BraveAttack()
        {
        }

        /// <summary>
        /// ブレイブ攻撃を受けた際のリアクション関数
        /// </summary>
        public virtual void BraveHitReaction()
        {
        }

        /// <summary>
        /// 死ぬ関数
        /// </summary>
        public virtual void Dead()
        {
        }

        /// <summary>
        /// 回避関数
        /// </summary>
        public virtual void Dodge()
        {
        }

        /// <summary>
        /// 防御関数
        /// </summary>
        public virtual void Guard()
        {
        }

        /// <summary>
        /// HP攻撃をする関数
        /// </summary>
        public virtual void HPAttack()
        {
        }

        /// <summary>
        /// HP攻撃を受けた際のリアクション関数
        /// </summary>
        public virtual void HPHitReaction()
        {
        }

        /// <summary>
        /// 移動関数
        /// </summary>
        public virtual void Move()
        {
        }

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
    public abstract partial class BaseCharactorScript
    {
        /// --------変数一覧-------- ///

        #region public変数
        /// -------public変数------- ///

        /// <summary>
        /// パラメータクラス
        /// </summary>
        public class Parameter
        {
            public float hp = 0; // HP
            public float brave = 0; // ブレイブ値
            public float speed = 0; // 移動スピード
            public float attack = 0; // 攻撃力

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Parameter() { }
            /// <summary>
            /// 引数付きコンストラクタ
            /// </summary>
            /// <param name="initialHP">初期HP</param>
            /// <param name="initialBrave">初期ブレイブ値</param>
            /// <param name="initialSpeed">初期移動スピード</param>
            public Parameter(float initialHP, float initialBrave, float initialSpeed, float initialAttack)
            {
                // 各パラメータに代入する
                hp = initialHP;
                brave = initialBrave;
                speed = initialSpeed;
                attack= initialAttack;
            }
        }


        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///

        protected Animator anim; // Animator変数

        protected Parameter parameter; // パラメーター変数

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
    }
}