using UnityEngine;
using System.Collections.Generic;

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
            SetTriggerColliders();
        }

        private void OnEnable()
        {
            SetTriggerColliders();
        }
        
        /// <summary>
        /// 自身のタグに応じてセットするコライダーを変える関数
        /// </summary>
        private void SetTriggerColliders()
        {
            // 自身のパーティクルシステムを取得してトリガーを取得する
            ParticleSystem ps = GetComponent<ParticleSystem>();
            ParticleSystem.TriggerModule trigger = ps.trigger;

            // タグによって初期化を変える
            if (tag == Tags.PlayerWepon.ToString()) InitializeTriggerModule(ps, ParticleSystemTrigger.E_PlayerWepon);
            else if (tag == Tags.EnemyWepon.ToString()) InitializeTriggerModule(ps, ParticleSystemTrigger.E_EnemyWepon);
        }

        /// <summary>
        /// パーティクルシステムのトリガーを初期化する関数
        /// </summary>
        /// <param name="ps">初期化したいパーティクルシステム</param>
        /// <param name="e">プレイヤーかエネミーの武器</param>
        private void InitializeTriggerModule(ParticleSystem ps, ParticleSystemTrigger e)
        {
            // GameManagerで設定したコライダーを代入
            List<Collider> colliders = GameManager.GetParticleTriggerColliderGroup[(int)e].targetColliders;
            for (int i = 0; i < colliders.Count; i++)
            {
                ps.trigger.SetCollider(i, colliders[i]);
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


        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///
    
    
    
        /// -------プロパティ------- ///
        #endregion
    
        /// --------変数一覧-------- ///
    }
}