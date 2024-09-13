using System;
using System.Collections.Generic;
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

            // 各エフェクトのプールを生成する
            effectGroups = new EffectGroup[effectObjects.Length]; // エフェクトの種類数の要素数で初期化
            EffectName name; // オブジェクトプール名を指定するために生成
            for (int i = 0; i<effectObjects.Length; i++)
            {
                name = (EffectName)i; // オブジェクトプール名を代入
                effectGroups[i] = new EffectGroup(); // インスタンス生成
                effectGroups[i].effect = effectObjects[i]; // エフェクトを代入
                effectGroups[i].pool= new GameObject(name.ToString() + "Pool").AddComponent<PoolManager>(); // オブジェクトプールをヒエラルキー上に生成
                effectGroups[i].pool.InitializePool(poolDefaultCapacity, poolMaxSize, effectGroups[i].effect); // オブジェクトプールを初期化
            }
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

        public static EffectGroup[] effectGroups; // エフェクトグループ配列

        // エフェクトグループ
        public class EffectGroup
        {
            public PoolManager pool; // プールマネージャー変数
            public GameObject effect; // エフェクト変数
            public GameObject effectPos; // エフェクト発生位置変数
        }

        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///



        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///

        [Header("エフェクトを入れてください"), SerializeField, EnumIndex(typeof(EffectName))]
        private GameObject[] effectObjects; // エフェクト配列

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