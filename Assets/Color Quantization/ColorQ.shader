Shader "Playground/ColorQ"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ColorPalette ("Color Palette", 2D) = "white" {}
		_Size ("Size", Float) = 2
		[Space(50)] 
		_LineW("Line Width", Float) = 2
		_LineP("Line Position", Range(0, 1)) = 0.5
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			//#define EUCLIDEAN_DISTANCE
			#define VISUAL_DEBUG_MODE
			//#define PALETTE_DEBUG_MODE

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
			sampler2D _ColorPalette;
			float _Size;

			float _LineP;
			float _LineW;

			fixed4 frag (v2f i) : SV_Target
			{
				#if defined(PALETTE_DEBUG_MODE)
					return tex2D(_ColorPalette, i.uv.yx);
				#endif

				fixed4 pixelCol = tex2D(_MainTex, i.uv);
				
				//Setting up initial values
				float minDist = 1000000.0;
				fixed4 color = fixed4(0., 0., 0., 1.);

				//Loop through the palette
				for (int x = 0; x < _Size; x ++)
				{
					fixed4 paletteColor = tex2D(_ColorPalette, float2(0, x/_Size + 0.01));

					//Calculating distance to each palette color
					float dist = 0;
					#if defined(EUCLIDIAN_DISTANCE)
						fixed4 diff = pixelCol - paletteColor;
						dist = dot(diff, diff);
					#endif
					#if !defined(EUCLIDIAN_DISTANCE)
						dist = 1 - dot(paletteColor, pixelCol)/sqrt(dot(paletteColor, paletteColor) * dot(pixelCol, pixelCol));
					#endif

					//Checking if it's the closest color
					if (dist < minDist)
					{
						minDist = dist;
						color = paletteColor;
					}
				}

				#if defined(VISUAL_DEBUG_MODE)
					if(i.uv.x > _LineP + _LineW/100)
						color = tex2D(_MainTex, i.uv);
					else if (i.uv.x > _LineP - _LineW/100)
						color = fixed4(0., 0., 0., 1.);
				#endif

				return color;
			}
			ENDCG
		}
	}
}
