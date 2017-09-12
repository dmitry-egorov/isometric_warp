// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Stripes"
{
	Properties
	{
        _Color ("Stripe Color", Color) = (1, 1, 1, 0)      
        _Width("Stripe Width", Float) = 0.1
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
                fixed3 normal: NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
                float3 normal: NORMAL;
			};
            
            fixed4 _Color;
            float _Width;
            
            //Global
            float _PixelScale;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed fraction = abs(frac(i.vertex.y * _Width / _PixelScale));
                fixed up_degree = normalize(i.normal).y;
                return up_degree > 0.1 && fraction < 0.5 ? _Color : fixed4(0, 0, 0, 0);
            }

			ENDCG
		}
	}
}
