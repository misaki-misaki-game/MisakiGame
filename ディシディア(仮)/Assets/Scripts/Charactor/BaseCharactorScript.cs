using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
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
        public virtual void ReceiveBraveDamage(float damage)
        {
        }

        /// <summary>
        /// HP値へダメージを与える関数
        /// </summary>
        public virtual void ReceiveHPDamage(float brave)
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
        public virtual IEnumerator BraveHitReaction()
        {
            // アニメーション状態を被ダメージ中にする
            animState = AnimState.E_HitReaction;

            // ヒットした位置を特定するまで待ってからエフェクトを生成する
            yield return new WaitUntil(() => ishit);
            InstantiateEffect(braveDamageEffect, effectPos);
            ishit = false;
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
        public virtual IEnumerator HPHitReaction()
        {
            // アニメーション状態を攻撃中にする
            animState = AnimState.E_HitReaction;

            // ヒットした位置を特定するまで待ってからエフェクトを生成する
            yield return new WaitUntil(() => ishit);
            InstantiateEffect(hpDamageEffect, effectPos);
            ishit = false;
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

        /// <summary>
        /// エフェクトを生成する関数
        /// </summary>
        /// <param name="effect">生成するエフェクト</param>
        /// <param name="pos">エフェクト生成位置</param>
        protected void InstantiateEffect(GameObject effect, Vector3 pos)
        {
            GameObject newEffect = Instantiate(effect, pos, Quaternion.identity, this.transform);
            Task.Run(() => Destroy(newEffect, 5));
        }

        protected void OnTriggerEnter(Collider col)
        {
            // コライダーがぶつかった場所を格納する
            if (col.CompareTag(Tags.EnemyWepon.ToString()))
            {
                Vector3 hitPos = col.ClosestPointOnBounds(transform.position);
                effectPos = new Vector3(hitPos.x, adjustEffectYPos, hitPos.z);
                ishit= true;
            }
        }

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
            public float maxHp = 0; // 最大HP
            public float brave = 0; // ブレイブ値
            public float standardBrave = 0; // 基準ブレイブ値
            public float regenerateSpeed = 0; // ブレイブのリジェネスピード
            public float breakSpeed = 0; // ブレイク時のリジェネスピード
            public float speed = 0; // 移動スピード
            public float attack = 0; // 攻撃力

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Parameter() { }

            /// <summary>
            /// 引数付きコンストラクタ
            /// </summary>
            /// <param name="initialHP">HP</param>
            /// <param name="initialBrave">ブレイブ値</param>
            /// <param name="initialRegenerateSpeed">ブレイブリジェネスピード</param>
            /// <param name="initialSpeed">移動スピード</param>
            /// <param name="initialAttack">攻撃力</param>
            /// <param name="initialBreakSpeed">ブレイク時のリジェネスピード</param>
            public Parameter(float initialHP, float initialBrave, float initialRegenerateSpeed, float initialBreakSpeed, float initialSpeed, float initialAttack)
            {
                // 各パラメータに代入する
                maxHp = initialHP;
                hp = maxHp;
                standardBrave = initialBrave;
                brave = standardBrave;
                regenerateSpeed = initialRegenerateSpeed;
                breakSpeed = initialBreakSpeed;
                speed = initialSpeed;
                attack= initialAttack;
            }
        }

        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///
        
        protected AnimState animState; // アニメーションの状態変数

        protected Animator anim; // Animator変数

        protected Parameter parameter; // パラメーター変数

        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///

        private bool ishit = false; // ヒットしたかどうか

        [SerializeField] private float adjustEffectYPos = 0.5f; // エフェクトのY軸補正値

        private Vector3 effectPos; // エフェクト表示位置

        [SerializeField] private GameObject braveDamageEffect; // 被ブレイブ攻撃のエフェクト
        [SerializeField] private GameObject hpDamageEffect; // 被HP攻撃のエフェクト

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}