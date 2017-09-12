// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Circle"
{
	Properties
	{
        _Color ("Color", Color) = (1, 1, 1, 0)      
		_Radius("Placeholder Radius", Float) = 0.1
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
				float4 vertex: POSITION;
                fixed3 normal: NORMAL;
			};

			struct v2f
			{
				float4 vertex: SV_POSITION;
                float2 local_pos: TEXCOORD0;
			};
            
            fixed4 _Color;
            float _Radius;
            
            //Global
            float _PixelScale;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.local_pos = v.vertex.xz;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float distance_to_center_squared = dot(i.local_pos, i.local_pos);
                float radius = _Radius * _PixelScale;
                return distance_to_center_squared < _Radius * _Radius ? _Color : fixed4(0,0,0,0);
            }

			ENDCG
		}
	}
}
