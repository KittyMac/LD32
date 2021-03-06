Shader "LD32/Billboard" {
	Properties {
	  _MainTex ("Texture Image", 2D) = "white" {}
  	  _Color ("Main Color (A=Opacity)", Color) = (1,1,1,1)	  
	}
	SubShader {

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }

		Pass {   
			CGPROGRAM

			#pragma vertex vert  
			#pragma fragment frag 

			// User-specified uniforms            
			uniform sampler2D _MainTex;
			uniform float4 _Color;

			struct vertexInput {
				float4 vertex    : POSITION;
				float2 texcoord  : TEXCOORD0;
				float2 texcoord1  : TEXCOORD1;
				float3 normal    : NORMAL;
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				float2 tex : TEXCOORD0;
			};


			vertexOutput vert(vertexInput input) 
			{
				vertexOutput output;

				float s, c, scale;

				sincos(0.0, s, c);
				scale = 1.0;

				float x = (input.normal.x * c - input.normal.y * s) * scale * input.texcoord1.x;
				float y = (input.normal.x * s + input.normal.y * c) * scale * input.texcoord1.y;

				output.pos = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_MV, float4(input.vertex.x, input.vertex.y, input.vertex.z, 1.0)) - float4(x, y, 0.0, 0.0));
				output.tex = input.texcoord;
				return output;
			}
				
			float4 frag(vertexOutput input) : COLOR
			{
				return tex2D(_MainTex, float2(input.tex.xy)) * _Color;   
			}

			ENDCG
		}
	}
}