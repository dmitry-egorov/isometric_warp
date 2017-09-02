// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Isometric Striped"
{
	Properties
	{
        _Color ("Color", Color) = (1, 1, 1, 0)      
		_TopColor ("Top Color", Color) = (1, 1, 1, 0)
        _LeftColor ("Left Color", Color) = (1, 1, 1, 0)
        _RightColor ("Right Color", Color) = (1, 1, 1, 0)
        _Angle ("Angle", Float) = 0
        _TopAmbientColor ("Top Ambient Color", Color) = (0, 0, 0, 0)
        _BottomAmbientColor ("Bottom Ambient Color", Color) = (0, 0, 0, 0)
        _AmbientRise ("Ambient Rise", Float) = 1
        _AmbientHighPoint ("Ambient High Point", Float) = 1
        _StripeColor("Stripe Color", Color) = (1, 1, 1, 1)
        _StripeWidth("Stripe Width", Float) = 0.1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
            #include "common.cginc"
                     
			struct appdata
			{
				float4 vertex : POSITION;
                fixed3 normal: NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
                fixed4 c: COLOR;
                float3 normal: NORMAL;
                float3 wpos: TEXCOORD1;
			};
            
            fixed3 _Color;
            fixed3 _TopColor;
            fixed3 _LeftColor;
            fixed3 _RightColor;
            fixed _Angle;
            fixed3 _TopAmbientColor;
            fixed3 _BottomAmbientColor;
            float _AmbientHighPoint;
            float _AmbientRise;
            fixed4 _StripeColor;
            float _StripeWidth;

            v2f vert (appdata v)
            {
                v2f o;

                fixed3 surround = calculate_isometric_surround_light(v.normal, _Angle, _TopColor, _LeftColor, _RightColor);
                fixed3 ambient = calculate_rising_ambient_light(v.vertex, _BottomAmbientColor, _TopAmbientColor, _AmbientHighPoint, _AmbientRise);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.c = fixed4(saturate(_Color * (surround + ambient)), 1);
                o.wpos = v.vertex;
                o.normal = v.normal;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed fraction = abs(frac((i.vertex.y) * _StripeWidth));
                fixed up_degree = normalize(i.normal).y;
                fixed4 stripe = up_degree > 0.1 && fraction < 0.5 ? _StripeColor : fixed4(0, 0, 0, 0);
                return lerp(i.c, stripe, stripe.w);
            }

			ENDCG
		}
	}
}
