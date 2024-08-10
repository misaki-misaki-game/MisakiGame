using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Unity.VisualScripting;
using TMPro;

namespace Misaki
{
    public partial class PlayerScript : BaseCharactorScript
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        public override void BraveAttack()
        {
            base.BraveAttack();

            // ブレイブ攻撃中ならリターン
            if (animState == AnimState.E_Attack)
            {
                anim.SetTrigger("At_BAttack");
            }
            else if (animState != AnimState.E_Idle && animState != AnimState.E_Move) return;

            // アニメーション状態をブレイブ攻撃中にする
            animState = AnimState.E_Attack;

            // 攻撃の所有者を自分にする
            attackScript.SetOwnOwner = this;

            // 対応アニメーションを再生
            anim.SetTrigger("At_BAttack");
        }

        public override IEnumerator BraveHitReaction()
        {
            StartCoroutine(base.BraveHitReaction());

            // ランダムに決めた小怯みアニメーションを再生
            int rnd = Random.Range(0, smallHitClip.Length);
            AllocateMotion("SmallHit01", smallHitClip[rnd]);
            anim.SetTrigger("At_SmallHit");

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
            base.Dodge();

        }

        public override void Guard()
        {
            base.Guard();
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
            // 攻撃中・戦闘不能中はリターン
            if (animState == AnimState.E_Attack || animState == AnimState.E_Dead) return;

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
            anim.ResetTrigger("At_BAttack"); // ブレイブ攻撃の入力状況保持を消す
            anim.ResetTrigger("At_HAttack"); // HP攻撃の入力状況保持を消す
            anim.SetTrigger("At_Idle"); // 待機状態に移動する
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
            // ノックバックしていなければリターン
            //if (!isKnockBack) return;
            if (animState != AnimState.E_HitReaction) return;

            con.Move(-transform.forward * knockBackDistance * Time.deltaTime);

        }

        public override void EndKnockBack()
        {
            base.EndAnim();
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
            // コンストラクタを呼び出し
            parameter = new Parameter(hp, brave, regenerateSpeed, breakSpeed, speed, attack);

            // コンポーネントを取得
            con ??= GetComponent<CharacterController>();
            anim ??= GetComponent<Animator>();
            animState = default; // アニメーション状態をなにもしていないに変更

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

                // playerInputsを起動
                playerInputs.Enable();

                startPos = transform.position; // 初期位置を取得
                cameraOffset = plCamera.transform.localPosition - transform.localPosition; // プレイヤーとカメラの距離を取得
            }

            overrideController = new AnimatorOverrideController(anim.runtimeAnimatorController); // インスタンス生成 上書きしたいAnimatorを代入
            anim.runtimeAnimatorController = overrideController; //Animatorを上書き
            overrideClips = new string[overrideController.animationClips.Length]; // 要素数を代入

            // クリップ配列に名前を代入
            for (int i = 0; i < overrideClips.Length; i++)
            {
                overrideClips[i] = overrideController.animationClips[i].name;
            }

            Random.InitState(System.DateTime.Now.Millisecond); // シード値を設定(日付データ)
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

        /// <summary>
        /// 指定のアニメーションクリップを差し替える関数
        /// </summary>
        /// <param name="name">アニメーションクリップの名称</param>
        /// <param name="clip">差し替えたいクリップ</param>
        private void AllocateMotion(string name, AnimationClip clip)
        {
            // アニメーションステートを取得
            AnimatorStateInfo[] layerInfo = new AnimatorStateInfo[anim.layerCount];
            for (int i = 0; i < anim.layerCount; i++)
            {
                layerInfo[i] = anim.GetCurrentAnimatorStateInfo(i);
            }

            // AnimationClipを差し替えて、強制的にアップデート
            // ステートがリセットされる
            overrideController[name] = clip;
            anim.Rebind();

            // ステートを戻す
            for (int i = 0; i < anim.layerCount; i++)
            {
                anim.Play(layerInfo[i].fullPathHash, i, layerInfo[i].normalizedTime);
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

        // 初期パラメータ
        [SerializeField] private float hp = 1000;
        [SerializeField] private float brave = 100;
        [SerializeField] private float regenerateSpeed = 3;
        [SerializeField] private float breakSpeed = 10;
        [SerializeField] private float speed = 10;
        [SerializeField] private float attack = 100;

        private string[] overrideClips; // 差し替えたいアニメーションクリップ名

        private Vector2 moveInputValue; // 入力した値

        private Vector3 moveDirection = Vector3.zero; // 移動した位置
        private Vector3 cameraOffset = Vector3.zero; // カメラとプレイヤーの差
        private Vector3 startPos; // 初期位置

        [Header("小怯みアニメーション")]
        [SerializeField] private AnimationClip[] smallHitClip = new AnimationClip[3];

        [SerializeField] private AnimatorOverrideController overrideController; // Animator上書き用変数

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