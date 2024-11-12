using Cinemachine;
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
        /// <param name="bone">揺らしたいキャラクタートランスフォーム</param>
        /// <param name="isCameraShake">カメラを揺らすかどうか</param>
        public void StartHitStop(Animator anim, float duration, bool isCameraShake = false, Transform bone = null)
        {
            // カメラ関係を初期化
            virtualCamera = camerawork.GetCurrentCamera;
            noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            // ヒットストップ開始
            if (bone != null)
            {
                StartCoroutine(HitStopCoroutine(anim, duration, isCameraShake, bone));
            }
            else
            {
                StartCoroutine(HitStopCoroutine(anim, duration, isCameraShake));
            }
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
            //hitStop = this;
            camerawork = GameManager.GetCamerawork;
        }

        /// <summary>
        /// ヒットストップさせるコルーチン
        /// </summary>
        /// <param name="anim">止めたいアニメーター/param>
        /// <param name="duration">止める秒数</param>
        /// <param name="bone">揺らしたいキャラクタートランスフォーム</param>
        /// <param name="isCameraShake">カメラを揺らすかどうか</param>
        /// <returns></returns>
        private IEnumerator HitStopCoroutine(Animator anim, float duration, bool isCameraShake = false, Transform bone = null)
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
            if (bone != null)
            {
                StartCoroutine(ShakeCoroutine(bone, duration, 0.15f));
            }
            if (isCameraShake)
            {
                StartCoroutine(ShakeCameraCoroutine(duration));
            }

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
        /// <param name="target">揺らしたいキャラクターのボーントランスフォーム</param>
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
        /// <param name="shakeTimer">揺らす秒数</param>
        private IEnumerator ShakeCameraCoroutine(float shakeTimer)
        {
            // 揺れの強さ速さを設定
            noise.m_AmplitudeGain = shakeAmplitude;
            noise.m_FrequencyGain = shakeFrequency;

            // 指定時間揺らす
            while (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;
                yield return null;
            }

            // シェイクが終了したら振幅と周波数をリセット
            noise.m_AmplitudeGain = 0;
            noise.m_FrequencyGain = 0;
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

        [SerializeField] private float shakeAmplitude = 2f; // 揺れの強さ
        [SerializeField] private float shakeFrequency = 2f; // 揺れの速さ

        [SerializeField] private CinemachineVirtualCamera virtualCamera; // 現在のカメラ
        [SerializeField] private CinemachineBasicMultiChannelPerlin noise; // カメラのノイズ

        [SerializeField] private Camerawork camerawork; // カメラワーク変数

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}