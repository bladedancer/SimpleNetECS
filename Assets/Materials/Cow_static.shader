Shader "PBR Master"
{
	Properties
	{
				[PerRendererData]_Seed("Seed", Int) = 2
				[PerRendererData]_Offset("Offset", Vector) = (0,0,0,0)
				[PerRendererData]_BaseColor("BaseColor", Color) = (0.8962264,0.8919989,0.8919989,0)
				[PerRendererData]_PatternColor("PatternColor", Color) = (0.1132075,0.1132075,0.1132075,0)
		
	}
	SubShader
	{
		Tags{ "RenderPipeline" = "LightweightPipeline"}
		Tags
		{
			"RenderType"="Opaque"
			"Queue"="Geometry"
		}
		
		Pass
		{
			Tags{"LightMode" = "LightweightForward"}
			
					Blend One Zero
		
					Cull Back
		
					ZTest LEqual
		
					ZWrite On
		
		
			HLSLPROGRAM
		    // Required to compile gles 2.0 with standard srp library
		    #pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0
		
			// -------------------------------------
			// Lightweight Pipeline keywords
			#pragma multi_compile _ _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _VERTEX_LIGHTS
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			#pragma multi_compile _ _SHADOWS_ENABLED
		
			// -------------------------------------
			// Unity defined keywords
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile_fog
		
			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
		
		    #pragma vertex vert
			#pragma fragment frag
		
			
		
			#include "LWRP/ShaderLibrary/Core.hlsl"
			#include "LWRP/ShaderLibrary/Lighting.hlsl"
			#include "CoreRP/ShaderLibrary/Color.hlsl"
			#include "CoreRP/ShaderLibrary/UnityInstancing.hlsl"
			#include "ShaderGraphLibrary/Functions.hlsl"
		
								float _Seed;
							float2 _Offset;
							float4 _BaseColor;
							float4 _PatternColor;
					
							struct SurfaceInputs{
								float3 ObjectSpaceNormal;
								float3 TangentSpaceNormal;
								float3 ObjectSpaceTangent;
								float3 ObjectSpaceBiTangent;
								half4 uv0;
							};
					
					
					        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
					        {
					            Out = UV * Tiling + Offset;
					        }
					
					        void Unity_RandomRange_float(float2 Seed, float Min, float Max, out float Out)
					        {
					             float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233)))*43758.5453);
					             Out = lerp(Min, Max, randomno);
					        }
					
					
inline float unity_noise_randomValue (float2 uv)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
}
					
inline float unity_noise_interpolate (float a, float b, float t)
{
    return (1.0-t)*a + (t*b);
}

					
inline float unity_valueNoise (float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    f = f * f * (3.0 - 2.0 * f);

    uv = abs(frac(uv) - 0.5);
    float2 c0 = i + float2(0.0, 0.0);
    float2 c1 = i + float2(1.0, 0.0);
    float2 c2 = i + float2(0.0, 1.0);
    float2 c3 = i + float2(1.0, 1.0);
    float r0 = unity_noise_randomValue(c0);
    float r1 = unity_noise_randomValue(c1);
    float r2 = unity_noise_randomValue(c2);
    float r3 = unity_noise_randomValue(c3);

    float bottomOfGrid = unity_noise_interpolate(r0, r1, f.x);
    float topOfGrid = unity_noise_interpolate(r2, r3, f.x);
    float t = unity_noise_interpolate(bottomOfGrid, topOfGrid, f.y);
    return t;
}
					        void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
					        {
					            float t = 0.0;
					            for(int i = 0; i < 3; i++)
					            {
					                float freq = pow(2.0, float(i));
					                float amp = pow(0.5, float(3-i));
					                t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;
					            }
					            Out = t;
					        }
					
					        void Unity_Contrast_float(float3 In, float Contrast, out float3 Out)
					        {
					            float midpoint = pow(0.5, 2.2);
					            Out =  (In - midpoint) * Contrast + midpoint;
					        }
					
					        void Unity_Round_float3(float3 In, out float3 Out)
					        {
					            Out = round(In);
					        }
					
					        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
					        {
					            Out = A + B;
					        }
					
					        void Unity_Multiply_float (float3 A, float3 B, out float3 Out)
					        {
					            Out = A * B;
					        }
					
					        void Unity_Clamp_float3(float3 In, float3 Min, float3 Max, out float3 Out)
					        {
					            Out = clamp(In, Min, Max);
					        }
					
							struct GraphVertexInput
							{
								float4 vertex : POSITION;
								float3 normal : NORMAL;
								float4 tangent : TANGENT;
								float4 texcoord0 : TEXCOORD0;
								float4 texcoord1 : TEXCOORD1;
								UNITY_VERTEX_INPUT_INSTANCE_ID
							};
					
							struct SurfaceDescription{
								float3 Albedo;
								float3 Normal;
								float3 Emission;
								float Metallic;
								float Smoothness;
								float Occlusion;
								float Alpha;
								float AlphaClipThreshold;
							};
					
							GraphVertexInput PopulateVertexData(GraphVertexInput v){
								return v;
							}
					
							SurfaceDescription PopulateSurfaceData(SurfaceInputs IN) {
								SurfaceDescription surface = (SurfaceDescription)0;
								float4 _Property_5F1D37CB_Out = _PatternColor;
								float2 _Property_6C4CC19F_Out = _Offset;
								float2 _TilingAndOffset_CF47D378_Out;
								Unity_TilingAndOffset_float(IN.uv0.xy, float2 (-0.74,1), _Property_6C4CC19F_Out, _TilingAndOffset_CF47D378_Out);
								float _Property_6549D366_Out = _Seed;
								float _RandomRange_A3F129CB_Out;
								Unity_RandomRange_float((_Property_6549D366_Out.xx), 10, 14, _RandomRange_A3F129CB_Out);
								float _SimpleNoise_FE7B3EE8_Out;
								Unity_SimpleNoise_float(_TilingAndOffset_CF47D378_Out, _RandomRange_A3F129CB_Out, _SimpleNoise_FE7B3EE8_Out);
								float _RandomRange_E1DFD624_Out;
								Unity_RandomRange_float((_Property_6549D366_Out.xx), 1, 2, _RandomRange_E1DFD624_Out);
								float3 _Contrast_1EDD65F3_Out;
								Unity_Contrast_float((_SimpleNoise_FE7B3EE8_Out.xxx), _RandomRange_E1DFD624_Out, _Contrast_1EDD65F3_Out);
								float3 _Round_AE5CFA55_Out;
								Unity_Round_float3(_Contrast_1EDD65F3_Out, _Round_AE5CFA55_Out);
								float3 _Add_A8EF4241_Out;
								Unity_Add_float3((_Property_5F1D37CB_Out.xyz), _Round_AE5CFA55_Out, _Add_A8EF4241_Out);
								float4 _Property_30FEB6AD_Out = _BaseColor;
								float3 _Multiply_CFF4E5AD_Out;
								Unity_Multiply_float(_Add_A8EF4241_Out, (_Property_30FEB6AD_Out.xyz), _Multiply_CFF4E5AD_Out);
								
								float3 _Clamp_2C827FB3_Out;
								Unity_Clamp_float3(_Multiply_CFF4E5AD_Out, float3(0, 0, 0), float3(255, 255, 255), _Clamp_2C827FB3_Out);
								surface.Albedo = _Clamp_2C827FB3_Out;
								surface.Normal = IN.TangentSpaceNormal;
								surface.Emission = IsGammaSpace() ? float3(0, 0, 0) : SRGBToLinear(float3(0, 0, 0));
								surface.Metallic = 0;
								surface.Smoothness = 0;
								surface.Occlusion = 1;
								surface.Alpha = 1;
								surface.AlphaClipThreshold = 0;
								return surface;
							}
					
		
		
			struct GraphVertexOutput
		    {
		        float4 clipPos                : SV_POSITION;
		        DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 0);
				half4 fogFactorAndVertexLight : TEXCOORD1; // x: fogFactor, yzw: vertex light
		    	float4 shadowCoord            : TEXCOORD2;
		        			float3 WorldSpaceNormal : TEXCOORD3;
					float3 WorldSpaceTangent : TEXCOORD4;
					float3 WorldSpaceBiTangent : TEXCOORD5;
					float3 WorldSpaceViewDirection : TEXCOORD6;
					float3 WorldSpacePosition : TEXCOORD7;
					half4 uv0 : TEXCOORD8;
					half4 uv1 : TEXCOORD9;
		
		        UNITY_VERTEX_INPUT_INSTANCE_ID
		    };
		
		    GraphVertexOutput vert (GraphVertexInput v)
			{
			    v = PopulateVertexData(v);
		
		        GraphVertexOutput o = (GraphVertexOutput)0;
		
		        UNITY_SETUP_INSTANCE_ID(v);
		    	UNITY_TRANSFER_INSTANCE_ID(v, o);
		
		        			o.WorldSpaceNormal = mul(v.normal,(float3x3)UNITY_MATRIX_I_M);
					o.WorldSpaceTangent = mul((float3x3)UNITY_MATRIX_M,v.tangent.xyz);
					o.WorldSpaceBiTangent = normalize(cross(o.WorldSpaceNormal, o.WorldSpaceTangent.xyz) * v.tangent.w);
					o.WorldSpaceViewDirection = SafeNormalize(_WorldSpaceCameraPos.xyz - mul(GetObjectToWorldMatrix(), float4(v.vertex.xyz, 1.0)).xyz);
					o.WorldSpacePosition = mul(UNITY_MATRIX_M,v.vertex).xyz;
					o.uv0 = v.texcoord0;
					o.uv1 = v.texcoord1;
		
		
				float3 lwWNormal = TransformObjectToWorldNormal(v.normal);
				float3 lwWorldPos = TransformObjectToWorld(v.vertex.xyz);
				float4 clipPos = TransformWorldToHClip(lwWorldPos);
		
		 		// We either sample GI from lightmap or SH.
			    // Lightmap UV and vertex SH coefficients use the same interpolator ("float2 lightmapUV" for lightmap or "half3 vertexSH" for SH)
		        // see DECLARE_LIGHTMAP_OR_SH macro.
			    // The following funcions initialize the correct variable with correct data
			    OUTPUT_LIGHTMAP_UV(v.texcoord1, unity_LightmapST, o.lightmapUV);
			    OUTPUT_SH(lwWNormal, o.vertexSH);
		
			    half3 vertexLight = VertexLighting(lwWorldPos, lwWNormal);
			    half fogFactor = ComputeFogFactor(clipPos.z);
			    o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
			    o.clipPos = clipPos;
		
			    #ifdef _SHADOWS_ENABLED
				#if SHADOWS_SCREEN
					o.shadowCoord = ComputeShadowCoord(clipPos);
				#else
					o.shadowCoord = TransformWorldToShadowCoord(lwWorldPos);
				#endif
				#endif
				
				return o;
			}
		
			half4 frag (GraphVertexOutput IN) : SV_Target
		    {
		    	UNITY_SETUP_INSTANCE_ID(IN);
		
		    				float3 WorldSpaceNormal = normalize(IN.WorldSpaceNormal);
					float3 WorldSpaceTangent = IN.WorldSpaceTangent;
					float3 WorldSpaceBiTangent = IN.WorldSpaceBiTangent;
					float3 WorldSpaceViewDirection = normalize(IN.WorldSpaceViewDirection);
					float3 WorldSpacePosition = IN.WorldSpacePosition;
					float3x3 tangentSpaceTransform = float3x3(WorldSpaceTangent,WorldSpaceBiTangent,WorldSpaceNormal);
					float3 ObjectSpaceNormal = mul(WorldSpaceNormal,(float3x3)UNITY_MATRIX_M);
					float3 TangentSpaceNormal = mul(WorldSpaceNormal,(float3x3)tangentSpaceTransform);
					float3 ObjectSpaceTangent = mul((float3x3)UNITY_MATRIX_I_M,WorldSpaceTangent);
					float3 ObjectSpaceBiTangent = mul((float3x3)UNITY_MATRIX_I_M,WorldSpaceBiTangent);
					float4 uv0 = IN.uv0;
					float4 uv1 = IN.uv1;
		
		
		        SurfaceInputs surfaceInput = (SurfaceInputs)0;
		        			surfaceInput.ObjectSpaceNormal = ObjectSpaceNormal;
					surfaceInput.TangentSpaceNormal = TangentSpaceNormal;
					surfaceInput.ObjectSpaceTangent = ObjectSpaceTangent;
					surfaceInput.ObjectSpaceBiTangent = ObjectSpaceBiTangent;
					surfaceInput.uv0 = uv0;
		
		
		        SurfaceDescription surf = PopulateSurfaceData(surfaceInput);
		
				float3 Albedo = float3(0.5, 0.5, 0.5);
				float3 Specular = float3(0, 0, 0);
				float Metallic = 1;
				float3 Normal = float3(0, 0, 1);
				float3 Emission = 0;
				float Smoothness = 0.5;
				float Occlusion = 1;
				float Alpha = 1;
				float AlphaClipThreshold = 0;
		
		        			Albedo = surf.Albedo;
					Normal = surf.Normal;
					Emission = surf.Emission;
					Metallic = surf.Metallic;
					Smoothness = surf.Smoothness;
					Occlusion = surf.Occlusion;
					Alpha = surf.Alpha;
					AlphaClipThreshold = surf.AlphaClipThreshold;
		
		
				InputData inputData;
				inputData.positionWS = WorldSpacePosition;
		
		#ifdef _NORMALMAP
			    inputData.normalWS = TangentToWorldNormal(Normal, WorldSpaceTangent, WorldSpaceBiTangent, WorldSpaceNormal);
		#else
		    #if !SHADER_HINT_NICE_QUALITY
		        inputData.normalWS = WorldSpaceNormal;
		    #else
			    inputData.normalWS = normalize(WorldSpaceNormal);
		    #endif
		#endif
		
		#if !SHADER_HINT_NICE_QUALITY
			    // viewDirection should be normalized here, but we avoid doing it as it's close enough and we save some ALU.
			    inputData.viewDirectionWS = WorldSpaceViewDirection;
		#else
			    inputData.viewDirectionWS = normalize(WorldSpaceViewDirection);
		#endif
		
			    inputData.shadowCoord = IN.shadowCoord;
		
			    inputData.fogCoord = IN.fogFactorAndVertexLight.x;
			    inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
			    inputData.bakedGI = SAMPLE_GI(IN.lightmapUV, IN.vertexSH, inputData.normalWS);
		
				half4 color = LightweightFragmentPBR(
					inputData, 
					Albedo, 
					Metallic, 
					Specular, 
					Smoothness, 
					Occlusion, 
					Emission, 
					Alpha);
		
				// Computes fog factor per-vertex
		    	ApplyFog(color.rgb, IN.fogFactorAndVertexLight.x);
		
		#if _AlphaClip
				clip(Alpha - AlphaClipThreshold);
		#endif
				return color;
		    }
		
			ENDHLSL
		}
		
		Pass
		{
			Tags{"LightMode" = "ShadowCaster"}
		
			ZWrite On
			ZTest LEqual
			Cull Back
		
			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0
		
			// -------------------------------------
			// Material Keywords
			#pragma shader_feature _ALPHATEST_ON
		
			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
		
			#pragma vertex ShadowPassVertex
			#pragma fragment ShadowPassFragment
		
			#include "LWRP/ShaderLibrary/InputSurfacePBR.hlsl"
			#include "LWRP/ShaderLibrary/LightweightPassShadow.hlsl"
		
			ENDHLSL
		}
		
		Pass
		{
			Tags{"LightMode" = "DepthOnly"}
		
			ZWrite On
			ColorMask 0
			Cull Back
		
			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma target 2.0
		
			#pragma vertex DepthOnlyVertex
			#pragma fragment DepthOnlyFragment
		
			// -------------------------------------
			// Material Keywords
			#pragma shader_feature _ALPHATEST_ON
		
			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
		
			#include "LWRP/ShaderLibrary/InputSurfacePBR.hlsl"
			#include "LWRP/ShaderLibrary/LightweightPassDepthOnly.hlsl"
			ENDHLSL
		}
		
		// This pass it not used during regular rendering, only for lightmap baking.
		Pass
		{
			Tags{"LightMode" = "Meta"}
		
			Cull Off
		
			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
		
			#pragma vertex LightweightVertexMeta
			#pragma fragment LightweightFragmentMeta
		
			#pragma shader_feature _SPECULAR_SETUP
			#pragma shader_feature _EMISSION
			#pragma shader_feature _METALLICSPECGLOSSMAP
			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature EDITOR_VISUALIZATION
		
			#pragma shader_feature _SPECGLOSSMAP
		
			#include "LWRP/ShaderLibrary/InputSurfacePBR.hlsl"
			#include "LWRP/ShaderLibrary/LightweightPassMetaPBR.hlsl"
			ENDHLSL
		}
		
	}
	
	FallBack "Hidden/InternalErrorShader"
}
