using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Misaki
{
    public partial class FlashingUI : MonoBehaviour
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
            // テキストを取得
            textMeshProUGUI= GetComponent<TextMeshProUGUI>();

            // カラーの取得
            color= textMeshProUGUI.color;
        }

        private void Update()
        {
            if(isCountUp)
            {
                color.a += Time.deltaTime;
                if(color.a>=1) isCountUp = false;
            }
            else
            {
                color.a -= Time.deltaTime;
                if (color.a <= flashingMin) isCountUp = true;

            }
            textMeshProUGUI.color = color;
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class FlashingUI
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

        private bool isCountUp; // カラーのアルファ値をカウントアップさせるか

        [SerializeField] private float flashingMin; // 透明にさせる下限

        private TextMeshProUGUI textMeshProUGUI; // テキスト

        private Color color; // カラー変数

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///
    
    
    
        /// -------プロパティ------- ///
        #endregion
    
        /// --------変数一覧-------- ///
    }
}