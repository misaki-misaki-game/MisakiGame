using UnityEngine;

namespace Misaki
{
    public partial class TrailController : MonoBehaviour
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        public void StartTrail(int trailNum)
        {
            if (trails[trailNum] != null)
            {
                TrailRenderer[] trailArray = trails[trailNum].GetTrailRenderer;
                foreach (TrailRenderer trail in trailArray) trail.emitting = true;  // TrailRendererの生成を開始
            }
        }

        public void StopTrail(int trailNum)
        {
            if (trails[trailNum] != null)
            {
                TrailRenderer[] trailArray = trails[trailNum].GetTrailRenderer;
                foreach (TrailRenderer trail in trailArray) trail.emitting = false;  // TrailRendererの生成を開始
            }
        }

        public void StopAllTrail()
        {
            // トレイルレンダラーを非表示にする
            for (int i = 0; i < trails.Length; i++) trails[i].InitializeTrail();
        }

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///



        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///

        private void Start()
        {
            // トレイルレンダラーを非表示にする
            for (int i = 0; i < trails.Length; i++) trails[i].InitializeTrail();
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class TrailController
    {
        /// --------変数一覧-------- ///

        #region public変数
        /// -------public変数------- ///

        /// <summary>
        /// トレイルクラス
        /// </summary>
        [System.Serializable]
        public class Trails
        {
            [SerializeField] private TrailRenderer[] trailRenderers; // トレイルレンダラー配列

            /// <summary>
            /// トレイルレンダラー初期化関数
            /// </summary>
            public void InitializeTrail()
            {
                foreach (TrailRenderer trail in trailRenderers) trail.emitting = false;
            }

            public TrailRenderer[] GetTrailRenderer { get { return trailRenderers; } }
        }

        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///



        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///

        [SerializeField] private Trails[] trails; // トレイルズクラスの配列

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}