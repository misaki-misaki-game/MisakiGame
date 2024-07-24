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
        public void ReceiveBraveDamage(float damage)
        {
            // Braveからdamage分を引く
            parameter.brave = parameter.brave - damage;
            StartCoroutine(BraveHitReaction());
        }

        /// <summary>
        /// HP値へダメージを与える関数
        /// </summary>
        public void ReceiveHPDamage(float brave)
        {
            // HPからdamageを引く
            parameter.hp = parameter.hp - brave;
            StartCoroutine(HPHitReaction());
        }

        /// <summary>
        /// 生まれる関数
        /// </summary>
        public void Born()
        {
            //anim
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
            // 対応アニメーションを再生
            anim.SetTrigger("At_Death");
        }

        /// <summary>
        /// 回避関数
        /// </summary>
        public virtual void Dodge()
        {
            // 回避中は無敵

            // 対応アニメーションを再生
            anim.SetTrigger("At_Dodge");
        }

        /// <summary>
        /// 防御関数
        /// </summary>
        public virtual void Guard()
        {
            // anim
            // 防御中はダメージ軽減か無敵
        }

        /// <summary>
        /// HP攻撃をする関数
        /// </summary>
        public virtual void HPAttack()
        {
            // HP攻撃中ならリターン
            if (animState == AnimState.E_Attack) return;

            // アニメーション状態をHP攻撃中にする
            animState = AnimState.E_Attack;

            // startIdleをfalseにして攻撃アクションが終了後Move()関数を動かすようにする
            if (startIdle) startIdle = false;

            // 攻撃の所有者を自分にする
            attackScript.SetOwnOwner = this;
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

        /// <summary>
        /// ブレイブ攻撃開始時の関数
        /// </summary>
        /// <param name="motionValue">攻撃モーション値</param>
        public void BeginBraveAttack(float motionValue)
        {
            // 武器のステートとブレイブ攻撃値を変更し、ヒットオブジェクトリストをリセットする
            attackScript.SetAttackState = AttackState.E_BraveAttack;
            attackScript.ClearHitObj();
            attackScript.SetBraveAttack = motionValue * parameter.attack;
        }

        /// <summary>
        /// HP攻撃開始時の関数
        /// </summary>
        public void BiginHPAttack()
        {
            // 武器のステートとHP攻撃値を変更し、ヒットオブジェクトリストをリセットする
            attackScript.SetAttackState = AttackState.E_HPAttack;
            attackScript.SetHPAttack = parameter.brave;
            attackScript.ClearHitObj();
        }

        /// <summary>
        /// 攻撃終了時の関数
        /// </summary>
        public void EndAttack()
        {
            // 武器のステートを変更し、ヒットオブジェクトリストをリセットする
            attackScript.SetAttackState = AttackState.E_None;
            attackScript.ClearHitObj();
        }

        /// <summary>
        /// アニメーション終了時の関数
        /// </summary>
        public virtual void EndAnim()
        {
            animState = default;
        }

        /// <summary>
        /// 自分のブレイブ攻撃が当たった時の関数
        /// </summary>
        /// <param name="obtainBrave">取得したブレイブ</param>
        public virtual void HitBraveAttack(float obtainBrave)
        {
            // ブレイブを加算する
            parameter.brave += obtainBrave;
        }

        /// <summary>
        /// 自分のHP攻撃が当たった時の関数
        /// </summary>
        public virtual void HitHPAttack()
        {
            // ブレイブを0にする
            parameter.brave = 0;

            // ブレイブ状態をリジェネ状態にする
            braveState = BraveState.E_Regenerate;
        }

        /// <summary>
        /// ブレイブを徐々に回復する関数
        /// </summary>
        public virtual void RegenerateBrave()
        {
            // 通常状態の場合はリターン
            // リジェネ状態の場合はregenerateSpeed秒掛かけ回復
            // ブレイク状態の場合はbrakSpeed秒掛けて回復
            if (braveState == BraveState.E_Default) return;
            else if (braveState == BraveState.E_Regenerate) parameter.brave += parameter.standardBrave / parameter.regenerateSpeed * Time.deltaTime;
            else parameter.brave += parameter.standardBrave / parameter.breakSpeed * Time.deltaTime;

            // ブレイブ値がブレイブ基準値以上まで回復したら回復を止める
            if (parameter.brave >= parameter.standardBrave)
            {
                parameter.brave = parameter.standardBrave;
                braveState = BraveState.E_Default;
            }
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

        protected bool startIdle = false; // 待機アニメーションがスタートしているか

        protected AnimState animState; // アニメーションの状態変数

        protected BraveState braveState; // ブレイブの状態変数

        protected Animator anim; // Animator変数

        protected Parameter parameter; // パラメーター変数

        [SerializeField] protected AttackScript attackScript; // 自身の武器の攻撃スクリプト

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