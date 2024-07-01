using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Unity.VisualScripting;
using UnityEditor;
using TMPro;

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

            // 武器のステートを変更
            attackScript.SetAttackState = AttackState.E_BraveAttack;

            // startIdleをfalseにして攻撃アクションが終了後Move()関数を動かすようにする
            if (startIdle) startIdle = false;

            // 対応アニメーションを再生
            anim.SetTrigger("At_BAttack");

            // ヒットしたら相手のAddBraveDamegeを呼び出す
            // 

        }

        public override void BraveHitReaction()
        {
            base.BraveHitReaction();

            // アニメーション状態を被ダメージ中にする
            animState = AnimState.E_HitReaction;

            // ランダムに決めた小怯みアニメーションを再生
            int rnd = Random.Range(0, smallHitClip.Length);
            AllocateMotion("SmallHitReaction", smallHitClip[rnd]);
            anim.SetTrigger("At_SmallHit");

        }

        public override void Dead()
        {
            base.Dead();

            // 対応アニメーションを再生
            anim.SetTrigger("At_Death");

            // ゲームオーバーにする

        }

        public override void Dodge()
        {
            base.Dodge();
            // 回避中は無敵

            // 対応アニメーションを再生
            anim.SetTrigger("At_Dodge");

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

            // 武器のステートを変更
            attackScript.SetAttackState = AttackState.E_HPAttack;

            // startIdleをfalseにして攻撃アクションが終了後Move()関数を動かすようにする
            if (startIdle) startIdle = false;

            // 対応アニメーションを再生
            anim.SetTrigger("At_HAttack");
            // ヒットしたら相手のAddHPDamegeを呼び出す
        }

        public override void HPHitReaction()
        {
            base.HPHitReaction();

            // アニメーション状態を攻撃中にする
            animState = AnimState.E_HitReaction;

            // ランダムに決めた小怯みアニメーションを再生
            anim.SetTrigger("At_LargeHit");
        }

        public override void Move()
        {
            base.Move();
            // 攻撃中・戦闘不能中はリターン
            if (animState == AnimState.E_Attack || animState == AnimState.E_Dead) return;

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

        /// <summary>
        /// アニメーション終了時の関数
        /// </summary>
        public void AnimEnd()
        {
            attackScript.SetAttackState = AttackState.E_None;
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
            con ??= GetComponent<CharacterController>();
            anim ??= GetComponent<Animator>();

            // マウスを固定する気はない
            // マウスカーソルを非表示にし、位置を固定
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            key ??= Keyboard.current; // 現在のキーボード情報を取得

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
            overrideController = new AnimatorOverrideController(anim.runtimeAnimatorController); // インスタンス生成 上書きしたいAnimatorを代入
            // anim.runtimeAnimatorController = overrideController; Animatorを上書き
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
        /// 待機アニメーション時の向きを矯正する関数
        /// </summary>
        private void StraighteningDirection()
        {
            if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle" && !startIdle)
            {
                // 待機アニメーションに遷移したときの向き矯正
                //transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y + reviseIdleDir, 0);
                startIdle = true;
                animState = AnimState.E_Idle;
            }
            else if (animState == AnimState.E_Attack && startIdle)
            {
                // 待機アニメーションから別のアニメーションに遷移した時の向き矯正
                //transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y - reviseIdleDir, 0);
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
            AnimatorStateInfo[] layerInfo = new AnimatorStateInfo[anim.layerCount];
            for (int i = 0; i < anim.layerCount; i++)
            {
                layerInfo[i] = anim.GetCurrentAnimatorStateInfo(i);
            }

            // (3) AnimationClipを差し替えて、強制的にアップデート
            // ステートがリセットされる
            overrideController[name] = clip;
            anim.Update(0.0f);

            // ステートを戻す
            for (int i = 0; i < anim.layerCount; i++)
            {
                anim.Play(layerInfo[i].fullPathHash, i, layerInfo[i].normalizedTime);
            }
        }

        //　キャラクターが他のゲームオブジェクトにぶつかったら
        public void OnControllerColliderHit(ControllerColliderHit col)
        {
            //　ぶつかった相手がEnWeponタグを持つ相手だったら
            if (col.gameObject.tag == Tags.EnemyWepon.ToString())
            {
                /*
                //　ぶつかった相手のRigidbodyコンポーネントを取得
                Rigidbody rigid = col.transform.GetComponent<Rigidbody>();
                //　Rigidbodyを持っていなかったらAddComponentで取り付ける
                if (rigid == null)
                {
                    //　まったく関係ないが自作のスクリプトを取り付けてみる
                    col.gameObject.AddComponent<AddComponentTest>();
                    //　ジェネリクス版
                    rigid = col.gameObject.AddComponent<Rigidbody>() as Rigidbody;
                    //　文字列で指定版
                    //				rigid = col.gameObject.AddComponent ("Rigidbody") as Rigidbody;
                    //　typeof使用版
                    //				rigid = col.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
                    //　constraintsのX、Y、Z軸で移動し、回転しないようにする
                    rigid.constraints = ~RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                }
                //　Rigidbodyに力を加える
                rigid.AddForce(transform.forward * power);
                */
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
        private bool isEnemy = false;

        // 初期パラメータ
        [SerializeField] private float hp = 1000;
        [SerializeField] private float brave = 100;
        [SerializeField] private float speed = 10;
        [SerializeField] private float attack = 10;

        private float gravity = 10f; // 重力

        [SerializeField]private string[] overrideClips;// = { "SmallHitReaction" }; // 差し替えたいアニメーションクリップ名

        private Vector2 moveInputValue; // 入力した値

        private Vector3 moveDirection = Vector3.zero; // 移動した位置
        private Vector3 cameraOffset = Vector3.zero; // カメラとプレイヤーの差
        private Vector3 startPos; // 初期位置

        private AnimState animState; // アニメーションの状態変数

        [Header("小怯みアニメーション")]
        [SerializeField] private AnimationClip[] smallHitClip = new AnimationClip[3];

        private AnimatorOverrideController overrideController; // Animator上書き用変数

        [SerializeField] private GameObject plCamera; // PLのカメラ

        private CharacterController con; // CharacterController変数

        private Keyboard key; // Keyboard変数

        private PlayerInputs playerInputs; // InputSystemから生成したスクリプト

        // HPとBrave値の表示テキスト
        [SerializeField] private TextMeshProUGUI textHP;
        [SerializeField] private TextMeshProUGUI textBrave;

        [SerializeField] private AttackScript attackScript; // 自身の武器の攻撃スクリプト

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}