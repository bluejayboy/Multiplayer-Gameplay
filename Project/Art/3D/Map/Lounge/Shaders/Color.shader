Shader "Cookie's shaders/Simple Color" {
	Properties {
		_MainTex ("Albedo", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		CGPROGRAM
		#pragma surface surf Standard
		#pragma target 3.0
		sampler2D _MainTex;
		struct Input {
			float2 uv_MainTex;
		};
		float4 _Color;
		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb*_Color;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
