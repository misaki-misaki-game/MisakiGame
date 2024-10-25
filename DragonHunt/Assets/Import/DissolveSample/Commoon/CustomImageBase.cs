// Copyright (C) 2022 Hatayama Masamichi
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Applibot
{
    [ExecuteAlways]  // 実行中、またはエディタ上での変更時にもこのクラスが有効
    [RequireComponent(typeof(Graphic))]  // このコンポーネントが存在するには Graphic が必須
    public class CustomImageBase : MonoBehaviour, IMaterialModifier
    {
        // Graphicコンポーネントのキャッシュ用変数
        [NonSerialized] private Graphic _graphic;

        // Imageコンポーネントのキャッシュ用変数
        [NonSerialized] private Image _image;

        // マテリアルの参照を保持する変数
        protected Material material;

        // CanvasScalerのキャッシュ用変数
        private CanvasScaler _canvasScaler;

        // シェーダーで使用するテクスチャの矩形情報を表すプロパティID
        private int _textureRectId = Shader.PropertyToID("_textureRect");

        // CanvasScaler プロパティ。CanvasScaler がキャッシュされていない場合、取得してキャッシュ
        protected CanvasScaler canvasScaler
        {
            get
            {
                if (_canvasScaler == null)
                {
                    // Graphic のキャンバスから CanvasScaler を取得
                    _canvasScaler = graphic.canvas.GetComponent<CanvasScaler>();
                }
                return _canvasScaler;
            }
        }

        // Graphic プロパティ。Graphic がキャッシュされていない場合、取得してキャッシュ
        public Graphic graphic
        {
            get
            {
                if (_graphic == null)
                {
                    _graphic = GetComponent<Graphic>();
                }
                return _graphic;
            }
        }

        // マテリアルを変更するためのインターフェース実装。元のマテリアルを渡して処理を行う
        public Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!isActiveAndEnabled || graphic == null)
            {
                // 無効または Graphic がない場合はベースマテリアルをそのまま返す
                return baseMaterial;
            }

            // マテリアルを更新
            UpdateMaterial(baseMaterial);
            // アトラス情報を設定
            SetAtlasInfo();
            return material;
        }

        // アニメーションプロパティが適用された後の処理
        private void OnDidApplyAnimationProperties()
        {
            if (!isActiveAndEnabled || graphic == null)
            {
                return;
            }

            // マテリアルが変更されたことを通知
            graphic.SetMaterialDirty();
        }

        // 派生クラスでオーバーライドされる、マテリアルの更新を行うメソッド
        protected virtual void UpdateMaterial(Material baseMaterial)
        {
            // 派生クラスでの実装を想定
        }

        // アトラス情報を設定するメソッド
        private void SetAtlasInfo()
        {
            // Imageコンポーネントがない場合は終了
            if (_image == null)
            {
                return;
            }

            // スプライトがアトラスにパックされていない場合、キーワードを無効化
            if (!_image.sprite.packed)
            {
                material.DisableKeyword("USE_ATLAS");
                return;
            }

            // スプライトのテクスチャ矩形情報を取得
            Rect textureRect = _image.sprite.textureRect;
            Vector4 r = new Vector4(
                textureRect.x,
                textureRect.y,
                textureRect.width,
                textureRect.height);
            // シェーダーにテクスチャ矩形情報を設定
            material.SetVector(_textureRectId, r);
            material.EnableKeyword("USE_ATLAS");
        }

        // コンポーネントが有効になった際の処理
        protected void OnEnable()
        {
            if (graphic == null)
            {
                return;
            }

            // GraphicをImageにキャスト
            _image = graphic as Image;
            // マテリアルを更新する必要があることを通知
            graphic.SetMaterialDirty();
        }

        // コンポーネントが無効になった際の処理
        protected void OnDisable()
        {
            if (material != null)
            {
                // マテリアルを破棄
                DestroyMaterial();
            }

            if (graphic != null)
            {
                // マテリアルの更新を通知
                graphic.SetMaterialDirty();
            }
        }

        // コンポーネントが破棄された際の処理
        protected void OnDestroy()
        {
            if (material != null)
            {
                // マテリアルを破棄
                DestroyMaterial();
            }
        }

        // マテリアルを破棄するメソッド
        public void DestroyMaterial()
        {
#if UNITY_EDITOR
            // エディタ内で再生中でない場合、即座にマテリアルを破棄
            if (EditorApplication.isPlaying == false)
            {
                DestroyImmediate(material);
                material = null;
                return;
            }
#endif
            // 再生中の場合、通常のマテリアル破棄処理
            Destroy(material);
            material = null;
        }

#if UNITY_EDITOR
        // エディタで検証時の処理
        protected void OnValidate()
        {
            if (!isActiveAndEnabled || graphic == null)
            {
                return;
            }

            // マテリアルの更新を通知
            graphic.SetMaterialDirty();
        }
#endif
    }
}