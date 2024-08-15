using System.Collections;
using UnityEngine;
namespace Misaki
{
    public abstract partial class BaseCharactorScript : DebugSetUp, IBattle
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
        public bool ReceiveBraveDamage(float damage, Vector3 direction)
        {
            // 無敵時間中ならfalseをリターン
            // 防御中ならtrueをリターン
            if (damageState == DamageState.E_Invincible) return false;
            else if (damageState == DamageState.E_Guard) return true;

            // キャラクターの向きを指定の向きに変え、エフェクトを生成するポジションを設定
            transform.LookAt(transform.position + direction);
            effectPos = new Vector3(transform.position.x, adjustEffectYPos, transform.position.z);

            // Braveからdamage分を引く
            parameter.brave = parameter.brave - damage;
            StartCoroutine(BraveHitReaction());

            // falseをリターン
            return false;
        }

        /// <summary>
        /// HP値へのダメージを受け取る関数
        /// </summary>
        /// <param name="brave">HPダメージ値</param>
        public void ReceiveHPDamage(float brave, Vector3 direction)
        {
            // 無敵時間中ならリターン
            if (damageState == DamageState.E_Invincible) return;

            // キャラクターの向きを指定の向きに変え、エフェクトを生成するポジションを設定
            transform.LookAt(transform.position + direction);
            effectPos = new Vector3(transform.position.x, adjustEffectYPos, transform.position.z);

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

            // ノックバック距離を代入
            knockBackDistance = 1f;

            // ヒットした位置を特定するまで待ってからエフェクトを生成する
            yield return new WaitUntil(() => ishit);

            PoolManager effect = EffectManager.braveDamageEffectPool;
            effect.GetGameObject(EffectManager.braveDamageEffect, effectPos, transform);
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
            animState = AnimState.E_Dodge;
        }

        /// <summary>
        /// 防御関数
        /// </summary>
        public virtual void Guard()
        {
            animState = AnimState.E_Guard;
            damageState = DamageState.E_Guard;
        }

        /// <summary>
        /// HP攻撃をする関数
        /// </summary>
        public virtual void HPAttack()
        {
            // アニメーション状態をHP攻撃中にする
            animState = AnimState.E_Attack;

            // 攻撃の所有者を自分にする
            attackScript.SetOwnOwner = this;
        }

        /// <summary>
        /// HP攻撃を受けた際のリアクション関数
        /// </summary>
        public virtual IEnumerator HPHitReaction()
        {
            // アニメーション状態を被ダメージ中にする
            animState = AnimState.E_HitReaction;

            // ノックバック距離を代入
            knockBackDistance = 3f;

            // ヒットした位置を特定するまで待ってからエフェクトを生成する
            yield return new WaitUntil(() => ishit);

            PoolManager effect = EffectManager.hpDamageEffectPool;
            effect.GetGameObject(EffectManager.hpDamageEffect, effectPos, transform);
            ishit = false;
        }

        /// <summary>
        /// 移動関数
        /// </summary>
        public virtual void Move()
        {
            // アニメーション状態を移動中にする
            animState = AnimState.E_Move;
        }

        /// <summary>
        /// ブレイブ攻撃開始時の関数
        /// 自身の武器で攻撃する場合
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
        /// ブレイブ攻撃開始時の関数
        /// 遠距離攻撃する場合
        /// </summary>
        /// <param name="motionValue">攻撃モーション値</param>
        /// <param name="bullet">遠距離攻撃のアタックスクリプト</param>
        public void BeginBraveBullet(float motionValue, AttackScript bullet)
        {
            bulletAttackScript = bullet; // アタックスクリプトを取得

            // 弾のステートとブレイブ攻撃値を変更し、ヒットオブジェクトリストをリセットする
            bulletAttackScript.SetAttackState = AttackState.E_BraveAttack;
            bulletAttackScript.ClearHitObj();
            bulletAttackScript.SetBraveAttack = motionValue * parameter.attack;
        }

        /// <summary>
        /// HP攻撃開始時の関数
        /// 自身の武器で攻撃する場合
        /// </summary>
        public void BiginHPAttack()
        {
            // 武器のステートとHP攻撃値を変更し、ヒットオブジェクトリストをリセットする
            attackScript.SetAttackState = AttackState.E_HPAttack;
            attackScript.SetHPAttack = parameter.brave;
            attackScript.ClearHitObj();
        }

        /// <summary>
        /// HP攻撃開始時の関数
        /// 自身の武器で攻撃する場合
        /// </summary>
        /// <param name="bullet">遠距離攻撃のアタックスクリプト</param>

        public void BiginHPBullet(/*AttackScript bullet*/)
        {
            // 衝撃波を生成し、そのアタックスクリプトを取得
            effectPos = new Vector3(transform.position.x, adjustEffectYPos, transform.position.z);

            PoolManager effect = EffectManager.hpShockWaveEffectPool;
            GameObject obj = effect.GetGameObject(EffectManager.hpShockWaveEffect, effectPos, transform);
            bulletAttackScript = obj.GetComponentInChildren<AttackScript>();

            // 武器のステートとHP攻撃値を変更し、ヒットオブジェクトリストをリセットする
            // アタックスクリプトの所有者を自分にする
            bulletAttackScript.SetOwnOwner = this;
            bulletAttackScript.SetAttackState = AttackState.E_HPAttack;
            bulletAttackScript.SetHPAttack = parameter.brave;
            bulletAttackScript.ClearHitObj();
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

        /// <summary>
        /// ノックバック開始関数
        /// </summary>
        public virtual void BiginKnockBack()
        {
        }

        /// <summary>
        /// ノックバック終了関数
        /// </summary>
        public virtual void EndKnockBack()
        {
        }

        /// <summary>
        /// ダメージエフェクトを発生させる許可を出す関数
        /// </summary>
        public void CanDamageEffect()
        {
            ishit = true;
        }

        /// <summary>
        /// 無敵時間開始関数
        /// </summary>
        public void BiginInvincible()
        {
            damageState = DamageState.E_Invincible;
        }

        /// <summary>
        /// スーパーアーマー開始関数
        /// </summary>
        public void BiginSuperArmor()
        {
            damageState = DamageState.E_SuperArmor;
        }

        /// <summary>
        /// 被ダメージの状態をリセットする関数
        /// </summary>
        public void ResetDamageState()
        {
            damageState = default;
        }

        /// <summary>
        /// 防御を受けた際のリアクション関数
        /// </summary>
        public virtual void GuardReaction()
        {
            // アニメーション状態を被ダメージ中にする
            animState = AnimState.E_HitReaction;
        }

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///

        protected override void Awake()
        {
            base.Awake();
            effectPos = new Vector3(0, adjustEffectYPos, 0);
        }

        protected void OnTriggerEnter(Collider col)
        {
            // 攻撃を受けたことを確認する
            if (col.CompareTag(Tags.EnemyWepon.ToString())) CanDamageEffect();
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

        protected float knockBackDistance; // ノックバック距離

        protected AnimState animState; // アニメーションの状態変数

        protected BraveState braveState; // ブレイブの状態変数

        protected DamageState damageState; // 被ダメージの状態

        protected Animator anim; // Animator変数

        protected Parameter parameter; // パラメーター変数

        protected AttackScript bulletAttackScript; // 遠距離攻撃スクリプト
        [SerializeField] protected AttackScript attackScript; // 自身の武器の攻撃スクリプト

        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///

        private bool ishit = false; // ヒットしたかどうか

        [SerializeField] private float adjustEffectYPos = 0.5f; // エフェクトのY軸補正値

        private Vector3 effectPos; // エフェクト表示位置


        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}