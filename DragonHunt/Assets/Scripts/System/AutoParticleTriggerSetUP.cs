using UnityEngine;

namespace Misaki
{
    public partial class AutoParticleTriggerSetUP : MonoBehaviour
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

        private void Start()
        {
            // 自身のパーティクルシステムを取得してトリガーを取得する
            ParticleSystem ps = GetComponent<ParticleSystem>();
            var trigger = ps.trigger;

            // タグで特定のオブジェクトのコライダーを自動取得
            GameObject target = GameObject.FindWithTag(hitTarget.ToString());
            if (target != null)
            {
                Collider col = target.GetComponent<Collider>();
                if (col != null)
                {
                    trigger.SetCollider(0, col); // コライダーを設定
                }
            }
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class AutoParticleTriggerSetUP
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

        [SerializeField] private Tags hitTarget; // ヒットする目標

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///
    
    
    
        /// -------プロパティ------- ///
        #endregion
    
        /// --------変数一覧-------- ///
    }
}