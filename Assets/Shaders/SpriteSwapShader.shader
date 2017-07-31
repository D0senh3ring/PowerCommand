Shader "Dosenhering/Unlit/SpriteSwapShader"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _SwapTex("Swap Texture", 2D) = "white" {}
        _SwapVal("Swap Value", Range(0.0, 1.0)) = 0.0
    }
    SubShader
    {
        Tags 
        {
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent"
        }
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha 
    
        Pass
        {            
            CGPROGRAM
            
            //pragmas
            #pragma vertex vert
            #pragma fragment frag
            
            //includes
            #include "UnityCG.cginc"
            
            //variables
            uniform sampler2D _MainTex;
            uniform sampler2D _SwapTex;
            uniform float _SwapVal;
            
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
                
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                return o;
            }
            
            fixed4 frag (fragmentInput i) : SV_Target
            {
                if(_SwapVal > 0)
                    return tex2D(_SwapTex, i.uv);
                else
                    return tex2D(_MainTex, i.uv);
            }
            
            ENDCG
        }
    }
}
