Shader "Outlined/Silhouette Only" {
    Properties {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _Outline ("Outline width", Range (0.0, 0.06)) = .005
    }
 
    SubShader {
        Tags { "Queue" = "Transparent" }
 
        Pass {
            Name "BASE"
            Cull Back
            Blend Zero One
 
            Offset -128, -8

            Lighting Off
            Color (0,0,0,0)
        }
 
        Pass {
            Name "OUTLINE"
            Cull Front
 
            // you can choose what kind of blending mode you want for the outline
            Blend SrcAlpha OneMinusSrcAlpha // Normal
            //Blend One One // Additive
            //Blend One OneMinusDstColor // Soft Additive
            //Blend DstColor Zero // Multiplicative
            //Blend DstColor SrcColor // 2x Multiplicative
 
            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            uniform float _Outline;
            uniform float4 _OutlineColor;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
             
            struct v2f {
                float4 pos : POSITION;
            };
             
            half4 frag(v2f i) : COLOR {
                return _OutlineColor;
            }

            v2f vert(appdata v) {
                v2f o;

                float4 pos = UnityObjectToClipPos(v.vertex);
                float3 norm   = mul (UNITY_MATRIX_IT_MV, float4(v.normal, 0.0)).xyz;
                float2 offset = TransformViewToProjection(norm).xy;
             
                o.pos = pos + float4(offset * _Outline, 0, 0);
                return o;
            }
            ENDCG
        }
    }
 
    Fallback "Diffuse"
}