Shader "Unlit/451Shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DiffuseStrength ("Diffuse Strength", float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			CGPROGRAM
			#pragma vertex MyVert
			#pragma fragment MyFrag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float3 vertexWC : TEXCOORD3;
			};

            // our own matrix
            float4x4 MyXformMat;  // our own transform matrix!!
            fixed4   MyColor;

			float4	LightPosition; //diffuse light
			float	_DiffuseStrength;

			// Texture support
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f MyVert (appdata v)
			{
				v2f o;
                
                // Can use one of the followings:
                // o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);  // Camera + GameObject transform TRS

                o.vertex = mul(MyXformMat, v.vertex);  // use our own transform matrix!
                    // MUST apply before camrea!

                o.vertex = mul(UNITY_MATRIX_VP, o.vertex);   // camera transform only                
				
				// Texture support
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				//to get lights working
				o.vertexWC = mul(UNITY_MATRIX_M, v.vertex); // this is in WC space!
                float3 p = v.vertex + v.normal;
                p = mul(UNITY_MATRIX_M, float4(p, 1));  // now in WC space
                o.normal = normalize(p - o.vertexWC); // NOTE: this is in the world space!!
				return o;
			}

			// calculate diffuse lighting
            fixed4 ComputeDiffuse(v2f i) {
                float3 l = normalize(LightPosition - i.vertexWC);
                return clamp(dot(i.normal, l), 0, 1) * _DiffuseStrength;
            }
			
			fixed4 MyFrag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col += MyColor;

				float diff = ComputeDiffuse(i);

				return col * diff;
			}
			ENDCG
		}
	}
}