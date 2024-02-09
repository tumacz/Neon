Shader "Unlit/TestShader"
{
    Properties
    {
        _Color("Test Color", color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert // dzia³a na ka¿dym wierzcho³ku
            #pragma fragment frag // dzia³a na ka¿dym pikselu
         
            #include "UnityCG.cginc" // biblioteki Unity

            struct appdata // dane objektu lub siatka
            {
                float4 vertex : POSITION;
            };

            struct v2f // podzia³ na fragmenty   (vertex to frament)
            {
                float4 vertex : SV_POSITION; // najprecyzyjniejsze wartoœci
            };

            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // MVP pobieranie danych z wierzcho³ków obiektu i wyswietlanie ich do kamery
                return o;
            }

            fixed4 frag (v2f i) : SV_Target //
            {
                fixed4 col = _Color; //niskoprecyzyjne wartosci, do kolorów
                return col;
            }
            ENDCG
        }
    }
}
