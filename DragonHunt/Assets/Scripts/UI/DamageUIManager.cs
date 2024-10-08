using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Misaki
{
    [System.Serializable]
    public partial class DamageUIManager : MonoBehaviour
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        /// <summary>
        /// ダメージUIをポップさせる関数
        /// </summary>
        /// <param name="damage">表示したいダメージ</param>
        /// <param name="isCritical">クリティカルかどうか</param>
        public IEnumerator PopDamageUI(float damage, bool isCritical)
        {
            // tarオブジェクトが存在しない場合
            if (!tar)
            {
                // 新しいターゲットオブジェクトとサブキャンパスを生成
                tar = new GameObject("Tar");
                subCan = Instantiate(subCanvas);

                // UIの位置更新のためにトランスフォームを代入
                subCan.GetComponent<UIUpdate>().SetTarget = tar.transform;

                // サブキャンバスをアクティブ化して表示可能にする
                subCan.SetActive(true);

                // tarをtargetの子オブジェクトとして設定する
                // subCanをメインキャンバス（canvas）の子オブジェクトとして設定する
                tar.transform.SetParent(target.transform);
                subCan.transform.SetParent(canvas.transform);

                // tarの位置をこのオブジェクトの位置から少し上に移動（Y軸方向に2.0）
                tar.transform.position = transform.position + Vector3.up * 2.0f;
            }

            // ダメージテキストを生成
            // 新しいUIをsubCanの子オブジェクトとして設定
            GameObject ui;
            if (isCritical) ui = criticalPool.GetGameObject(criticalDamageText, PoolType.E_DamageText, Vector3.zero, Quaternion.identity, subCan.transform);
            else ui = damagePool.GetGameObject(damageText, PoolType.E_DamageText, Vector3.zero, Quaternion.identity, subCan.transform);

            // 半径viewRadiusの内ランダムなX,Y座標を取得し、ターゲットの上部の座標を設定する
            // UIのRectTransformの位置を変更する
            Vector2 circlePos = Random.insideUnitCircle * viewRadius;
            ui.GetComponent<RectTransform>().position = subCan.transform.position + Vector3.up * Random.Range(viewHeightMin, viewHeightMax) + new Vector3(circlePos.x, circlePos.y, 0);

            // テキストにダメージを代入
            // UIをアクティブにして表示
            if (isCritical) ui.GetComponent<TextMeshProUGUI>().text = damage.ToString() + "!!";
            else ui.GetComponent<TextMeshProUGUI>().text = damage.ToString();
            ui.SetActive(true);

            // 2秒後にUIオブジェクトを削除（自動で消える）
            yield return new WaitForSeconds(2.0f);
            if (isCritical)criticalPool.ReleaseGameObject(ui);
            else damagePool.ReleaseGameObject(ui);
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
            // オブジェクトプールを生成・初期化
            damagePool = new GameObject("DamagePool").AddComponent<PoolManager>(); 
            damagePool.InitializePool(poolDefaultCapacity, poolMaxSize, damageText);
            criticalPool = new GameObject("CriticalPool").AddComponent<PoolManager>();
            criticalPool.InitializePool(poolDefaultCapacity, poolMaxSize, criticalDamageText);
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class DamageUIManager
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

        [SerializeField] private float viewRadius = 100f; // ダメージテキストを表示する半径
        [SerializeField] private float viewHeightMax = 40f; // ダメージテキストを表示する高さ上限
        [SerializeField] private float viewHeightMin = 0f; // ダメージテキストを表示する高さ下限

        private GameObject tar; // 変更用ターゲット変数
        private GameObject subCan; // 変更用ターゲット変数
        [SerializeField] private GameObject target; // ターゲット
        [SerializeField] private GameObject canvas; // キャンパス
        [SerializeField] private GameObject subCanvas; // サブキャンパス
        [SerializeField] private GameObject damageText; // ダメージテキスト
        [SerializeField] private GameObject criticalDamageText; // クリティカルダメージテキスト

        private PoolManager damagePool; // 通常ダメージプール
        private PoolManager criticalPool; // クリティカルダメージプール

        [SerializeField] private int poolDefaultCapacity = 20; // オブジェクトプールのデフォルト容量
        [SerializeField] private int poolMaxSize = 30; // オブジェクトプールの最大容量

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}