using UnityEngine;
using UnityEngine.Pool;

namespace Misaki
{
    public partial class PoolManager : MonoBehaviour
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- /// 

        /// <summary>
        /// オブジェクトをオブジェクトプールから取り出す関数
        /// </summary>
        /// <param name="prefab">オブジェクト</param>
        /// <param name="position">指定したい位置</param>
        /// <param name="rotation">指定したい向き</param>
        /// <returns></returns>
        public GameObject GetGameObject(GameObject prefab, PoolType poolType, Vector3 position, Quaternion rotation, Transform parent)
        {
            Prefab = prefab; // 指定のオブジェクトをプレハブに代入

            GameObject obj = pool.Get(); // オブジェクトプールからオブジェクトを取り出す
            if (poolType == PoolType.E_Effect) obj.GetComponent<PooledEffectObject>().SetPoolManager = this;

            obj.transform.SetParent(parent); // 親オブジェクトを代入

            // トランスフォームを代入
            Transform tf = obj.transform;
            tf.position = position;
            tf.rotation = rotation;

            return obj; // 返す
        }

        /// <summary>
        /// プールに指定のオブジェクトを戻す関数
        /// </summary>
        /// <param name="obj">プールに戻したいオブジェクト</param>
        public void ReleaseGameObject(GameObject obj)
        {
            pool.Release(obj);
        }

        /// <summary>
        /// プールを初期化する関数
        /// オブジェクトの指定があればオブジェクトを生成する
        /// </summary>
        /// <param name="defaultCapacity">初期容量</param>
        /// <param name="maxSize">最大容量</param>
        /// <param name="obj">プール化したいオブジェクト</param>
        public void InitializePool(int defaultCapacity, int maxSize, GameObject obj = null)
        {
            // プールを生成
            pool = new ObjectPool<GameObject>(OnCreatePooledObject, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, false, defaultCapacity, maxSize);
        }

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///



        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///

        /// <summary>
        /// オブジェクトプールに指定のオブジェクトがない際の処理関数
        /// </summary>
        /// <returns></returns>
        private GameObject OnCreatePooledObject()
        {
            return Instantiate(Prefab); // Prefabを生成して返す
        }

        /// <summary>
        /// オブジェクトプールから指定のオブジェクトを取り出した際の処理関数
        /// </summary>
        /// <param name="obj">指定のオブジェクト</param>
        private void OnGetFromPool(GameObject obj)
        {
            obj.SetActive(true); // オブジェクトを表示する
        }

        /// <summary>
        /// オブジェクトプールに指定のオブジェクトが戻ってきた際の処理関数
        /// </summary>
        /// <param name="obj">指定のオブジェクト</param>
        private void OnReleaseToPool(GameObject obj)
        {
            obj.SetActive(false); // オブジェクトを非表示にする
        }

        /// <summary>
        /// オブジェクトプールの許容量を超えた際にオブジェクトを破壊する処理関数
        /// </summary>
        /// <param name="obj">指定のオブジェクト</param>
        private void OnDestroyPooledObject(GameObject obj)
        {
            Destroy(obj); // オブジェクトを破壊する
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class PoolManager
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

        ObjectPool<GameObject> pool; // オブジェクトプール

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///

        // プレハブのゲッターセッター関数
        public GameObject Prefab { get; private set; }


        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}