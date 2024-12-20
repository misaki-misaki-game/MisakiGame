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

            // ターゲット方向を向く
            FaceTarget(target);

            base.BraveAttack();
        }
        public override void Dead()
        {
            // EnemyScriptが破棄されたことを通知
            MessageBroker.Default.Publish(new EnemyScriptDestroyedMessage(this));

            // UIを消去
            Destroy(tar);
            Destroy(subCan);

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

            // ターゲット方向を向く
            FaceTarget(target);

            base.HPAttack();

            // HP攻撃ボリュームに変更
            isHPAttack = true;
            GameManager.BeginHPAttackTime();
        }

        public override void Move()
        {
            // 待機・移動中以外はリターン
            if (AnimState != AnimState.E_Idle && AnimState != AnimState.E_Move) return;

            // 自身とターゲットの距離を取得
            float distance = Vector3.Distance(transform.position, target.position);

            // 距離が設定した停止距離より大きいなら
            if (distance > attackList[attackID].stopDistance)
            {
                base.Move(); // 移動中に変更

                // 待機時間をリセット
                idleTime = defaultIdleTime;

                isWandering = false; // うろうろしないようにする

                // 行先を設定し、そこに移動
                agent.SetDestination(target.position); // 行先をターゲットのポジションに設定
                agent.stoppingDistance = attackList[attackID].stopDistance; // 停止距離設定
                agent.isStopped = false; // 移動開始
                float speed = agent.velocity.magnitude; // 速度ベクトルの長さ(速度)を取得
                anim.SetFloat("Af_Running", speed); // 移動のアニメーション開始
            }
            // 停止距離内
            else if (!isWandering)
            {
                AnimState = AnimState.E_Idle; // 待機中に変更

                agent.velocity = Vector3.zero; // 速度ベクトルを0に変更
                agent.isStopped = true; // 移動停止
                anim.SetFloat("Af_Running", 0f); // 待機のアニメーション開始

                idleTime -= Time.deltaTime; // 待機時間をカウントダウンする

                // 敵の上に乗る問題が解決しないとうろうろさせられない
                // 待機時間が0以下になったら
                if (idleTime <= 0f)
                {
                    // ランダムな位置(外周)を取得してその方向へ向かうように設定
                    Vector3 wanderPoint = GetRandomOuterPoint(target.position, transform.position, attackList[attackID].stopDistance, 120); // ランダム位置を取得
                    agent.SetDestination(wanderPoint);
                    agent.stoppingDistance = 1f; // 停止距離設定
                    isWandering = true; // うろうろさせる
                }
            }

            // 自身とうろうろ位置の距離を取得
            float wanderPointDistance = Vector3.Distance(transform.position, agent.destination);

            // うろうろ位置にたどり着いてない場合
            if (isWandering && wanderPointDistance > agent.stoppingDistance)
            {
                base.Move(); // 移動中に変更

                agent.isStopped = false; // 移動開始
                float speed = agent.velocity.magnitude; // 速度ベクトルの長さ(速度)を取得
                anim.SetFloat("Af_Running", speed); // 移動のアニメーション開始
            }
            // たどり着いた場合
            else if (isWandering && wanderPointDistance <= agent.stoppingDistance)
            {
                // 待機時間をリセット
                idleTime = defaultIdleTime;

                isWandering = false; // うろうろしないようにする
            }

            // ターゲット方向を向く
            FaceTarget(target, true);
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

            // ブレイク状態の場合のみ0ダメージにする
            if (BraveState != BraveState.E_Break) bulletAttackScript.SetHPAttack = parameter.brave;
            else bulletAttackScript.SetHPAttack = 0;

            bulletAttackScript.ClearHitObj();
        }

        public override void EndAnim()
        {
            base.EndAnim();
            GetAttackPattern();
            // 通常ボリュームに戻す
            if(isHPAttack)
            {
                GameManager.EndHPAttackTime();
            }
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
                tar = new GameObject("HPTarget");
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
            braveTextAnimators = new TextMeshProGeometryAnimator[1];
            braveTextAnimators[0] = eUI.transform.Find("BrText").gameObject.GetComponent<TextMeshProGeometryAnimator>();

            // HPUIと変数を連動
            InitializeHPUI();
        }

        /// <summary>
        /// ターゲットとキャンパスを代入する関数
        /// </summary>
        /// <param name="targetTransform">追跡ターゲット</param>
        /// <param name="damageCanvas">ダメージポップアップ用キャンパス</param>
        /// <param name="enemyCanvas">エネミーHP用キャンパス</param>
        public void SetupTargetAndCanvas(Transform targetTransform, GameObject damageCanvas, GameObject enemyCanvas)
        {
            target = targetTransform;
            GetComponent<DamageUIScript>().SetCanvas = damageCanvas;
            ui = enemyCanvas;
        }

        /// <summary>
        /// フェードを開始する関数
        /// </summary>
        public void StartFade()
        {
            StartCoroutine(FadeAllMaterials());
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

            // 待機時間を初期化
            idleTime = defaultIdleTime;

            // 攻撃パターンの辞書を初期化
            foreach (AttackDictionary dict in attackList)
            {
                attackDict.Add(dict.attackPattern, dict.attackRate);
            }

            // 攻撃IDをランダムに決める
            GetAttackPattern();

            // 全てのマテリアルを取得
            StartCoroutine(InitializeMaterials());
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
                if (attackState == AttackState.E_BraveAttack)
                {
                    BraveAttack();
                }
                else if (attackState == AttackState.E_HPAttack)
                {
                    HPAttack();
                }
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
            attackTime = Random.Range(1.5f, 4);

            // ターゲットとの距離をIDによって変える
            agent.stoppingDistance = attackList[attackID].stopDistance;

            // IDによってモーションを変える
            if (attackID < braveAttackClip.Length)
            {
                BraveAttackSetUP(attackID);
            }
            else
            {
                HPAttackSetUP(attackID - braveAttackClip.Length);
            }
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
            attackTime -= Time.deltaTime;

            // 待機時間が0以下になったらtrueを返す
            // まだの場合はfalseを返す
            if (attackTime <= 0 && AnimState == AnimState.E_Idle) return true;

            return false;
        }

        /// <summary>
        /// ベクトル方向を取得し、指定角度の外周の位置を返す関数
        /// </summary>
        /// <param name="center">円の中心</param>
        /// <param name="enemyPosition">ベクトル方向のポジション</param>
        /// <param name="radius">半径</param>
        /// <param name="angleRange">取得したい位置の角度</param>
        /// <returns></returns>
        private Vector3 GetRandomOuterPoint(Vector3 center, Vector3 enemyPosition, float radius, float angleRange)
        {
            // プレイヤーからエネミーへの方向ベクトルを基準に設定
            Vector3 directionToEnemy = (enemyPosition - center).normalized;

            // 外周のランダムな角度を -angleRange/2 から +angleRange/2 の範囲で取得
            float angle = Random.Range(-angleRange / 2f, angleRange / 2f);
            float radians = angle * Mathf.Deg2Rad;

            // 基準ベクトル（エネミー方向）に対して角度を回転させた方向ベクトルを計算
            Vector3 rotatedDirection = Quaternion.Euler(0, angle, 0) * directionToEnemy;

            // 中心点から回転方向に半径分だけ移動した位置を返す
            return center + rotatedDirection * radius;
        }

        /// <summary>
        /// 全てのマテリアルを取得する関数
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitializeMaterials()
        {
            // 全ての子オブジェクトのRendererを取得して、マテリアルをリストに追加
            Renderer[] allRenderers = GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in allRenderers)
            {
                foreach (Material mat in renderer.materials)
                {
                    materials.Add(mat);
                }
            }
            yield return null;
        }

        /// <summary>
        /// 全マテリアルを徐々に透明にする関数
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeAllMaterials()
        {
            // 経過時間と透明度の変数を生成
            float elapsedTime = 0f;
            List<float> initialTransparencyValues = new List<float>();

            // 各マテリアルの初期の_Tweak_transparency値を保持
            foreach (Material mat in materials)
            {
                float initialTransparency = mat.HasProperty("_Tweak_transparency") ? mat.GetFloat("_Tweak_transparency") : 0f;
                initialTransparencyValues.Add(initialTransparency);
            }

            // 経過時間が設定した時間を超えるまで、フェード処理を行う
            while (elapsedTime < fadeDuration)
            {
                // 経過時間を保持、経過時間に基づき透明度を算出
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);

                // 全マテリアルの透明度を変更する
                for (int i = 0; i < materials.Count; i++)
                {
                    Material mat = materials[i];

                    if (mat.HasProperty("_Tweak_transparency"))
                    {
                        float newTransparency = Mathf.Lerp(initialTransparencyValues[i], -1f, elapsedTime / fadeDuration);
                        mat.SetFloat("_Tweak_transparency", newTransparency);
                    }
                }

                yield return null;
            }

            // 最終的に完全に透明に設定
            foreach (Material mat in materials)
            {
                if (mat.HasProperty("_Tweak_transparency"))
                {
                    mat.SetFloat("_Tweak_transparency", -1f);
                }
            }
        }

        /// <summary>
        /// ランダムな場所を決める関数
        /// </summary>
        /// <param name="center">生成したい範囲の中心</param>
        /// <param name="radius">生成したい範囲</param>
        /// <returns></returns>
        Vector3 GetRandomWanderPoint(Vector3 center, float radius)
        {
            Vector2 randomPoint = Random.insideUnitCircle * radius;
            return new Vector3(center.x + randomPoint.x, center.y, center.z + randomPoint.y);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(Tags.Player.ToString()))
            {
                // コライダーに乗り上げないように押し出す関数の許可を出す
                inCollider = true;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag(Tags.Player.ToString()))
            {
                // 押し出す関数の許可を取り消す
                inCollider = false;
            }
        }

        /// <summary>
        /// 自身を削除する関数
        /// </summary>
        private void DestroySelf()
        {
            Destroy(gameObject);
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
            public Collider Collider { get; }

            public EnemyScriptCreatedMessage(EnemyScript script)
            {
                Script = script;
                Collider = script.GetComponent<Collider>();
            }
        }

        // エネミースクリプトが削除された際のイベントメッセージ
        public class EnemyScriptDestroyedMessage
        {
            public EnemyScript Script { get; }
            public Collider Collider { get; }

            public EnemyScriptDestroyedMessage(EnemyScript script)
            {
                Script = script;
                Collider = script.GetComponent<Collider>();
            }
        }

        /// <summary>
        /// アタックスクリプト配列を複数持つリスト
        /// </summary>
        [System.Serializable]
        public class AttackDictionary
        {
            // コンストラクタ
            public AttackDictionary(int pattern, float rate, float distance)
            {
                attackPattern = pattern;
                attackRate = rate;
                stopDistance = distance;
            }

            public int attackPattern; // 攻撃パターン
            public float attackRate; // 攻撃の発生率
            public float stopDistance; // 相手との距離
        }

        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///
         
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

        protected Dictionary<int, float> attackDict = new Dictionary<int, float>(); // 敵の行動パターンの辞書

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
        private bool isWandering; // うろうろするかのフラグ
        private bool isHPAttack; // HP攻撃中かのフラグ

        private int attackID; // 攻撃ID

        private float attackTime; // 攻撃までの待機時間
        private float idleTime; // 待機モーションの持続時間
        private float defaultIdleTime = 1.5f; // 待機モーションの持続時間のデフォルト
        [SerializeField] private float fadeDuration = 2f; // フェードにかける時間

        private GameObject tar; // 変更用ターゲット変数
        private GameObject subCan; // 変更用ターゲット変数
        [SerializeField] private GameObject enemyUI; // エネミー用のHPUI
        [SerializeField] private GameObject cameraAnchor; // カメラアンカー

        private Rigidbody rigid; // リギッドボディ

        private CapsuleCollider col; // コライダー

        private AttackState attackState; // 攻撃の種類

        private List<Material> materials = new List<Material>(); // マテリアルリスト

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///

        public GameObject GetCameraAnchor {  get { return cameraAnchor; } }

        public Transform SetTarget { set { target = value; } }

        public NavMeshAgent GetNavMeshAgent { get { return agent; } }

        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}