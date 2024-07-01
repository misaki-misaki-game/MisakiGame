using Unity.VisualScripting;
using UnityEngine;

namespace Misaki
{
    // 自動的にコンポーネントを追加 MeshFilter,MeshRendererを追加
    [RequireComponent(typeof(CapsuleCollider))]
    public partial class AttackScript : MonoBehaviour
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
            // 自身のカプセルコライダーを取得し、無効化
            myCollider = GetComponent<CapsuleCollider>();
            //myCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider col)
        {
            // ステートによって処理を変える
            if (attack == AttackState.E_BraveAttack) AddBrave(col);
            else if (attack == AttackState.E_HPAttack) AddHP(col);
        }

        private void AddBrave(Collider col)
        {
            if (col.tag == Tags.Enemy.ToString())
            {
                Debug.Log("Brave攻撃が敵に当たった");
                // col.GetComponent<Enemy>().SetState(Enemy.EnemyState.Damage);
            }
        }

        private void AddHP(Collider col)
        {
            if (col.tag == Tags.Enemy.ToString())
            {
                Debug.Log("HP攻撃が敵に当たった");
                // col.GetComponent<Enemy>().SetState(Enemy.EnemyState.Damage);
            }
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- /// 
    }
    public partial class AttackScript
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

        private CapsuleCollider myCollider; // 自身のコライダー

        private AttackState attack = AttackState.E_None; // 攻撃の種類

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///
    
        // attackのセッター関数
        public AttackState SetAttackState { set { attack = value; } }
    
        /// -------プロパティ------- ///
        #endregion
    
        /// --------変数一覧-------- ///
    }
}