Shader "Custom/ColorMask"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _ColorMaskTex("ColorMask (A)", 2D) = "white" {}
       
        _Normal("Normal", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
       
       
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

      
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
       // LOD 200

        /*ZWrite Off*/
                 Blend SrcAlpha OneMinusSrcAlpha
                 ColorMask RGB
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _ColorMaskTex;
        sampler2D _Normal;
       

        struct vertexInput
        {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
        };
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_ColorMaskTex;
            float2 uv_Normal;
        };


        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

/* 
        fragmentInput vert(vertexInput i)
        {
            vertexInput o;
            
            o.uv_MainTex = i.texcoord;
            o.uv_ColorMaskTex = o.uv_MainTex + _ColorMaskTex.zw;

            return o;
        }
        */
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 mask = tex2D(_ColorMaskTex, IN.uv_ColorMaskTex);
            
            float isMask = (mask.a) > 0;
          
            fixed4 color = isMask ? _Color : (1,1,1,1);

            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Alpha = c.a;
            o.Albedo = c.rgb * color;
            o.Normal = tex2D(_Normal, IN.uv_Normal);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
           
        }
        ENDCG
    }
            FallBack "Standard"
}
