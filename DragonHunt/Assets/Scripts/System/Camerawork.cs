using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

namespace Misaki
{
    public partial class Camerawork : MonoBehaviour
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        /// <summary>
        /// ロックオンのオンオフを切り替える関数
        /// </summary>
        public void Lockon()
        {
            // bool値を反転させる
            isLockon = !isLockon;

            // trueならロックする falseならロック解除
            if (isLockon)
            {
                List<GameObject> cameraAnchorList = GameManager.GetEnemyCameraAnchor;
                ActiveLockonCamera(cameraAnchorList[lockonNumber]);
            }
            else
            {
                InactiveLockonCamera();
            }
        }

        /// <summary>
        /// ロックオンしているターゲットを変更する関数
        /// </summary>
        /// <param name="value">ロック切り替えの昇順降順入力</param>
        public void ChangeTarget(float value)
        {
            // ロックオンしていないならリターン
            if (!isLockon) return;

            // ロックオン位置リストを取得
            List<GameObject> cameraAnchorList = GameManager.GetEnemyCameraAnchor;

            // エネミーの数が0の場合ロックオンカーソルを外す
            if (cameraAnchorList.Count == 0)
            {
                lockonCursor.SetActive(false);
                return;
            }
            // リストの要素数が1を超過しているなら
            else if (cameraAnchorList.Count > 1)
            {
                // イテレーターをずらす
                if (value <= 0)
                {
                    lockonNumber++;
                }
                else
                {
                    lockonNumber--;
                }
            }

            // ロックオン位置リストのアウトレンジにならないようにイテレーターを調整
            if (lockonNumber >= cameraAnchorList.Count)
            {
                lockonNumber = 0;
            }
            else if (lockonNumber < 0)
            {
                lockonNumber = cameraAnchorList.Count - 1;
            }

            // ターゲット切り替え
            ActiveLockonCamera(cameraAnchorList[lockonNumber]);
        }

        /// -------public関数------- ///
        #endregion

        #region protected関数
        /// -----protected関数------ ///



        /// -----protected関数------ ///
        #endregion

        #region private関数
        /// ------private関数------- ///

        private void Update()
        {
            // ロックオンカーソルを追尾させる
            if (isLockon && lockonCamera.LookAt != null)
            {
                lockonCursor.transform.position = Camera.main.WorldToScreenPoint(lockonCamera.LookAt.position);
            }
        }

        /// <summary>
        /// ロックオン時のVirtualCamera切り替え
        /// </summary>
        /// <param name="target">ロックオンしたいエネミー</param>
        private void ActiveLockonCamera(GameObject target)
        {
            // ロックオンカメラの優先度を最上位にして、ターゲットを代入する
            lockonCamera.Priority = lockonCameraActivePriority;
            lockonCamera.LookAt = target.transform;
            lockonTargetTransform= target.transform;
            currentCameraTransform = lockonCamera.transform;
            lockonCursor.SetActive(true);
        }

        /// <summary>
        /// ロックオン解除時のVirtualCamera切り替え
        /// </summary>
        private void InactiveLockonCamera()
        {
            // ロックオンカメラの優先度を最下位にして、ターゲットをはずす
            lockonCamera.Priority = lockonCameraInactivePriority;
            lockonCamera.LookAt = null;
            lockonTargetTransform = null;
            lockonCursor.SetActive(false);

            // 直前のLockonCameraの角度を引き継ぐ
            CinemachinePOV pov = freeLockCamera.GetCinemachineComponent<CinemachinePOV>();
            pov.m_VerticalAxis.Value = Mathf.Repeat(lockonCamera.transform.eulerAngles.x + 180, 360) - 180;
            pov.m_HorizontalAxis.Value = lockonCamera.transform.eulerAngles.y;
            currentCameraTransform = freeLockCamera.transform;
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class Camerawork
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

        private bool isLockon = false; // ロックオンしているか

        private int lockonNumber = 0; // ロックしている番号

        readonly int lockonCameraActivePriority = 21; // ロックオンカメラを優先する際の値
        readonly int lockonCameraInactivePriority = 0; // 優先しない際の値

        [SerializeField] private CinemachineVirtualCamera freeLockCamera; // フリーカメラ
        [SerializeField] private CinemachineVirtualCamera lockonCamera; // ロックオンカメラ

        private static Transform currentCameraTransform; // 現在使用しているカメラ
        private Transform lockonTargetTransform; // ロックオンしているターゲットのトランスフォーム

        [SerializeField] private GameObject lockonCursor; // ロックオンカーソル

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///

        public static Transform GetCurrentCameraTransform { get { return currentCameraTransform; } }

        public Transform GetLockonTargetTransform { get { return lockonTargetTransform; } }

        public CinemachineVirtualCamera GetFreeLockCamera { get { return freeLockCamera; } }

        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}