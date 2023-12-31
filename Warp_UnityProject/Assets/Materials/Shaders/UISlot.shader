﻿// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/UISlot"
{
	Properties
	{
        _TopColor ("Top Color", Color) = (1, 1, 1, 1)
        _BottomColor ("Bottom Color", Color) = (1, 1, 1, 1)
        _OutlineColor("Outline color", Color) = (1, 1, 1, 1)
        _OutlineWidth ("Outline width", Float) = 2
        _Falloff("Falloff", Float) = 1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
            #include "common.cginc"
                     
			struct appdata
			{
				float4 vertex : POSITION;
                fixed2 uv: TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
                fixed2 uv: TEXCOORD0;
			};

            //Parameters
            fixed4 _TopColor;
            fixed4 _BottomColor;
            fixed4 _OutlineColor;
            float _OutlineWidth;
            float _Falloff;

            //Global
            float _PixelPerfectScale;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //return fixed4(fwidth(i.uv) * 20, 0, 1);

                fixed4 gradient = lerp(_BottomColor, _TopColor, saturate(i.uv.y));

                fixed2 uvmasks = min(i.uv, 1.0 - i.uv) / fwidth(i.uv);
                fixed mask = min(uvmasks.x, uvmasks.y);
                //return mask < (_OutlineWidth * floor(_ScreenParams.y / 600 * 2) / 2) ? _OutlineColor : gradient;
                //return lerp(_OutlineColor, gradient, mask);

                return lerp(_OutlineColor, gradient, saturate((mask - _OutlineWidth * _PixelPerfectScale) / _Falloff));
            }

			ENDCG
		}
	}
}
