using System.Collections;
using UnityEngine;

namespace Misaki
{
    public partial class HitStopManager : MonoBehaviour
    {
        /// --------関数一覧-------- ///

        #region public関数
        /// -------public関数------- ///

        /// <summary>
        /// ヒットストップを開始する関数
        /// </summary>
        /// <param name="anim">止めたいアニメーター/param>
        /// <param name="duration">止める秒数</param>
        /// <param name="isCameraShake">カメラを揺らすかどうか</param>
        public void StartHitStop(Animator anim, float duration, bool isCameraShake = false)
        {
            StartCoroutine(HitStopCoroutine(anim, duration, isCameraShake));
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
            hitStop = this;
        }

        /// <summary>
        /// ヒットストップさせるコルーチン
        /// </summary>
        /// <param name="anim">止めたいアニメーター/param>
        /// <param name="duration">止める秒数</param>
        /// <param name="isCameraShake">カメラを揺らすかどうか</param>
        /// <returns></returns>
        private IEnumerator HitStopCoroutine(Animator anim, float duration, bool isCameraShake = false)
        {
            // ヒットストップ中なら処理を中断する
            if(isHitStop) yield break;
            isHitStop = true;

            // アニメーターのスピードを保持する
            float animSpeed = anim.speed;

            // チャンスタイムではない場合
            if (!GameManager.GetIsChanceTime)
            {
                // ヒットストップの開始
                anim.speed = 0.04f;
            }

            // キャラクター及びカメラを揺らす
            StartCoroutine(ShakeCoroutine(anim.transform, duration, 0.2f));
            if (isCameraShake) StartCoroutine(ShakeCameraCoroutine(2f, 8, duration));

            // チャンスタイムではない場合
            if (!GameManager.GetIsChanceTime)
            {
                // 指定した時間だけ停止
                yield return new WaitForSecondsRealtime(duration);

                // ヒットストップの終了
                anim.speed = animSpeed;
            }

            isHitStop = false;
        }

        /// <summary>
        /// キャラクターを揺らすコルーチン
        /// </summary>
        /// <param name="target">揺らしたいキャラクターのトランスフォーム</param>
        /// <param name="duration">揺らす秒数</param>
        /// <param name="magnitude">振動の掛け率</param>
        /// <returns></returns>
        private IEnumerator ShakeCoroutine(Transform target, float duration, float magnitude)
        {
            // 元の位置を保存
            Vector3 originalPosition = target.localPosition; 

            // 経過時間変数
            float elapsed = 0.0f;

            // 揺らす時間より経過時間が小さければ揺らす
            while (elapsed < duration)
            {
                // ランダムなオフセットを計算
                float offsetX = Random.Range(-1f, 1f) * magnitude;
                float offsetY = Random.Range(-1f, 1f) * magnitude;
                float offsetZ = Random.Range(-1f, 1f) * magnitude;

                // 新しい位置に適用
                target.localPosition = originalPosition + new Vector3(offsetX, offsetY, offsetZ);

                // 経過時間を更新
                elapsed += Time.deltaTime;

                // フレームの終了を待つ
                yield return null;
            }

            // 揺れが終了したら元の位置に戻す
            target.localPosition = originalPosition;
        }

        /// <summary>
        /// カメラを揺らすコルーチン
        /// </summary>
        /// <param name="width">揺れ幅</param>
        /// <param name="count">揺らす回数</param>
        /// <param name="duration">揺らす秒数</param>
        /// <returns></returns>
        private IEnumerator ShakeCameraCoroutine(float width, int count, float duration)
        {
            // メインカメラを取得し、元のローテーションを保持する
            Transform camera = Camerawork.GetCurrentCameraTransform;//GameManager.GetCameraTransform;
            Quaternion originalRotation = camera.localRotation;

            // 振れ演出の片道の揺れ分の時間
            var partDuration = duration / count / 2f;
            // 振れ幅の半分の値
            var widthHalf = width / 2f;

            for (int i = 0; i < count - 1; i++)
            {
                // 相対的にカメラを左に回転させる
                yield return RotateCamera(camera, originalRotation, new Vector3(-widthHalf, 0f, 0f), partDuration);
                // 相対的にカメラを右に回転させる
                yield return RotateCamera(camera, originalRotation, new Vector3(widthHalf, 0f, 0f), partDuration);
            }

            // 最後の揺れは元の角度に戻す工程
            yield return RotateCamera(camera, originalRotation, Vector3.zero, partDuration);

            // カメラの回転を元に戻す
            camera.localRotation = originalRotation;
        }

        /// <summary>
        /// カメラを回転させる関数
        /// </summary>
        /// <param name="camera">カメラのトランスフォーム</param>
        /// <param name="originalRotation">元のカメラローテーション</param>
        /// <param name="targetOffset">最終的なローテーション座標</param>
        /// <param name="duration">回転させる秒数</param>
        /// <returns></returns>
        private IEnumerator RotateCamera(Transform camera, Quaternion originalRotation, Vector3 targetOffset, float duration)
        {
            Quaternion startRotation = camera.localRotation;
            Quaternion endRotation = originalRotation * Quaternion.Euler(targetOffset); // 元の回転に対してオフセットを加える

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                // 線形補間で回転を徐々に変更
                camera.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
                yield return null;
            }

            // 最終的な回転をターゲットに合わせる
            camera.localRotation = endRotation;
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class HitStopManager
    {
        /// --------変数一覧-------- ///

        #region public変数
        /// -------public変数------- ///

        public static HitStopManager hitStop; // インスタンス

        /// -------public変数------- ///
        #endregion

        #region protected変数
        /// -----protected変数------ ///



        /// -----protected変数------ ///
        #endregion

        #region private変数
        /// ------private変数------- ///

        private bool isHitStop = false; // ヒットストップしているかどうか

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}