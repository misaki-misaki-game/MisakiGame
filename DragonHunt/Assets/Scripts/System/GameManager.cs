using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;
using UniRx;

namespace Misaki
{
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

        private void Start()
        {
            // Titleスクリプトのインスタンス生成
            titleInputs = new TitleInputs();

            // アクションマップを設定
            titleInputs.Enable();

            // イベント登録
            titleInputs.Title.Start.started += OnStart;

            // プレイヤーの戦闘不能イベントを監視
            player.OnPlayerDead.Subscribe(_ =>{ StartCoroutine(GameOverSequence(false)); })
                .AddTo(this);  // サブスクライバーの自動解除

            // ドラゴンの戦闘不能イベントを監視
            dragon.OnDragonDead.Subscribe(_ => { StartCoroutine(GameOverSequence(true)); })
                .AddTo(this);  // サブスクライバーの自動解除

            // タイトルのBGMを流す
            SoundManager.SoundPlay(BGMList.E_TitleBGM, true);

            // メインカメラのトランスフォームを取得
            mainCamera = playerFollowCamera.transform;
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
            playerFollowCamera.Priority = 20;

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

            // インゲームUIを表示にする
            foreach(GameObject obj in inGameUI) obj.SetActive(true);

            // BGMを流す
            SoundManager.SoundPlay(BGMList.E_InGameBGM, true);
        }


        // ゲームオーバー演出とシーンリロード
        private IEnumerator GameOverSequence(bool isWon)
        {
            // 勝利または敗北UIを表示
            if (isWon)
            {
                winUI.gameObject.SetActive(true);

                // BGMを流す
                SoundManager.SoundPlay(BGMList.E_VictoryBGM, true);
            }
            else
            {
                loseUI.gameObject.SetActive(true);

                // BGMを流す
                SoundManager.SoundPlay(BGMList.E_DefeatBGM, true);
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

        [SerializeField] private float gameOverDelay = 5f; // ゲームをリセットするまでの時間
        [SerializeField] private static float breakBonus = 500; // 相手をブレイクした際のボーナスブレイブ

        [SerializeField] private GameObject titleUI; // タイトルのUI
        [SerializeField] private GameObject[] inGameUI; // インゲームのUI
        [SerializeField] private GameObject winUI; // 勝った時のUI
        [SerializeField] private GameObject loseUI;// 負けた時のUI

        private TitleInputs titleInputs; // InputSystemから生成したスクリプト

        private static Transform mainCamera; // メインカメラのトランスフォーム

        [SerializeField] private PlayerScript player; // プレイヤー

        [SerializeField] private DragonScript dragon; // ドラゴン

        [SerializeField] private PlayableDirector playableDirector; // タイムラインのプレイアブルディレクター

        [SerializeField] private CinemachineVirtualCamera playerFollowCamera; // プレイヤーに追従するカメラ

        private static GameState gameState = GameState.E_Title; // ゲームの状態変数 

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///

        public static float GetBreakBonus { get { return breakBonus; } }

        public static GameState GetGameState {  get { return gameState; } }

        public static Transform GetCameraTransform { get { return mainCamera; } }

        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}