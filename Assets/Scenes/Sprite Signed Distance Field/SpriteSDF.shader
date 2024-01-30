Shader "Sprites/SDF Sprite"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        
        _Sharpness("Sharpness", Range(0.01, 1)) = 1.0
		
		_FillColor ("Fill Color", Color) = (0,0,0,1)
		_OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth("Outline Width", Range(0, 1)) = 0
        _OutlineSoftness("Outline Softness", Range(0, 1)) = 0
        
        _UpdateInterval("Update Interval", Range(0, 1)) = 0
        _NoiseFrequency("Noise Frequency", Range(0, 20)) = 1
        _SDFNoiseStrength("SDF Noise Strength", Range(0, 1)) = 0.2
        _UVDistortionNoiseStrength("UV Distort Noise Strength", Range(0, 1)) = 0.2
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
			#pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA

			#ifndef UNITY_SPRITES_INCLUDED
			#define UNITY_SPRITES_INCLUDED

			#include "UnityCG.cginc"
            #include "noiseSimplex.cginc"
            
			#ifdef UNITY_INSTANCING_ENABLED

				UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
					// SpriteRenderer.Color while Non-Batched/Instanced.
					UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
					// this could be smaller but that's how bit each entry is regardless of type
					UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
				UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

				#define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
				#define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)

			#endif // instancing

			CBUFFER_START(UnityPerDrawSprite)
			#ifndef UNITY_INSTANCING_ENABLED
				fixed4 _RendererColor;
				fixed2 _Flip;
			#endif
				float _EnableExternalAlpha;
			CBUFFER_END

			// Material Color.
			fixed4 _Color;
			fixed4 _FillColor;
            float _Sharpness;
			fixed4 _OutlineColor;
            float _OutlineWidth;
            float _OutlineSoftness;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				float3 worldPos   : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
			{
				return float4(pos.xy * flip, pos.z, 1.0);
			}

			v2f SpriteVert(appdata_t IN)
			{
				v2f OUT;

				UNITY_SETUP_INSTANCE_ID (IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
				OUT.vertex = UnityObjectToClipPos(OUT.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color * _RendererColor;

				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif
				OUT.worldPos = mul(unity_ObjectToWorld, float4(IN.vertex.xyz, 1.0)).xyz;


				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _UpdateInterval;
			float _NoiseFrequency;
			float _SDFNoiseStrength;
			float _UVDistortionNoiseStrength;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

			#if ETC1_EXTERNAL_ALPHA
				fixed4 alpha = tex2D (_AlphaTex, uv);
				color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
			#endif

				return color;
			}

			fixed4 SpriteFrag(v2f IN) : SV_Target
			{
			    float steppedTime = floor(_Time.y / _UpdateInterval) * _UpdateInterval;
			          
			    float worldSpaceNoiseSample = snoise(float3(IN.worldPos.x*_NoiseFrequency, IN.worldPos.y*_NoiseFrequency, steppedTime));
                //return float4(worldSpaceNoiseSample, worldSpaceNoiseSample, worldSpaceNoiseSample, 1.0f);
                
                float2 uv = IN.texcoord + worldSpaceNoiseSample * _UVDistortionNoiseStrength;
                
				fixed4 spriteColor = SampleSpriteTexture (uv);
				float sdf = spriteColor.r - 0.5;
				
				// Add noise to signed distance field
				sdf += worldSpaceNoiseSample * _SDFNoiseStrength;
				

                /*
				float delta = fwidth(sdf);
                // Calculate the different masks based on the SDF
                float graphicAlpha = 1 - smoothstep(-delta, 0, sdf);
                float outlineAlpha = (1 - smoothstep(_OutlineWidth - delta, _OutlineWidth, sdf));
                outlineAlpha *= _OutlineColor.a;
                graphicAlpha *= _FillColor.a;
                return graphicAlpha;
                float4 effects = lerp(float4(_OutlineColor.rgb, outlineAlpha), float4(_FillColor.rgb, graphicAlpha), graphicAlpha);
                return effects; 
				*/
				
				// This stuff can probably be simplified massively. I nicked it from some old Pendragon code; looks like I tried to do some fancy stuff to make the lines sharp-but-not-jaggy (ddx/ddy are the derivatives of the SDF)
				fixed4 faceColor = _FillColor;
                float2 gradient = float2(ddx(sdf), ddy(sdf));
                float speed = length(gradient);
                float distance = sdf / speed;
                float coverage = saturate(distance * _Sharpness + 0.5f);
				float m = coverage * coverage * (3.0 - 2.0f * coverage);
				faceColor.a *= m;
                faceColor.rgb *= faceColor.a;
				
				fixed4 outlineColor = _OutlineColor;
				float outlineAlpha = smoothstep(-_OutlineWidth*0.5+0.01,-_OutlineWidth*0.5+_OutlineSoftness*_OutlineWidth*0.5+0.01, sdf) * _OutlineColor.a;
				
				float l = saturate(outlineAlpha * 1-m);
				fixed4 color = lerp(faceColor, outlineColor, l);
				
				color *= IN.color;
                color.rgb *= IN.color.a;

				return color;
			}

			#endif
            ENDCG
        }
    }
}