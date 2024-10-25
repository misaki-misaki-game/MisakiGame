using UnityEngine;
using TMPro;

namespace Misaki
{
    public partial class TextMeshProAnimator : MonoBehaviour
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
            // Startで一度だけ取得
            textComponent = GetComponent<TMP_Text>();
            textInfo = textComponent.textInfo;

            // メッシュの更新
            textComponent.ForceMeshUpdate();
        }

        private void Update()
        {
           UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            textComponent.ForceMeshUpdate(true);
            textInfo = textComponent.textInfo;

            WaveLoop();
            GradationColor();
        }

        private void GradationColor()
        {
            // ②頂点データを編集した配列の作成
            int count = Mathf.Min(textInfo.characterCount, textInfo.characterInfo.Length);
            for (int i = 0; i < count; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                    continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                // Gradient
                Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

                float timeOffset = -0.5f * i;
                float time1 = Mathf.PingPong(timeOffset + Time.realtimeSinceStartup, 1.0f);
                float time2 = Mathf.PingPong(timeOffset + Time.realtimeSinceStartup - 0.1f, 1.0f);

                colors[vertexIndex + 0] = gradientColor.Evaluate(time1); // 左下
                colors[vertexIndex + 1] = gradientColor.Evaluate(time1); // 左上
                colors[vertexIndex + 2] = gradientColor.Evaluate(time2); // 右上
                colors[vertexIndex + 3] = gradientColor.Evaluate(time2); // 右下
            }

            // ③ メッシュを更新
            for (int i = 0; i < textInfo.materialCount; i++)
            {
                if (textInfo.meshInfo[i].mesh == null) continue;

                textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
                textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
        }
        private void WaveLoop()
        {
            // ② 頂点データを編集した配列の作成
            var count = Mathf.Min(textInfo.characterCount, textInfo.characterInfo.Length);
            for (int i = 0; i < count; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                    continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;
                // Wave
                Vector3[] verts = textInfo.meshInfo[materialIndex].vertices;

                float sinWaveOffset = 2f * i;
                float sinWave = Mathf.Sin(sinWaveOffset + Time.realtimeSinceStartup * Mathf.PI);
                verts[vertexIndex + 0].y += sinWave;
                verts[vertexIndex + 1].y += sinWave;
                verts[vertexIndex + 2].y += sinWave;
                verts[vertexIndex + 3].y += sinWave;
            }

            // ③ メッシュを更新
            for (int i = 0; i < textInfo.materialCount; i++)
            {
                if (this.textInfo.meshInfo[i].mesh == null) { continue; }

                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;  // 変更
                textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
        }

        /// ------private関数------- ///
        #endregion

        /// --------関数一覧-------- ///
    }
    public partial class TextMeshProAnimator
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
        
        [SerializeField] private Gradient gradientColor;
        private TMP_Text textComponent;
        private TMP_TextInfo textInfo;

        /// ------private変数------- ///
        #endregion

        #region プロパティ
        /// -------プロパティ------- ///



        /// -------プロパティ------- ///
        #endregion

        /// --------変数一覧-------- ///
    }
}