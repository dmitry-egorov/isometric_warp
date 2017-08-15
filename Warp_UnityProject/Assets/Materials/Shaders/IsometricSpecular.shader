// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/IsometricSpecular"
{
	Properties
	{
        _Color ("Color", Color) = (1, 1, 1, 0)      
		_TopColor ("Top Color", Color) = (1, 1, 1, 0)
        _LeftColor ("Left Color", Color) = (1, 1, 1, 0)
        _RightColor ("Right Color", Color) = (1, 1, 1, 0)
        _AmbientColor ("Ambient Color", Color) = (0, 0, 0, 0)
        _SpecularColor ("_SpecularColor", Color) = (1, 1, 1, 0)
        _Shininess ("Shininess", Float) = 1
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
                float3 normal: NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
                float3 c: COLOR;
                float3 n: NORMAL;
                float3 wpos: TEXCOORD1;
			};
            
            fixed3 _Color;
            fixed3 _TopColor;
            fixed3 _LeftColor;
            fixed3 _RightColor;
            fixed3 _AmbientColor;
            fixed3 _SpecularColor;
            float _Shininess;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                fixed3 wpos = mul(unity_ObjectToWorld, v.vertex).xyz;

                fixed3 surround = calculate_isometric_surround_light(v.normal, _TopColor, _LeftColor, _RightColor);

                o.c = surround + _AmbientColor;
                o.wpos = wpos;

                return o;
            }
            
            fixed3 frag (v2f i) : SV_Target
            {
                fixed3 specular = calculate_fixed_light_specular(i.wpos, _SpecularColor, _TopColor, _Shininess);

                return saturate(_Color * (i.c + specular));
            }
			ENDCG
		}
	}
}
