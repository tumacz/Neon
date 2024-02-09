Shader "Unlit/BarShader"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.BlendMode)]
        _SrcFactor("Src Factor", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]
        _DstFactor("Dst Factor", Float) = 1
        [Enum(UnityEngine.Rendering.BlendOp)]
        _Opp("Operation", Float) = 0

        _BarIntesity("Bar Intensity", Float) = 1
        _AnimationIntesity("Animation Intensity", Float) = 1
        _Rotator("Rotator", Range(0,1)) = 1
        _ColorOne("Color One", color) = (1,1,1,1)
        _ColorTwo("Color Two", color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend [_SrcFactor] [_DstFactor]
        BlendOp [_Opp]

        Pass
        {
            Cull Back
            //Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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

            float _BarIntesity;
            float _AnimationIntesity;
            float _Rotator;
            float4 _ColorOne, _ColorTwo;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float rotate = lerp(i.uv.x, i.uv.y, _Rotator);
                float anim = _AnimationIntesity * _Time.y;
                float barEffect = sin(rotate * _BarIntesity + anim) * 0.5 + 0.5;
            
                float4 mixedColor = lerp(_ColorOne, _ColorTwo, rotate) * barEffect;

                return fixed4(mixedColor.rgb, barEffect * mixedColor.a);
            }

            ENDCG
        }

        Pass
        {
            Cull Front
            //Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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

            float _BarIntesity;
            float _AnimationIntesity;
            float _Rotator;
            float4 _ColorOne, _ColorTwo;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float rotate = lerp(i.uv.x, i.uv.y, _Rotator);
                float anim =_AnimationIntesity * _Time.y;
                float barEffect = sin(rotate * _BarIntesity + anim) * 0.5 + 0.5;
            
                float4 mixedColor = lerp(_ColorOne, _ColorTwo, rotate) * barEffect;

                return fixed4(mixedColor.rgb, barEffect * mixedColor.a);
            }

            ENDCG
    }
}
}
