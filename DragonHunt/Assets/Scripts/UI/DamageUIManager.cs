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
        /*public void PopDamageUI()
        {
            // ターゲットの子オブジェクトとdamageUIを生成
            GameObject obj = new GameObject("Damage");
            GameObject ui = Instantiate(damageText);

            // 子オブジェクトに設定
            obj.transform.SetParent(target.transform);
            ui.transform.SetParent(canvas.transform);

            // UIの位置更新のためにトランスフォームを代入
            ui.GetComponent<PopUIUpdate>().SetTarget = obj.transform;

            // UIを表示
            ui.SetActive(true);

            // 半径viewRadiusの内ランダムなX,Y座標を取得し、ターゲットの上部の座標を設定する
            Vector2 circlePos = Random.insideUnitCircle * viewRadius;
            obj.transform.position = transform.position + Vector3.up * Random.Range(viewHeightMin, viewHeightMax) + new Vector3(circlePos.x, 0, circlePos.y);

            // UIをobjの位置に移動させる
            ui.GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, obj.transform.position);

            // 表示を消す
            Destroy(obj, 2.0f);
            Destroy(ui, 2.0f);
        }*/

        /// <summary>
        /// ダメージUIをポップさせる関数
        /// </summary>
        /// <param name="damage">表示したいダメージ</param>
        /// <param name="isCritical">クリティカルかどうか</param>
        public void PopDamageUI(float damage, bool isCritical)
        {
            // tarオブジェクトが存在しない場合
            if (!tar)
            {
                // 新しいターゲットオブジェクトとサブキャンパスを生成
                tar = new GameObject("Tar");
                subCan = Instantiate(subCanvas);

                // UIの位置更新のためにトランスフォームを代入
                subCan.GetComponent<PopUIUpdate>().SetTarget = tar.transform;

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
            GameObject ui;
            if (isCritical) ui = Instantiate(criticalDamageText);
            else ui = Instantiate(damageText);

            // 新しいUIをsubCanの子オブジェクトとして設定
            ui.transform.SetParent(subCan.transform);

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
            Destroy(ui, 2f);
        }

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///



        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///



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

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}