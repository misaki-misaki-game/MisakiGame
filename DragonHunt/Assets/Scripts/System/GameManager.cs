using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;
using UniRx;
using System.Collections.Generic;
using static Misaki.AttackScript;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static Misaki.EnemyScript;
using static Misaki.PlayerScript;
using System.Reflection;

namespace Misaki
{
    // 自動的にコンポーネントを追加 Cameraworkを追加
    [RequireComponent(typeof(Camerawork))]
    public partial class GameManager : SingletonMonoBehaviour<GameManager>
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///



        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///



        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///

        protected override void Awake()
        {
            base.Awake();
            Application.targetFrameRate = 60; // 60fpsに設定

            // コライダーグループを初期化
            particleTriggerColliderGroup = new ParticleTriggerColliderGroup[2];
            for (int i = 0; i < particleTriggerColliderGroup.Length; i++)
            {
                particleTriggerColliderGroup[i] = new ParticleTriggerColliderGroup();
            }

            // PlayerScriptの生成と破棄を監視
            MessageBroker.Default.Receive<PlayerScriptCreatedMessage>()
                .Subscribe(message => { AddCollider(ParticleSystemTrigger.E_EnemyWepon, message.Collider);})
                .AddTo(this);
            MessageBroker.Default.Receive<PlayerScriptDestroyedMessage>()
                .Subscribe(message => { RemoveCollider(ParticleSystemTrigger.E_EnemyWepon, message.Collider); })
                .AddTo(this);

            // EnemyScriptの生成と破棄を監視
            MessageBroker.Default.Receive<EnemyScriptCreatedMessage>()
                .Subscribe(message =>
                {
                    AddScript(message.Script);
                    AddCollider(ParticleSystemTrigger.E_PlayerWepon, message.Collider);
                })
                .AddTo(this);
            MessageBroker.Default.Receive<EnemyScriptDestroyedMessage>()
                .Subscribe(message =>
                {
                    RemoveScript(message.Script);
                    RemoveCollider(ParticleSystemTrigger.E_PlayerWepon, message.Collider);
                })
                .AddTo(this);

            // AttackScriptの生成と破棄を監視
            MessageBroker.Default.Receive<AttackScriptCreatedMessage>()
                .Subscribe(message => AddScript(message.Script))
                .AddTo(this);
            MessageBroker.Default.Receive<AttackScriptDestroyedMessage>()
                .Subscribe(message => RemoveScript(message.Script))
                .AddTo(this);

            // 回避成功イベントをグローバルに購読
            MessageBroker.Default.Receive<DodgeSuccessMessage>()
                .Subscribe(message => OnDodgeSuccess())
                .AddTo(this);

            // ブレイクイベントをグローバルに購読
            MessageBroker.Default.Receive<BreakSuccessMessage>()
                .Subscribe(message => OnBreakSuccess())
                .AddTo(this);
        }

        private void Start()
        {
            // マウスを固定する気はない
            // マウスカーソルを非表示にし、位置を固定
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Titleスクリプトのインスタンス生成
            titleInputs = new TitleInputs();

            // カメラワークを取得
            camerawork = GetComponent<Camerawork>();

            // アクションマップを設定
            titleInputs.Enable();

            // イベント登録
            titleInputs.Title.Start.started += OnStart;

            // プレイヤーの戦闘不能イベントを監視
            player.OnPlayerDead.Subscribe(_ =>{ StartCoroutine(GameOverSequence(false)); })
                .AddTo(this);

            // ドラゴンのイベントを監視
            dragon.OnDragonDead.Subscribe(_ => { StartCoroutine(GameOverSequence(true)); })
                .AddTo(this);
            dragon.OnPhaseChange.Subscribe(_ => {  StartCoroutine(SummonMook()); })
                .AddTo(this);

            // タイトルのBGMを流す
            SoundManager.SoundPlay(BGMList.E_TitleBGM, true);

            // 通常時ボリューム情報を初期化
            InitializeCustomVolume();
        }

        private void OnDestroy()
        {
            // 自身でインスタンス化したActionクラスはIDisposableを実装しているので、
            // 必ずDisposeする必要がある
            titleInputs?.Dispose();
        }

        private void OnEnable()
        {
            // オブジェクトがアクティブになった時にtitleInputsを起動
            titleInputs?.Enable();
        }

        private void OnDisable()
        {
            // オブジェクトが非アクティブになった時にtitleInputsを停止
            titleInputs?.Dispose();
        }

        /// <summary>
        /// ゲームスタートのコールバック登録関数
        /// </summary>
        /// <param name="context"></param>
        private void OnStart(InputAction.CallbackContext context)
        {
            // タイムラインを再生
            playableDirector.Play();

            // 入力を受け付けないようにする
            titleInputs.Dispose();

            // タイトルUIを非表示にする
            titleUI.gameObject.SetActive(false);

            // プレイヤー追従カメラの優先度を上げて画面に表示する
            camerawork.GetFreeLockCamera.Priority = 20;

            // BGMを流す
            SoundManager.SoundPlay(BGMList.E_OpeningBGM, true);

            // タイムライン終了時したらインゲームに遷移
            playableDirector.stopped += EndTimeline;
        }

        /// <summary>
        /// タイムラインが終了したときに呼ぶ関数
        /// </summary>
        /// <param name="director"></param>
        private void EndTimeline(PlayableDirector director)
        {
            // ゲームの状態をインゲームにする
            gameState = GameState.E_InGame;

            // カメラワークを許可し、敵をロックオン
            camerawork.GetFreeLockCamera.GetComponent<CinemachineInputProvider>().enabled = true;
            camerawork.Lockon();

            // インゲームUIを表示にする
            foreach (GameObject obj in inGameUI)
            {
                obj.SetActive(true);
            }

            // BGMを流す
            SoundManager.SoundPlay(BGMList.E_InGameBGM, true);

            // エネミー全てのUI初期化
            foreach (EnemyScript enemy in enemeis)
            {
                enemy.InitializeEnemyUI();
            }
        }

        /// <summary>
        /// 雑魚敵を召喚する関数
        /// </summary>
        private IEnumerator SummonMook()
        {
            // エネミー生成位置を取得
            GameObject[] obj = new GameObject[spawnPos.Length];

            // エネミーを生成し、必要な情報を渡す
            for (int i = 0; i < spawnPos.Length; i++)
            {
                obj[i] = Instantiate(mook, spawnPos[i].transform.localPosition, Quaternion.identity);
                obj[i].GetComponent<EnemyScript>().SetupTargetAndCanvas(player.transform, damageCanvas, enemyHPCanvas);
            }

            // 生成から少し時間を置いてから、UIの初期化を行う
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < spawnPos.Length; i++)
            {
                obj[i].GetComponent<EnemyScript>().InitializeEnemyUI();
            }
        }

        /// <summary>
        /// ゲームオーバー演出とシーンリロード
        /// </summary>
        /// <param name="isWon">勝利したかどうか</param>
        /// <returns></returns>
        private IEnumerator GameOverSequence(bool isWon)
        {
            // 勝利または敗北UIを表示
            if (isWon)
            {
                winUI.gameObject.SetActive(true);

                // BGMを流す
                SoundManager.SoundPlay(BGMList.E_VictoryBGM, false);
            }
            else
            {
                loseUI.gameObject.SetActive(true);

                // BGMを流す
                SoundManager.SoundPlay(BGMList.E_DefeatBGM, false);
            }

            // アニメーションが完了するまで待つ
            yield return new WaitForSeconds(gameOverDelay);

            // 現在のシーンを再読み込み
            ReloadScene();
        }

        /// <summary>
        /// シーンをやり直す関数
        /// </summary>
        private void ReloadScene()
        {
            // シーンをやり直す
            gameState = GameState.E_Title;
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }

        /// <summary>
        /// スクリプトリストに指定のスクリプトを加える関数
        /// </summary>
        /// <param name="script">指定のスクリプト</param>
        private void AddScript<T>(T script)
        {
            if (script is AttackScript attackScript && !attackScripts.Contains(attackScript))
            {
                attackScripts.Add(attackScript);
            }
            else if (script is EnemyScript enemyScript && !enemeis.Contains(enemyScript))
            {
                // エネミーリストとロックオン位置リストを追加
                enemeis.Add(enemyScript);
                enemyCameraAnchor.Add(enemyScript.GetCameraAnchor);
            }
        }

        /// <summary>
        /// スクリプトリストから指定のスクリプトを外す関数
        /// </summary>
        /// <param name="script">指定のスクリプト</param>
        private void RemoveScript<T>(T script)
        {
            if (script is AttackScript attackScript && attackScripts.Contains(attackScript))
            {
                attackScripts.Remove(attackScript);
            }
            else if (script is EnemyScript enemyScript && enemeis.Contains(enemyScript))
            {
                // エネミーリストとロックオン位置リストから除外
                enemeis.Remove(enemyScript);
                enemyCameraAnchor.Remove(enemyScript.GetCameraAnchor);
                camerawork.ChangeTarget(1f);
            }
        }

        /// <summary>
        /// コライダーグループを追加する関数
        /// </summary>
        /// <param name="e">追加したいコライダーのタグ</param>
        /// <param name="col">追加したいコライダー</param>
        private void AddCollider(ParticleSystemTrigger e, Collider col)
        {
            particleTriggerColliderGroup[(int)e].targetColliders.Add(col);
        }

        /// <summary>
        /// コライダーグループから外す関数
        /// </summary>
        /// <param name="e">除外したいコライダーのタグ</param>
        /// <param name="col">除外したいコライダー</param>
        private void RemoveCollider(ParticleSystemTrigger e, Collider col)
        {
            particleTriggerColliderGroup[(int)e].targetColliders.Remove(col);
        }

        /// <summary>
        /// 回避成功時の関数
        /// </summary>
        private void OnDodgeSuccess()
        {
            // 回避成功時の処理
            StartCoroutine(BeginChanceTime());
        }

        /// <summary>
        /// ブレイク時の関数
        /// </summary>
        private void OnBreakSuccess()
        {
            // ブレイク時の処理
            StartCoroutine(BreakingTime());
        }

        /// <summary>
        /// 通常時のボリューム情報を初期化する関数
        /// </summary>
        private void InitializeCustomVolume()
        {
            // 通常時ボリューム情報を保持する
            customVolume[0] = new CustomVolume(volume);

            CustomVolume defaultVolume = customVolume[0]; // 基準となるcustomVolume[0]
            CustomVolume currentVolume = new CustomVolume();

            // customVolume[1]以降をチェックして必要に応じて値をコピー
            for (int i = 1; i < customVolume.Length; i++)
            {
                currentVolume = customVolume[i];

                // CustomVolumeクラスの各フィールドを取得
                foreach (FieldInfo field in typeof(CustomVolume).GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    object currentValue = field.GetValue(currentVolume);
                    object defaultValue = field.GetValue(defaultVolume);

                    // フィールドの型ごとにコピー条件を判定
                    if (field.FieldType == typeof(float) && (float)currentValue == -1)
                    {
                        field.SetValue(currentVolume, defaultValue);
                    }
                    else if (field.FieldType == typeof(Color) && (Color)currentValue == Color.clear) // Color.clearを基準に
                    {
                        field.SetValue(currentVolume, defaultValue);
                    }
                }
            }

            // 静的変数に代入
            staticVolume = volume;
            staticCustomVolume = new CustomVolume[customVolume.Length];
            for (int i = 0; i < staticCustomVolume.Length; i++)
            {
                staticCustomVolume[i] = customVolume[i];
            }
        }

        /// <summary>
        /// 指定のボリューム情報に変更する関数
        /// </summary>
        /// <param name="type">指定のボリューム</param>
        private static void ChangeVolume(VolumeType type)
        {
            // 指定のボリュームに変更する
            int i = (int)type;
            if (staticVolume.profile.TryGet(out vignette))
            {
                staticVolume.weight = staticCustomVolume[i].weight;
                vignette.rounded.value = staticCustomVolume[i].vignetteRounded;
                vignette.intensity.value = staticCustomVolume[i].vignetteIntensity;
                vignette.smoothness.value = staticCustomVolume[i].vignetteSmoothness;
                vignette.color.value = staticCustomVolume[i].vignetteColor;
            }
            if (staticVolume.profile.TryGet(out bloom))
            {
                bloom.intensity.value = staticCustomVolume[i].bloomIntensity;
            }
        }

        /// <summary>
        /// チャンスタイムを開始する関数
        /// </summary>
        /// <returns></returns>
        private IEnumerator BeginChanceTime()
        {
            // 既にチャンスタイム中なら処理を中断
            if (isChanceTime) yield break;
            isChanceTime = true;

            SoundManager.SoundPlay(SoundManager.GetMainAudioSource, SEList.E_BeginChanceTime);

            // アニメーションスピードを変更
            if (enemeis.Count > 0)
            {
                foreach (EnemyScript enemy in enemeis)
                {
                    enemy.GetAnimator.speed = 0.1f;

                    // 現在のNavMeshAgentの速度を保持し、移動速度を遅くする
                    enemy.GetNavMeshAgent.speed /= 5f;
                }
            }
            player.GetAnimator.speed = 2f;

            // クリティカル発生率を100%にする
            int currentCriticalRate = player.CriticalRate;
            player.CriticalRate = 0;

            // ボリュームを変更
            ChangeVolume(VolumeType.E_ChanceTime);

            // オーラを出す
            player.GetAura.SetActive(true);

            // 5秒間待つ
            yield return new WaitForSeconds(chanceTime);

            SoundManager.SoundPlay(SoundManager.GetMainAudioSource, SEList.E_EndChanceTime);

            // アニメーションスピードとクリティカル発生率,ボリューム情報を元に戻す
            if (enemeis.Count > 0)
            {
                foreach (EnemyScript enemy in enemeis)
                {
                    enemy.GetAnimator.speed = 1f;

                    // NavMeshAgentの速度を元に戻す
                    enemy.GetNavMeshAgent.speed *= 5f;
                }
            }
            player.GetAnimator.speed = 1f;
            player.CriticalRate = currentCriticalRate;
            ChangeVolume(VolumeType.E_Normal);

            // オーラを消す
            player.GetAura.SetActive(false);

            isChanceTime = false;
        }

        /// <summary>
        /// ブレイク時の演出を開始する関数
        /// </summary>
        /// <returns></returns>
        private IEnumerator BreakingTime()
        {
            // 既にブレイクまたはチャンス演出中なら処理を中断
            if (isBreaking || isChanceTime) yield break;
            isBreaking = true;

            SoundManager.SoundPlay(SoundManager.GetMainAudioSource, SEList.E_BreakSE1);
            SoundManager.SoundPlay(SoundManager.GetMainAudioSource, SEList.E_BreakSE2);

            // アニメーションスピードを変更
            if (enemeis.Count > 0)
            {
                foreach (EnemyScript enemy in enemeis)
                {
                    enemy.GetAnimator.speed = 0.1f;

                    // 現在のNavMeshAgentの速度を保持し、移動速度を遅くする
                    enemy.GetNavMeshAgent.speed /= 5f;
                }
            }
            player.GetAnimator.speed = 0.1f;

            // ボリューム情報を変更する
            ChangeVolume(VolumeType.E_BreakingTime);

            // 1.5秒間待つ
            yield return new WaitForSeconds(1.5f);

            // アニメーションスピードを元に戻す
            if (enemeis.Count > 0)
            {
                foreach (EnemyScript enemy in enemeis)
                {
                    enemy.GetAnimator.speed = 1f;

                    // NavMeshAgentの速度を元に戻す
                    enemy.GetNavMeshAgent.speed *= 5f;
                }
            }
            player.GetAnimator.speed = 1f;

            // ボリューム情報を元に戻す
            ChangeVolume(VolumeType.E_Normal);

            isBreaking = false;
        }

        public static void BeginHPAttackTime()
        {
            // 既にブレイクまたはチャンス演出中なら処理を中止
            if (isBreaking || isChanceTime) return;

            // 発生個数を増やす
            hpAttackCount++;

            // ボリューム情報を変更する
            ChangeVolume(VolumeType.E_HPAttackTime);
        }

        public static void EndHPAttackTime()
        {
            // 既にブレイク演出中なら処理を中止
            if (isBreaking || isChanceTime) return;

            // HP攻撃が発生してない場合は処理を中止
            if (hpAttackCount <= 0) return;
            
            // HP攻撃発生個数を減らして0ならボリュームを通常に戻す
            hpAttackCount--;
            if (hpAttackCount <= 0)
            {
                // ボリューム情報を元に戻す
                ChangeVolume(VolumeType.E_Normal);
            }
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class GameManager
    {
        /// --------変数一覧-------- ///

        #region public変数
        /// -------public変数------- ///

        [System.Serializable]
        public class ParticleTriggerColliderGroup
        {
            public List<Collider> targetColliders = new List<Collider>(); // パーティクルシステムのトリガーに設定するためのコライダー配列
        }

        /// <summary>
        /// ボリューム情報を格納するクラス
        /// </summary>
        [System.Serializable]
        public class CustomVolume
        {
            // 各変数
            public float weight;
            public float bloomIntensity;
            public bool vignetteRounded;
            public float vignetteIntensity;
            public float vignetteSmoothness;
            public Color vignetteColor;

            private Vignette vignette; // ビネット変数
            private Bloom bloom; // ブルーム変数

            public CustomVolume() { }

            public CustomVolume(Volume _volume)
            {
                weight = _volume.weight;
                if (_volume.profile.TryGet(out vignette))
                {
                    vignetteRounded = vignette.rounded.value;
                    vignetteIntensity = vignette.intensity.value;
                    vignetteSmoothness = vignette.smoothness.value;
                    vignetteColor = vignette.color.value;
                }
                if (_volume.profile.TryGet(out bloom))
                {
                    bloomIntensity = bloom.intensity.value;
                }
            }
        }

        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///



        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///

        private static bool isChanceTime = false; // チャンスタイムかどうか
        private static bool isBreaking = false; // ブレイク演出中かどうか

        [SerializeField] private float gameOverDelay = 5f; // ゲームをリセットするまでの時間
        [SerializeField] private float chanceTime = 5f; // チャンスタイムの制限時間
        [SerializeField] private static float breakBonus = 500; // 相手をブレイクした際のボーナスブレイブ

        private static int hpAttackCount = 0; // HP攻撃が発生している個数

        private static List<GameObject> enemyCameraAnchor = new List<GameObject>(); // ロックオン位置リスト
        [SerializeField] private GameObject titleUI; // タイトルのUI
        [SerializeField] private GameObject[] inGameUI; // インゲームのUI
        [SerializeField] private GameObject winUI; // 勝った時のUI
        [SerializeField] private GameObject loseUI;// 負けた時のUI
        [SerializeField] private GameObject mook; // 雑魚エネミー
        [SerializeField] private GameObject[] spawnPos; // 雑魚エネミー出現場所
        [SerializeField] private GameObject damageCanvas; // ダメージポップアップ用キャンパス
        [SerializeField] private GameObject enemyHPCanvas; // エネミーHP用キャンパス

        private TitleInputs titleInputs; // InputSystemから生成したスクリプト

        private static Transform mainCamera; // メインカメラのトランスフォーム

        [SerializeField] private PlayerScript player; // プレイヤー

        private List<EnemyScript> enemeis = new List<EnemyScript>(); // エネミーリスト

        [SerializeField] private DragonScript dragon; // ドラゴン

        [SerializeField] private PlayableDirector playableDirector; // タイムラインのプレイアブルディレクター

        private List<AttackScript> attackScripts = new List<AttackScript>(); // アタックスクリプトのリスト

        [SerializeField] private Volume volume; // ボリューム変数　インスペクター用
        [SerializeField] private static Volume staticVolume; // ボリューム変数

        private static Vignette vignette; // ビネット変数

        private static Bloom bloom; // ブルーム変数

        private static Color color; // カラー変数

        [SerializeField, EnumIndex(typeof(VolumeType)),
            Header("要素数0はゲーム起動時自動で取得します。\n要素数1以降で小素数0と同じ値で問題ない場合は-1を入力\nColorの場合はコード000000\nboolは必ずどちらかにしてください")]
        private CustomVolume[] customVolume; // 現在のボリューム情報を格納する変数 インスペクター用

        private static CustomVolume[] staticCustomVolume; // 現在のボリューム情報を格納する変数

        private static Camerawork camerawork; // カメラワーク変数

        private static ParticleTriggerColliderGroup[] particleTriggerColliderGroup;  // トリガー対象とするコライダー

        private static GameState gameState = GameState.E_Title; // ゲームの状態変数 

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///

        public static bool GetIsChanceTime {  get { return isChanceTime; } }

        public static float GetBreakBonus { get { return breakBonus; } }

        public static GameState GetGameState {  get { return gameState; } }

        public static List<GameObject> GetEnemyCameraAnchor { get { return enemyCameraAnchor; } }

        public static Transform GetCameraTransform { get { return mainCamera; } }

        public static ParticleTriggerColliderGroup[] GetParticleTriggerColliderGroup { get { return particleTriggerColliderGroup; } }

        public static Camerawork GetCamerawork { get { return camerawork; } }

        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}