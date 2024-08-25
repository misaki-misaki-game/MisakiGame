using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.AI;

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
            base.BraveAttack();

            // ブレイブ攻撃中ならコンボを出す
            // 待機・移動中以外ならリターン
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

            // 自身とターゲットの距離を取得
            float distance = Vector3.Distance(transform.position, target.position);

            // 距離が設定した停止距離より大きいなら
            if (distance > agent.stoppingDistance)
            {
                base.Move(); // 移動中に変更

                // 行先を設定し、そこに移動
                agent.SetDestination(target.position); // 行先をターゲットのポジションに設定
                float speed = agent.velocity.magnitude; // 速度ベクトルの長さ(速度)を取得
                anim.SetFloat("Af_Running", speed); // 移動のアニメーション開始
            }
            // 距離が設定した停止距離以下なら
            else
            {
                animState = AnimState.E_Idle; // 待機中に変更

                agent.ResetPath(); // 経路を削除して移動しないようにする
                anim.SetFloat("Af_Running", 0f); // 待機のアニメーション開始

            }
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
            if (animState != AnimState.E_HitReaction) return;
            rigid.AddForce(-transform.forward * knockBackDistance * Time.deltaTime);

        }

        public override void EndKnockBack()
        {
            base.EndAnim();
            knockBackDistance = 0;
        }

        /// <summary>
        /// 防御を受けた際のリアクション関数
        /// </summary>
        public override void GuardReaction()
        {
            base.GuardReaction();
            SmallHitReaction(0);
        }

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///

        protected virtual void Start()
        {
            // コンストラクタを呼び出し
            parameter = new Parameter(hp, brave, regenerateSpeed, breakSpeed, speed, attack);

            // コンポーネントを取得
            anim ??= GetComponent<Animator>();
            agent ??= GetComponent<NavMeshAgent>(); // ナビメッシュエージェントを取得
            rigid ??= GetComponent<Rigidbody>(); // リギッドボディ
            col ??=GetComponent<CapsuleCollider>() ; // コライダー

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
                playerInputs.Player.Guard.started += OnGuard;
                playerInputs.Player.Dodge.started += OnDodge;

                // playerInputsを起動
                playerInputs.Enable();

                startPos = transform.position; // 初期位置を取得
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

        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///

        private void Update()
        {
            if (!isEnemy)
            {
                // キーボードチェック
                CheckKeyBoard();
            }
            // 移動関数
            Move();

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

        /// <summary>
        /// 小怯みモーションを再生する関数
        /// </summary>
        /// <param name="rnd">指定の小怯みモーション</param>
        private void SmallHitReaction(int rnd)
        {
            AllocateMotion("SmallHit01", smallHitClip[rnd]);
            anim.SetTrigger("At_SmallHit");
        }


        private void LateUpdate()
        {
            ui.transform.rotation = Camera.main.transform.rotation;
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



        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///

        [SerializeField] protected bool isEnemy = true;

        protected float gravity = 10f; // 重力

        Rigidbody rigid; // リギッドボディ
        CapsuleCollider col; // コライダー

        // 初期パラメータ
        [SerializeField] protected float hp = 1000;
        [SerializeField] protected float brave = 100;
        [SerializeField] protected float regenerateSpeed = 3;
        [SerializeField] protected float breakSpeed = 10;
        [SerializeField] protected float speed = 10;
        [SerializeField] protected float attack = 100;

        protected string[] overrideClips; // 差し替えたいアニメーションクリップ名

        protected Vector2 moveInputValue; // 入力した値

        protected Vector3 moveDirection = Vector3.zero; // 移動した位置
        protected Vector3 cameraOffset = Vector3.zero; // カメラとプレイヤーの差
        protected Vector3 startPos; // 初期位置

        [SerializeField] protected Transform target; // ターゲット

        protected NavMeshAgent agent; // ナビゲーションエージェント変数

        [Header("小怯みアニメーション")]
        [SerializeField] protected AnimationClip[] smallHitClip = new AnimationClip[3];

        [SerializeField] protected AnimatorOverrideController overrideController; // Animator上書き用変数

        protected Keyboard key; // Keyboard変数

        protected PlayerInputs playerInputs; // InputSystemから生成したスクリプト

        [SerializeField] protected GameObject ui; // エネミーのUI

        // HPとBrave値の表示テキスト
        [SerializeField] protected TextMeshProUGUI textHP;
        [SerializeField] protected TextMeshProUGUI textBrave;

        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///



        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}