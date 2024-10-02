using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace Misaki
{
    // 自動的にコンポーネントを追加 CapsuleColliderを追加
    [RequireComponent(typeof(CapsuleCollider))]
    public partial class AttackScript : MonoBehaviour
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        /// <summary>
        /// hitObjをリセットする関数
        /// </summary>
        public void ClearHitObj()
        {
            hitObj.Clear();
        }

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///


        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///

        private void Start()
        {
            // AttackScriptが生成されたことを通知
            MessageBroker.Default.Publish(new AttackScriptCreatedMessage(this));
        }

        private void OnDestroy()
        {
            // AttackScriptが破棄されたことを通知
            MessageBroker.Default.Publish(new AttackScriptDestroyedMessage(this));
        }

        /// <summary>
        /// 回避成功時のイベント発火関数
        /// </summary>
        private void TriggerDodgeSuccess()
        {
            // 回避成功イベントを発火
            MessageBroker.Default.Publish(new DodgeSuccessMessage());
        }

        /// <summary>
        /// コライダーを用いた当たり判定関数
        /// </summary>
        /// <param name="col">ヒットしたコライダー</param>
        private void OnTriggerEnter(Collider col)
        {
            // ステートによって処理を変える
            if (attack == AttackState.E_BraveAttack) CalcBrave(col);
            else if (attack == AttackState.E_HPAttack) CalcHP(col);
        }

        /// <summary>
        /// パーティクルシステムのコライダーを用いた当たり判定関数
        /// </summary>
        /// <param name="other">ヒットしたコライダー</param>
        private void OnParticleCollision(GameObject other)
        {
            // ヒットしたオブジェクトのコライダーを取得する
            Collider col = other.GetComponent<Collider>();

            //　イベントの取得
            particle.GetCollisionEvents(other, collisionEventList);

            //　衝突した位置を取得し、ダメージスクリプトを呼び出す
            foreach (var collisionEvent in collisionEventList)
            {
                Vector3 pos = collisionEvent.intersection;
                col.GetComponent<PlayerScript>().CanDamageEffect();
            }

            // ステートによって処理を変える
            if (attack == AttackState.E_BraveAttack) CalcBrave(col);
            else if (attack == AttackState.E_HPAttack) CalcHP(col);
        }

        /// <summary>
        /// パーティクルシステムのトリガーを用いた当たり判定関数
        /// </summary>
        private void OnParticleTrigger()
        {
            // Triggerで設定したコライダーを全て検索
            for (int i = 0; i < particle.trigger.colliderCount; i++)
            {
                // コライダーを取得
                Collider col = particle.trigger.GetCollider(i).GetComponent<Collider>();
                if (tag == Tags.PlayerWepon.ToString() && !col.CompareTag(Tags.Enemy.ToString())) return;
                if (tag == Tags.EnemyWepon.ToString() && !col.CompareTag(Tags.Player.ToString())) return;

                // コライダーがnullで無ければ、処理を開始
                if (col != null)
                {
                    col.GetComponent<BaseCharactorScript>().CanDamageEffect();

                    // ステートによって処理を変える
                    if (attack == AttackState.E_BraveAttack) CalcBrave(col);
                    else if (attack == AttackState.E_HPAttack) CalcHP(col);
                }
            }
        }

        /// <summary>
        /// ヒットしたコライダーにブレイブダメージを与える
        /// 及び与えたブレイブダメージを自身の所有者に渡す関数
        /// </summary>
        /// <param name="col">ヒットしたコライダー</param>
        private void CalcBrave(Collider col)
        {
            // エネミータグかつヒットオブジェクトリストに入っていなければ
            if (col.CompareTag(Tags.Enemy.ToString()) && !ChackInHit(col.gameObject) || col.CompareTag(Tags.Player.ToString()) && !ChackInHit(col.gameObject))
            {
                // 所有者の向きを取得
                Vector3 dir = -ownOwner.transform.forward;
          
                // ヒットオブジェクトリストに入れる
                hitObj.Add(col.gameObject);

                // 回避が成功していたら回避成功イベントを発火させる
                if(col.GetComponent<BaseCharactorScript>().IsDodge())
                {
                    TriggerDodgeSuccess();
                    return;
                }

                // 防御が成功していたら攻撃側のひるみが発生
                if (col.GetComponent<BaseCharactorScript>().IsGuard())
                {
                    // 防御ひるみ関数を呼び出し
                    ownOwner.GuardReaction();
                    return;
                }

                // ブレイブダメージを与えて与えたブレイブを自身の所有者に渡す
                ownOwner.HitBraveAttack(braveAttack, col.GetComponent<BaseCharactorScript>().ReceiveBraveDamage(braveAttack, dir, isCritical));

                // ヒットストップさせる
                HitStopManager.hitStop.StartHitStop(ownOwner.GetAnimator, 0.1f);
            }
        }
        
        /// <summary>
        /// ブレイブ値をヒットしたコライダーのHPに与える
        /// ヒットしたら自身の所有者のブレイブを0にする関数
        /// </summary>
        /// <param name="col">ヒットしたコライダー</param>
        private void CalcHP(Collider col)
        {
            // エネミータグかつヒットオブジェクトリストに入っていなければ
            // ヒットオブジェクトリストに入れて被ダメリアクションを取るように指示する
            if (col.CompareTag(Tags.Enemy.ToString()) && !ChackInHit(col.gameObject) || col.CompareTag(Tags.Player.ToString()) && !ChackInHit(col.gameObject))
            {
                // 所有者の向きを取得
                Vector3 dir = -ownOwner.transform.forward;

                // ヒットオブジェクトリストに入れる
                hitObj.Add(col.gameObject);

                // 回避が成功していたら回避成功イベントを発火させる
                if (col.GetComponent<BaseCharactorScript>().IsDodge())
                {
                    TriggerDodgeSuccess();
                    return;
                }

                // 被ダメリアクションを取るように指示する
                col.GetComponent<BaseCharactorScript>().ReceiveHPDamage(hpAttack, dir);

                // ヒットストップさせる
                HitStopManager.hitStop.StartHitStop(ownOwner.GetAnimator, 0.2f);

                // 所有者のブレイブを0にしてリジェネを開始する
                ownOwner.HitHPAttack();
            }
        }

        /// <summary>
        /// リストの中にエネミーがあるかどうかをチェックする関数
        /// </summary>
        /// <param name="obj">エネミーGameObject</param>
        /// <returns>ヒットしているかどうか</returns>
        private bool ChackInHit(GameObject obj)
        {
            return hitObj.Contains(obj);
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- /// 
    }
    public partial class AttackScript
    {
        /// --------変数一覧-------- ///

        #region public変数
        /// -------public変数------- ///

        // アタックスクリプトが生成された際のイベントメッセージ
        public class AttackScriptCreatedMessage
        {
            public AttackScript Script { get; }
            public AttackScriptCreatedMessage(AttackScript script)
            {
                Script = script;
            }
        }

        // アタックスクリプトが削除された際のイベントメッセージ
        public class AttackScriptDestroyedMessage
        {
            public AttackScript Script { get; }
            public AttackScriptDestroyedMessage(AttackScript script)
            {
                Script = script;
            }
        }

        // 回避が成功した際のイベントメッセージ
        public class DodgeSuccessMessage { }

        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///



        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///

        private bool isCritical; // クリティカルかどうか

        private float braveAttack = 0; // 攻撃値
        private float hpAttack = 0; // HP値

        private AttackState attack = default; // 攻撃の種類

        private List<ParticleCollisionEvent> collisionEventList = new List<ParticleCollisionEvent>(); // コリジョンイベントリスト

        [SerializeField] private ParticleSystem particle; // パーティクルシステム

        private List<GameObject> hitObj = new List<GameObject>(); // ヒットしたオブジェクト格納用リスト

        private BaseCharactorScript ownOwner; // このアタックスクリプトの所有者

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///

        // isCriticalのセッター関数
        public bool SetCritical { set { isCritical = value; } }

        // braveAttackとhpAttackのセッター関数
        public float SetBraveAttack { set { braveAttack = value; } }
        public float SetHPAttack { set { hpAttack = value; } }

        // attackのセッター関数
        public AttackState SetAttackState { set { attack = value; } }

        // ownerScriptのセッター関数
        public BaseCharactorScript SetOwnOwner { set { ownOwner = value; } }

        /// -------プロパティ------- ///
        #endregion
    
        /// --------変数一覧-------- ///
    }
}