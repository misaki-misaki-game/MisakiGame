using UnityEngine;

namespace Misaki
{
    public partial class PooledEffectObject : MonoBehaviour
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        /// <summary>
        /// パーティクル終了時に動かす関数
        /// </summary>
        public void ParticleEnd()
        {
            // パーティクルカウントを増やす
            particleCount++;

            // 全てのパーティクルが終了すれば、オブジェクトをプールに入れる
            if (particleCount >= particleSize)
            {
                particleCount = default;
                objectPool.ReleaseGameObject(gameObject);
            }
        }

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///



        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///

        private void Awake()
        {
            // エフェクトコールバックを取得する
            effectCallbacks = GetComponentsInChildren<EffectCallback>();

            // エフェクトコールバックの個数を取得する
            particleSize = effectCallbacks.Length;
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class PooledEffectObject
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

        private int particleCount; // パーティクルシステムの終了回数
        private int particleSize = 0; // パーティクルシステムの個数

        private EffectCallback[] effectCallbacks; // エフェクトコールバック配列

        private PoolManager objectPool; // プールマネージャー

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///

        // プールマネージャーのセッター関数
        public PoolManager SetPoolManager { set { objectPool = value; } }

        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}