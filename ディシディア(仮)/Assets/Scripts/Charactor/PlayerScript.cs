using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Unity.VisualScripting;
using UnityEditor;

namespace Misaki
{
    public partial class PlayerScript : BaseCharactorScript
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        public override void AddBraveDamage(float damage)
        {
            base.AddBraveDamage(damage);
            // Braveからdamage分を引く
            parameter.brave = parameter.brave - damage;
            BraveHitReaction();
        }

        public override void AddHPDamage(float brave)
        {
            base.AddHPDamage(brave);
            // HPからdamageを引く
            parameter.hp = parameter.hp - brave;
            HPHitReaction();
        }

        public override void Born()
        {
            base.Born();
            //anim
        }

        public override void BraveAttack()
        {
            base.BraveAttack();

            // 攻撃中ならリターン
            if (animState == AnimState.E_Attack) return;

            // アニメーション状態を攻撃中にする
            animState = AnimState.E_Attack;

            // 対応アニメーションを再生
            anim.SetTrigger("At_BAttack");

            // startIdleをfalseにして攻撃アクションが終了後Move()関数を動かすようにする
            if (startIdle) startIdle = false;

            // ヒットしたら相手のAddBraveDamegeを呼び出す
            // 

        }

        public override void BraveHitReaction()
        {
            base.BraveHitReaction();
            // anim
        }

        public override void Dead()
        {
            base.Dead();
            // ゲームオーバーにする
        }

        public override void Dodge()
        {
            base.Dodge();
            // anim
            // 回避中は無敵
        }

        public override void Guard()
        {
            base.Guard();
            // anim
            // 防御中はダメージ軽減か無敵
        }

        public override void HPAttack()
        {
            base.HPAttack();

            // 攻撃中ならリターン
            if (animState == AnimState.E_Attack) return;

            // アニメーション状態を攻撃中にする
            animState = AnimState.E_Attack;

            // アニメーション状態が待機中だった場合、向きを矯正する
            StraighteningDirection();

            // startIdleをfalseにして攻撃アクションが終了後Move()関数を動かすようにする
            if (startIdle) startIdle = false;

            // 対応アニメーションを再生
            anim.SetTrigger("At_HAttack");
            // ヒットしたら相手のAddHPDamegeを呼び出す
        }

        public override void HPHitReaction()
        {
            base.HPHitReaction();
            // anim　再生中は無敵

        }

        public override void Move()
        {
            base.Move();
            // 攻撃中はリターン
            if (animState == AnimState.E_Attack) return;

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

            // Idleアニメーション時はrotation.yの向きを矯正し
            // 各アニメーションに合わせて状態を変更する
            StraighteningDirection();
            if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "RunF")
            {
                if (startIdle) startIdle = false;
                if (animState != AnimState.E_Attack) animState = AnimState.E_Move;
            }

            // Move は指定したベクトルだけ移動させる命令
            con.Move(moveDirection * Time.deltaTime);
        }

        /// <summary>
        /// 待機アニメーション時の向きを矯正する関数
        /// </summary>
        private void StraighteningDirection()
        {
            if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle" && !startIdle)
            {
                // 待機アニメーションに遷移したときの向き矯正
                transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y + 49.436f, 0);
                startIdle = true;
                animState = AnimState.E_Idle;
            }
            else if (animState == AnimState.E_Attack && startIdle)
            {
                // 待機アニメーションから別のアニメーションに遷移した時の向き矯正
                transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y - 49.436f, 0);
            }
        }

        /// <summary>
        /// カメラを追従させる関数
        /// </summary>
        private void TrackingCamera()
        {
            plCamera.transform.localPosition = new Vector3(transform.position.x, 0, transform.position.z) + cameraOffset;
        }

        // アニメーションが終わった際にアニメーション状態をなにもしていないに変更
        public void AnimEnd()
        {
            animState = default;
            anim.SetTrigger("At_Idle");
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
            parameter = new Parameter(hp, brave, speed, attack);

            // コンポーネントを取得
            con = GetComponent<CharacterController>();
            anim = GetComponent<Animator>();

            // マウスを固定する気はない
            // マウスカーソルを非表示にし、位置を固定
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            key = Keyboard.current; // 現在のキーボード情報を取得

            // Actionスクリプトのインスタンス生成
            playerInputs = new PlayerInputs();

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
            animState = default; // アニメーション状態をなにもしていないに変更
        }

        private void Update()
        {
            // キーボードチェック
            CheckKeyBoard();

            // 移動関数
            Move();

            // カメラを追従させる
            TrackingCamera();
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

        private bool startIdle = false; // 待機アニメーションがスタートしているか

        // 初期パラメータ
        [SerializeField] float hp = 1000;
        [SerializeField] float brave = 100;
        [SerializeField] float speed = 10;
        [SerializeField] float attack = 10;

        private float gravity = 10f; // 重力

        private Vector2 moveInputValue; // 入力した値

        private Vector3 moveDirection = Vector3.zero; // 移動した位置
        private Vector3 cameraOffset = Vector3.zero; // カメラとプレイヤーの差
        private Vector3 startPos; // 初期位置

        [SerializeField] private GameObject plCamera; // PLのカメラ

        private CharacterController con; // CharacterController変数

        private Keyboard key; // Keyboard変数

        private PlayerInputs playerInputs; // InputSystemから生成したスクリプト

        private AnimState animState; // アニメーションの状態変数

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}