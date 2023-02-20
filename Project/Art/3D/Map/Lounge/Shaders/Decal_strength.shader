Shader "Cookie's shaders/Decal strength" {
	Properties {
		_MainTex ("Albedo", 2D) = "white" {}
		_DecalTex ("Decal", 2D) = "white" {}
		_Strength ("Intensity", Range(0,1)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		CGPROGRAM
		#pragma surface surf Standard
		#pragma target 3.0
		float _Strength;
		sampler2D _MainTex;
		sampler2D _DecalTex;
		struct Input {
			float2 uv_MainTex;
			float2 uv_DecalTex;
		};
		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 d = tex2D (_DecalTex, IN.uv_DecalTex);
			o.Albedo = c.rgb*(1-_Strength*d.a)+d.rgb*d.a*_Strength;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
