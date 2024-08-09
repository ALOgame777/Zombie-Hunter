// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Toby Fredson/The Toby Foliage Engine/Utility/(TTFE) Global Controller"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[Header(__________(TTFE) TREE GIZMO SHADER___________)][Header(_____________________________________________________)][Header(Texture Maps)][NoScaleOffset]_Albedo("Albedo", 2D) = "white" {}
		[NoScaleOffset][Normal]_Normal("Normal", 2D) = "bump" {}
		[NoScaleOffset]_Mask("Mask", 2D) = "white" {}
		[Header(_____________________________________________________)][Header(Wind Settings)][Header((Global Wind Settings))]_GlobalWindStrength("Global Wind Strength", Range( 0 , 1)) = 1
		[KeywordEnum(GentleBreeze,WindOff)] _WindType("Wind Type", Float) = 0
		[Header((Trunk and Branch))]_BranchWindLarge("Branch Wind Large", Range( 0 , 20)) = 1
		_BranchWindSmall("Branch Wind Small", Range( 0 , 20)) = 1
		[Header((Wind Mask))]_Radius("Radius", Float) = 1
		_Hardness("Hardness", Float) = 1
		[Toggle]_CenterofMass("Center of Mass", Float) = 0
		[Toggle]_PivotSway("Pivot Sway", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}


		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		//_TransStrength( "Trans Strength", Range( 0, 50 ) ) = 1
		//_TransNormal( "Trans Normal Distortion", Range( 0, 1 ) ) = 0.5
		//_TransScattering( "Trans Scattering", Range( 1, 50 ) ) = 2
		//_TransDirect( "Trans Direct", Range( 0, 1 ) ) = 0.9
		//_TransAmbient( "Trans Ambient", Range( 0, 1 ) ) = 0.1
		//_TransShadow( "Trans Shadow", Range( 0, 1 ) ) = 0.5
		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector][ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[HideInInspector][ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0
		[HideInInspector][ToggleOff] _ReceiveShadows("Receive Shadows", Float) = 1.0

		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" "UniversalMaterialType"="Lit" }

		Cull Back
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 4.5
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 140008


			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS

			
            #pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX
		

			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION

			
			#pragma multi_compile_fragment _ _SHADOWS_SOFT
           

			

			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
			#pragma multi_compile _ _LIGHT_LAYERS
			#pragma multi_compile_fragment _ _LIGHT_COOKIES
			#pragma multi_compile _ _FORWARD_PLUS
		
			

			

			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fragment _ DEBUG_DISPLAY

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_FORWARD

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			
			#if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
				#define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_WORLD_VIEW_DIR
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#pragma shader_feature_local _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				float4 lightmapUVOrVertexSH : TEXCOORD1;
				half4 fogFactorAndVertexLight : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					float4 shadowCoord : TEXCOORD6;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
					float2 dynamicLightmapUV : TEXCOORD7;
				#endif
				float4 ase_texcoord8 : TEXCOORD8;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _GlobalWindStrength;
			float _Radius;
			float _Hardness;
			float _BranchWindLarge;
			float _CenterofMass;
			float _BranchWindSmall;
			float _PivotSway;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			sampler2D _Albedo;
			sampler2D _Mask;
			sampler2D _Normal;


			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 appendResult939_g1 = (float3(0.0 , 0.0 , saturate( v.positionOS.xyz ).z));
				float3 break989_g1 = v.positionOS.xyz;
				float3 appendResult938_g1 = (float3(break989_g1.x , ( break989_g1.y * 0.15 ) , 0.0));
				float mulTime975_g1 = _TimeParameters.x * 2.1;
				float3 temp_output_624_0_g1 = ( ( v.positionOS.xyz - float3(0,-1,0) ) / _Radius );
				float dotResult625_g1 = dot( temp_output_624_0_g1 , temp_output_624_0_g1 );
				float temp_output_628_0_g1 = pow( saturate( dotResult625_g1 ) , _Hardness );
				float SphearicalMaskCM763_g1 = saturate( temp_output_628_0_g1 );
				float3 temp_cast_0 = (v.positionOS.xyz.y).xxx;
				float2 appendResult928_g1 = (float2(v.positionOS.xyz.x , v.positionOS.xyz.z));
				float3 temp_output_996_0_g1 = ( cross( temp_cast_0 , float3( appendResult928_g1 ,  0.0 ) ) * 0.005 );
				float3 appendResult931_g1 = (float3(0.0 , v.positionOS.xyz.y , 0.0));
				float3 break971_g1 = v.positionOS.xyz;
				float3 appendResult967_g1 = (float3(break971_g1.x , 0.0 , ( break971_g1.z * 0.15 )));
				float mulTime976_g1 = _TimeParameters.x * 2.3;
				float dotResult849_g1 = dot( (v.positionOS.xyz*0.02 + 0.0) , v.positionOS.xyz );
				float CenterOfMassThicknessMask854_g1 = saturate( dotResult849_g1 );
				float3 appendResult981_g1 = (float3(v.positionOS.xyz.x , 0.0 , 0.0));
				float3 break984_g1 = v.positionOS.xyz;
				float3 appendResult966_g1 = (float3(0.0 , ( break984_g1.y * 0.2 ) , ( break984_g1.z * 0.4 )));
				float mulTime977_g1 = _TimeParameters.x * 2.0;
				float3 ase_worldPos = TransformObjectToWorld( (v.positionOS).xyz );
				float3 normalizeResult765_g1 = normalize( ase_worldPos );
				float mulTime772_g1 = _TimeParameters.x * 0.25;
				float simplePerlin2D769_g1 = snoise( ( normalizeResult765_g1 + mulTime772_g1 ).xy*0.43 );
				float WindMask_LargeB770_g1 = ( simplePerlin2D769_g1 * 1.5 );
				float3 normalizeResult1092_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP_C1098_g1 = saturate( distance( normalizeResult1092_g1 , float3(0,1,0) ) );
				float3 normalizeResult774_g1 = normalize( ase_worldPos );
				float mulTime780_g1 = _TimeParameters.x * 0.26;
				float simplePerlin2D778_g1 = snoise( ( normalizeResult774_g1 + mulTime780_g1 ).xy*0.7 );
				float WindMask_LargeC779_g1 = ( simplePerlin2D778_g1 * 1.5 );
				float mulTime906_g1 = _TimeParameters.x * 3.2;
				float3 worldToObj907_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_872_0_g1 = ( mulTime906_g1 + ( 0.02 * worldToObj907_g1.x ) + ( worldToObj907_g1.y * 0.14 ) + ( worldToObj907_g1.z * 0.16 ) + float3(0.4,0.3,0.1) );
				float3 normalizeResult632_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP636_g1 = saturate( (distance( normalizeResult632_g1 , float3(0,1,0) )*1.0 + -0.05) );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float mulTime905_g1 = _TimeParameters.x * 2.3;
				float3 worldToObj908_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_866_0_g1 = ( mulTime905_g1 + ( 0.2 * worldToObj908_g1 ) + float3(0.4,0.3,0.1) );
				float mulTime904_g1 = _TimeParameters.x * 3.6;
				float3 temp_cast_4 = (v.positionOS.xyz.x).xxx;
				float3 worldToObj910_g1 = mul( GetWorldToObjectMatrix(), float4( temp_cast_4, 1 ) ).xyz;
				float temp_output_898_0_g1 = ( mulTime904_g1 + ( 0.2 * worldToObj910_g1.x ) );
				float3 normalizeResult697_g1 = normalize( v.positionOS.xyz );
				float CenterOfMass701_g1 = saturate( (distance( normalizeResult697_g1 , float3(0,1,0) )*2.0 + 0.0) );
				float SphericalMaskProxySphere704_g1 = (( _CenterofMass )?( ( temp_output_628_0_g1 * CenterOfMass701_g1 ) ):( temp_output_628_0_g1 ));
				float3 worldToObj1131_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float mulTime1138_g1 = _TimeParameters.x * 4.0;
				float mulTime1129_g1 = _TimeParameters.x * 0.2;
				float2 appendResult1126_g1 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 normalizeResult1128_g1 = normalize( appendResult1126_g1 );
				float simpleNoise1139_g1 = SimpleNoise( ( mulTime1129_g1 + normalizeResult1128_g1 )*1.0 );
				float WindMask_SimpleSway1145_g1 = ( simpleNoise1139_g1 * 1.5 );
				float3 rotatedValue1151_g1 = RotateAroundAxis( float3( 0,0,0 ), v.positionOS.xyz, normalize( float3(0.6,1,0.1) ), ( ( cos( ( ( worldToObj1131_g1 * 0.02 ) + mulTime1138_g1 + ( float3(0.6,1,0.8) * 0.3 * worldToObj1131_g1 ) ) ) * 0.1 ) * WindMask_SimpleSway1145_g1 * saturate( ase_objectScale ) ).x );
				float3 temp_cast_6 = (0.0).xxx;
				#if defined(_WINDTYPE_GENTLEBREEZE)
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#elif defined(_WINDTYPE_WINDOFF)
				float3 staticSwitch1044_g1 = temp_cast_6;
				#else
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#endif
				float3 FinalWind_Output1060_g1 = ( _GlobalWindStrength * staticSwitch1044_g1 );
				
				o.ase_texcoord8.xy = v.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord8.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1060_g1;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif
				v.normalOS = v.normalOS;
				v.tangentOS = v.tangentOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.positionOS.xyz );
				VertexNormalInputs normalInput = GetVertexNormalInputs( v.normalOS, v.tangentOS );

				o.tSpace0 = float4( normalInput.normalWS, vertexInput.positionWS.x );
				o.tSpace1 = float4( normalInput.tangentWS, vertexInput.positionWS.y );
				o.tSpace2 = float4( normalInput.bitangentWS, vertexInput.positionWS.z );

				#if defined(LIGHTMAP_ON)
					OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				#endif

				#if !defined(LIGHTMAP_ON)
					OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
					o.dynamicLightmapUV.xy = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord.xy;
					o.lightmapUVOrVertexSH.xy = v.texcoord.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( vertexInput.positionWS, normalInput.normalWS );

				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( vertexInput.positionCS.z );
				#else
					half fogFactor = 0;
				#endif

				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.positionCS = vertexInput.positionCS;
				o.clipPosV = vertexInput.positionCS;
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.tangentOS = v.tangentOS;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				o.texcoord2 = v.texcoord2;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag ( VertexOutput IN
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out float4 outRenderingLayers : SV_Target1
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( IN.positionCS );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif

				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				float4 ClipPos = IN.clipPosV;
				float4 ScreenPos = ComputeScreenPos( IN.clipPosV );

				float2 NormalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.positionCS);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif

				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 uv_Albedo2 = IN.ase_texcoord8.xy;
				float4 tex2DNode2 = tex2D( _Albedo, uv_Albedo2 );
				float2 uv_Mask4 = IN.ase_texcoord8.xy;
				float4 tex2DNode4 = tex2D( _Mask, uv_Mask4 );
				
				float2 uv_Normal3 = IN.ase_texcoord8.xy;
				
				float4 color10 = IsGammaSpace() ? float4(0.2156863,0.5607843,0.2,1) : float4(0.03820438,0.2746773,0.03310476,1);
				float fresnelNdotV5 = dot( WorldNormal, WorldViewDirection );
				float fresnelNode5 = ( -0.1 + 5.0 * pow( 1.0 - fresnelNdotV5, 5.0 ) );
				

				float3 BaseColor = ( tex2DNode2 * saturate( tex2DNode4.g ) ).rgb;
				float3 Normal = UnpackNormalScale( tex2D( _Normal, uv_Normal3 ), 1.0f );
				float3 Emission = ( saturate( ( color10 * fresnelNode5 ) ) + (tex2DNode2*0.4 + 0.0) ).rgb;
				float3 Specular = 0.5;
				float Metallic = 0;
				float Smoothness = tex2DNode4.a;
				float Occlusion = tex2DNode4.g;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = IN.positionCS.z;
				#endif

				#ifdef _CLEARCOAT
					float CoatMask = 0;
					float CoatSmoothness = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;

				#ifdef _NORMALMAP
						#if _NORMAL_DROPOFF_TS
							inputData.normalWS = TransformTangentToWorld(Normal, half3x3(WorldTangent, WorldBiTangent, WorldNormal));
						#elif _NORMAL_DROPOFF_OS
							inputData.normalWS = TransformObjectToWorldNormal(Normal);
						#elif _NORMAL_DROPOFF_WS
							inputData.normalWS = Normal;
						#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					inputData.shadowCoord = ShadowCoords;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
				#else
					inputData.shadowCoord = float4(0, 0, 0, 0);
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif
					inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
					inputData.bakedGI = SAMPLE_GI(IN.lightmapUVOrVertexSH.xy, IN.dynamicLightmapUV.xy, SH, inputData.normalWS);
				#else
					inputData.bakedGI = SAMPLE_GI(IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS);
				#endif

				#ifdef ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif

				inputData.normalizedScreenSpaceUV = NormalizedScreenSpaceUV;
				inputData.shadowMask = SAMPLE_SHADOWMASK(IN.lightmapUVOrVertexSH.xy);

				#if defined(DEBUG_DISPLAY)
					#if defined(DYNAMICLIGHTMAP_ON)
						inputData.dynamicLightmapUV = IN.dynamicLightmapUV.xy;
					#endif
					#if defined(LIGHTMAP_ON)
						inputData.staticLightmapUV = IN.lightmapUVOrVertexSH.xy;
					#else
						inputData.vertexSH = SH;
					#endif
				#endif

				SurfaceData surfaceData;
				surfaceData.albedo              = BaseColor;
				surfaceData.metallic            = saturate(Metallic);
				surfaceData.specular            = Specular;
				surfaceData.smoothness          = saturate(Smoothness),
				surfaceData.occlusion           = Occlusion,
				surfaceData.emission            = Emission,
				surfaceData.alpha               = saturate(Alpha);
				surfaceData.normalTS            = Normal;
				surfaceData.clearCoatMask       = 0;
				surfaceData.clearCoatSmoothness = 1;

				#ifdef _CLEARCOAT
					surfaceData.clearCoatMask       = saturate(CoatMask);
					surfaceData.clearCoatSmoothness = saturate(CoatSmoothness);
				#endif

				#ifdef _DBUFFER
					ApplyDecalToSurfaceData(IN.positionCS, surfaceData, inputData);
				#endif

				half4 color = UniversalFragmentPBR( inputData, surfaceData);

				#ifdef ASE_TRANSMISSION
				{
					float shadow = _TransmissionShadow;

					#define SUM_LIGHT_TRANSMISSION(Light)\
						float3 atten = Light.color * Light.distanceAttenuation;\
						atten = lerp( atten, atten * Light.shadowAttenuation, shadow );\
						half3 transmission = max( 0, -dot( inputData.normalWS, Light.direction ) ) * atten * Transmission;\
						color.rgb += BaseColor * transmission;

					SUM_LIGHT_TRANSMISSION( GetMainLight( inputData.shadowCoord ) );

					#if defined(_ADDITIONAL_LIGHTS)
						uint meshRenderingLayers = GetMeshRenderingLayer();
						uint pixelLightCount = GetAdditionalLightsCount();
						#if USE_FORWARD_PLUS
							for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
							{
								FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK

								Light light = GetAdditionalLight(lightIndex, inputData.positionWS);
								#ifdef _LIGHT_LAYERS
								if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
								#endif
								{
									SUM_LIGHT_TRANSMISSION( light );
								}
							}
						#endif
						LIGHT_LOOP_BEGIN( pixelLightCount )
							Light light = GetAdditionalLight(lightIndex, inputData.positionWS);
							#ifdef _LIGHT_LAYERS
							if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
							#endif
							{
								SUM_LIGHT_TRANSMISSION( light );
							}
						LIGHT_LOOP_END
					#endif
				}
				#endif

				#ifdef ASE_TRANSLUCENCY
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;

					#define SUM_LIGHT_TRANSLUCENCY(Light)\
						float3 atten = Light.color * Light.distanceAttenuation;\
						atten = lerp( atten, atten * Light.shadowAttenuation, shadow );\
						half3 lightDir = Light.direction + inputData.normalWS * normal;\
						half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );\
						half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;\
						color.rgb += BaseColor * translucency * strength;

					SUM_LIGHT_TRANSLUCENCY( GetMainLight( inputData.shadowCoord ) );

					#if defined(_ADDITIONAL_LIGHTS)
						uint meshRenderingLayers = GetMeshRenderingLayer();
						uint pixelLightCount = GetAdditionalLightsCount();
						#if USE_FORWARD_PLUS
							for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
							{
								FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK

								Light light = GetAdditionalLight(lightIndex, inputData.positionWS);
								#ifdef _LIGHT_LAYERS
								if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
								#endif
								{
									SUM_LIGHT_TRANSLUCENCY( light );
								}
							}
						#endif
						LIGHT_LOOP_BEGIN( pixelLightCount )
							Light light = GetAdditionalLight(lightIndex, inputData.positionWS);
							#ifdef _LIGHT_LAYERS
							if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
							#endif
							{
								SUM_LIGHT_TRANSLUCENCY( light );
							}
						LIGHT_LOOP_END
					#endif
				}
				#endif

				#ifdef ASE_REFRACTION
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal,0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4( EncodeMeshRenderingLayer( renderingLayers ), 0, 0, 0 );
				#endif

				return color;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			ColorMask 0

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 140008


			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_SHADOWCASTER

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature_local _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD1;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD2;
				#endif				
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _GlobalWindStrength;
			float _Radius;
			float _Hardness;
			float _BranchWindLarge;
			float _CenterofMass;
			float _BranchWindSmall;
			float _PivotSway;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			

			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			float3 _LightDirection;
			float3 _LightPosition;

			VertexOutput VertexFunction( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float3 appendResult939_g1 = (float3(0.0 , 0.0 , saturate( v.positionOS.xyz ).z));
				float3 break989_g1 = v.positionOS.xyz;
				float3 appendResult938_g1 = (float3(break989_g1.x , ( break989_g1.y * 0.15 ) , 0.0));
				float mulTime975_g1 = _TimeParameters.x * 2.1;
				float3 temp_output_624_0_g1 = ( ( v.positionOS.xyz - float3(0,-1,0) ) / _Radius );
				float dotResult625_g1 = dot( temp_output_624_0_g1 , temp_output_624_0_g1 );
				float temp_output_628_0_g1 = pow( saturate( dotResult625_g1 ) , _Hardness );
				float SphearicalMaskCM763_g1 = saturate( temp_output_628_0_g1 );
				float3 temp_cast_0 = (v.positionOS.xyz.y).xxx;
				float2 appendResult928_g1 = (float2(v.positionOS.xyz.x , v.positionOS.xyz.z));
				float3 temp_output_996_0_g1 = ( cross( temp_cast_0 , float3( appendResult928_g1 ,  0.0 ) ) * 0.005 );
				float3 appendResult931_g1 = (float3(0.0 , v.positionOS.xyz.y , 0.0));
				float3 break971_g1 = v.positionOS.xyz;
				float3 appendResult967_g1 = (float3(break971_g1.x , 0.0 , ( break971_g1.z * 0.15 )));
				float mulTime976_g1 = _TimeParameters.x * 2.3;
				float dotResult849_g1 = dot( (v.positionOS.xyz*0.02 + 0.0) , v.positionOS.xyz );
				float CenterOfMassThicknessMask854_g1 = saturate( dotResult849_g1 );
				float3 appendResult981_g1 = (float3(v.positionOS.xyz.x , 0.0 , 0.0));
				float3 break984_g1 = v.positionOS.xyz;
				float3 appendResult966_g1 = (float3(0.0 , ( break984_g1.y * 0.2 ) , ( break984_g1.z * 0.4 )));
				float mulTime977_g1 = _TimeParameters.x * 2.0;
				float3 ase_worldPos = TransformObjectToWorld( (v.positionOS).xyz );
				float3 normalizeResult765_g1 = normalize( ase_worldPos );
				float mulTime772_g1 = _TimeParameters.x * 0.25;
				float simplePerlin2D769_g1 = snoise( ( normalizeResult765_g1 + mulTime772_g1 ).xy*0.43 );
				float WindMask_LargeB770_g1 = ( simplePerlin2D769_g1 * 1.5 );
				float3 normalizeResult1092_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP_C1098_g1 = saturate( distance( normalizeResult1092_g1 , float3(0,1,0) ) );
				float3 normalizeResult774_g1 = normalize( ase_worldPos );
				float mulTime780_g1 = _TimeParameters.x * 0.26;
				float simplePerlin2D778_g1 = snoise( ( normalizeResult774_g1 + mulTime780_g1 ).xy*0.7 );
				float WindMask_LargeC779_g1 = ( simplePerlin2D778_g1 * 1.5 );
				float mulTime906_g1 = _TimeParameters.x * 3.2;
				float3 worldToObj907_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_872_0_g1 = ( mulTime906_g1 + ( 0.02 * worldToObj907_g1.x ) + ( worldToObj907_g1.y * 0.14 ) + ( worldToObj907_g1.z * 0.16 ) + float3(0.4,0.3,0.1) );
				float3 normalizeResult632_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP636_g1 = saturate( (distance( normalizeResult632_g1 , float3(0,1,0) )*1.0 + -0.05) );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float mulTime905_g1 = _TimeParameters.x * 2.3;
				float3 worldToObj908_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_866_0_g1 = ( mulTime905_g1 + ( 0.2 * worldToObj908_g1 ) + float3(0.4,0.3,0.1) );
				float mulTime904_g1 = _TimeParameters.x * 3.6;
				float3 temp_cast_4 = (v.positionOS.xyz.x).xxx;
				float3 worldToObj910_g1 = mul( GetWorldToObjectMatrix(), float4( temp_cast_4, 1 ) ).xyz;
				float temp_output_898_0_g1 = ( mulTime904_g1 + ( 0.2 * worldToObj910_g1.x ) );
				float3 normalizeResult697_g1 = normalize( v.positionOS.xyz );
				float CenterOfMass701_g1 = saturate( (distance( normalizeResult697_g1 , float3(0,1,0) )*2.0 + 0.0) );
				float SphericalMaskProxySphere704_g1 = (( _CenterofMass )?( ( temp_output_628_0_g1 * CenterOfMass701_g1 ) ):( temp_output_628_0_g1 ));
				float3 worldToObj1131_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float mulTime1138_g1 = _TimeParameters.x * 4.0;
				float mulTime1129_g1 = _TimeParameters.x * 0.2;
				float2 appendResult1126_g1 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 normalizeResult1128_g1 = normalize( appendResult1126_g1 );
				float simpleNoise1139_g1 = SimpleNoise( ( mulTime1129_g1 + normalizeResult1128_g1 )*1.0 );
				float WindMask_SimpleSway1145_g1 = ( simpleNoise1139_g1 * 1.5 );
				float3 rotatedValue1151_g1 = RotateAroundAxis( float3( 0,0,0 ), v.positionOS.xyz, normalize( float3(0.6,1,0.1) ), ( ( cos( ( ( worldToObj1131_g1 * 0.02 ) + mulTime1138_g1 + ( float3(0.6,1,0.8) * 0.3 * worldToObj1131_g1 ) ) ) * 0.1 ) * WindMask_SimpleSway1145_g1 * saturate( ase_objectScale ) ).x );
				float3 temp_cast_6 = (0.0).xxx;
				#if defined(_WINDTYPE_GENTLEBREEZE)
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#elif defined(_WINDTYPE_WINDOFF)
				float3 staticSwitch1044_g1 = temp_cast_6;
				#else
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#endif
				float3 FinalWind_Output1060_g1 = ( _GlobalWindStrength * staticSwitch1044_g1 );
				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1060_g1;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.positionWS = positionWS;
				#endif

				float3 normalWS = TransformObjectToWorldDir(v.normalOS);

				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
				#else
					float3 lightDirectionWS = _LightDirection;
				#endif

				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

				#if UNITY_REVERSED_Z
					positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
				#else
					positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.positionCS = positionCS;
				o.clipPosV = positionCS;
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(	VertexOutput IN
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float4 ClipPos = IN.clipPosV;
				float4 ScreenPos = ComputeScreenPos( IN.clipPosV );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				

				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = IN.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( IN.positionCS );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM

			

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 140008


			

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_DEPTHONLY

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature_local _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 positionWS : TEXCOORD1;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD2;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _GlobalWindStrength;
			float _Radius;
			float _Hardness;
			float _BranchWindLarge;
			float _CenterofMass;
			float _BranchWindSmall;
			float _PivotSway;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			

			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 appendResult939_g1 = (float3(0.0 , 0.0 , saturate( v.positionOS.xyz ).z));
				float3 break989_g1 = v.positionOS.xyz;
				float3 appendResult938_g1 = (float3(break989_g1.x , ( break989_g1.y * 0.15 ) , 0.0));
				float mulTime975_g1 = _TimeParameters.x * 2.1;
				float3 temp_output_624_0_g1 = ( ( v.positionOS.xyz - float3(0,-1,0) ) / _Radius );
				float dotResult625_g1 = dot( temp_output_624_0_g1 , temp_output_624_0_g1 );
				float temp_output_628_0_g1 = pow( saturate( dotResult625_g1 ) , _Hardness );
				float SphearicalMaskCM763_g1 = saturate( temp_output_628_0_g1 );
				float3 temp_cast_0 = (v.positionOS.xyz.y).xxx;
				float2 appendResult928_g1 = (float2(v.positionOS.xyz.x , v.positionOS.xyz.z));
				float3 temp_output_996_0_g1 = ( cross( temp_cast_0 , float3( appendResult928_g1 ,  0.0 ) ) * 0.005 );
				float3 appendResult931_g1 = (float3(0.0 , v.positionOS.xyz.y , 0.0));
				float3 break971_g1 = v.positionOS.xyz;
				float3 appendResult967_g1 = (float3(break971_g1.x , 0.0 , ( break971_g1.z * 0.15 )));
				float mulTime976_g1 = _TimeParameters.x * 2.3;
				float dotResult849_g1 = dot( (v.positionOS.xyz*0.02 + 0.0) , v.positionOS.xyz );
				float CenterOfMassThicknessMask854_g1 = saturate( dotResult849_g1 );
				float3 appendResult981_g1 = (float3(v.positionOS.xyz.x , 0.0 , 0.0));
				float3 break984_g1 = v.positionOS.xyz;
				float3 appendResult966_g1 = (float3(0.0 , ( break984_g1.y * 0.2 ) , ( break984_g1.z * 0.4 )));
				float mulTime977_g1 = _TimeParameters.x * 2.0;
				float3 ase_worldPos = TransformObjectToWorld( (v.positionOS).xyz );
				float3 normalizeResult765_g1 = normalize( ase_worldPos );
				float mulTime772_g1 = _TimeParameters.x * 0.25;
				float simplePerlin2D769_g1 = snoise( ( normalizeResult765_g1 + mulTime772_g1 ).xy*0.43 );
				float WindMask_LargeB770_g1 = ( simplePerlin2D769_g1 * 1.5 );
				float3 normalizeResult1092_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP_C1098_g1 = saturate( distance( normalizeResult1092_g1 , float3(0,1,0) ) );
				float3 normalizeResult774_g1 = normalize( ase_worldPos );
				float mulTime780_g1 = _TimeParameters.x * 0.26;
				float simplePerlin2D778_g1 = snoise( ( normalizeResult774_g1 + mulTime780_g1 ).xy*0.7 );
				float WindMask_LargeC779_g1 = ( simplePerlin2D778_g1 * 1.5 );
				float mulTime906_g1 = _TimeParameters.x * 3.2;
				float3 worldToObj907_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_872_0_g1 = ( mulTime906_g1 + ( 0.02 * worldToObj907_g1.x ) + ( worldToObj907_g1.y * 0.14 ) + ( worldToObj907_g1.z * 0.16 ) + float3(0.4,0.3,0.1) );
				float3 normalizeResult632_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP636_g1 = saturate( (distance( normalizeResult632_g1 , float3(0,1,0) )*1.0 + -0.05) );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float mulTime905_g1 = _TimeParameters.x * 2.3;
				float3 worldToObj908_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_866_0_g1 = ( mulTime905_g1 + ( 0.2 * worldToObj908_g1 ) + float3(0.4,0.3,0.1) );
				float mulTime904_g1 = _TimeParameters.x * 3.6;
				float3 temp_cast_4 = (v.positionOS.xyz.x).xxx;
				float3 worldToObj910_g1 = mul( GetWorldToObjectMatrix(), float4( temp_cast_4, 1 ) ).xyz;
				float temp_output_898_0_g1 = ( mulTime904_g1 + ( 0.2 * worldToObj910_g1.x ) );
				float3 normalizeResult697_g1 = normalize( v.positionOS.xyz );
				float CenterOfMass701_g1 = saturate( (distance( normalizeResult697_g1 , float3(0,1,0) )*2.0 + 0.0) );
				float SphericalMaskProxySphere704_g1 = (( _CenterofMass )?( ( temp_output_628_0_g1 * CenterOfMass701_g1 ) ):( temp_output_628_0_g1 ));
				float3 worldToObj1131_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float mulTime1138_g1 = _TimeParameters.x * 4.0;
				float mulTime1129_g1 = _TimeParameters.x * 0.2;
				float2 appendResult1126_g1 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 normalizeResult1128_g1 = normalize( appendResult1126_g1 );
				float simpleNoise1139_g1 = SimpleNoise( ( mulTime1129_g1 + normalizeResult1128_g1 )*1.0 );
				float WindMask_SimpleSway1145_g1 = ( simpleNoise1139_g1 * 1.5 );
				float3 rotatedValue1151_g1 = RotateAroundAxis( float3( 0,0,0 ), v.positionOS.xyz, normalize( float3(0.6,1,0.1) ), ( ( cos( ( ( worldToObj1131_g1 * 0.02 ) + mulTime1138_g1 + ( float3(0.6,1,0.8) * 0.3 * worldToObj1131_g1 ) ) ) * 0.1 ) * WindMask_SimpleSway1145_g1 * saturate( ase_objectScale ) ).x );
				float3 temp_cast_6 = (0.0).xxx;
				#if defined(_WINDTYPE_GENTLEBREEZE)
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#elif defined(_WINDTYPE_WINDOFF)
				float3 staticSwitch1044_g1 = temp_cast_6;
				#else
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#endif
				float3 FinalWind_Output1060_g1 = ( _GlobalWindStrength * staticSwitch1044_g1 );
				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1060_g1;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.positionWS = vertexInput.positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.positionCS = vertexInput.positionCS;
				o.clipPosV = vertexInput.positionCS;
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(	VertexOutput IN
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float4 ClipPos = IN.clipPosV;
				float4 ScreenPos = ComputeScreenPos( IN.clipPosV );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				

				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = IN.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( IN.positionCS );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 140008


			#pragma vertex vert
			#pragma fragment frag

			#pragma shader_feature EDITOR_VISUALIZATION

			#define SHADERPASS SHADERPASS_META

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#pragma shader_feature_local _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 texcoord0 : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef EDITOR_VISUALIZATION
					float4 VizUV : TEXCOORD2;
					float4 LightCoord : TEXCOORD3;
				#endif
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _GlobalWindStrength;
			float _Radius;
			float _Hardness;
			float _BranchWindLarge;
			float _CenterofMass;
			float _BranchWindSmall;
			float _PivotSway;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			sampler2D _Albedo;
			sampler2D _Mask;


			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 appendResult939_g1 = (float3(0.0 , 0.0 , saturate( v.positionOS.xyz ).z));
				float3 break989_g1 = v.positionOS.xyz;
				float3 appendResult938_g1 = (float3(break989_g1.x , ( break989_g1.y * 0.15 ) , 0.0));
				float mulTime975_g1 = _TimeParameters.x * 2.1;
				float3 temp_output_624_0_g1 = ( ( v.positionOS.xyz - float3(0,-1,0) ) / _Radius );
				float dotResult625_g1 = dot( temp_output_624_0_g1 , temp_output_624_0_g1 );
				float temp_output_628_0_g1 = pow( saturate( dotResult625_g1 ) , _Hardness );
				float SphearicalMaskCM763_g1 = saturate( temp_output_628_0_g1 );
				float3 temp_cast_0 = (v.positionOS.xyz.y).xxx;
				float2 appendResult928_g1 = (float2(v.positionOS.xyz.x , v.positionOS.xyz.z));
				float3 temp_output_996_0_g1 = ( cross( temp_cast_0 , float3( appendResult928_g1 ,  0.0 ) ) * 0.005 );
				float3 appendResult931_g1 = (float3(0.0 , v.positionOS.xyz.y , 0.0));
				float3 break971_g1 = v.positionOS.xyz;
				float3 appendResult967_g1 = (float3(break971_g1.x , 0.0 , ( break971_g1.z * 0.15 )));
				float mulTime976_g1 = _TimeParameters.x * 2.3;
				float dotResult849_g1 = dot( (v.positionOS.xyz*0.02 + 0.0) , v.positionOS.xyz );
				float CenterOfMassThicknessMask854_g1 = saturate( dotResult849_g1 );
				float3 appendResult981_g1 = (float3(v.positionOS.xyz.x , 0.0 , 0.0));
				float3 break984_g1 = v.positionOS.xyz;
				float3 appendResult966_g1 = (float3(0.0 , ( break984_g1.y * 0.2 ) , ( break984_g1.z * 0.4 )));
				float mulTime977_g1 = _TimeParameters.x * 2.0;
				float3 ase_worldPos = TransformObjectToWorld( (v.positionOS).xyz );
				float3 normalizeResult765_g1 = normalize( ase_worldPos );
				float mulTime772_g1 = _TimeParameters.x * 0.25;
				float simplePerlin2D769_g1 = snoise( ( normalizeResult765_g1 + mulTime772_g1 ).xy*0.43 );
				float WindMask_LargeB770_g1 = ( simplePerlin2D769_g1 * 1.5 );
				float3 normalizeResult1092_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP_C1098_g1 = saturate( distance( normalizeResult1092_g1 , float3(0,1,0) ) );
				float3 normalizeResult774_g1 = normalize( ase_worldPos );
				float mulTime780_g1 = _TimeParameters.x * 0.26;
				float simplePerlin2D778_g1 = snoise( ( normalizeResult774_g1 + mulTime780_g1 ).xy*0.7 );
				float WindMask_LargeC779_g1 = ( simplePerlin2D778_g1 * 1.5 );
				float mulTime906_g1 = _TimeParameters.x * 3.2;
				float3 worldToObj907_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_872_0_g1 = ( mulTime906_g1 + ( 0.02 * worldToObj907_g1.x ) + ( worldToObj907_g1.y * 0.14 ) + ( worldToObj907_g1.z * 0.16 ) + float3(0.4,0.3,0.1) );
				float3 normalizeResult632_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP636_g1 = saturate( (distance( normalizeResult632_g1 , float3(0,1,0) )*1.0 + -0.05) );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float mulTime905_g1 = _TimeParameters.x * 2.3;
				float3 worldToObj908_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_866_0_g1 = ( mulTime905_g1 + ( 0.2 * worldToObj908_g1 ) + float3(0.4,0.3,0.1) );
				float mulTime904_g1 = _TimeParameters.x * 3.6;
				float3 temp_cast_4 = (v.positionOS.xyz.x).xxx;
				float3 worldToObj910_g1 = mul( GetWorldToObjectMatrix(), float4( temp_cast_4, 1 ) ).xyz;
				float temp_output_898_0_g1 = ( mulTime904_g1 + ( 0.2 * worldToObj910_g1.x ) );
				float3 normalizeResult697_g1 = normalize( v.positionOS.xyz );
				float CenterOfMass701_g1 = saturate( (distance( normalizeResult697_g1 , float3(0,1,0) )*2.0 + 0.0) );
				float SphericalMaskProxySphere704_g1 = (( _CenterofMass )?( ( temp_output_628_0_g1 * CenterOfMass701_g1 ) ):( temp_output_628_0_g1 ));
				float3 worldToObj1131_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float mulTime1138_g1 = _TimeParameters.x * 4.0;
				float mulTime1129_g1 = _TimeParameters.x * 0.2;
				float2 appendResult1126_g1 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 normalizeResult1128_g1 = normalize( appendResult1126_g1 );
				float simpleNoise1139_g1 = SimpleNoise( ( mulTime1129_g1 + normalizeResult1128_g1 )*1.0 );
				float WindMask_SimpleSway1145_g1 = ( simpleNoise1139_g1 * 1.5 );
				float3 rotatedValue1151_g1 = RotateAroundAxis( float3( 0,0,0 ), v.positionOS.xyz, normalize( float3(0.6,1,0.1) ), ( ( cos( ( ( worldToObj1131_g1 * 0.02 ) + mulTime1138_g1 + ( float3(0.6,1,0.8) * 0.3 * worldToObj1131_g1 ) ) ) * 0.1 ) * WindMask_SimpleSway1145_g1 * saturate( ase_objectScale ) ).x );
				float3 temp_cast_6 = (0.0).xxx;
				#if defined(_WINDTYPE_GENTLEBREEZE)
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#elif defined(_WINDTYPE_WINDOFF)
				float3 staticSwitch1044_g1 = temp_cast_6;
				#else
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#endif
				float3 FinalWind_Output1060_g1 = ( _GlobalWindStrength * staticSwitch1044_g1 );
				
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.normalOS);
				o.ase_texcoord5.xyz = ase_worldNormal;
				
				o.ase_texcoord4.xy = v.texcoord0.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord4.zw = 0;
				o.ase_texcoord5.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1060_g1;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.positionWS = positionWS;
				#endif

				o.positionCS = MetaVertexPosition( v.positionOS, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );

				#ifdef EDITOR_VISUALIZATION
					float2 VizUV = 0;
					float4 LightCoord = 0;
					UnityEditorVizData(v.positionOS.xyz, v.texcoord0.xy, v.texcoord1.xy, v.texcoord2.xy, VizUV, LightCoord);
					o.VizUV = float4(VizUV, 0, 0);
					o.LightCoord = LightCoord;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 texcoord0 : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.texcoord0 = v.texcoord0;
				o.texcoord1 = v.texcoord1;
				o.texcoord2 = v.texcoord2;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.texcoord0 = patch[0].texcoord0 * bary.x + patch[1].texcoord0 * bary.y + patch[2].texcoord0 * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_Albedo2 = IN.ase_texcoord4.xy;
				float4 tex2DNode2 = tex2D( _Albedo, uv_Albedo2 );
				float2 uv_Mask4 = IN.ase_texcoord4.xy;
				float4 tex2DNode4 = tex2D( _Mask, uv_Mask4 );
				
				float4 color10 = IsGammaSpace() ? float4(0.2156863,0.5607843,0.2,1) : float4(0.03820438,0.2746773,0.03310476,1);
				float3 ase_worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = IN.ase_texcoord5.xyz;
				float fresnelNdotV5 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode5 = ( -0.1 + 5.0 * pow( 1.0 - fresnelNdotV5, 5.0 ) );
				

				float3 BaseColor = ( tex2DNode2 * saturate( tex2DNode4.g ) ).rgb;
				float3 Emission = ( saturate( ( color10 * fresnelNode5 ) ) + (tex2DNode2*0.4 + 0.0) ).rgb;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = BaseColor;
				metaInput.Emission = Emission;
				#ifdef EDITOR_VISUALIZATION
					metaInput.VizUV = IN.VizUV.xy;
					metaInput.LightCoord = IN.LightCoord;
				#endif

				return UnityMetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 140008


			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature_local _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _GlobalWindStrength;
			float _Radius;
			float _Hardness;
			float _BranchWindLarge;
			float _CenterofMass;
			float _BranchWindSmall;
			float _PivotSway;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			sampler2D _Albedo;
			sampler2D _Mask;


			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float3 appendResult939_g1 = (float3(0.0 , 0.0 , saturate( v.positionOS.xyz ).z));
				float3 break989_g1 = v.positionOS.xyz;
				float3 appendResult938_g1 = (float3(break989_g1.x , ( break989_g1.y * 0.15 ) , 0.0));
				float mulTime975_g1 = _TimeParameters.x * 2.1;
				float3 temp_output_624_0_g1 = ( ( v.positionOS.xyz - float3(0,-1,0) ) / _Radius );
				float dotResult625_g1 = dot( temp_output_624_0_g1 , temp_output_624_0_g1 );
				float temp_output_628_0_g1 = pow( saturate( dotResult625_g1 ) , _Hardness );
				float SphearicalMaskCM763_g1 = saturate( temp_output_628_0_g1 );
				float3 temp_cast_0 = (v.positionOS.xyz.y).xxx;
				float2 appendResult928_g1 = (float2(v.positionOS.xyz.x , v.positionOS.xyz.z));
				float3 temp_output_996_0_g1 = ( cross( temp_cast_0 , float3( appendResult928_g1 ,  0.0 ) ) * 0.005 );
				float3 appendResult931_g1 = (float3(0.0 , v.positionOS.xyz.y , 0.0));
				float3 break971_g1 = v.positionOS.xyz;
				float3 appendResult967_g1 = (float3(break971_g1.x , 0.0 , ( break971_g1.z * 0.15 )));
				float mulTime976_g1 = _TimeParameters.x * 2.3;
				float dotResult849_g1 = dot( (v.positionOS.xyz*0.02 + 0.0) , v.positionOS.xyz );
				float CenterOfMassThicknessMask854_g1 = saturate( dotResult849_g1 );
				float3 appendResult981_g1 = (float3(v.positionOS.xyz.x , 0.0 , 0.0));
				float3 break984_g1 = v.positionOS.xyz;
				float3 appendResult966_g1 = (float3(0.0 , ( break984_g1.y * 0.2 ) , ( break984_g1.z * 0.4 )));
				float mulTime977_g1 = _TimeParameters.x * 2.0;
				float3 ase_worldPos = TransformObjectToWorld( (v.positionOS).xyz );
				float3 normalizeResult765_g1 = normalize( ase_worldPos );
				float mulTime772_g1 = _TimeParameters.x * 0.25;
				float simplePerlin2D769_g1 = snoise( ( normalizeResult765_g1 + mulTime772_g1 ).xy*0.43 );
				float WindMask_LargeB770_g1 = ( simplePerlin2D769_g1 * 1.5 );
				float3 normalizeResult1092_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP_C1098_g1 = saturate( distance( normalizeResult1092_g1 , float3(0,1,0) ) );
				float3 normalizeResult774_g1 = normalize( ase_worldPos );
				float mulTime780_g1 = _TimeParameters.x * 0.26;
				float simplePerlin2D778_g1 = snoise( ( normalizeResult774_g1 + mulTime780_g1 ).xy*0.7 );
				float WindMask_LargeC779_g1 = ( simplePerlin2D778_g1 * 1.5 );
				float mulTime906_g1 = _TimeParameters.x * 3.2;
				float3 worldToObj907_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_872_0_g1 = ( mulTime906_g1 + ( 0.02 * worldToObj907_g1.x ) + ( worldToObj907_g1.y * 0.14 ) + ( worldToObj907_g1.z * 0.16 ) + float3(0.4,0.3,0.1) );
				float3 normalizeResult632_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP636_g1 = saturate( (distance( normalizeResult632_g1 , float3(0,1,0) )*1.0 + -0.05) );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float mulTime905_g1 = _TimeParameters.x * 2.3;
				float3 worldToObj908_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_866_0_g1 = ( mulTime905_g1 + ( 0.2 * worldToObj908_g1 ) + float3(0.4,0.3,0.1) );
				float mulTime904_g1 = _TimeParameters.x * 3.6;
				float3 temp_cast_4 = (v.positionOS.xyz.x).xxx;
				float3 worldToObj910_g1 = mul( GetWorldToObjectMatrix(), float4( temp_cast_4, 1 ) ).xyz;
				float temp_output_898_0_g1 = ( mulTime904_g1 + ( 0.2 * worldToObj910_g1.x ) );
				float3 normalizeResult697_g1 = normalize( v.positionOS.xyz );
				float CenterOfMass701_g1 = saturate( (distance( normalizeResult697_g1 , float3(0,1,0) )*2.0 + 0.0) );
				float SphericalMaskProxySphere704_g1 = (( _CenterofMass )?( ( temp_output_628_0_g1 * CenterOfMass701_g1 ) ):( temp_output_628_0_g1 ));
				float3 worldToObj1131_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float mulTime1138_g1 = _TimeParameters.x * 4.0;
				float mulTime1129_g1 = _TimeParameters.x * 0.2;
				float2 appendResult1126_g1 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 normalizeResult1128_g1 = normalize( appendResult1126_g1 );
				float simpleNoise1139_g1 = SimpleNoise( ( mulTime1129_g1 + normalizeResult1128_g1 )*1.0 );
				float WindMask_SimpleSway1145_g1 = ( simpleNoise1139_g1 * 1.5 );
				float3 rotatedValue1151_g1 = RotateAroundAxis( float3( 0,0,0 ), v.positionOS.xyz, normalize( float3(0.6,1,0.1) ), ( ( cos( ( ( worldToObj1131_g1 * 0.02 ) + mulTime1138_g1 + ( float3(0.6,1,0.8) * 0.3 * worldToObj1131_g1 ) ) ) * 0.1 ) * WindMask_SimpleSway1145_g1 * saturate( ase_objectScale ) ).x );
				float3 temp_cast_6 = (0.0).xxx;
				#if defined(_WINDTYPE_GENTLEBREEZE)
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#elif defined(_WINDTYPE_WINDOFF)
				float3 staticSwitch1044_g1 = temp_cast_6;
				#else
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#endif
				float3 FinalWind_Output1060_g1 = ( _GlobalWindStrength * staticSwitch1044_g1 );
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1060_g1;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.positionOS.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.positionWS = vertexInput.positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.positionCS = vertexInput.positionCS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_Albedo2 = IN.ase_texcoord2.xy;
				float4 tex2DNode2 = tex2D( _Albedo, uv_Albedo2 );
				float2 uv_Mask4 = IN.ase_texcoord2.xy;
				float4 tex2DNode4 = tex2D( _Mask, uv_Mask4 );
				

				float3 BaseColor = ( tex2DNode2 * saturate( tex2DNode4.g ) ).rgb;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				half4 color = half4(BaseColor, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormals" }

			ZWrite On
			Blend One Zero
			ZTest LEqual
			ZWrite On

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 140008


			#pragma vertex vert
			#pragma fragment frag

			

			

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
			//#define SHADERPASS SHADERPASS_DEPTHNORMALS

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			
			#if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature_local _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float4 worldTangent : TEXCOORD2;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 positionWS : TEXCOORD3;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD4;
				#endif
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _GlobalWindStrength;
			float _Radius;
			float _Hardness;
			float _BranchWindLarge;
			float _CenterofMass;
			float _BranchWindSmall;
			float _PivotSway;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			sampler2D _Normal;


			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 appendResult939_g1 = (float3(0.0 , 0.0 , saturate( v.positionOS.xyz ).z));
				float3 break989_g1 = v.positionOS.xyz;
				float3 appendResult938_g1 = (float3(break989_g1.x , ( break989_g1.y * 0.15 ) , 0.0));
				float mulTime975_g1 = _TimeParameters.x * 2.1;
				float3 temp_output_624_0_g1 = ( ( v.positionOS.xyz - float3(0,-1,0) ) / _Radius );
				float dotResult625_g1 = dot( temp_output_624_0_g1 , temp_output_624_0_g1 );
				float temp_output_628_0_g1 = pow( saturate( dotResult625_g1 ) , _Hardness );
				float SphearicalMaskCM763_g1 = saturate( temp_output_628_0_g1 );
				float3 temp_cast_0 = (v.positionOS.xyz.y).xxx;
				float2 appendResult928_g1 = (float2(v.positionOS.xyz.x , v.positionOS.xyz.z));
				float3 temp_output_996_0_g1 = ( cross( temp_cast_0 , float3( appendResult928_g1 ,  0.0 ) ) * 0.005 );
				float3 appendResult931_g1 = (float3(0.0 , v.positionOS.xyz.y , 0.0));
				float3 break971_g1 = v.positionOS.xyz;
				float3 appendResult967_g1 = (float3(break971_g1.x , 0.0 , ( break971_g1.z * 0.15 )));
				float mulTime976_g1 = _TimeParameters.x * 2.3;
				float dotResult849_g1 = dot( (v.positionOS.xyz*0.02 + 0.0) , v.positionOS.xyz );
				float CenterOfMassThicknessMask854_g1 = saturate( dotResult849_g1 );
				float3 appendResult981_g1 = (float3(v.positionOS.xyz.x , 0.0 , 0.0));
				float3 break984_g1 = v.positionOS.xyz;
				float3 appendResult966_g1 = (float3(0.0 , ( break984_g1.y * 0.2 ) , ( break984_g1.z * 0.4 )));
				float mulTime977_g1 = _TimeParameters.x * 2.0;
				float3 ase_worldPos = TransformObjectToWorld( (v.positionOS).xyz );
				float3 normalizeResult765_g1 = normalize( ase_worldPos );
				float mulTime772_g1 = _TimeParameters.x * 0.25;
				float simplePerlin2D769_g1 = snoise( ( normalizeResult765_g1 + mulTime772_g1 ).xy*0.43 );
				float WindMask_LargeB770_g1 = ( simplePerlin2D769_g1 * 1.5 );
				float3 normalizeResult1092_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP_C1098_g1 = saturate( distance( normalizeResult1092_g1 , float3(0,1,0) ) );
				float3 normalizeResult774_g1 = normalize( ase_worldPos );
				float mulTime780_g1 = _TimeParameters.x * 0.26;
				float simplePerlin2D778_g1 = snoise( ( normalizeResult774_g1 + mulTime780_g1 ).xy*0.7 );
				float WindMask_LargeC779_g1 = ( simplePerlin2D778_g1 * 1.5 );
				float mulTime906_g1 = _TimeParameters.x * 3.2;
				float3 worldToObj907_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_872_0_g1 = ( mulTime906_g1 + ( 0.02 * worldToObj907_g1.x ) + ( worldToObj907_g1.y * 0.14 ) + ( worldToObj907_g1.z * 0.16 ) + float3(0.4,0.3,0.1) );
				float3 normalizeResult632_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP636_g1 = saturate( (distance( normalizeResult632_g1 , float3(0,1,0) )*1.0 + -0.05) );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float mulTime905_g1 = _TimeParameters.x * 2.3;
				float3 worldToObj908_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_866_0_g1 = ( mulTime905_g1 + ( 0.2 * worldToObj908_g1 ) + float3(0.4,0.3,0.1) );
				float mulTime904_g1 = _TimeParameters.x * 3.6;
				float3 temp_cast_4 = (v.positionOS.xyz.x).xxx;
				float3 worldToObj910_g1 = mul( GetWorldToObjectMatrix(), float4( temp_cast_4, 1 ) ).xyz;
				float temp_output_898_0_g1 = ( mulTime904_g1 + ( 0.2 * worldToObj910_g1.x ) );
				float3 normalizeResult697_g1 = normalize( v.positionOS.xyz );
				float CenterOfMass701_g1 = saturate( (distance( normalizeResult697_g1 , float3(0,1,0) )*2.0 + 0.0) );
				float SphericalMaskProxySphere704_g1 = (( _CenterofMass )?( ( temp_output_628_0_g1 * CenterOfMass701_g1 ) ):( temp_output_628_0_g1 ));
				float3 worldToObj1131_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float mulTime1138_g1 = _TimeParameters.x * 4.0;
				float mulTime1129_g1 = _TimeParameters.x * 0.2;
				float2 appendResult1126_g1 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 normalizeResult1128_g1 = normalize( appendResult1126_g1 );
				float simpleNoise1139_g1 = SimpleNoise( ( mulTime1129_g1 + normalizeResult1128_g1 )*1.0 );
				float WindMask_SimpleSway1145_g1 = ( simpleNoise1139_g1 * 1.5 );
				float3 rotatedValue1151_g1 = RotateAroundAxis( float3( 0,0,0 ), v.positionOS.xyz, normalize( float3(0.6,1,0.1) ), ( ( cos( ( ( worldToObj1131_g1 * 0.02 ) + mulTime1138_g1 + ( float3(0.6,1,0.8) * 0.3 * worldToObj1131_g1 ) ) ) * 0.1 ) * WindMask_SimpleSway1145_g1 * saturate( ase_objectScale ) ).x );
				float3 temp_cast_6 = (0.0).xxx;
				#if defined(_WINDTYPE_GENTLEBREEZE)
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#elif defined(_WINDTYPE_WINDOFF)
				float3 staticSwitch1044_g1 = temp_cast_6;
				#else
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#endif
				float3 FinalWind_Output1060_g1 = ( _GlobalWindStrength * staticSwitch1044_g1 );
				
				o.ase_texcoord5.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord5.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1060_g1;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;
				v.tangentOS = v.tangentOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.positionOS.xyz );

				float3 normalWS = TransformObjectToWorldNormal( v.normalOS );
				float4 tangentWS = float4( TransformObjectToWorldDir( v.tangentOS.xyz ), v.tangentOS.w );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.positionWS = vertexInput.positionWS;
				#endif

				o.worldNormal = normalWS;
				o.worldTangent = tangentWS;

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.positionCS = vertexInput.positionCS;
				o.clipPosV = vertexInput.positionCS;
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.tangentOS = v.tangentOS;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			void frag(	VertexOutput IN
						, out half4 outNormalWS : SV_Target0
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out float4 outRenderingLayers : SV_Target1
						#endif
						 )
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.positionWS;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				float3 WorldNormal = IN.worldNormal;
				float4 WorldTangent = IN.worldTangent;

				float4 ClipPos = IN.clipPosV;
				float4 ScreenPos = ComputeScreenPos( IN.clipPosV );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_Normal3 = IN.ase_texcoord5.xy;
				

				float3 Normal = UnpackNormalScale( tex2D( _Normal, uv_Normal3 ), 1.0f );
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = IN.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( IN.positionCS );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				#if defined(_GBUFFER_NORMALS_OCT)
					float2 octNormalWS = PackNormalOctQuadEncode(WorldNormal);
					float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);
					half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);
					outNormalWS = half4(packedNormalWS, 0.0);
				#else
					#if defined(_NORMALMAP)
						#if _NORMAL_DROPOFF_TS
							float crossSign = (WorldTangent.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
							float3 bitangent = crossSign * cross(WorldNormal.xyz, WorldTangent.xyz);
							float3 normalWS = TransformTangentToWorld(Normal, half3x3(WorldTangent.xyz, bitangent, WorldNormal.xyz));
						#elif _NORMAL_DROPOFF_OS
							float3 normalWS = TransformObjectToWorldNormal(Normal);
						#elif _NORMAL_DROPOFF_WS
							float3 normalWS = Normal;
						#endif
					#else
						float3 normalWS = WorldNormal;
					#endif
					outNormalWS = half4(NormalizeNormalPerPixel(normalWS), 0.0);
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4( EncodeMeshRenderingLayer( renderingLayers ), 0, 0, 0 );
				#endif
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "GBuffer"
			Tags { "LightMode"="UniversalGBuffer" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 140008


			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF
			#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION

			
			#pragma multi_compile_fragment _ _SHADOWS_SOFT
           

			

			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
			#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
			#pragma multi_compile_fragment _ _RENDER_PASS_ENABLED
      
			

			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			#pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fragment _ DEBUG_DISPLAY

			

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_GBUFFER

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			
			#if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif
			
			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
				#define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_WORLD_VIEW_DIR
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#pragma shader_feature_local _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float4 clipPosV : TEXCOORD0;
				float4 lightmapUVOrVertexSH : TEXCOORD1;
				half4 fogFactorAndVertexLight : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD6;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
				float2 dynamicLightmapUV : TEXCOORD7;
				#endif
				float4 ase_texcoord8 : TEXCOORD8;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _GlobalWindStrength;
			float _Radius;
			float _Hardness;
			float _BranchWindLarge;
			float _CenterofMass;
			float _BranchWindSmall;
			float _PivotSway;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			sampler2D _Albedo;
			sampler2D _Mask;
			sampler2D _Normal;


			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"

			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 appendResult939_g1 = (float3(0.0 , 0.0 , saturate( v.positionOS.xyz ).z));
				float3 break989_g1 = v.positionOS.xyz;
				float3 appendResult938_g1 = (float3(break989_g1.x , ( break989_g1.y * 0.15 ) , 0.0));
				float mulTime975_g1 = _TimeParameters.x * 2.1;
				float3 temp_output_624_0_g1 = ( ( v.positionOS.xyz - float3(0,-1,0) ) / _Radius );
				float dotResult625_g1 = dot( temp_output_624_0_g1 , temp_output_624_0_g1 );
				float temp_output_628_0_g1 = pow( saturate( dotResult625_g1 ) , _Hardness );
				float SphearicalMaskCM763_g1 = saturate( temp_output_628_0_g1 );
				float3 temp_cast_0 = (v.positionOS.xyz.y).xxx;
				float2 appendResult928_g1 = (float2(v.positionOS.xyz.x , v.positionOS.xyz.z));
				float3 temp_output_996_0_g1 = ( cross( temp_cast_0 , float3( appendResult928_g1 ,  0.0 ) ) * 0.005 );
				float3 appendResult931_g1 = (float3(0.0 , v.positionOS.xyz.y , 0.0));
				float3 break971_g1 = v.positionOS.xyz;
				float3 appendResult967_g1 = (float3(break971_g1.x , 0.0 , ( break971_g1.z * 0.15 )));
				float mulTime976_g1 = _TimeParameters.x * 2.3;
				float dotResult849_g1 = dot( (v.positionOS.xyz*0.02 + 0.0) , v.positionOS.xyz );
				float CenterOfMassThicknessMask854_g1 = saturate( dotResult849_g1 );
				float3 appendResult981_g1 = (float3(v.positionOS.xyz.x , 0.0 , 0.0));
				float3 break984_g1 = v.positionOS.xyz;
				float3 appendResult966_g1 = (float3(0.0 , ( break984_g1.y * 0.2 ) , ( break984_g1.z * 0.4 )));
				float mulTime977_g1 = _TimeParameters.x * 2.0;
				float3 ase_worldPos = TransformObjectToWorld( (v.positionOS).xyz );
				float3 normalizeResult765_g1 = normalize( ase_worldPos );
				float mulTime772_g1 = _TimeParameters.x * 0.25;
				float simplePerlin2D769_g1 = snoise( ( normalizeResult765_g1 + mulTime772_g1 ).xy*0.43 );
				float WindMask_LargeB770_g1 = ( simplePerlin2D769_g1 * 1.5 );
				float3 normalizeResult1092_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP_C1098_g1 = saturate( distance( normalizeResult1092_g1 , float3(0,1,0) ) );
				float3 normalizeResult774_g1 = normalize( ase_worldPos );
				float mulTime780_g1 = _TimeParameters.x * 0.26;
				float simplePerlin2D778_g1 = snoise( ( normalizeResult774_g1 + mulTime780_g1 ).xy*0.7 );
				float WindMask_LargeC779_g1 = ( simplePerlin2D778_g1 * 1.5 );
				float mulTime906_g1 = _TimeParameters.x * 3.2;
				float3 worldToObj907_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_872_0_g1 = ( mulTime906_g1 + ( 0.02 * worldToObj907_g1.x ) + ( worldToObj907_g1.y * 0.14 ) + ( worldToObj907_g1.z * 0.16 ) + float3(0.4,0.3,0.1) );
				float3 normalizeResult632_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP636_g1 = saturate( (distance( normalizeResult632_g1 , float3(0,1,0) )*1.0 + -0.05) );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float mulTime905_g1 = _TimeParameters.x * 2.3;
				float3 worldToObj908_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_866_0_g1 = ( mulTime905_g1 + ( 0.2 * worldToObj908_g1 ) + float3(0.4,0.3,0.1) );
				float mulTime904_g1 = _TimeParameters.x * 3.6;
				float3 temp_cast_4 = (v.positionOS.xyz.x).xxx;
				float3 worldToObj910_g1 = mul( GetWorldToObjectMatrix(), float4( temp_cast_4, 1 ) ).xyz;
				float temp_output_898_0_g1 = ( mulTime904_g1 + ( 0.2 * worldToObj910_g1.x ) );
				float3 normalizeResult697_g1 = normalize( v.positionOS.xyz );
				float CenterOfMass701_g1 = saturate( (distance( normalizeResult697_g1 , float3(0,1,0) )*2.0 + 0.0) );
				float SphericalMaskProxySphere704_g1 = (( _CenterofMass )?( ( temp_output_628_0_g1 * CenterOfMass701_g1 ) ):( temp_output_628_0_g1 ));
				float3 worldToObj1131_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float mulTime1138_g1 = _TimeParameters.x * 4.0;
				float mulTime1129_g1 = _TimeParameters.x * 0.2;
				float2 appendResult1126_g1 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 normalizeResult1128_g1 = normalize( appendResult1126_g1 );
				float simpleNoise1139_g1 = SimpleNoise( ( mulTime1129_g1 + normalizeResult1128_g1 )*1.0 );
				float WindMask_SimpleSway1145_g1 = ( simpleNoise1139_g1 * 1.5 );
				float3 rotatedValue1151_g1 = RotateAroundAxis( float3( 0,0,0 ), v.positionOS.xyz, normalize( float3(0.6,1,0.1) ), ( ( cos( ( ( worldToObj1131_g1 * 0.02 ) + mulTime1138_g1 + ( float3(0.6,1,0.8) * 0.3 * worldToObj1131_g1 ) ) ) * 0.1 ) * WindMask_SimpleSway1145_g1 * saturate( ase_objectScale ) ).x );
				float3 temp_cast_6 = (0.0).xxx;
				#if defined(_WINDTYPE_GENTLEBREEZE)
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#elif defined(_WINDTYPE_WINDOFF)
				float3 staticSwitch1044_g1 = temp_cast_6;
				#else
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#endif
				float3 FinalWind_Output1060_g1 = ( _GlobalWindStrength * staticSwitch1044_g1 );
				
				o.ase_texcoord8.xy = v.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord8.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1060_g1;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;
				v.tangentOS = v.tangentOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.positionOS.xyz );
				VertexNormalInputs normalInput = GetVertexNormalInputs( v.normalOS, v.tangentOS );

				o.tSpace0 = float4( normalInput.normalWS, vertexInput.positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, vertexInput.positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, vertexInput.positionWS.z);

				#if defined(LIGHTMAP_ON)
					OUTPUT_LIGHTMAP_UV(v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy);
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
					o.dynamicLightmapUV.xy = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif

				#if !defined(LIGHTMAP_ON)
					OUTPUT_SH(normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz);
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord.xy;
					o.lightmapUVOrVertexSH.xy = v.texcoord.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( vertexInput.positionWS, normalInput.normalWS );

				o.fogFactorAndVertexLight = half4(0, vertexLight);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.positionCS = vertexInput.positionCS;
				o.clipPosV = vertexInput.positionCS;
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				o.tangentOS = v.tangentOS;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				o.texcoord2 = v.texcoord2;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			FragmentOutput frag ( VertexOutput IN
								#ifdef ASE_DEPTH_WRITE_ON
								,out float outputDepth : ASE_SV_DEPTH
								#endif
								 )
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( IN.positionCS );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif

				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				float4 ClipPos = IN.clipPosV;
				float4 ScreenPos = ComputeScreenPos( IN.clipPosV );

				float2 NormalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.positionCS);

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#else
					ShadowCoords = float4(0, 0, 0, 0);
				#endif

				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 uv_Albedo2 = IN.ase_texcoord8.xy;
				float4 tex2DNode2 = tex2D( _Albedo, uv_Albedo2 );
				float2 uv_Mask4 = IN.ase_texcoord8.xy;
				float4 tex2DNode4 = tex2D( _Mask, uv_Mask4 );
				
				float2 uv_Normal3 = IN.ase_texcoord8.xy;
				
				float4 color10 = IsGammaSpace() ? float4(0.2156863,0.5607843,0.2,1) : float4(0.03820438,0.2746773,0.03310476,1);
				float fresnelNdotV5 = dot( WorldNormal, WorldViewDirection );
				float fresnelNode5 = ( -0.1 + 5.0 * pow( 1.0 - fresnelNdotV5, 5.0 ) );
				

				float3 BaseColor = ( tex2DNode2 * saturate( tex2DNode4.g ) ).rgb;
				float3 Normal = UnpackNormalScale( tex2D( _Normal, uv_Normal3 ), 1.0f );
				float3 Emission = ( saturate( ( color10 * fresnelNode5 ) ) + (tex2DNode2*0.4 + 0.0) ).rgb;
				float3 Specular = 0.5;
				float Metallic = 0;
				float Smoothness = tex2DNode4.a;
				float Occlusion = tex2DNode4.g;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;

				#ifdef ASE_DEPTH_WRITE_ON
					float DepthValue = IN.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = WorldPosition;
				inputData.positionCS = IN.positionCS;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
						inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
						inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
						inputData.normalWS = Normal;
					#endif
				#else
					inputData.normalWS = WorldNormal;
				#endif

				inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				inputData.viewDirectionWS = SafeNormalize( WorldViewDirection );

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				#ifdef ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#else
					#if defined(DYNAMICLIGHTMAP_ON)
						inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, IN.dynamicLightmapUV.xy, SH, inputData.normalWS);
					#else
						inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );
					#endif
				#endif

				inputData.normalizedScreenSpaceUV = NormalizedScreenSpaceUV;
				inputData.shadowMask = SAMPLE_SHADOWMASK(IN.lightmapUVOrVertexSH.xy);

				#if defined(DEBUG_DISPLAY)
					#if defined(DYNAMICLIGHTMAP_ON)
						inputData.dynamicLightmapUV = IN.dynamicLightmapUV.xy;
						#endif
					#if defined(LIGHTMAP_ON)
						inputData.staticLightmapUV = IN.lightmapUVOrVertexSH.xy;
					#else
						inputData.vertexSH = SH;
					#endif
				#endif

				#ifdef _DBUFFER
					ApplyDecal(IN.positionCS,
						BaseColor,
						Specular,
						inputData.normalWS,
						Metallic,
						Occlusion,
						Smoothness);
				#endif

				BRDFData brdfData;
				InitializeBRDFData
				(BaseColor, Metallic, Specular, Smoothness, Alpha, brdfData);

				Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, inputData.shadowMask);
				half4 color;
				MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, inputData.shadowMask);
				color.rgb = GlobalIllumination(brdfData, inputData.bakedGI, Occlusion, inputData.positionWS, inputData.normalWS, inputData.viewDirectionWS);
				color.a = Alpha;

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return BRDFDataToGbuffer(brdfData, inputData, Smoothness, Emission + color.rgb, Occlusion);
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "SceneSelectionPass"
			Tags { "LightMode"="SceneSelectionPass" }

			Cull Off
			AlphaToMask Off

			HLSLPROGRAM

			

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 140008


			

			#pragma vertex vert
			#pragma fragment frag

			#define SCENESELECTIONPASS 1

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature_local _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _GlobalWindStrength;
			float _Radius;
			float _Hardness;
			float _BranchWindLarge;
			float _CenterofMass;
			float _BranchWindSmall;
			float _PivotSway;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			

			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 appendResult939_g1 = (float3(0.0 , 0.0 , saturate( v.positionOS.xyz ).z));
				float3 break989_g1 = v.positionOS.xyz;
				float3 appendResult938_g1 = (float3(break989_g1.x , ( break989_g1.y * 0.15 ) , 0.0));
				float mulTime975_g1 = _TimeParameters.x * 2.1;
				float3 temp_output_624_0_g1 = ( ( v.positionOS.xyz - float3(0,-1,0) ) / _Radius );
				float dotResult625_g1 = dot( temp_output_624_0_g1 , temp_output_624_0_g1 );
				float temp_output_628_0_g1 = pow( saturate( dotResult625_g1 ) , _Hardness );
				float SphearicalMaskCM763_g1 = saturate( temp_output_628_0_g1 );
				float3 temp_cast_0 = (v.positionOS.xyz.y).xxx;
				float2 appendResult928_g1 = (float2(v.positionOS.xyz.x , v.positionOS.xyz.z));
				float3 temp_output_996_0_g1 = ( cross( temp_cast_0 , float3( appendResult928_g1 ,  0.0 ) ) * 0.005 );
				float3 appendResult931_g1 = (float3(0.0 , v.positionOS.xyz.y , 0.0));
				float3 break971_g1 = v.positionOS.xyz;
				float3 appendResult967_g1 = (float3(break971_g1.x , 0.0 , ( break971_g1.z * 0.15 )));
				float mulTime976_g1 = _TimeParameters.x * 2.3;
				float dotResult849_g1 = dot( (v.positionOS.xyz*0.02 + 0.0) , v.positionOS.xyz );
				float CenterOfMassThicknessMask854_g1 = saturate( dotResult849_g1 );
				float3 appendResult981_g1 = (float3(v.positionOS.xyz.x , 0.0 , 0.0));
				float3 break984_g1 = v.positionOS.xyz;
				float3 appendResult966_g1 = (float3(0.0 , ( break984_g1.y * 0.2 ) , ( break984_g1.z * 0.4 )));
				float mulTime977_g1 = _TimeParameters.x * 2.0;
				float3 ase_worldPos = TransformObjectToWorld( (v.positionOS).xyz );
				float3 normalizeResult765_g1 = normalize( ase_worldPos );
				float mulTime772_g1 = _TimeParameters.x * 0.25;
				float simplePerlin2D769_g1 = snoise( ( normalizeResult765_g1 + mulTime772_g1 ).xy*0.43 );
				float WindMask_LargeB770_g1 = ( simplePerlin2D769_g1 * 1.5 );
				float3 normalizeResult1092_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP_C1098_g1 = saturate( distance( normalizeResult1092_g1 , float3(0,1,0) ) );
				float3 normalizeResult774_g1 = normalize( ase_worldPos );
				float mulTime780_g1 = _TimeParameters.x * 0.26;
				float simplePerlin2D778_g1 = snoise( ( normalizeResult774_g1 + mulTime780_g1 ).xy*0.7 );
				float WindMask_LargeC779_g1 = ( simplePerlin2D778_g1 * 1.5 );
				float mulTime906_g1 = _TimeParameters.x * 3.2;
				float3 worldToObj907_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_872_0_g1 = ( mulTime906_g1 + ( 0.02 * worldToObj907_g1.x ) + ( worldToObj907_g1.y * 0.14 ) + ( worldToObj907_g1.z * 0.16 ) + float3(0.4,0.3,0.1) );
				float3 normalizeResult632_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP636_g1 = saturate( (distance( normalizeResult632_g1 , float3(0,1,0) )*1.0 + -0.05) );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float mulTime905_g1 = _TimeParameters.x * 2.3;
				float3 worldToObj908_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_866_0_g1 = ( mulTime905_g1 + ( 0.2 * worldToObj908_g1 ) + float3(0.4,0.3,0.1) );
				float mulTime904_g1 = _TimeParameters.x * 3.6;
				float3 temp_cast_4 = (v.positionOS.xyz.x).xxx;
				float3 worldToObj910_g1 = mul( GetWorldToObjectMatrix(), float4( temp_cast_4, 1 ) ).xyz;
				float temp_output_898_0_g1 = ( mulTime904_g1 + ( 0.2 * worldToObj910_g1.x ) );
				float3 normalizeResult697_g1 = normalize( v.positionOS.xyz );
				float CenterOfMass701_g1 = saturate( (distance( normalizeResult697_g1 , float3(0,1,0) )*2.0 + 0.0) );
				float SphericalMaskProxySphere704_g1 = (( _CenterofMass )?( ( temp_output_628_0_g1 * CenterOfMass701_g1 ) ):( temp_output_628_0_g1 ));
				float3 worldToObj1131_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float mulTime1138_g1 = _TimeParameters.x * 4.0;
				float mulTime1129_g1 = _TimeParameters.x * 0.2;
				float2 appendResult1126_g1 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 normalizeResult1128_g1 = normalize( appendResult1126_g1 );
				float simpleNoise1139_g1 = SimpleNoise( ( mulTime1129_g1 + normalizeResult1128_g1 )*1.0 );
				float WindMask_SimpleSway1145_g1 = ( simpleNoise1139_g1 * 1.5 );
				float3 rotatedValue1151_g1 = RotateAroundAxis( float3( 0,0,0 ), v.positionOS.xyz, normalize( float3(0.6,1,0.1) ), ( ( cos( ( ( worldToObj1131_g1 * 0.02 ) + mulTime1138_g1 + ( float3(0.6,1,0.8) * 0.3 * worldToObj1131_g1 ) ) ) * 0.1 ) * WindMask_SimpleSway1145_g1 * saturate( ase_objectScale ) ).x );
				float3 temp_cast_6 = (0.0).xxx;
				#if defined(_WINDTYPE_GENTLEBREEZE)
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#elif defined(_WINDTYPE_WINDOFF)
				float3 staticSwitch1044_g1 = temp_cast_6;
				#else
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#endif
				float3 FinalWind_Output1060_g1 = ( _GlobalWindStrength * staticSwitch1044_g1 );
				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1060_g1;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );

				o.positionCS = TransformWorldToHClip(positionWS);

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				

				surfaceDescription.Alpha = 1;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;

				#ifdef SCENESELECTIONPASS
					outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				#elif defined(SCENEPICKINGPASS)
					outColor = _SelectionID;
				#endif

				return outColor;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ScenePickingPass"
			Tags { "LightMode"="Picking" }

			AlphaToMask Off

			HLSLPROGRAM

			

			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 140008


			

			#pragma vertex vert
			#pragma fragment frag

		    #define SCENEPICKINGPASS 1

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"

			

			

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			
            #if ASE_SRP_VERSION >=140007
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#endif
		

			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature_local _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _GlobalWindStrength;
			float _Radius;
			float _Hardness;
			float _BranchWindLarge;
			float _CenterofMass;
			float _BranchWindSmall;
			float _PivotSway;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			

			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 appendResult939_g1 = (float3(0.0 , 0.0 , saturate( v.positionOS.xyz ).z));
				float3 break989_g1 = v.positionOS.xyz;
				float3 appendResult938_g1 = (float3(break989_g1.x , ( break989_g1.y * 0.15 ) , 0.0));
				float mulTime975_g1 = _TimeParameters.x * 2.1;
				float3 temp_output_624_0_g1 = ( ( v.positionOS.xyz - float3(0,-1,0) ) / _Radius );
				float dotResult625_g1 = dot( temp_output_624_0_g1 , temp_output_624_0_g1 );
				float temp_output_628_0_g1 = pow( saturate( dotResult625_g1 ) , _Hardness );
				float SphearicalMaskCM763_g1 = saturate( temp_output_628_0_g1 );
				float3 temp_cast_0 = (v.positionOS.xyz.y).xxx;
				float2 appendResult928_g1 = (float2(v.positionOS.xyz.x , v.positionOS.xyz.z));
				float3 temp_output_996_0_g1 = ( cross( temp_cast_0 , float3( appendResult928_g1 ,  0.0 ) ) * 0.005 );
				float3 appendResult931_g1 = (float3(0.0 , v.positionOS.xyz.y , 0.0));
				float3 break971_g1 = v.positionOS.xyz;
				float3 appendResult967_g1 = (float3(break971_g1.x , 0.0 , ( break971_g1.z * 0.15 )));
				float mulTime976_g1 = _TimeParameters.x * 2.3;
				float dotResult849_g1 = dot( (v.positionOS.xyz*0.02 + 0.0) , v.positionOS.xyz );
				float CenterOfMassThicknessMask854_g1 = saturate( dotResult849_g1 );
				float3 appendResult981_g1 = (float3(v.positionOS.xyz.x , 0.0 , 0.0));
				float3 break984_g1 = v.positionOS.xyz;
				float3 appendResult966_g1 = (float3(0.0 , ( break984_g1.y * 0.2 ) , ( break984_g1.z * 0.4 )));
				float mulTime977_g1 = _TimeParameters.x * 2.0;
				float3 ase_worldPos = TransformObjectToWorld( (v.positionOS).xyz );
				float3 normalizeResult765_g1 = normalize( ase_worldPos );
				float mulTime772_g1 = _TimeParameters.x * 0.25;
				float simplePerlin2D769_g1 = snoise( ( normalizeResult765_g1 + mulTime772_g1 ).xy*0.43 );
				float WindMask_LargeB770_g1 = ( simplePerlin2D769_g1 * 1.5 );
				float3 normalizeResult1092_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP_C1098_g1 = saturate( distance( normalizeResult1092_g1 , float3(0,1,0) ) );
				float3 normalizeResult774_g1 = normalize( ase_worldPos );
				float mulTime780_g1 = _TimeParameters.x * 0.26;
				float simplePerlin2D778_g1 = snoise( ( normalizeResult774_g1 + mulTime780_g1 ).xy*0.7 );
				float WindMask_LargeC779_g1 = ( simplePerlin2D778_g1 * 1.5 );
				float mulTime906_g1 = _TimeParameters.x * 3.2;
				float3 worldToObj907_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_872_0_g1 = ( mulTime906_g1 + ( 0.02 * worldToObj907_g1.x ) + ( worldToObj907_g1.y * 0.14 ) + ( worldToObj907_g1.z * 0.16 ) + float3(0.4,0.3,0.1) );
				float3 normalizeResult632_g1 = normalize( v.positionOS.xyz );
				float CenterOfMassTrunkUP636_g1 = saturate( (distance( normalizeResult632_g1 , float3(0,1,0) )*1.0 + -0.05) );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float mulTime905_g1 = _TimeParameters.x * 2.3;
				float3 worldToObj908_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float3 temp_output_866_0_g1 = ( mulTime905_g1 + ( 0.2 * worldToObj908_g1 ) + float3(0.4,0.3,0.1) );
				float mulTime904_g1 = _TimeParameters.x * 3.6;
				float3 temp_cast_4 = (v.positionOS.xyz.x).xxx;
				float3 worldToObj910_g1 = mul( GetWorldToObjectMatrix(), float4( temp_cast_4, 1 ) ).xyz;
				float temp_output_898_0_g1 = ( mulTime904_g1 + ( 0.2 * worldToObj910_g1.x ) );
				float3 normalizeResult697_g1 = normalize( v.positionOS.xyz );
				float CenterOfMass701_g1 = saturate( (distance( normalizeResult697_g1 , float3(0,1,0) )*2.0 + 0.0) );
				float SphericalMaskProxySphere704_g1 = (( _CenterofMass )?( ( temp_output_628_0_g1 * CenterOfMass701_g1 ) ):( temp_output_628_0_g1 ));
				float3 worldToObj1131_g1 = mul( GetWorldToObjectMatrix(), float4( v.positionOS.xyz, 1 ) ).xyz;
				float mulTime1138_g1 = _TimeParameters.x * 4.0;
				float mulTime1129_g1 = _TimeParameters.x * 0.2;
				float2 appendResult1126_g1 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 normalizeResult1128_g1 = normalize( appendResult1126_g1 );
				float simpleNoise1139_g1 = SimpleNoise( ( mulTime1129_g1 + normalizeResult1128_g1 )*1.0 );
				float WindMask_SimpleSway1145_g1 = ( simpleNoise1139_g1 * 1.5 );
				float3 rotatedValue1151_g1 = RotateAroundAxis( float3( 0,0,0 ), v.positionOS.xyz, normalize( float3(0.6,1,0.1) ), ( ( cos( ( ( worldToObj1131_g1 * 0.02 ) + mulTime1138_g1 + ( float3(0.6,1,0.8) * 0.3 * worldToObj1131_g1 ) ) ) * 0.1 ) * WindMask_SimpleSway1145_g1 * saturate( ase_objectScale ) ).x );
				float3 temp_cast_6 = (0.0).xxx;
				#if defined(_WINDTYPE_GENTLEBREEZE)
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#elif defined(_WINDTYPE_WINDOFF)
				float3 staticSwitch1044_g1 = temp_cast_6;
				#else
				float3 staticSwitch1044_g1 = ( ( ( ( ( ( ( ( ( appendResult939_g1 + ( appendResult938_g1 * cos( mulTime975_g1 ) ) + ( cross( float3(1.2,0.6,1) , ( appendResult938_g1 * float3(0.7,1,0.8) ) ) * sin( mulTime975_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.08 ) + ( ( ( appendResult931_g1 + ( appendResult967_g1 * cos( mulTime976_g1 ) ) + ( cross( float3(0.9,1,1.2) , ( appendResult967_g1 * float3(1,1,1) ) ) * sin( mulTime976_g1 ) ) ) * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * temp_output_996_0_g1 ) * 0.1 ) + ( ( ( appendResult981_g1 + ( appendResult966_g1 * cos( mulTime977_g1 ) ) + ( cross( float3(1.1,1.3,0.8) , ( appendResult966_g1 * float3(1.4,0.8,1.1) ) ) * sin( mulTime977_g1 ) ) ) * SphearicalMaskCM763_g1 * temp_output_996_0_g1 ) * 0.05 ) ) * _BranchWindLarge ) * WindMask_LargeB770_g1 ) * CenterOfMassTrunkUP_C1098_g1 ) + ( ( ( WindMask_LargeC779_g1 * ( ( ( ( cos( temp_output_872_0_g1 ) * sin( temp_output_872_0_g1 ) * CenterOfMassTrunkUP636_g1 * SphearicalMaskCM763_g1 * CenterOfMassThicknessMask854_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( cos( temp_output_866_0_g1 ) * sin( temp_output_866_0_g1 ) * CenterOfMassTrunkUP636_g1 * CenterOfMassThicknessMask854_g1 * SphearicalMaskCM763_g1 * saturate( ase_objectScale ) ) * 0.2 ) + ( ( sin( temp_output_898_0_g1 ) * cos( temp_output_898_0_g1 ) * SphericalMaskProxySphere704_g1 * CenterOfMassThicknessMask854_g1 * CenterOfMassTrunkUP636_g1 ) * 0.2 ) ) * _BranchWindSmall ) ) * 0.3 ) * CenterOfMassTrunkUP_C1098_g1 ) + (( _PivotSway )?( ( ( rotatedValue1151_g1 - v.positionOS.xyz ) * 0.4 ) ):( float3( 0,0,0 ) )) ) * saturate( v.positionOS.xyz.y ) );
				#endif
				float3 FinalWind_Output1060_g1 = ( _GlobalWindStrength * staticSwitch1044_g1 );
				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1060_g1;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				v.normalOS = v.normalOS;

				float3 positionWS = TransformObjectToWorld( v.positionOS.xyz );
				o.positionCS = TransformWorldToHClip(positionWS);

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.positionOS;
				o.normalOS = v.normalOS;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].vertex.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				

				surfaceDescription.Alpha = 1;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
						clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;

				#ifdef SCENESELECTIONPASS
					outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				#elif defined(SCENEPICKINGPASS)
					outColor = _SelectionID;
				#endif

				return outColor;
			}

			ENDHLSL
		}
		
	}
	
	CustomEditor "UnityEditor.ShaderGraphLitGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.FresnelNode;5;-769.7689,-336.0271;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;-0.1;False;2;FLOAT;5;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;10;-697.8334,-522.2871;Inherit;False;Constant;_Color2;Color 2;0;0;Create;True;0;0;0;False;0;False;0.2156863,0.5607843,0.2,1;0.2196077,0.5529411,0.1999998,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-434.1929,-403.9306;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;4;-748.3151,281.4371;Inherit;True;Property;_Mask;Mask;2;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;97cbfaa1a982c434d9829a9ab41c5b0d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-770.438,-110.2618;Inherit;True;Property;_Albedo;Albedo;0;2;[Header];[NoScaleOffset];Create;True;3;__________(TTFE) TREE GIZMO SHADER___________;_____________________________________________________;Texture Maps;0;0;False;0;False;-1;None;4465c0aae8371694d8400e4dc45b23e3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;7;-294.5463,-290.6969;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;18;-466.0032,206.8213;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;15;-445.4461,284.9381;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;12;-429.237,-109.4421;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0.4;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;3;-765.7982,90.28165;Inherit;True;Property;_Normal;Normal;1;2;[NoScaleOffset];[Normal];Create;True;0;0;0;False;0;False;3;None;4199ccd0e0911f74f9589bfd1dc792a4;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-146.635,-179.5742;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-267.7063,175.2692;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;31;-112.2989,322.6999;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;32;-122.6989,284.9999;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;1;-319.9271,429.1129;Inherit;False;(TTFE) Tree Bark_Wind System;3;;1;58360699feb112c40b86ba9ba75062e6;0;0;1;FLOAT3;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;21;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;22;0,0;Float;False;True;-1;2;UnityEditor.ShaderGraphLitGUI;0;12;Toby Fredson/The Toby Foliage Engine/Utility/(TTFE) Global Controller;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;21;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForward;False;False;0;;0;0;Standard;39;Workflow;1;0;Surface;0;0;  Refraction Model;0;0;  Blend;0;0;Two Sided;1;0;Fragment Normal Space,InvertActionOnDeselection;0;0;Forward Only;0;0;Transmission;0;0;  Transmission Shadow;0.5,False,;0;Translucency;0;0;  Translucency Strength;1,False,;0;  Normal Distortion;0.5,False,;0;  Scattering;2,False,;0;  Direct;0.9,False,;0;  Ambient;0.1,False,;0;  Shadow;0.5,False,;0;Cast Shadows;1;0;  Use Shadow Threshold;0;0;GPU Instancing;0;638460934587043395;LOD CrossFade;0;638460934575536284;Built-in Fog;1;0;_FinalColorxAlpha;0;0;Meta Pass;1;0;Override Baked GI;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Write Depth;0;0;  Early Z;0;0;Vertex Position,InvertActionOnDeselection;1;0;Debug Display;0;0;Clear Coat;0;0;0;10;False;True;True;True;True;True;True;True;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;23;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;24;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;25;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;26;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;27;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthNormals;0;6;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormals;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;28;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;GBuffer;0;7;GBuffer;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalGBuffer;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;29;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;SceneSelectionPass;0;8;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;30;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ScenePickingPass;0;9;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
WireConnection;8;0;10;0
WireConnection;8;1;5;0
WireConnection;7;0;8;0
WireConnection;18;0;2;0
WireConnection;15;0;4;2
WireConnection;12;0;2;0
WireConnection;6;0;7;0
WireConnection;6;1;12;0
WireConnection;14;0;18;0
WireConnection;14;1;15;0
WireConnection;31;0;4;4
WireConnection;32;0;4;2
WireConnection;22;0;14;0
WireConnection;22;1;3;0
WireConnection;22;2;6;0
WireConnection;22;4;31;0
WireConnection;22;5;32;0
WireConnection;22;8;1;0
ASEEND*/
//CHKSM=8DEB22B2B7888B982A7F6F1011EA2F9EDB60546C