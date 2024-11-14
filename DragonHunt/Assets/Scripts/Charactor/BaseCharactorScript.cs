using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Misaki
{
    // 自動的にコンポーネントを追加 AudioSource,TrailController,DamageUIScriptを追加
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(TrailController))]
    [RequireComponent(typeof(DamageUIScript))]
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
        /// <returns>ブレイク状態になったかどうか</returns>
        public bool ReceiveBraveDamage(float damage, Vector3 direction, bool isCritical)
        {
            // 無敵時間中または戦闘不能中ならfalseをリターン
            if (damageState == DamageState.E_Invincible || AnimState == AnimState.E_Dead) return false;

            // 被攻撃SEを鳴らす
            SoundManager.SoundPlay(GetComponent<AudioSource>(), SEList.E_GetHitSE);

            // キャラクターの向きを指定の向きに変え、エフェクトを生成するポジションを設定
            if (damageState != DamageState.E_SuperArmor)
            {
                transform.LookAt(transform.position + direction);
            }

            // Braveからdamage分を引く
            if (braveState != BraveState.E_Break)
            {
                parameter.brave = parameter.brave - damage;
            }

            // ダメージを表示する
            StartCoroutine(damageUI.PopDamageUI(damage, isCritical));

            StartCoroutine(BraveHitReaction());

            return IsBreak();
        }

        /// <summary>
        /// HP値へのダメージを受け取る関数
        /// </summary>
        /// <param name="brave">HPダメージ値</param>
        public virtual void ReceiveHPDamage(float brave, Vector3 direction)
        {
            // 無敵時間中または戦闘不能中ならリターン
            if (damageState == DamageState.E_Invincible || AnimState == AnimState.E_Dead) return;

            // 被攻撃SEを鳴らす
            SoundManager.SoundPlay(GetComponent<AudioSource>(), SEList.E_GetHitSE);

            // キャラクターの向きを指定の向きに変え、エフェクトを生成するポジションを設定
            transform.LookAt(transform.position + direction);

            // HPからdamageを引く
            parameter.hp = parameter.hp - brave;
            FluctuationHPBar();
            StartCoroutine(HPHitReaction());
        }

        /// <summary>
        /// 生まれる関数
        /// </summary>
        public virtual void Born()
        {
            
        }

        /// <summary>
        /// ブレイブ攻撃する関数
        /// </summary>
        public virtual void BraveAttack()
        {
            // アニメーション状態をブレイブ攻撃中にする
            AnimState = AnimState.E_Attack;

            // 攻撃の所有者を自分にする
            for (int i = 0; i < attackScripts.Count; i++)
            {
                attackScripts[i].SetOwnOwner = this;
            }

            // 対応アニメーションを再生
            anim.SetTrigger("At_BAttack");
        }

        /// <summary>
        /// ブレイブ攻撃を受けた際のリアクション関数
        /// </summary>
        public virtual IEnumerator BraveHitReaction()
        {
            // スーパーアーマーでなければ、アニメーション状態を被ダメージ中にする
            if (damageState != DamageState.E_SuperArmor)
            {
                AnimState = AnimState.E_HitReaction;
            }

            // ノックバック距離を代入
            knockBackDistance = 1f;

            // ヒットした位置を特定するまで待ってからエフェクトを生成する
            yield return new WaitUntil(() => ishit);
            GenerateEffect(EffectName.braveDamageEffect);
            ishit = false;

            // ヒットストップさせる
            hitStopManager.StartHitStop(anim, 0.1f, true);

            // テキストを変更する
            textBrave.text = string.Format("{0:0}", parameter.brave);

            // リジェネ中なら通常状態にしてリジェネを止める
            if (BraveState == BraveState.E_Regenerate)
            {
                BraveState = BraveState.E_Default;
            }
        }

        /// <summary>
        /// 死ぬ関数
        /// </summary>
        public virtual void Dead()
        {
            // アニメーション状態を戦闘不能にする
            AnimState = AnimState.E_Dead;

            // アニメーターのスピードを少しゆっくりして
            // 対応アニメーションを再生
            anim.speed = 0.9f;
            anim.SetTrigger("At_Dead");
        }

        /// <summary>
        /// 回避関数
        /// </summary>
        public virtual void Dodge()
        {
            AnimState = AnimState.E_Dodge;

            // 対応アニメーションを再生
            anim.SetTrigger("At_Dodge");
        }

        /// <summary>
        /// 防御関数
        /// </summary>
        public virtual void Guard()
        {
            AnimState = AnimState.E_Guard;

            // 対応アニメーションを再生
            anim.SetTrigger("At_Guard");
        }

        /// <summary>
        /// HP攻撃をする関数
        /// </summary>
        public virtual void HPAttack()
        {
            // アニメーション状態をHP攻撃中にする
            AnimState = AnimState.E_Attack;

            // 攻撃の所有者を自分にする
            for (int i = 0; i < attackScripts.Count; i++)
            {
                attackScripts[i].SetOwnOwner = this;
            }

            // 対応アニメーションを再生
            anim.SetTrigger("At_HAttack");
        }

        /// <summary>
        /// HP攻撃を受けた際のリアクション関数
        /// </summary>
        public virtual IEnumerator HPHitReaction()
        {
            // アニメーション状態を被ダメージ中にする
            AnimState = AnimState.E_HitReaction;

            // ノックバック距離を代入
            knockBackDistance = 3f;

            // ヒットした位置を特定するまで待ってからエフェクトを生成する
            yield return new WaitUntil(() => ishit);
            GenerateEffect(EffectName.hpDamageEffect);
            ishit = false;

            if (parameter.hp > 0)
            {
                // 怯みアニメーションを再生
                anim.SetTrigger("At_LargeHit");
            }
            else
            {
                // 戦闘不能にする
                parameter.hp = 0;
                Dead();
            }

            // ヒットストップさせる
            hitStopManager.StartHitStop(anim, 0.2f, true);

            // テキストを変更する
            textHP.text = string.Format("{0:0} / {1:0}", parameter.hp, parameter.maxHp);
        }

        /// <summary>
        /// 移動関数
        /// </summary>
        public virtual void Move()
        {
            // アニメーション状態を移動中にする
            AnimState = AnimState.E_Move;
        }

        /// <summary>
        /// ブレイブ攻撃開始時の関数
        /// 自身の武器で攻撃する場合
        /// </summary>
        /// <param name="motionValue">攻撃モーション値</param>
        public void BeginBraveAttack(float motionValue)
        {
            // クリティカルかどうかを判定
            bool isCritical = IsCriticalHit();

            // 武器のステートとブレイブ攻撃値を変更し、ヒットオブジェクトリストをリセットする
            for (int i = 0; i < attackScripts.Count; i++)
            {
                attackScripts[i].SetAttackState = AttackState.E_BraveAttack;
                attackScripts[i].ClearHitObj();
                attackScripts[i].SetBraveAttack = CalculateDamage(motionValue, isCritical);
                attackScripts[i].SetCritical = isCritical;
            }
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

            // クリティカルかどうかを判定
            bool isCritical = IsCriticalHit();

            // 弾のステートとブレイブ攻撃値を変更し、ヒットオブジェクトリストをリセットする
            bulletAttackScript.SetAttackState = AttackState.E_BraveAttack;
            bulletAttackScript.ClearHitObj();
            bulletAttackScript.SetBraveAttack = CalculateDamage(motionValue, isCritical);
            bulletAttackScript.SetCritical = isCritical;
        }

        /// <summary>
        /// HP攻撃開始時の関数
        /// 自身の武器で攻撃する場合
        /// </summary>
        public void BeginHPAttack()
        {
            // 武器のステートとHP攻撃値を変更し、ヒットオブジェクトリストをリセットする
            for (int i = 0; i < attackScripts.Count; i++)
            {
                attackScripts[i].SetAttackState = AttackState.E_HPAttack;

                // ブレイク状態の場合のみ0ダメージにする
                if (BraveState != BraveState.E_Break)
                {
                    attackScripts[i].SetHPAttack = parameter.brave;
                }
                else
                {
                    attackScripts[i].SetHPAttack = 0;
                }

                attackScripts[i].ClearHitObj();
            }
        }

        /// <summary>
        /// HP攻撃開始時の関数
        /// 遠距離で攻撃する場合
        /// </summary>
        /// <param name="effectName">エフェクト名</param>
        public void BeginHPBullet(EffectName effectName)
        {
            // エフェクトを生成し、そのアタックスクリプトを取得
            effectPos = new Vector3(transform.position.x, adjustEffectYPos, transform.position.z);
            GameObject obj = GenerateEffect(effectName);
            bulletAttackScript = obj.GetComponentInChildren<AttackScript>();

            // 武器のステートとHP攻撃値を変更し、ヒットオブジェクトリストをリセットする
            // アタックスクリプトの所有者を自分にする
            bulletAttackScript.SetOwnOwner = this;
            bulletAttackScript.SetAttackState = AttackState.E_HPAttack;

            // ブレイク状態の場合のみ0ダメージにする
            if (BraveState != BraveState.E_Break)
            {
                bulletAttackScript.SetHPAttack = parameter.brave;
            }
            else
            {
                bulletAttackScript.SetHPAttack = 0;
            }

            bulletAttackScript.ClearHitObj();
        }

        /// <summary>
        /// HP攻撃開始時の関数
        /// 遠距離で攻撃する場合
        /// </summary>
        /// <param name="effectName">エフェクト名</param>
        /// <param name="effectPos">エフェクト生成場所</param>
        public void BeginHPBullet(EffectName effectName, GameObject effectPos)
        {
            // エフェクトを生成し、そのアタックスクリプトを取得
            GameObject obj = GenerateEffect(effectName, effectPos);
            bulletAttackScript = obj.GetComponentInChildren<AttackScript>();

            // 武器のステートとHP攻撃値を変更し、ヒットオブジェクトリストをリセットする
            // アタックスクリプトの所有者を自分にする
            bulletAttackScript.SetOwnOwner = this;
            bulletAttackScript.SetAttackState = AttackState.E_HPAttack;

            // ブレイク状態の場合のみ0ダメージにする
            if (BraveState != BraveState.E_Break)
            {
                bulletAttackScript.SetHPAttack = parameter.brave;
            }
            else
            {
                bulletAttackScript.SetHPAttack = 0;
            }

            bulletAttackScript.ClearHitObj();
        }

        /// <summary>
        /// HP攻撃開始時の関数
        /// 遠距離で特定の位置に攻撃する場合
        /// </summary>
        /// <param name="effectName">エフェクト名</param>
        public virtual void BeginHPSearch(EffectName effectName)
        {
        }

        /// <summary>
        /// 攻撃終了時の関数
        /// </summary>
        public void EndAttack()
        {
            // 武器のステートを変更し、ヒットオブジェクトリストをリセットする
            for (int i = 0; i < attackScripts.Count; i++)
            {
                attackScripts[i].SetAttackState = AttackState.E_None;
                attackScripts[i].ClearHitObj();
            }
        }

        /// <summary>
        /// アニメーション終了時の関数
        /// </summary>
        public virtual void EndAnim()
        {
            AnimState = AnimState.E_Idle; // 待機中に変更
            anim.ResetTrigger("At_BAttack"); // ブレイブ攻撃の入力状況保持を消す
            anim.ResetTrigger("At_HAttack"); // HP攻撃の入力状況保持を消す
            anim.SetTrigger("At_Idle"); // 待機状態に移動する
        }

        /// <summary>
        /// 自分のブレイブ攻撃が当たった時の関数
        /// </summary>
        /// <param name="obtainBrave">取得したブレイブ</param>
        /// <param name="braveBreak">相手をブレイクしたかどうか</param>
        public virtual void HitBraveAttack(float obtainBrave, bool braveBreak)
        {
            // ブレイブを加算する
            parameter.brave += obtainBrave;
            // ブレイクした場合はボーナスも加算する
            if (braveBreak)
            {
                parameter.brave += GameManager.GetBreakBonus;
            }

            // ブレイブテキストを変更する
            SetTextBrave();
        }

        /// <summary>
        /// 自分のHP攻撃が当たった時の関数
        /// </summary>
        /// <param name="hpDamage">表示するダメージ量</param>
        public virtual void HitHPAttack(float hpDamage)
        {
            // ブレイブの状態によって処理を変える
            if (BraveState != BraveState.E_Break)
            {
                // ブレイブを0にする
                parameter.brave = 0;

                // ブレイブ状態をリジェネ状態にしてリジェネ開始
                BraveState = BraveState.E_Regenerate;
            }
            else
            {
                // ブレイブを基準値にして、リジェネを停止
                parameter.brave = parameter.standardBrave;
            }

            // ブレイブテキストを変更する
            SetTextBrave();
        }

        /// <summary>
        /// ブレイブを徐々に回復する関数
        /// </summary>
        public void RegenerateBrave()
        {
            // 通常状態の場合はリターン
            // リジェネ状態の場合はregenerateSpeed秒掛かけ回復
            // ブレイク状態の場合はbrakSpeed秒掛けて回復
            if (BraveState == BraveState.E_Default || animState == AnimState.E_Dead) return;
            else if (BraveState == BraveState.E_Regenerate)
            {
                parameter.brave += parameter.standardBrave / parameter.regenerateSpeed * Time.deltaTime;
            }
            else
            {
                parameter.brave += parameter.standardBrave / parameter.breakSpeed * Time.deltaTime;
            }

            // ブレイブ値がブレイブ基準値以上まで回復したら回復を止める
            if (parameter.brave >= parameter.standardBrave)
            {
                // リジェネ中にブレイクボーナスを取得出来た際には下記の処理はしない
                if (parameter.brave <= GameManager.GetBreakBonus - parameter.standardBrave)
                {
                    parameter.brave = parameter.standardBrave;
                }

                BraveState = BraveState.E_Default;
                textBreak.gameObject.SetActive(false);
            }

            // ブレイブテキストを変更する
            SetTextBrave();
        }

        /// <summary>
        /// ノックバック開始関数
        /// </summary>
        public virtual void BeginKnockBack()
        {
        }

        /// <summary>
        /// ノックバック終了関数
        /// </summary>
        public void EndKnockBack()
        {
            knockBackDistance = 0; // ノックバック距離を0にする
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
        public void BeginInvincible()
        {
            damageState = DamageState.E_Invincible;
        }

        /// <summary>
        /// スーパーアーマー開始関数
        /// </summary>
        public void BeginSuperArmor()
        {
            damageState = DamageState.E_SuperArmor;
        }

        /// <summary>
        /// 防御開始関数
        /// </summary>
        public void BeginGuard()
        {
            damageState = DamageState.E_Guard;
        }

        /// <summary>
        /// 回避開始関数
        /// </summary>
        public void BeginDodge()
        {
            damageState = DamageState.E_Dodge;
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
            AnimState = AnimState.E_HitReaction;
        }

        /// <summary>
        /// 防御しているかどうかを返す関数
        /// </summary>
        /// <returns></returns>
        public bool IsGuard()
        {
            if(damageState == DamageState.E_Guard) return true;
            else return false;
        }

        /// <summary>
        /// 回避しているかどうかを返す関数
        /// </summary>
        /// <returns></returns>
        public bool IsDodge()
        {
            if (damageState == DamageState.E_Dodge) return true;
            return false;
        }

        /// <summary>
        /// SEを鳴らす関数
        /// </summary>
        /// <param name="seClip">SE</param>
        public void SEPlay(SEList seClip)
        {
            SoundManager.SoundPlay(GetComponent<AudioSource>(), seClip);
        }

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///

        protected override void Awake()
        {
            base.Awake();
            effectPos = new Vector3(0, adjustEffectYPos, 0);

            gameObject.AddComponent<HitStopManager>();
            hitStopManager = GetComponent<HitStopManager>();
        }

        protected virtual void Start()
        {
            // コンストラクタを呼び出し
            parameter = new Parameter(hp, brave, regenerateSpeed, breakSpeed, speed, attack);

            // コンポーネントを取得
            anim ??= GetComponent<Animator>();
            damageUI ??= GetComponent<DamageUIScript>();

            AnimState = default; // アニメーション状態をなにもしていないに変更

            Random.InitState(System.DateTime.Now.Millisecond); // シード値を設定(日付データ)

            attackScripts = new List<AttackScript>(attackScriptList[0].attackScriptGroup); // アタックスクリプトリストを初期化
        }

        /// <summary>
        /// HPUIと変数を連動させる関数
        /// </summary>
        protected void InitializeHPUI()
        {
            // テキストを変更する
            textHP.text = string.Format("{0:0} / {1:0}", parameter.hp, parameter.maxHp);
            textBrave.text = string.Format("{0:0}", parameter.brave);
        }

        protected virtual void Update()
        {
            // ノックバック処理を行う
            BeginKnockBack();

            // リジェネ処理を行う
            RegenerateBrave();

            // 位置調整
            PushOutCollider();
        }

        protected void OnTriggerEnter(Collider col)
        {
            // 攻撃を受けたことを確認する
            if (col.CompareTag(Tags.EnemyWepon.ToString()) && CompareTag(Tags.Player.ToString()) ||
                col.CompareTag(Tags.PlayerWepon.ToString()) && CompareTag(Tags.Enemy.ToString()))
            {
                CanDamageEffect();
            }
        }

        /// <summary>
        /// 小怯みモーションを再生する関数
        /// </summary>
        /// <param name="rnd">指定の小怯みモーション</param>
        protected void SmallHitReaction(int rnd)
        {
            anim.SetInteger("Ai_SmallHit" , rnd);
            anim.SetTrigger("At_SmallHit");
        }

        /// <summary>
        /// エフェクトを生成する関数
        /// </summary>
        /// <param name="effectName">生成するエフェクト名</param>
        /// <returns>オブジェクト型</returns>
        protected GameObject GenerateEffect(EffectName effectName)
        {
            int num = (int)effectName; // エフェクト名をint型に変更
            PoolManager effect = EffectManager.effectGroups[num].pool; // プールマネージャーを選択
            return effect.GetGameObject(EffectManager.effectGroups[num].effect, PoolType.E_Effect, effectPositions[num].transform.position, 
                effectPositions[num].transform.rotation, effectPositions[num].transform); // プールマネージャーからエフェクトをとりだす
        }

        /// <summary>
        /// エフェクトを生成する関数
        /// </summary>
        /// <param name="effectName">生成するエフェクト名</param>
        /// <param name="effectPos">生成する場所</param>
        protected GameObject GenerateEffect(EffectName effectName, GameObject effectPos)
        {
            int num = (int)effectName; // エフェクト名をint型に変更
            PoolManager effect = EffectManager.effectGroups[num].pool; // プールマネージャーを選択
            return effect.GetGameObject(EffectManager.effectGroups[num].effect, PoolType.E_Effect, effectPos.transform.position, 
                effectPos.transform.rotation, effectPos.transform); // プールマネージャーからエフェクトをとりだす
        }

        /// <summary>
        /// エフェクトを生成する関数(親設定なし)
        /// </summary>
        /// <param name="effectName">生成するエフェクト名</param>
        /// <param name="effectPos">生成する場所</param>
        protected GameObject GenerateEffectNoneParent(EffectName effectName, GameObject effectPos)
        {
            int num = (int)effectName; // エフェクト名をint型に変更
            PoolManager effect = EffectManager.effectGroups[num].pool; // プールマネージャーを選択
            return effect.GetGameObject(EffectManager.effectGroups[num].effect, PoolType.E_Effect, effectPos.transform.position,
                effectPos.transform.rotation); // プールマネージャーからエフェクトをとりだす
        }

        /// <summary>
        /// HPバーを増減する関数
        /// </summary>
        protected void FluctuationHPBar()
        {
            hpBar.fillAmount= parameter.hp / parameter.maxHp;
        }

        /// <summary>
        /// エネミーにめり込まないようにプレイヤーを押し出す関数
        /// </summary>
        protected void PushOutCollider()
        {
            // エネミーコライダー内でないならリターン
            if (!inCollider) return;

            // プレイヤーかどうか、動きを司るコンポーネントとコライダー用配列を生成
            bool isPlayer = false;
            CharacterController charaCon = null;
            Rigidbody rb = null;
            Collider[] colliders;

            // 自身のタグによって代入するものを変更
            if (CompareTag(Tags.Player.ToString()))
            {
                // trueにする
                isPlayer = true;

                // キャラクターコントローラーを取得
                charaCon = GetComponent<CharacterController>();

                // 自身に接触しているコライダーを取得
                colliders = Physics.OverlapSphere(transform.position, charaCon.radius);
            }
            else if (CompareTag(Tags.Enemy.ToString()))
            {
                // falseにする
                isPlayer = false;

                // リギットボディを取得
                rb = GetComponent<Rigidbody>();

                // 自身に接触しているコライダーを取得
                colliders = Physics.OverlapSphere(transform.position, rb.GetComponent<Collider>().bounds.extents.magnitude);
            }
            else return;

            // コライダーの位置を取得し、位置座標の逆方向にプレイヤーを押し出す
            foreach (Collider collider in colliders)
            {
                // プレイヤーまたはエネミータグ以外であればコンティニュー
                if (!collider.CompareTag(Tags.Player.ToString()) && !collider.CompareTag(Tags.Enemy.ToString())) continue;

                // 押し出す方向を取得
                Vector3 direction = (transform.position - collider.transform.position).normalized;
                direction.y = 0;

                // コライダーの外に押し出す
                if (isPlayer && collider.CompareTag(Tags.Enemy.ToString()))
                {
                    // 方向に向かって押し出す
                    charaCon.Move(direction * pushStrength);
                    break; // 一つ見つけたら処理を終了
                }
                else if (!isPlayer && collider.CompareTag(Tags.Player.ToString()))
                {
                    // 方向に向かって力を加える
                    rb.AddForce(direction * pushStrength, ForceMode.Impulse);
                    break; // 一つ見つけたら処理を終了
                }
            }
        }

        /// <summary>
        /// ターゲットの方向へ体を向ける関数
        /// </summary>
        /// <param name="target">ターゲット</param>
        /// <param name="isSmoothRotation">スムーズに回転させるかどうか</param>
        protected void FaceTarget(Transform target, bool isSmoothRotation = false)
        {
            // ターゲットの方向を向く（回転のみ）
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // 水平回転のみ適用
            // スムーズに回転させる
            if (isSmoothRotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 4f);
            }
            else
            {
                transform.rotation = lookRotation;
            }
        }

        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///

        /// <summary>
        /// クリティカルかどうかの判定関数
        /// </summary>
        /// <returns>クリティカルかどうか</returns>
        private bool IsCriticalHit()
        {
            // 1/3でクリティカル
            return Random.Range(0, criticalRate) == 0;
        }

        /// <summary>
        /// ブレイブダメージ計算関数
        /// </summary>
        /// <param name="motionValue">モーション値</param>
        /// <param name="isCritical">クリティカルかどうか</param>
        /// <returns></returns>
        private float CalculateDamage(float motionValue, bool isCritical)
        {
            // モーション値*攻撃力*クリティカル倍率
            if (isCritical)
            {
                return motionValue * parameter.attack * criticalDamageRate;
            }
            else
            {
                return motionValue * parameter.attack;
            }
        }

        /// <summary>
        /// ブレイク状態になったかどうか
        /// </summary>
        /// <returns>ブレイク状態であればtrue</returns>
        private bool IsBreak()
        {
            // 既にブレイク状態ならfalseを返す
            if (BraveState == BraveState.E_Break) return false;

            // ブレイブが0以下になったらブレイク状態にする
            if (parameter.brave <= 0)
            {
                parameter.brave = 0;
                BraveState = BraveState.E_Break;
                textBreak.gameObject.SetActive(true);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// テキストアニメーションを開始する関数
        /// </summary>
        /// <param name="animator">スタートさせたいアニメーター</param>
        private void TextAnimationPlay(TextMeshProGeometryAnimator[] animator)
        {
            for (int i = 0; i < animator.Length; i++)
            {
                animator[i].Play();
            }
        }

        /// <summary>
        /// テキストアニメーションを終了する関数
        /// </summary>
        /// <param name="animator">スタートさせたいアニメーター</param>
        private void TextAnimationFinish(TextMeshProGeometryAnimator[] animator)
        {
            for (int i = 0; i < animator.Length; i++)
            {
                animator[i].Finish();
            }
        }

        /// <summary>
        /// ブレイブテキストを変更する関数
        /// </summary>
        private void SetTextBrave()
        {
            // テキストを変更する
            textBrave.text = string.Format("{0:0}", parameter.brave);

            // ブレイブテキストのアニメーションを再生・停止する
            if (parameter.brave >= 500)
            {
                TextAnimationPlay(braveTextAnimators);
            }
            else
            {
                TextAnimationFinish(braveTextAnimators);
            }
        }

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

        /// <summary>
        /// アタックスクリプト配列を複数持つリスト
        /// </summary>
        [System.Serializable]
        public class AttackScriptList
        {
            // コンストラクタ
            public AttackScriptList(AttackScript[] script)
            {
                attackScriptGroup = script;
            }

            // リスト変数
            public AttackScript[] attackScriptGroup;
        }

        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///

        protected bool inCollider; // コライダー内かどうか

        protected float knockBackDistance; // ノックバック距離

        protected BraveState braveState; // ブレイブの状態変数

        protected DamageState damageState; // 被ダメージの状態

        protected Animator anim; // Animator変数

        protected Parameter parameter; // パラメーター変数

        protected AttackScript bulletAttackScript; // 遠距離攻撃スクリプト
        [SerializeField] protected List<AttackScript> attackScripts; // 自身の武器の攻撃スクリプト
        [Header("必ずアニメーションで呼び出したいAttackListと同じにすること")]
        [SerializeField] protected List<AttackScriptList> attackScriptList = new List<AttackScriptList>(); // 攻撃スクリプトリスト

        // HPとBrave値の表示テキスト
        [SerializeField] protected TextMeshProUGUI textHP; // HPテキスト
        [SerializeField] protected TextMeshProUGUI textBrave; // ブレイブテキスト

        [SerializeField] protected TextMeshProUGUI textBreak; // ブレイク状態表示テキスト

        [SerializeField] protected Image hpBar; // HPバー

        [SerializeField] protected TextMeshProGeometryAnimator[] braveTextAnimators = new TextMeshProGeometryAnimator[1]; // ブレイブテキストアニメーション配列

        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///

        private bool ishit = false; // ヒットしたかどうか

        private int criticalRate = 3; // クリティカル発生率

        // 初期パラメータ
        [SerializeField] private float hp = 1000;
        [SerializeField] private float brave = 100;
        [SerializeField] private float regenerateSpeed = 3;
        [SerializeField] private float breakSpeed = 10;
        [SerializeField] private float speed = 10;
        [SerializeField] private float attack = 100;
        [SerializeField] private float adjustEffectYPos = 0.5f; // エフェクトのY軸補正値

        private float criticalDamageRate = 2f; // クリティカルダメージ倍率
        [SerializeField] private float pushStrength = 0.3f; // 位置調整

        [SerializeField] private AnimState animState; // アニメーションの状態変数

        private Vector3 effectPos; // エフェクト表示位置

        [Header("エフェクトの親オブジェクトを入れてください, 使用しないエフェクトの場合はnullにしてください"),
            SerializeField, EnumIndex(typeof(EffectName))]
        private GameObject[] effectPositions; // エフェクト発生位置配列

        private DamageUIScript damageUI; // ダメージポップアップUI

        private HitStopManager hitStopManager; // ヒットストップマネージャー変数

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///

        public int CriticalRate { get { return criticalRate; } set { criticalRate = value; } }

        public Animator GetAnimator { get { return anim; } }

        public virtual BraveState BraveState
        { 
            get { return braveState; }
            set
            {
                braveState = value;
                if (BraveState != BraveState.E_Default)
                {
                    textBrave.alpha = 0.5f;
                }
                else
                {
                    textBrave.alpha = 1f;
                }
            } 
        }

        public HitStopManager GetHitStopManager { get { return hitStopManager; } }

        protected virtual AnimState AnimState { get { return animState; } set { animState = value; } }

        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}

