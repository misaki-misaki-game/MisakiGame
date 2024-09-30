using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Misaki
{
    public partial class PopUIUpdate : MonoBehaviour
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

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            // UIの位置を更新する
            rectTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, target.position) + offset;
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class PopUIUpdate
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

        private RectTransform rectTransform; // UIの座標

        private Transform target; // ターゲットの座標

        [SerializeField] private Vector2 offset = Vector2.zero; // オフセット

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///
    
        public Transform SetTarget { set { target = value; } }
    
        /// -------プロパティ------- ///
        #endregion
    
        /// --------変数一覧-------- ///
    }
}