Shader "Unlit/RotationShader"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.BlendMode)]
        _ScrFactor("Scr Factor", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)]
        _DstFactor("Dst Factor", Float) = 10
        [Enum(UnityEngine.Rendering.BlendOp)]
        _Opp("Operation", Float) = 0

        _MainTex ("Texture One", 2D) = "white" {}
        _Rotation("Rotation", float) = 0

        _SecondTex ("Texture Two", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend [_ScrFactor] [_DstFactor]
        BlendOp [_Opp]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _SecondTex;
            float4 _MainTex_ST;
            float4 _SecondTex_ST;
            float _Rotation;

            float2 RotateMat(float2 uv, float t)
            {
                float2 newUV;

                newUV = uv * 2 - 1; //przesuniêcie wierzcho³ka (0,0) do œrodka wyœwietlanej powierzchni

                float c = cos(_Rotation + t);// liczymy cos
                float s = sin(_Rotation + t);// liczymy sin

                float2x2 mat = float2x2(c,-s,
                                        s,c); //maciez obrotu

                newUV = mul(mat, newUV); // wyswietlanie o.u
                newUV = newUV * 0.5 + 0.5; // powrot wierzcho³ka (inaczej nadana tekstura jest przesunieta)

                return newUV;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv.xy, _MainTex);
                o.uv.xy = RotateMat(o.uv.xy, _Time.y); // o.uv.xy zmien na v.uv.xy
                o.uv.zw = RotateMat(v.uv.xy, -_Time.y);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv.xy);
                fixed4 col2 = tex2D(_SecondTex, i.uv.zw);
                //return fixed4(i.uv, 0, 1);

                return col * 0.5 + col2 * 0.5;
            }
            ENDCG
        }
    }
}
