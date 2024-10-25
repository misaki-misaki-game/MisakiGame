// Copyright (C) 2022 Hatayama Masamichi
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

using UnityEngine;

namespace Applibot
{
    public class DissolveImage : CustomImageBase
    {
        // ディゾルブテクスチャを格納する変数
        [SerializeField] private Texture2D _dissovleTex;

        // 発光のための色を設定（HDRカラーが使える）
        [SerializeField, ColorUsage(false, true)]
        private Color _glowColor;

        // ディゾルブがどの高さ（Y座標）から開始するか
        [SerializeField, Range(0, 1)] private float _yAmount = 0.5f;

        // Y座標の範囲でどれだけの幅を持つか
        [SerializeField, Range(0, 1)] private float _yRange = 0.5f;

        // ディゾルブの進行度を示す変数（0で未ディゾルブ、1で完全ディゾルブ）
        [SerializeField, Range(0, 1)] private float _dissolveRange;

        // ディゾルブ時の歪み具合を制御
        [SerializeField, Range(0, 1)] private float _distortion = 0.1f;

        // テクスチャをスクロールさせる際のベクトル
        [SerializeField] private Vector2 _scroll = new Vector2(0, 0);

        // シェーダープロパティのIDをキャッシュするための変数（毎回名前で検索しないように最適化）
        private int _dissolveTexId = Shader.PropertyToID("_DissolveTex");
        private int _dissolveRangeId = Shader.PropertyToID("_DissolveRange");
        private int _yAmountId = Shader.PropertyToID("_YAmount");
        private int _yRangeId = Shader.PropertyToID("_YRange");
        private int _scrollId = Shader.PropertyToID("_Scroll");
        private int _glowColorId = Shader.PropertyToID("_GlowColor");
        private int _distortionId = Shader.PropertyToID("_Distortion");

        // カスタムUI画像のマテリアルを更新するメソッド
        protected override void UpdateMaterial(Material baseMaterial)
        {
            // マテリアルがまだない場合、新しく作成する
            if (material == null)
            {
                // "Applibot/UI/Dissolve" シェーダーを使って新しいマテリアルを作成
                material = new Material(Shader.Find("Applibot/UI/Dissolve"));
                material.hideFlags = HideFlags.HideAndDontSave;  // マテリアルを非表示かつ保存しないように設定
            }

            // シェーダーのプロパティに値を設定
            material.SetTexture(_dissolveTexId, _dissovleTex);  // ディゾルブテクスチャを設定
            material.SetFloat(_dissolveRangeId, _dissolveRange);  // ディゾルブ範囲を設定
            material.SetFloat(_yAmountId, _yAmount);  // Y軸開始位置を設定
            material.SetFloat(_yRangeId, _yRange);  // Y軸の範囲を設定
            material.SetVector(_scrollId, _scroll);  // スクロール量を設定
            material.SetColor(_glowColorId, _glowColor);  // 発光色を設定
            material.SetFloat(_distortionId, _distortion);  // 歪み具合を設定
        }
    }
}