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

            // EnemyScriptの生成と破棄を監視
            MessageBroker.Default.Receive<EnemyScriptCreatedMessage>()
                .Subscribe(message => AddScript(message.Script))
                .AddTo(this);
            MessageBroker.Default.Receive<EnemyScriptDestroyedMessage>()
                .Subscribe(message => RemoveScript(message.Script))
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
                .Subscribe(_ => OnDodgeSuccess())
                .AddTo(this);
        }

        private void Start()
        {
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
            camerawork.GetFreeLookCamera.Priority = 20;

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

            // カメラワークを許可
            camerawork.GetFreeLookCamera.GetComponent<CinemachineInputProvider>().enabled = true;

            // インゲームUIを表示にする
            foreach(GameObject obj in inGameUI) obj.SetActive(true);

            // BGMを流す
            SoundManager.SoundPlay(BGMList.E_InGameBGM, true);

            // エネミー全てのUI初期化
            foreach (EnemyScript enemy in enemeis) enemy.InitializeEnemyUI();
        }

        /// <summary>
        /// 雑魚敵を召喚する関数
        /// </summary>
        private IEnumerator SummonMook()
        {
            Debug.Log("雑魚敵召喚");

            yield return null;
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
            if (script is AttackScript attackScript && !attackScripts.Contains(attackScript)) attackScripts.Add(attackScript);
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
            if (script is AttackScript attackScript && !attackScripts.Contains(attackScript)) attackScripts.Remove(attackScript);
            else if (script is EnemyScript enemyScript && !enemeis.Contains(enemyScript))
            {
                // エネミーリストとロックオン位置リストから除外
                enemeis.Remove(enemyScript);
                enemyCameraAnchor.Remove(enemyScript.GetCameraAnchor);
                camerawork.ChangeTarget(-1f);
            }
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
            dragon.GetAnimator.speed = 0.1f;
            player.GetAnimator.speed = 2f;

            // クリティカル発生率を100%にする
            int currentCriticalRate = player.CriticalRate;
            player.CriticalRate = 0;

            // ビネットを保持して演出を変更する
            bool currentRounded = false;
            float currentIntensity = 0;
            float currentSmoothness = 0;
            if (volume.profile.TryGet(out vignette))
            {
                currentRounded = vignette.rounded.value;
                currentIntensity = vignette.intensity.value;
                currentSmoothness = vignette.smoothness.value;
                vignette.rounded.value = true;
                vignette.intensity.value = 1f;
                vignette.smoothness.value = 1f;
            }

            // オーラを出す
            player.GetAura.SetActive(true);

            // 5秒間待つ
            yield return new WaitForSeconds(chanceTime);

            SoundManager.SoundPlay(SoundManager.GetMainAudioSource, SEList.E_EndChanceTime);

            // アニメーションスピードとクリティカル発生率,ビネットを元に戻す
            dragon.GetAnimator.speed = 1f;
            player.GetAnimator.speed = 1f;
            player.CriticalRate = currentCriticalRate;
            if (volume.profile.TryGet(out vignette))
            {
                vignette.rounded.value = currentRounded;
                vignette.intensity.value = currentIntensity;
                vignette.smoothness.value = currentSmoothness;
            }

            // オーラを消す
            player.GetAura.SetActive(false);

            isChanceTime = false;
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

        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///



        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///

        private static bool isChanceTime = false;

        [SerializeField] private float gameOverDelay = 5f; // ゲームをリセットするまでの時間
        [SerializeField] private float chanceTime = 5f; // チャンスタイムの制限時間
        [SerializeField] private static float breakBonus = 500; // 相手をブレイクした際のボーナスブレイブ

        private static List<GameObject> enemyCameraAnchor = new List<GameObject>(); // ロックオン位置リスト
        [SerializeField] private List<GameObject[]> triggerObjects = new List<GameObject[]>();
        [SerializeField] private GameObject titleUI; // タイトルのUI
        [SerializeField] private GameObject[] inGameUI; // インゲームのUI
        [SerializeField] private GameObject winUI; // 勝った時のUI
        [SerializeField] private GameObject loseUI;// 負けた時のUI

        private TitleInputs titleInputs; // InputSystemから生成したスクリプト

        private static Transform mainCamera; // メインカメラのトランスフォーム

        [SerializeField] private PlayerScript player; // プレイヤー

        private List<EnemyScript> enemeis = new List<EnemyScript>(); // エネミーリスト
        [SerializeField] private DragonScript dragon; // ドラゴン

        [SerializeField] private PlayableDirector playableDirector; // タイムラインのプレイアブルディレクター

        private List<AttackScript> attackScripts = new List<AttackScript>(); // アタックスクリプトのリスト

        [SerializeField] private Volume volume; // ボリューム変数

        private Vignette vignette; // ビネット変数

        private Camerawork camerawork; // カメラワーク変数

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

        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}