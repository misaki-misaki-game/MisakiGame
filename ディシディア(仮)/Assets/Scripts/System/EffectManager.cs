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

            // 各エフェクトのプールを生成する
            braveDamageEffectPool = new GameObject("EffectPool").AddComponent<PoolManager>();
            hpDamageEffectPool = new GameObject("EffectPool").AddComponent<PoolManager>();
            braveDamageEffectPool.InitializePool(poolDefaultCapacity, poolMaxSize, braveDamageEffect);
            hpDamageEffectPool.InitializePool(poolDefaultCapacity, poolMaxSize, hpDamageEffect);
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

        static public GameObject braveDamageEffect; // 被ブレイブ攻撃のエフェクト
        static public GameObject hpDamageEffect; // 被HP攻撃のエフェクト

        static public PoolManager braveDamageEffectPool; // 非ブレイブダメージ時のエフェクトプール
        static public PoolManager hpDamageEffectPool; // 非HPダメージ時のエフェクトプール

        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///



        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///

        [Header("エフェクトを入れてください")]
        [SerializeField] private GameObject braveDamage; // 被ブレイブ攻撃のエフェクト
        [SerializeField] private GameObject hpDamage; // 被HP攻撃のエフェクト

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