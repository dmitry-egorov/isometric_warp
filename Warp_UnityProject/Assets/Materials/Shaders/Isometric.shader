// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Isometric"
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
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
		    Blend off
		    
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

            v2f vert (appdata v)
            {
                v2f o;

                fixed3 surround = calculate_isometric_surround_light(v.normal, _Angle, _TopColor, _LeftColor, _RightColor);
                fixed3 ambient = calculate_rising_ambient_light(v.vertex, _BottomAmbientColor, _TopAmbientColor, _AmbientHighPoint, _AmbientRise);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.c = fixed4(saturate(_Color * (surround + ambient)), 1);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.c;
            }

			ENDCG
		}
	}
}
