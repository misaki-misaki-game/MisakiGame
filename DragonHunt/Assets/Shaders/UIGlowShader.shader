Shader "Unlit/UIGlowShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // �e�N�X�`��
        _GlowColor ("Glow Color", Color) = (1, 1, 1, 1) // �F
        _GlowIntensity ("Glow Intensity", Range(0, 1)) = 0.5 // ���x
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha // �����x�̒���
        Cull Off ZWrite Off // ���Ԃ�Ȃ��悤��Z�ɂ͏������܂Ȃ�
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _GlowColor;
            float _GlowIntensity;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb += _GlowColor.rgb * _GlowIntensity;
                return col;
            }
            ENDCG
        }
    }
}
