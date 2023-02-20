Shader "Cookie's shaders/Simple standard cull off" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off
		CGPROGRAM
		#pragma surface surf Standard
		#pragma target 3.0
		sampler2D _MainTex;
		struct Input {
			float2 uv_MainTex;
		};
		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
