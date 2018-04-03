Shader "Unlit/Wireframe"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM

			#pragma target 4.0

			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry Geometry
			
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

			struct g2f
			{
				v2f data;
				float3 barycentricCoordinates : TEXCOORD9;
			};


			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			[maxvertexcount(3)]
			void Geometry (triangleadj v2f i[6], inout TriangleStream<g2f> stream)
			{
				g2f g0, g1, g2;
				g0.data = i[0];
				g1.data = i[1];
				g2.data = i[2];

				float edge1 = distance(i[0].vertex, i[1].vertex);
				float edge2 = distance(i[1].vertex, i[2].vertex);
				float edge3 = distance(i[2].vertex, i[0].vertex);

				float biggestLen = max(edge1, max(edge2, edge3));

				g0.barycentricCoordinates = float3(1., 0, 0);
				g1.barycentricCoordinates = float3(0, 1., 0);
				g2.barycentricCoordinates = float3(0, 0, 1.);

				if(edge1 == biggestLen)
				{
					g0.barycentricCoordinates = float3(1., 0, 1.);
					g1.barycentricCoordinates = float3(0, 1., 1.);
				}
				else if (edge2 == biggestLen)
				{
					g1.barycentricCoordinates = float3(1., 1., 0);
					g2.barycentricCoordinates = float3(1., 0, 1.);
				}
				else
				{
					g0.barycentricCoordinates = float3(1., 1., 0);
					g2.barycentricCoordinates = float3(0, 1., 1.);
				}

				stream.Append(g0);
				stream.Append(g1);
				stream.Append(g2);
			}
			
			fixed4 frag (g2f i) : SV_Target
			{
				float3 albedo;
				float3 barys = i.barycentricCoordinates;
				//barys.xy = i.barycentricCoordinates;
				//barys.z = 1. - barys.x - barys.y;
				float3 deltas = fwidth(barys);
				barys = smoothstep(0, deltas, barys);
				float minBary = min(barys.x, min(barys.y, barys.z));

				albedo = minBary;
				
				return float4(albedo, 1.);
			}
			ENDCG
		}
	}
}
