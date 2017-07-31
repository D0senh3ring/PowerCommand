Shader "Dosenhering/Unlit/Shield Distortion Shader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _DistMap("Distortion Map", 2D) = "white" {}
        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Amount ("Distortion Amount", Range(0.001, 0.5)) = 1
    }
    SubShader
    {
        Pass
        {
            Tags 
            { 
                "Queue" = "Transparent" 
                "RenderType" = "Transparent" 
            }
            
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            
            //pragmas
            #pragma vertex vert
            #pragma fragment frag
            
            //includes
            #include "UnityCG.cginc"
            
            //variables
            uniform sampler2D _MainTex;
            uniform sampler2D _DistMap;
            uniform float4 _Color;
            uniform float _Amount;
            
            //structs
            struct vertexInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct fragmentInput
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            //functions
            
            //frag and vert
            fragmentInput vert (vertexInput v)
            {
                fragmentInput o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (fragmentInput i) : SV_Target
            {                
                float2 displacement = tex2D(_DistMap, float2(i.uv.x + _Time.x * 2, i.uv.y + _Time.x * 2)).xy;
                displacement = ((displacement * 2) - 1) * _Amount;
                 
                fixed4 color = tex2D(_MainTex, i.uv + displacement);
                
                fixed occlusion = tex2D(_DistMap, i.uv).z;
                
                if(color.a == 0 || occlusion == 0)
                    discard;
                
                return color;
            }
            
            ENDCG
        }
    }
}
