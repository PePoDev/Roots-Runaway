// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SmartDestruction2D/ModifierShader" {
    Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}
    }
    //Single pass for programmable pipelines
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        ZWrite Off
        ColorMask RGB
        Blend One OneMinusSrcAlpha
        Pass {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"
               
                struct appdata_tiny {
                    float4 vertex : POSITION;
                    float4 texcoord : TEXCOORD0;
                };
               
                struct v2f {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };
               
                uniform float4 _MainTex_ST;
               
                v2f vert (appdata_tiny v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos (v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                    return o;
                }
               
                uniform sampler2D _MainTex;
               
                fixed4 frag (v2f i) : COLOR
                {
                    half4 tA = tex2D(_MainTex, i.uv);
                    return fixed4(tA.rgb, tA.a);
                }
            ENDCG
        }
    }
}
