Shader "Unlit/TextureSampleShader"
{
    Properties
    {
        _MainTexture("Main Texture", 2D) = "white" {}
        _AnimateXY("Animate XY", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        //Blend One One
        Pass
        {
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

            sampler2D _MainTexture;
            float4 _MainTexture_ST;
            float4 _AnimateXY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTexture);
                // o.uv *= 2; //to samo tyle ze wydajniej(wszystkie obliczenia dodawaæ do v2f, liczenie po wierzcholkach jest wydajniejsze od liczenia kazdego pixela)
                // o.uv.y += 0.5;
                o.uv += frac(_AnimateXY.xy * _MainTexture_ST.xy * _Time.yy); //frac resetuje czas, bez niego luczy sie czas od rozpoczecia dzialania srodowiska unity
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uvs = i.uv;
                // uvs *= 2; //tiling
                // uvs.x += 0.5; //offset
                //return fixed4(uvs,0,1);

                fixed4 textureColor = tex2D(_MainTexture, uvs);
                //fixed4 col = fixed4(i.uv,0,1);
                return textureColor;
            }
            ENDCG
        }
    }
}
