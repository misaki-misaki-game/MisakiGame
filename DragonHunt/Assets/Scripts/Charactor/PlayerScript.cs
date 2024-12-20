using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using UniRx;
using TMPro;

namespace Misaki
{
    // 自動的にコンポーネントを追加 CharacterControllerを追加
    [RequireComponent(typeof(CharacterController))]
    public partial class PlayerScript : BaseCharactorScript
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        public override void ReceiveHPDamage(float brave, Vector3 direction)
        {
            base.ReceiveHPDamage(brave, direction);

            // HP攻撃ヒット時UIを表示
            StartCoroutine(ViewHPAttackHitUI(enemyHPAttackHitUI, brave));
        }

        public override void BraveAttack()
        {
            // ブレイブ攻撃中ならコンボを出す
            // 待機・移動中以外ならリターン
            if (AnimState == AnimState.E_Attack)
            {
                anim.SetTrigger("At_BAttack");
            }
            else if (AnimState != AnimState.E_Idle && AnimState != AnimState.E_Move) return;

            // ターゲット方向を向く
            if (camerawork.GetLockonTargetTransform != null)
            {
                FaceTarget(camerawork.GetLockonTargetTransform);
            }

            base.BraveAttack();
        }

        public override IEnumerator BraveHitReaction()
        {
            // 被ダメージ状態がスーパーアーマーでなければ
            if (damageState != DamageState.E_SuperArmor)
            {
                // ランダムに決めた小怯みアニメーションを再生
                int rnd = UnityEngine.Random.Range(0, smallHitClip.Length);
                SmallHitReaction(rnd);
            }

            StartCoroutine(base.BraveHitReaction());

            yield return null;
        }

        public override void Dead()
        {
            onPlayerDead.OnNext(Unit.Default);  // イベントを発行
            onPlayerDead.OnCompleted();  // イベント終了

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
            if (camerawork.GetLockonTargetTransform != null)
            {
                FaceTarget(camerawork.GetLockonTargetTransform);
            }

            base.HPAttack();
        }

        public override void Move()
        {
            // 待機・移動中以外はリターン
            if (AnimState != AnimState.E_Idle && AnimState != AnimState.E_Move) return;

            base.Move();

            // 移動速度を取得 
            float spd = parameter.speed;

            // カメラの向きを基準にした正面方向のベクトル
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

            // 前後左右の入力（WASDキー）から、移動のためのベクトルを計算
            Vector3 moveZ = cameraForward * moveInputValue.y * spd;  //　前後（カメラ基準）　 
            Vector3 moveX = Camera.main.transform.right * moveInputValue.x * spd; // 左右（カメラ基準）

            // 移動距離計算と重力計算
            moveDirection.y -= gravity * Time.deltaTime;
            moveDirection = moveZ + moveX + new Vector3(0, moveDirection.y, 0);

            // 移動のアニメーション
            anim.SetFloat("Af_Running", (moveZ + moveX).magnitude);

            // プレイヤーの向きを入力の向きに変更　
            transform.LookAt(transform.position + moveZ + moveX);

            // Move は指定したベクトルだけ移動させる命令
            con.Move(moveDirection * Time.deltaTime);
        }

        public override void HitBraveAttack(float obtainBrave, bool braveBreak)
        {
            // エネミーブレイクUIを表示
            if (braveBreak)
            {
                StartCoroutine(ViewBreakUI(enemyBreakUI));
            }
            base.HitBraveAttack(obtainBrave, braveBreak);
        }

        public override void HitHPAttack(float damage)
        {
            // ブレイブの状態によって処理を変える
            if (BraveState != BraveState.E_Break)
            {
                // HP攻撃ヒット時UIを表示
                StartCoroutine(ViewHPAttackHitUI(playerHPAttackHitUI, damage));
            }
            else
            {
                // HP攻撃ヒット時UIを表示(ブレイク状態なので0で表示)
                StartCoroutine(ViewHPAttackHitUI(playerHPAttackHitUI, 0f));
            }

            base.HitHPAttack(damage);
        }

        public override void BeginKnockBack()
        {
            if (AnimState != AnimState.E_HitReaction) return;
            con.Move(-transform.forward * knockBackDistance * Time.deltaTime);
        }

        public override void GuardReaction()
        {
            base.GuardReaction();

            // ランダムに小怯みモーションを発生させる
            int rnd = UnityEngine.Random.Range(0, smallHitClip.Length);
            SmallHitReaction(rnd);
        }

        public IObservable<Unit> OnPlayerDead => onPlayerDead; // プレイヤーが戦闘不能になった際に監視者に通知

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///

        protected override void Start()
        {
            base.Start();

            // コンポーネントを取得
            con ??= GetComponent<CharacterController>();

            key ??= Keyboard.current; // 現在のキーボード情報を取得

            playerInputs = new PlayerInputs(); // Actionスクリプトのインスタンス生成

            // イベント登録
            playerInputs.Player.Move.started += OnMove;
            playerInputs.Player.Move.performed += OnMove;
            playerInputs.Player.Move.canceled += OnMove;
            playerInputs.Player.BAttack.started += OnBAttack;
            playerInputs.Player.HAttack.started += OnHAttack;
            playerInputs.Player.Guard.started += OnGuard;
            playerInputs.Player.Dodge.started += OnDodge;
            playerInputs.Player.Lockon.started += OnLockon;
            playerInputs.Player.SwitchTarget.started += OnSwitchTarget;

            startPos = transform.position; // 初期位置を取得
            cameraOffset = plCamera.transform.localPosition - transform.localPosition; // プレイヤーとカメラの距離を取得

            InitializeHPUI(); // HPに数値を反映

            // PlayerScriptが生成されたことを通知
            MessageBroker.Default.Publish(new PlayerScriptCreatedMessage(this));
        }

        protected override void Update()
        {
            // タイトル画面ならリターン
            if (GameManager.GetGameState == GameState.E_Title) return;
            // インゲーム画面に遷移したらアクションマップを設定する
            else if (GameManager.GetGameState == GameState.E_InGame && !isInGame)
            {
                playerInputs.Enable();
                isInGame = true;
            }

            // キーボードチェック
            CheckKeyBoard();

            // 移動関数
            Move();

            base.Update();
        }

        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///

        private void OnDestroy()
        {
            // 自身でインスタンス化したActionクラスはIDisposableを実装しているので、
            // 必ずDisposeする必要がある
            playerInputs?.Dispose();
        }

        private void OnEnable()
        {
            // オブジェクトがアクティブになった時にplayerInputsを起動
            playerInputs?.Enable();
        }

        private void OnDisable()
        {
            // オブジェクトが非アクティブになった時にplayerInputを停止
            playerInputs?.Dispose();
        }

        /// <summary>
        /// 移動のコールバック登録関数
        /// </summary>
        /// <param name="context"></param>
        private void OnMove(InputAction.CallbackContext context)
        {
            // Moveアクションの入力取得
            moveInputValue = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// ブレイブ攻撃のコールバック登録関数
        /// </summary>
        /// <param name="context"></param>
        private void OnBAttack(InputAction.CallbackContext context)
        {
            BraveAttack();
        }

        /// <summary>
        /// HP攻撃のコールバック登録関数
        /// </summary>
        /// <param name="context"></param>
        private void OnHAttack(InputAction.CallbackContext context)
        {
            HPAttack();
        }

        /// <summary>
        /// 防御のコールバック登録関数
        /// </summary>
        /// <param name="context"></param>
        private void OnGuard(InputAction.CallbackContext context)
        {
            Guard();
        }

        /// <summary>
        /// 回避のコールバック登録関数
        /// </summary>
        /// <param name="context"></param>
        private void OnDodge(InputAction.CallbackContext context)
        {
            Dodge();
        }

        /// <summary>
        /// ロックオンのコールバック登録関数
        /// </summary>
        private void OnLockon(InputAction.CallbackContext context)
        {
            camerawork.Lockon();
        }

        /// <summary>
        /// 視点移動・ロックターゲット切り替えのコールバック登録関数
        /// </summary>
        private void OnSwitchTarget(InputAction.CallbackContext context)
        {
            // switchTargetアクションの入力取得し、ターゲットを切り替える
            switchInputValue = context.ReadValue<Vector2>();
            camerawork.ChangeTarget(switchInputValue.x);
        }

        /// <summary>
        /// キーボードの接続チェック関数
        /// </summary>
        private void CheckKeyBoard()
        {
            // キーボード接続チェック
            if (key == null)
            {
                // キーボードが接続されていないと
                // Keyboard.currentがnullになる
                Debug.LogError("キーボードが接続されていません");
                return;
            }
        }

        /// <summary>
        /// 顔ウィンドウの表示を変える関数
        /// </summary>
        /// <param name="animState"></param>
        private void ChangeFace(AnimState animState)
        {
            faceWindow.sprite = faceSprite[(int)animState];
        }

        /// <summary>
        /// HP攻撃ヒット時にUIを呼び出す関数
        /// </summary>
        /// <param name="ui">呼び出したいUI構造体</param>
        /// <param name="damage">ダメージ量</param>
        /// <returns></returns>
        private IEnumerator ViewHPAttackHitUI(HPAttackHitUI ui, float damage)
        {
            // UIを表示
            ui.hpAttackUI.SetActive(true);

            // ダメージ量をテキストに代入
            if(ui.isPlayerAttack)
            {
                ui.damageText.text = string.Format("{0:0}", damage) + " !!!";
            }
            else
            {
                ui.damageText.text = string.Format("{0:0}", damage);
            }

            // 2秒間待つ
            yield return new WaitForSeconds(2f);

            // UIを非表示
            ui.hpAttackUI.SetActive(false);
        }

        /// <summary>
        /// HP攻撃ヒット時にUIを呼び出す関数
        /// </summary>
        /// <param name="ui">呼び出したいUI構造体</param>
        /// <returns></returns>
        private IEnumerator ViewBreakUI(BreakUI ui)
        {
            // UIを表示
            ui.breakUI.SetActive(true);

            // 2秒間待つ
            yield return new WaitForSeconds(2f);

            // UIを非表示
            ui.breakUI.SetActive(false);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.collider.CompareTag(Tags.Enemy.ToString()))
            {
                // コライダーに乗り上げないように押し出す関数の許可を出す
                inCollider = true;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag(Tags.Enemy.ToString()))
            {
                // 押し出す関数の許可を取り消す
                inCollider = false;
            }
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class PlayerScript
    {
        /// --------変数一覧-------- ///

        #region public変数
        /// -------public変数------- ///

        // プレイヤースクリプトが生成された際のイベントメッセージ
        public class PlayerScriptCreatedMessage
        {
            public PlayerScript Script { get; }
            public Collider Collider { get; }
            public PlayerScriptCreatedMessage(PlayerScript script)
            {
                Script = script;
                Collider = script.GetComponent<Collider>();
            }
        }

        // プレイヤースクリプトが削除された際のイベントメッセージ
        public class PlayerScriptDestroyedMessage
        {
            public PlayerScript Script { get; }
            public Collider Collider { get; }

            public PlayerScriptDestroyedMessage(PlayerScript script)
            {
                Script = script;
                Collider = script.GetComponent<Collider>();
            }
        }

        // HP攻撃ヒット時の構造体
        [Serializable]
        public struct HPAttackHitUI
        {
            public bool isPlayerAttack; // プレイヤーの攻撃かどうか
            public GameObject hpAttackUI; // UIオブジェクト
            public TextMeshProUGUI damageText; // ダメージ表示テキスト
        }

        // ブレイク成功時の構造体
        [Serializable]
        public struct BreakUI
        {
            public GameObject breakUI; // ブレイクUI
        }

        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///



        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///

        private bool isInGame = false; // インゲーム中かどうか

        private float gravity = 10f; // 重力

        private Vector2 moveInputValue; // 移動入力した値
        private Vector2 switchInputValue; // 視点入力した値

        private Vector3 moveDirection = Vector3.zero; // 移動した位置
        private Vector3 cameraOffset = Vector3.zero; // カメラとプレイヤーの差
        private Vector3 startPos; // 初期位置

        [Header("小怯みアニメーション")]
        [SerializeField] private AnimationClip[] smallHitClip = new AnimationClip[3];

        [SerializeField] private GameObject plCamera; // PLのカメラ
        [SerializeField] private GameObject aura; // オーラエフェクト

        private CharacterController con; // CharacterController変数

        private Keyboard key; // Keyboard変数

        private PlayerInputs playerInputs; // InputSystemから生成したスクリプト

        private Subject<Unit> onPlayerDead = new Subject<Unit>(); // 戦闘不能イベントのためのSubject

        [SerializeField] private Image faceWindow; // 顔表示ウィンドウ

        [SerializeField, EnumIndex(typeof(AnimState))] private Sprite[] faceSprite; // 顔グラフィック

        [SerializeField] private Camerawork camerawork; // カメラワーク

        [SerializeField] private HPAttackHitUI playerHPAttackHitUI; // プレイヤーのHP攻撃がヒットした際のUI
        [SerializeField] private HPAttackHitUI enemyHPAttackHitUI; // エネミーのHP攻撃がヒットした際のUI

        [SerializeField] private BreakUI playerBreakUI; // プレイヤーがブレイクした際のUI
        [SerializeField] private BreakUI enemyBreakUI; // エネミーがブレイクした際のUI

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///

        public GameObject GetAura {  get { return aura; } }

        public override BraveState BraveState
        {
            get { return base.BraveState; }
            set
            {
                base.BraveState = value;
                if (BraveState == BraveState.E_Break)
                {
                    StartCoroutine(ViewBreakUI(playerBreakUI));
                }
            }
        }

        protected override AnimState AnimState
        {
            get {return base.AnimState;} 
            set
            {
                // 取得したアニメーション状態によって顔を変える
                base.AnimState = value;
                ChangeFace(AnimState);
            }  
        }

        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}