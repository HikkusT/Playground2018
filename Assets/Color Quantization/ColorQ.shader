Shader "Playground/ColorQ"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ColorPalette ("Color Palette", 2D) = "white" {}
		_Size ("Size", Float) = 2
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _Size;
			sampler2D _ColorPalette;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				//fixed4 col = tex2D(_ColorPalette, i.uv);
				// just invert the colors
				
				float minDist = 1000000.0;
				fixed4 color = fixed4(0., 0., 0., 1.);
				for (int i = 0; i < _Size; i ++)
					for (int j = 0; j < _Size; j ++)
					{
						fixed4 currColor = tex2D(_ColorPalette, float2(i/_Size + 0.01, j/_Size + 0.01));
						fixed4 curr = col - currColor;
						float currDist = dot (curr, curr);
						if (currDist < minDist)
						{
							minDist = currDist;
							color = currColor;
						}
					}

				//col = tex2D(_ColorPalette, float2(2/_Size + 0.01, 0/_Size));

				return color;
				//return col;
			}
			ENDCG
		}
	}
}
