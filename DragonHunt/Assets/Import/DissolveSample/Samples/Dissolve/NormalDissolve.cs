// Copyright (C) 2022 Hatayama Masamichi
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

using UnityEngine;

namespace Applibot
{
    public class NormalDissolve : CustomImageBase
    {
        // ディゾルブエフェクト用のテクスチャを指定する変数
        [SerializeField] private Texture2D _dissovleTex;

        // ディゾルブの進行度を制御するための変数（0で未ディゾルブ、1で完全ディゾルブ）
        [SerializeField, Range(0, 1)] private float _dissolveAmount;

        // ディゾルブの範囲を設定する変数（この値が高いほどディゾルブ範囲が広がる）
        [SerializeField, Range(0, 1)] private float _dissolveRange;

        // 発光効果を加えるための色を指定する（HDRカラーを使用可能）
        [SerializeField, ColorUsage(false, true)]
        private Color _glowColor;

        // シェーダーのプロパティIDをキャッシュするための変数
        private int _dissolveTexId = Shader.PropertyToID("_DissolveTex");
        private int _dissolveRangeId = Shader.PropertyToID("_DissolveRange");
        private int _dissolveAmountId = Shader.PropertyToID("_DissolveAmount");
        private int _glowColorId = Shader.PropertyToID("_GlowColor");

        // カスタムUI画像のマテリアルを更新するメソッド
        protected override void UpdateMaterial(Material baseMaterial)
        {
            // マテリアルがまだない場合、新しく作成する
            if (material == null)
            {
                // "Applibot/UI/NormalDissolve" シェーダーを見つけて新しいマテリアルを作成
                Shader s = Shader.Find("Applibot/UI/NormalDissolve");
                material = new Material(s);

                // ベースとなるマテリアルのプロパティをコピー
                material.CopyPropertiesFromMaterial(baseMaterial);

                // マテリアルを非表示かつ保存しないように設定
                material.hideFlags = HideFlags.HideAndDontSave;
            }

            // シェーダーのプロパティに対応する値を設定
            material.SetTexture(_dissolveTexId, _dissovleTex);  // ディゾルブテクスチャを設定
            material.SetFloat(_dissolveAmountId, _dissolveAmount);  // ディゾルブ進行度を設定
            material.SetFloat(_dissolveRangeId, _dissolveRange);  // ディゾルブ範囲を設定
            material.SetColor(_glowColorId, _glowColor);  // 発光色を設定
        }
    }
}