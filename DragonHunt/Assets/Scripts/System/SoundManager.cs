using UnityEngine;

namespace Misaki
{
    public partial class SoundManager : SingletonMonoBehaviour<SoundManager>
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        /// <summary>
        /// BGMを流す関数
        /// </summary>
        /// <param name="audioSource">再生したオーディオソース</param>
        /// <param name="bgmClip">再生したオーディオクリップ</param>
        /// <param name="isLoop">ループするかどうか</param>
        public static void SoundPlay(BGMList bgmClip, bool isLoop = false)
        {
            // BGMが再生中の場合はBGMをストップする
            if(bgmAudioSource.isPlaying) bgmAudioSource.Stop();
            // ループするかを設定し、指定したオーディオソースでクリップを再生する
            bgmAudioSource.loop = isLoop;
            bgmAudioSource.clip = bgmClipArray[(int)bgmClip];
            bgmAudioSource.Play();
        }

        /// <summary>
        /// SEを流す関数
        /// </summary>
        /// <param name="audioSource">再生したオーディオソース</param>
        /// <param name="seClip">再生したオーディオクリップ</param>
        /// <param name="isLoop">ループするかどうか</param>
        public static void SoundPlay(AudioSource audioSource, SEList seClip)
        {
            // ループするかを設定し、指定したオーディオソースでクリップを再生する
            audioSource.PlayOneShot(seClipArray[(int)seClip]);
        }

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
            // サウンド関係を初期化する
            InitializeSound();
        }

        /// <summary>
        /// サウンド関係を初期化する関数
        /// </summary>
        private void InitializeSound()
        {
            // オーディオソースとクリップを初期化する
            bgmAudioSource = mainAudioSource;
            seClipArray = new AudioClip[seClips.Length];
            seClipArray = seClips;
            bgmClipArray = new AudioClip[bgmClips.Length];
            bgmClipArray = bgmClips;
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class SoundManager
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

        [SerializeField] private int poolDefaultCapacity; // オブジェクトプールのデフォルト容量
        [SerializeField] private int poolMaxSize; // オブジェクトプールの最大容量

        private static AudioClip[] seClipArray; // SE配列
        private static AudioClip[] bgmClipArray; // BGM配列
        [Header("SEを入れてください"), SerializeField, EnumIndex(typeof(SEList))]
        private AudioClip[] seClips; // SEクリップ
        [Header("BGMを入れてください"), SerializeField, EnumIndex(typeof(BGMList))]
        private AudioClip[] bgmClips; // BGMクリップ

        private static AudioSource bgmAudioSource; // BGM用のオーディオソース
        [Header("BGMを流すオーディオソースを入れてください"), SerializeField]
        private AudioSource mainAudioSource; // メインで使うBGMオーディオソース

        public static PoolManager[] seAudioPool; // プールマネージャー変数


        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}