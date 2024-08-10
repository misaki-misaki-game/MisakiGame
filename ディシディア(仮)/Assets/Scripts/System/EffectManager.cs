using UnityEngine;

namespace Misaki
{
    public partial class EffectManager : SingletonMonoBehaviour<EffectManager>
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
            base.Awake();
            // static変数に代入　インスペクター上にstatic変数が表示されないため
            braveDamageEffect = braveDamage;
            hpDamageEffect = hpDamage;
            hpShockWaveEffect = hpShockWave;

            // 遠距離攻撃のアタックスクリプトを代入
            hpShockWaveAttack = hpShockWaveEffect.GetComponentInChildren<AttackScript>();

            // 各エフェクトのプールを生成する
            braveDamageEffectPool = new GameObject("BraveDamageEffectPool").AddComponent<PoolManager>();
            hpDamageEffectPool = new GameObject("HPDamageEffectPool").AddComponent<PoolManager>();
            hpShockWaveEffectPool = new GameObject("HPShockWaveEffectPool").AddComponent<PoolManager>();
            braveDamageEffectPool.InitializePool(poolDefaultCapacity, poolMaxSize, braveDamageEffect);
            hpDamageEffectPool.InitializePool(poolDefaultCapacity, poolMaxSize, hpDamageEffect);
            hpShockWaveEffectPool.InitializePool(poolDefaultCapacity, poolMaxSize, hpShockWaveEffect);
        }

        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///



        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class EffectManager
    {
        /// --------変数一覧-------- ///

        #region public変数
        /// -------public変数------- ///

        public static GameObject braveDamageEffect; // 被ブレイブダメージのエフェクト
        public static GameObject hpDamageEffect; // 被HPダメージのエフェクト
        public static GameObject hpShockWaveEffect; // HP攻撃のエフェクト

        public static PoolManager braveDamageEffectPool; // 非ブレイブダメージ時のエフェクトプール
        public static PoolManager hpDamageEffectPool; // 非HPダメージ時のエフェクトプール
        public static PoolManager hpShockWaveEffectPool; // HP攻撃のエフェクトプール

        public static AttackScript hpShockWaveAttack; // HP攻撃のアタックスクリプト

        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///



        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///

        [Header("エフェクトを入れてください")]
        [SerializeField] private GameObject braveDamage; // 被ブレイブダメージのエフェクト
        [SerializeField] private GameObject hpDamage; // 被HPダメージのエフェクト
        [SerializeField] private GameObject hpShockWave; // HP攻撃のエフェクト

        [SerializeField] private int poolDefaultCapacity; // オブジェクトプールのデフォルト容量
        [SerializeField] private int poolMaxSize; // オブジェクトプールの最大容量

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///


        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}