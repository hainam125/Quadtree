Shader "Custom/VoxelTerrain"
{
    Properties
    {
        _Layer1 ("Albedo (R)", 2D) = "white" {}
        _Layer2 ("Albedo (G)", 2D) = "white" {}
        _Layer3 ("Albedo (B)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _Layer1;
        sampler2D _Layer2;
        sampler2D _Layer3;

        struct Input
        {
            float2 uv_Layer1;
            float4 color : COLOR;
        };

        half _Glossiness;
        half _Metallic;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 l1 = tex2D (_Layer1, IN.uv_Layer1);
            fixed4 l2 = tex2D (_Layer2, IN.uv_Layer1);
            fixed4 l3 = tex2D (_Layer3, IN.uv_Layer1);

            fixed3 smoothingValue = normalize(IN.color.rgb);
            fixed3 color = l1 * smoothingValue.r + l2 * smoothingValue.b + l3 * smoothingValue.g;

            o.Albedo = color;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
