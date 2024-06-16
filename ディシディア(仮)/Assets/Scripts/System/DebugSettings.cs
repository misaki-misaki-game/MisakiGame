using UnityEngine;
namespace Misaki
{
    //ゲームの設定用ScriptableObject
    [CreateAssetMenu(menuName = "ScriptableObject/DebugSettings", fileName = "DebugSettings")]
    public class DebugSettings : ScriptableObject
    {
        /// --------変数一覧-------- ///

        #region public変数
        /// -------public変数------- ///

        [Header("デバッグログを表示したい場合はチェックをつける")]
        public bool debugLogEnabled; //デバッグログを表示するか

        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///



        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///



        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}