using UnityEngine;

namespace Misaki
{
    [System.Serializable]
    public partial class DamageUIManager : SingletonMonoBehaviour<EffectManager>
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///


        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///

        protected override void Awake()
        {
            // オブジェクトプールを生成・初期化
            damageEnemyPool = new GameObject("DamagePool(Enemy)").AddComponent<PoolManager>();
            damageEnemyPool.InitializePool(poolDefaultCapacity, poolMaxSize);
            criticalEnemyPool = new GameObject("CriticalPool(Enemy)").AddComponent<PoolManager>();
            criticalEnemyPool.InitializePool(poolDefaultCapacity, poolMaxSize);
            damagePlayerPool = new GameObject("DamagePool(Player)").AddComponent<PoolManager>();
            damagePlayerPool.InitializePool(poolDefaultCapacity, poolMaxSize);
            criticalPlayerPool = new GameObject("CriticalPool(Player)").AddComponent<PoolManager>();
            criticalPlayerPool.InitializePool(poolDefaultCapacity, poolMaxSize);
        }

        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///



        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class DamageUIManager
    {
        /// --------変数一覧-------- ///

        #region public変数
        /// -------public変数------- ///

        public static PoolManager damageEnemyPool; // 通常ダメージプール
        public static PoolManager criticalEnemyPool; // クリティカルダメージプール
        public static PoolManager damagePlayerPool; // 通常ダメージプール
        public static PoolManager criticalPlayerPool; // クリティカルダメージプール

        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///



        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///

        [SerializeField] private int poolDefaultCapacity = 20; // オブジェクトプールのデフォルト容量
        [SerializeField] private int poolMaxSize = 30; // オブジェクトプールの最大容量

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///


        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}