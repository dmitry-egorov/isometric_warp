Shader "Outlined/Silhouette Only" {
    Properties {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _Outline ("Outline width", Range (0.0, 0.06)) = .005
    }
 
    SubShader {
        Tags { "RenderType" = "Opaque" }
 
        Pass {
            Name "BASE"
            Cull Back
            Blend Zero One
 
            //Offset -256, -8

            //Lighting Off
            //Color (0,0,0,0)
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata {
                float4 vertex : POSITION;
            };
             
            struct v2f {
                float4 pos : POSITION;
            };

            v2f vert(appdata v) {
                v2f o;

                float4 pos = UnityObjectToClipPos(v.vertex);
             
                o.pos = pos + float4(0, 0, -0.5, 0);
                return o;
            }
             
            half4 frag(v2f i) : COLOR {
                return half4(0,0,0,0);
            }
            ENDCG
        }
 
        Pass {
            Name "OUTLINE"
            Cull Front
            Blend One Zero
 
            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            float _Outline;
            float4 _OutlineColor;
            
            //Global
            float _PixelScale;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
             
            struct v2f {
                float4 pos : POSITION;
            };

            v2f vert(appdata v) {
                v2f o;

                float4 pos = UnityObjectToClipPos(v.vertex);
                float3 norm   = mul (UNITY_MATRIX_IT_MV, float4(v.normal, 0.0)).xyz;
                float2 offset = TransformViewToProjection(norm).xy;
             
                o.pos = pos + float4(offset * _Outline * _PixelScale, -0.3, 0);
                return o;
            }
             
            half4 frag(v2f i) : COLOR {
                return _OutlineColor;
            }
            ENDCG
        }
    }
 
    Fallback "Diffuse"
}