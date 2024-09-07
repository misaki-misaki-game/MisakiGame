using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

namespace Misaki
{
    // 自動的にコンポーネントを追加 CharacterControllerを追加
    [RequireComponent(typeof(CharacterController))]
    public partial class PlayerScript : BaseCharactorScript
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        public override void BraveAttack()
        {
            // ブレイブ攻撃中ならコンボを出す
            // 待機・移動中以外ならリターン
            if (animState == AnimState.E_Attack)
            {
                anim.SetTrigger("At_BAttack");
            }
            else if (animState != AnimState.E_Idle && animState != AnimState.E_Move) return;

            base.BraveAttack();

            // 対応アニメーションを再生
            anim.SetTrigger("At_BAttack");
        }

        public override IEnumerator BraveHitReaction()
        {
            StartCoroutine(base.BraveHitReaction());

            // 被ダメージ状態がスーパーアーマーでなければ
            if (damageState != DamageState.E_SuperArmor)
            {
                // ランダムに決めた小怯みアニメーションを再生
                int rnd = Random.Range(0, smallHitClip.Length);
                SmallHitReaction(rnd);
            }

            // テキストを変更する
            textBrave.text = string.Format("{0:0}", parameter.brave);

            // リジェネ中なら通常状態にしてリジェネを止める
            if (braveState == BraveState.E_Regenerate) braveState = BraveState.E_Default;

            // ブレイブが0以下になったらブレイク状態にする
            if (parameter.brave <= 0)
            {
                parameter.brave = 0;
                braveState = BraveState.E_Break;
            }

            yield return null;
        }

        public override void Dead()
        {
            base.Dead();

            // ゲームオーバーにする
        }

        public override void Dodge()
        {
            // 待機・移動中以外はリターン
            if (animState != AnimState.E_Idle && animState != AnimState.E_Move) return;

            base.Dodge();
            // 対応アニメーションを再生
            anim.SetTrigger("At_Dodge");
        }

        public override void Guard()
        {
            // 待機・移動中以外はリターン
            if (animState != AnimState.E_Idle && animState != AnimState.E_Move) return;

            base.Guard();
            // 対応アニメーションを再生
            anim.SetTrigger("At_Guard");
        }

        public override void HPAttack()
        {
            // HP攻撃中ならリターン
            if (animState == AnimState.E_Attack) return;

            base.HPAttack();

            // 対応アニメーションを再生
            anim.SetTrigger("At_HAttack");
        }

        public override IEnumerator HPHitReaction()
        {
            StartCoroutine(base.HPHitReaction());

            // 怯みアニメーションを再生
            anim.SetTrigger("At_LargeHit");

            // テキストを変更する
            textHP.text = string.Format("{0:0}", parameter.hp);

            yield return null;
        }

        public override void Move()
        {
            // 待機・移動中以外はリターン
            if (animState != AnimState.E_Idle && animState != AnimState.E_Move) return;

            base.Move();

            // 移動速度を取得 
            float spd = parameter.speed;//Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : normalSpeed;

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

        public override void EndAnim()
        {
            base.EndAnim();
        }

        /// <summary>
        /// 自分のブレイブ攻撃が当たった時の関数
        /// </summary>
        /// <param name="obtainBrave">取得したブレイブ</param>
        public override void HitBraveAttack(float obtainBrave)
        {
            base.HitBraveAttack(obtainBrave);

            // テキストを変更する
            textBrave.text = string.Format("{0:0}", parameter.brave);
        }

        /// <summary>
        /// 自分のHP攻撃が当たった時の関数
        /// </summary>
        public override void HitHPAttack()
        {
            base.HitHPAttack();

            // テキストを変更する
            textBrave.text = string.Format("{0:0}", parameter.brave);
        }

        /// <summary>
        /// ブレイブを徐々に回復する関数
        /// </summary>
        public override void RegenerateBrave()
        {
            base.RegenerateBrave();

            // テキストを変更する
            textBrave.text = string.Format("{0:0}", parameter.brave);
        }

        /// <summary>
        /// ノックバック関数
        /// </summary>
        public override void BiginKnockBack()
        {
            if (animState != AnimState.E_HitReaction) return;
            con.Move(-transform.forward * knockBackDistance * Time.deltaTime);
        }

        /// <summary>
        /// ノックバック終了関数
        /// </summary>
        public override void EndKnockBack()
        {
            base.EndAnim();
            knockBackDistance = 0; // ノックバック距離を0にする
        }

        /// <summary>
        /// 防御を受けた際のリアクション関数
        /// </summary>
        public override void GuardReaction()
        {
            base.GuardReaction();

            // ランダムに小怯みモーションを発生させる
            int rnd = Random.Range(0, smallHitClip.Length);
            SmallHitReaction(rnd);
        }

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///


        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///

        protected override void Start()
        {
            base.Start();

            // コンポーネントを取得
            con ??= GetComponent<CharacterController>();

            if (!isEnemy)
            {
                // マウスを固定する気はない
                // マウスカーソルを非表示にし、位置を固定
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                key ??= Keyboard.current; // 現在のキーボード情報を取得

                playerInputs = new PlayerInputs(); // Actionスクリプトのインスタンス生成

                // Actionイベント登録
                playerInputs.Player.Move.started += OnMove;
                playerInputs.Player.Move.performed += OnMove;
                playerInputs.Player.Move.canceled += OnMove;
                playerInputs.Player.BAttack.started += OnBAttack;
                playerInputs.Player.HAttack.started += OnHAttack;
                playerInputs.Player.Guard.started += OnGuard;
                playerInputs.Player.Dodge.started += OnDodge;

                // playerInputsを起動
                playerInputs.Enable();

                startPos = transform.position; // 初期位置を取得
                cameraOffset = plCamera.transform.localPosition - transform.localPosition; // プレイヤーとカメラの距離を取得
            }
        }

        private void Update()
        {
            if (!isEnemy)
            {
                // キーボードチェック
                CheckKeyBoard();

                // 移動関数
                Move();

                // カメラを追従させる
                TrackingCamera();
            }

            // ノックバック処理を行う
            BiginKnockBack();

            // リジェネ処理を行う
            RegenerateBrave();
        }

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
            // オブジェクトが非アクティブになった時にplayerInputsを停止
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
        /// カメラを追従させる関数
        /// </summary>
        private void TrackingCamera()
        {
            plCamera.transform.localPosition = new Vector3(transform.position.x, 0, transform.position.z) + cameraOffset;
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            // エネミーが移動中にぶつかるとエネミーの上にのってしまうので
            // 乗らないようにプレイヤーを後ろに下げる
            if (hit.collider.tag == "Enemy")
            {
                // 衝突時にプレイヤーを敵から押し戻す
                Vector3 pushBack = hit.normal * 0.01f; // 0.01fは距離、必要に応じて調整
                CharacterController controller = GetComponent<CharacterController>();
                controller.Move(pushBack);
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



        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///



        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///

        [SerializeField] private bool isEnemy = false;

        private float gravity = 10f; // 重力

        private Vector2 moveInputValue; // 入力した値

        private Vector3 moveDirection = Vector3.zero; // 移動した位置
        private Vector3 cameraOffset = Vector3.zero; // カメラとプレイヤーの差
        private Vector3 startPos; // 初期位置

        [Header("小怯みアニメーション")]
        [SerializeField] private AnimationClip[] smallHitClip = new AnimationClip[3];

        [SerializeField] private GameObject plCamera; // PLのカメラ

        private CharacterController con; // CharacterController変数

        private Keyboard key; // Keyboard変数

        private PlayerInputs playerInputs; // InputSystemから生成したスクリプト

        // HPとBrave値の表示テキスト
        [SerializeField] private TextMeshProUGUI textHP;
        [SerializeField] private TextMeshProUGUI textBrave;

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}