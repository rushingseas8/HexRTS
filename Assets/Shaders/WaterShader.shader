Shader "Custom/WaterShader 1"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_WaterLookupTex("Water Lookup", 2D) = "white" {}
		_HeightNoiseMap("Height/Noise Map", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_WaterFogColor("Water Fog Color", Color) = (0, 0, 0, 0)
		_WaterFogDensity("Water Fog Density", Range(0, 3)) = 0.1
		_WaterDepthOffset("Water Depth Offset", Range(-1, 1)) = -0.3
		_WaterDepthScale("Water Depth Scale", Range(-5, 25)) = 4.0
		_WaterAlpha("Water Alpha", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

		GrabPass { "_WaterBackground" }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard alpha fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _WaterLookupTex;
		sampler2D _HeightNoiseMap;
		sampler2D _CameraDepthTexture, _WaterBackground;
		float4 _CameraDepthTexture_TexelSize;

        struct Input
        {
            float2 uv_MainTex;
			float4 screenPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		float3 _WaterFogColor;
		float _WaterFogDensity;
		float _WaterDepthOffset;
		float _WaterDepthScale;
		float _WaterAlpha;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		float3 ColorBelowWater(float4 screenPos) {
			float2 uv = screenPos.xy / screenPos.w;
			#if UNITY_UV_STARTS_AT_TOP
			if (_CameraDepthTexture_TexelSize.y < 0) {
				uv.y = 1 - uv.y;
			}
			#endif
			float backgroundDepth =
				LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
			float surfaceDepth = UNITY_Z_0_FAR_FROM_CLIPSPACE(screenPos.z);

			float depthDifference = backgroundDepth - surfaceDepth;
			//return 1.0 - (depthDifference / 2);

			float3 backgroundColor = tex2D(_WaterBackground, uv).rgb;
			float fogFactor = exp2(-_WaterFogDensity * depthDifference);


			// Code for pulling the color from the computed depth
			float waterTexX = clamp(depthDifference / _WaterDepthScale, 0, 1);
			float2 waterSample = float2(waterTexX, 0.0);
			float4 waterColor = tex2D(_WaterLookupTex, waterSample);

			return lerp(waterColor, backgroundColor, fogFactor);
			//return waterColor;
			//return backgroundColor;
		}

		float4 ColorFromDepth(float2 texUV, float4 screenPos)
		{
			// Lookup the depth from the noise heightmap
			// _WaterDepthOffset = -0.3
			// _WaterDepthScale = 4.0
			float noise = (tex2D(_HeightNoiseMap, texUV).r + _WaterDepthOffset) * _WaterDepthScale;
			float waterTexX = clamp(noise, 0, 1);
			float2 waterSample = float2(waterTexX, 0.0);
			float4 waterColor = tex2D(_WaterLookupTex, waterSample);

			// Compute camera-based depth
			float2 uv = screenPos.xy / screenPos.w;
			#if UNITY_UV_STARTS_AT_TOP
			if (_CameraDepthTexture_TexelSize.y < 0) {
				uv.y = 1 - uv.y;
			}
			#endif
			float backgroundDepth =
				LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
			float surfaceDepth = UNITY_Z_0_FAR_FROM_CLIPSPACE(screenPos.z);

			float depthDifference = backgroundDepth - surfaceDepth;
			//return 1.0 - (depthDifference / 2);

			// pass-through background
			//float4 backgroundColor = tex2D(_WaterBackground, uv);
			//float4 backgroundColor = float4(23/255.0, 64/255.0, 109/255.0, 1);
			float4 backgroundColor = float4(0, 0, 0, 1);
			float fogFactor = exp2(-_WaterFogDensity * depthDifference);

			return lerp(waterColor, backgroundColor, fogFactor);
			//return waterColor;
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

			//o.Albedo = noise;
			o.Albedo = ColorFromDepth(IN.uv_MainTex, IN.screenPos);
			o.Alpha = _WaterAlpha;

			//o.Albedo = ColorBelowWater(IN.screenPos);
			//o.Emission = ColorBelowWater(IN.screenPos) * (1 - c.a);
			//o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
