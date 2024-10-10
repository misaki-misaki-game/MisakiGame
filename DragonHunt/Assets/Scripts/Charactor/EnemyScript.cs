using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Misaki
{
    // 自動的にコンポーネントを追加 NavMeshAgent Rigidbody CapsuleCollider
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public partial class EnemyScript : BaseCharactorScript
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        public override void BraveAttack()
        {
            // 待機・移動中以外ならリターン
            if (AnimState != AnimState.E_Idle && AnimState != AnimState.E_Move) return;

            base.BraveAttack();
        }
        public override void Dead()
        {
            // EnemyScriptが破棄されたことを通知
            MessageBroker.Default.Publish(new EnemyScriptDestroyedMessage(this));

            base.Dead();
        }

        public override void Dodge()
        {
            // 待機・移動中以外はリターン
            if (AnimState != AnimState.E_Idle && AnimState != AnimState.E_Move) return;

            base.Dodge();
        }

        public override void Guard()
        {
            // 待機・移動中以外はリターン
            if (AnimState != AnimState.E_Idle && AnimState != AnimState.E_Move) return;

            base.Guard();
        }

        public override void HPAttack()
        {
            // HP攻撃中ならリターン
            if (AnimState == AnimState.E_Attack) return;

            base.HPAttack();
        }

        public override void Move()
        {
            // 待機・移動中以外はリターン
            if (AnimState != AnimState.E_Idle && AnimState != AnimState.E_Move) return;

            // 自身とターゲットの距離を取得
            float distance = Vector3.Distance(transform.position, target.position);
            agent.SetDestination(target.position); // 行先をターゲットのポジションに設定

            // 距離が設定した停止距離より大きいなら
            if (distance > agent.stoppingDistance)
            {
                base.Move(); // 移動中に変更

                // 行先を設定し、そこに移動
                agent.isStopped = false; // 移動開始
                float speed = agent.velocity.magnitude; // 速度ベクトルの長さ(速度)を取得
                anim.SetFloat("Af_Running", speed); // 移動のアニメーション開始
            }
            // 距離が設定した停止距離以下なら
            else
            {
                AnimState = AnimState.E_Idle; // 待機中に変更

                agent.velocity = Vector3.zero; // 速度ベクトルを0に変更
                agent.isStopped = true; // 移動停止
                anim.SetFloat("Af_Running", 0f); // 待機のアニメーション開始

                // ターゲットの方向を向く（回転のみ）
                Vector3 direction = (target.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // 水平回転のみ適用
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 4f); // スムーズに回転
            }
        }

        /// <summary>
        /// HP攻撃開始時の関数
        /// 遠距離で特定の位置に攻撃する場合
        /// </summary>
        /// <param name="effectName">エフェクト名</param>
        public override void BeginHPSearch(EffectName effectName)
        {
            // エフェクトを生成し、そのアタックスクリプトを取得
            GameObject obj = GenerateEffectNoneParent(effectName, target.gameObject);
            bulletAttackScript = obj.GetComponentInChildren<AttackScript>();
            
            // 武器のステートとHP攻撃値を変更し、ヒットオブジェクトリストをリセットする
            // アタックスクリプトの所有者を自分にする
            bulletAttackScript.SetOwnOwner = this;
            bulletAttackScript.SetAttackState = AttackState.E_HPAttack;
            bulletAttackScript.SetHPAttack = parameter.brave;
            bulletAttackScript.ClearHitObj();
        }

        public override void EndAnim()
        {
            base.EndAnim();
            GetAttackPattern();
        }

        public override void BeginKnockBack()
        {
            if (AnimState != AnimState.E_HitReaction) return;
            rigid.AddForce(-transform.forward * knockBackDistance * Time.deltaTime);
        }

        public override void GuardReaction()
        {
            base.GuardReaction();
            SmallHitReaction(0);
        }

        /// <summary>
        /// エネミーのHPUIを初期化・表示する関数
        /// </summary>
        public void InitializeEnemyUI()
        {
            // tarオブジェクトが存在しない場合
            if (!tar)
            {
                // 新しいターゲットオブジェクトとサブキャンパスを生成
                tar = new GameObject("Tar");
                subCan = Instantiate(subCanvas);

                // UIの位置更新のためにトランスフォームを代入
                subCan.GetComponent<UIUpdate>().SetTarget = tar.transform;

                // サブキャンバスをアクティブ化して表示可能にする
                subCan.SetActive(true);

                // tarをtargetの子オブジェクトとして設定する
                // subCanをメインキャンバス（canvas）の子オブジェクトとして設定する
                tar.transform.SetParent(transform);
                subCan.transform.SetParent(ui.transform);

                // tarの位置をこのオブジェクトの位置から少し上に移動
                tar.transform.position = transform.position + Vector3.up * offsetUI;
            }

            // エネミー用HPUI生成
            GameObject eUI = Instantiate(enemyUI, subCan.transform);

            // エネミー用のHPUIの位置調整
            eUI.GetComponent<RectTransform>().position = ui.transform.position + Vector3.up;

            // 各変数に代入
            textHP = eUI.transform.Find("HPGauge/HPText").gameObject.GetComponent<TextMeshProUGUI>();
            textBrave = eUI.transform.Find("BrText").gameObject.GetComponent<TextMeshProUGUI>();
            textBreak = eUI.transform.Find("BreakText").gameObject.GetComponent<TextMeshProUGUI>();
            textBreak.gameObject.SetActive(false);
            hpBar = eUI.transform.Find("HPGauge/HPBar").gameObject.GetComponent<Image>();

            // HPUIと変数を連動
            InitializeHPUI();
        }

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///

        protected override void Start()
        {
            base.Start();

            // コンポーネントを取得
            agent ??= GetComponent<NavMeshAgent>(); // ナビメッシュエージェントを取得
            rigid ??= GetComponent<Rigidbody>(); // リギッドボディ
            col ??= GetComponent<CapsuleCollider>(); // コライダー

            // EnemyScriptが生成されたことを通知
            MessageBroker.Default.Publish(new EnemyScriptCreatedMessage(this));

            // ナビメッシュエージェントのスピードを初期化
            agent.speed = parameter.speed;

            // 攻撃パターンの辞書を初期化
            foreach (AttackDictionary dict in attackList)
            {
                attackDict.Add(dict.attackPattern, dict.attackRate);
            }

            // 攻撃IDをランダムに決める
            GetAttackPattern();
        }

        protected override void Update()
        {
            // タイトル画面ならリターン
            if (GameManager.GetGameState == GameState.E_Title) return;

            // 移動関数
            Move();

            // 攻撃許可かつ待機中または移動中なら
            if (CanAttack())
            {
                if (attackState == AttackState.E_BraveAttack) BraveAttack();
                else if (attackState == AttackState.E_HPAttack) HPAttack();
            }

            base.Update();
        }

        /// <summary>
        /// 攻撃パターンの抽選関数
        /// </summary>
        protected void GetAttackPattern()
        {
            // 攻撃IDの抽選
            attackID = Choose();

            // 攻撃までの待機時間を抽選
            idleTime = Random.Range(1.5f, 4);

            // IDによってモーションを変える
            if(attackID < braveAttackClip.Length) BraveAttackSetUP(attackID);
            else HPAttackSetUP(attackID - braveAttackClip.Length);
        }

        /// <summary>
        /// 攻撃パターン辞書から発生率に基づき抽選する関数
        /// </summary>
        /// <returns></returns>
        protected int Choose()
        {
            // 確率の合計値を格納
            float total = 0;

            // 攻撃パターン辞書から攻撃発生率を合計する
            foreach (KeyValuePair<int, float> elem in attackDict)
            {
                total += elem.Value;
            }

            // Random.valueでは0から1までのfloat値を返すので
            // そこにドロップ率の合計を掛ける
            float randomPoint = Random.value * total;

            // randomPointの位置に該当するキーを返す
            foreach (KeyValuePair<int, float> elem in attackDict)
            {
                // 発生率の高いものから順に比較する
                // 低ければ該当するキー(ID)を返す
                if (randomPoint < elem.Value)
                {
                    return elem.Key;
                }
                // 高ければランダムポイントから発生率を引いて次の発生率と比較する
                else
                {
                    randomPoint -= elem.Value;
                }
            }
            // 100の場合は0を返す
            return 0;
        }

        /// <summary>
        /// ブレイブ攻撃モーションを設定する関数
        /// </summary>
        protected void BraveAttackSetUP(int rnd)
        {
            // ブレイブ攻撃状態に変更
            attackState = AttackState.E_BraveAttack;

            // アニメーションを選択
            anim.SetInteger("Ai_BAttack", rnd);

            // 使用するアタックスクリプトリストを変更
            attackScripts = new List<AttackScript>(attackScriptList[rnd].attackScriptGroup);
        }

        /// <summary>
        /// HP攻撃モーションを設定する関数
        /// </summary>
        protected void HPAttackSetUP(int rnd)
        {
            // HP攻撃状態に変更
            attackState = AttackState.E_HPAttack;

            // アニメーションを選択
            anim.SetInteger("Ai_HAttack", rnd);
        }

        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///

        /// <summary>
        /// 攻撃の許可を出す関数
        /// </summary>
        /// <returns>trueが出たら攻撃開始</returns>
        private bool CanAttack()
        {
            // 待機時間を減らす
            idleTime -= Time.deltaTime;

            // 待機時間が0以下になったらtrueを返す
            // まだの場合はfalseを返す
            if (idleTime <= 0 && AnimState == AnimState.E_Idle) return true;

            return false;
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class EnemyScript
    {
        /// --------変数一覧-------- ///

        #region public変数
        /// -------public変数------- ///

        // エネミースクリプトが生成された際のイベントメッセージ
        public class EnemyScriptCreatedMessage
        {
            public EnemyScript Script { get; }
            public EnemyScriptCreatedMessage(EnemyScript script)
            {
                Script = script;
            }
        }

        // エネミースクリプトが削除された際のイベントメッセージ
        public class EnemyScriptDestroyedMessage
        {
            public EnemyScript Script { get; }
            public EnemyScriptDestroyedMessage(EnemyScript script)
            {
                Script = script;
            }
        }

        /// <summary>
        /// アタックスクリプト配列を複数持つリスト
        /// </summary>
        [System.Serializable]
        public class AttackDictionary
        {
            // コンストラクタ
            public AttackDictionary(int pattern, float rate)
            {
                attackPattern = pattern;
                attackRate = rate;
            }

            public int attackPattern; // 攻撃パターン
            public float attackRate; // 攻撃の発生率
        }

        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///
         
        [SerializeField] protected bool isEnemy = true; // 敵かどうか

        [SerializeField] protected float offsetUI = 2.0f; // エネミーUIの配置位置

        protected Vector2 moveInputValue; // 入力した値

        protected Vector3 moveDirection = Vector3.zero; // 移動した位置
        protected Vector3 cameraOffset = Vector3.zero; // カメラとプレイヤーの差
        protected Vector3 startPos; // 初期位置

        [SerializeField] protected Transform target; // ターゲット

        protected NavMeshAgent agent; // ナビゲーションエージェント変数

        [Header("ブレイブ攻撃アニメーション")]
        [SerializeField] protected AnimationClip[] braveAttackClip = new AnimationClip[2];
        [Header("HP攻撃アニメーション")]
        [SerializeField] protected AnimationClip[] hpAttackClip = new AnimationClip[2];

        protected Dictionary<int, float> attackDict= new Dictionary<int, float>(); // 敵の行動パターンの辞書

        [Header("必ずアニメーションで呼び出したいAttackScriptListと同じにすること")]
        [SerializeField] protected List<AttackDictionary> attackList; // 攻撃パターンリスト

        protected Keyboard key; // Keyboard変数

        protected PlayerInputs playerInputs; // InputSystemから生成したスクリプト

        [SerializeField] protected GameObject ui; // エネミーのUI
        [SerializeField] protected GameObject subCanvas; // サブキャンパス

        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///

        private bool canAttack; // 攻撃フラグ

        private int attackID; // 攻撃ID

        private float idleTime; // 攻撃までの待機時間

        private GameObject tar; // 変更用ターゲット変数
        private GameObject subCan; // 変更用ターゲット変数
        [SerializeField] private GameObject enemyUI; // エネミー用のHPUI
        [SerializeField] private GameObject cameraAnchor; // カメラアンカー

        private Rigidbody rigid; // リギッドボディ

        private CapsuleCollider col; // コライダー

        private AttackState attackState; // 攻撃の種類

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///

        public GameObject GetCameraAnchor {  get { return cameraAnchor; } }

        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}