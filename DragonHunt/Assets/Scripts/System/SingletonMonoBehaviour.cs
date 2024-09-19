using UnityEngine;

namespace Misaki
{
    /// <summary>
    /// MonoBehaviourに対応したシングルトンクラス
    /// （例）public class GameManager : SingletonMonoBehaviour<GameManager>
    /// </summary>
    public partial class SingletonMonoBehaviour<T> : DebugSetUp where T : MonoBehaviour
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        /// <summary>インスタンスを取得します</summary>
        public static T Instance
        {
            get
            {
                // instanceがnullならTクラスをヒエラルキー上から探す　無ければエラー
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));
                    Debug.Assert(instance != null, typeof(T) + "をアタッチしているGameObjectがありません");
                }
                return instance;
            }
        }

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///

        protected override void Awake()
        {
            base.Awake();
            // 他のGameObjectにアタッチされているか調べる
            if (this != Instance)
            {
                // アタッチされている場合は破棄する
                Debug.LogWarning("既に" + typeof(T) + "があるのでオブジェクトが破棄されます");
                Destroy(this.gameObject);
                return;
            }
            // trueならこのオブジェクトをシーンしても消さないようにする
            if(isDontDestroy) DontDestroyOnLoad(this.gameObject);
        }

        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///



        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class SingletonMonoBehaviour<T>
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

        private static T instance; // インスタンス変数

        [SerializeField] bool isDontDestroy; // DontDestroyに設定するかどうか

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}