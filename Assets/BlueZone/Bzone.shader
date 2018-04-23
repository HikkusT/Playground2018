Shader "Playground/Bzone"
{
	Properties
	{
		_MainTex ("Line Pattern", 2D) = "white" {}
		_SquareTex ("Square Pattern", 2D) = "black" {}
		_SquareSin ("Square Frequency", 2D) = "black" {}
		_Color ("Color", Color) = (1., 1., 1., 1.)
		_HColor ("Highlight Color", Color) = (1., 1., 1., 1.)
		_GWidth ("Ground Line Width", Float) = 0.5
		_Velocity ("Velocity", Float) = 1
		_Threshold ("Threshold", Range(0, 1)) = 0.5
		_SquareAttenuation("Square Attenuation", Range(0, 1)) = 0.7
	}
	SubShader
	{
		Blend SrcAlpha OneMinusSrcAlpha	
		ZWrite Off
		Cull Off

		Tags
		{
			"RenderType"="Transparent"
			"Queue"="Transparent"
		}

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
				float2 uvSquare : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float depth : TEXCOORD2;
				float2 screenuv : TEXCOORD3;
			};

			sampler2D _MainTex;
			sampler2D _SquareTex;
			sampler2D _SquareSin;
			sampler2D _CameraDepthNormalsTexture;
			float4 _MainTex_ST;
			float4 _SquareTex_ST;
			fixed4 _Color;
			fixed4 _HColor;
			float _GWidth;
			float _Velocity;
			float _Threshold;
			float _SquareAttenuation;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uvSquare = TRANSFORM_TEX(v.uv, _SquareTex);
				o.depth = -mul(UNITY_MATRIX_MV, v.vertex).z * _ProjectionParams.w;
				o.screenuv = ((o.vertex.xy/o.vertex.w) + 1) / 2;
				o.screenuv.y = 1 - o.screenuv.y;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 pattern = tex2D(_MainTex, i.uv + _Time.x * _Velocity) * _HColor;
				float lerpConst = max(0, (_Threshold - i.depth)/_Threshold);
				pattern = fixed4(lerp(fixed4(0., 0., 0., 0.), pattern.rgba, lerpConst));
				//pattern.a = lerp(0., pattern.a, lerpConst);

				float screenDepth = DecodeFloatRG(tex2D(_CameraDepthNormalsTexture, i.screenuv).zw);
				float diff = screenDepth - i.depth;
				float intersect = 0;
				
				if (diff > 0)
					intersect = 1 - smoothstep(0, _ProjectionParams.w * _GWidth, diff);

				fixed4 groundColor = fixed4(lerp(_Color.rgb, _HColor.rgb, pow(intersect, 1)), 1.);

				fixed4 squares = tex2D(_SquareTex, i.uvSquare) * _SquareAttenuation;
				squares *= saturate(sin(_Time.z + tex2D(_SquareSin, i.uvSquare).r * 5) + 1);
				squares.a = 0;
				squares = fixed4(lerp(fixed4(0., 0., 0., 0.), squares.rgba, lerpConst));
				
				return _Color + groundColor * intersect + pattern + squares;
			}
			ENDCG
		}
	}
}
