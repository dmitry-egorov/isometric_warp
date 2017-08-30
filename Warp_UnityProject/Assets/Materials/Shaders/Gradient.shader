// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Gradient"
{
	Properties
	{
        _TopColor ("Top Color", Color) = (1, 1, 1, 0)
        _BottomColor ("Bottom Color", Color) = (1, 1, 1, 0)
	}
	SubShader
	{
		Tags { "RenderQueue"="Transparent" }
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
            
            fixed4 _TopColor;
            fixed4 _BottomColor;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                return lerp(_BottomColor, _TopColor, saturate(i.uv.y));
            }

			ENDCG
		}
	}
}
