﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Transparent" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _Color;

            struct VertexInputVertex {
              float4 pos : POSITION;
            };

            struct FragmenInputVertex {
              float4 pos : SV_POSITION;
            };

            FragmenInputVertex vert (VertexInputVertex v) {
              FragmenInputVertex o;
              o.pos = UnityObjectToClipPos(v.pos);
              return o;
            }

            // fragment shader
            fixed4 frag (FragmenInputVertex v) : COLOR {
              return _Color;
            }
            ENDCG
         }
    }
}