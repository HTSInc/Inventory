Shader "Custom/LabelShader" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _QRCodeTex("QR Code Texture", 2D) = "white" {}
        _TextTexture1("Text Texture 1", 2D) = "white" {}
        _TextTexture2("Text Texture 2", 2D) = "white" {}
        _TextTexture3("Text Texture 3", 2D) = "white" {}
        _CompanyText("Company Text", 2D) = "white" {}
        _ProductText("Product Text", 2D) = "white" {}
        _SubheadingText("Subheading Text", 2D) = "white" {}
        _LabelNameText("Label Name Text", 2D) = "white" {}
        _PkNumText("Pk Number Text", 2D) = "white" {}
        _UPCABarcode("UPC-A Barcode", 2D) = "white" {}
        _LabelDimensions("Label Dimensions", Vector) = (1200, 1800, 0, 0)
        _TextureSize("Texture Size", Vector) = (1200, 1800, 0, 0)
    }
        SubShader{
         Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
         LOD 100
         Blend SrcAlpha OneMinusSrcAlpha

         CGPROGRAM
         #pragma surface surf Lambert

         sampler2D _MainTex;
         sampler2D _QRCodeTex;
         sampler2D _TextTexture1;
         sampler2D _TextTexture2;
         sampler2D _TextTexture3;
         sampler2D _CompanyText;
         sampler2D _ProductText;
         sampler2D _SubheadingText;
         sampler2D _LabelNameText;
         sampler2D _PkNumText;
         sampler2D _UPCABarcode;

         float4 _LabelDimensions;
         float4 _TextureSize;

         struct Input {
             float2 uv_MainTex;
         };

         // Function to calculate positions and scales
         float2 CalculatePositionAndScale(float2 uv, float2 position, float2 scale) {
             uv -= position;
             uv /= scale;
             return uv;
         }
         fixed4 ProcessTexture(fixed4 color) {
             color.rgb = 1.0 - (1.0 - color.rgb) * (1.0 - color.a);
             return color;
         }

         // Function to render a texture based on the given position and scale
         fixed4 RenderTexture(sampler2D tex, float2 uv, float2 position, float2 scale, fixed4 currentColor) {
             fixed4 sampledTex = tex2D(tex, CalculatePositionAndScale(uv, position, scale));
             sampledTex.rgb = 1.0 - (1.0 - sampledTex.rgb) * (1.0 - currentColor.a);
             return sampledTex;
         }
        
        void surf(Input IN, inout SurfaceOutput o) {
            fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 finalColor = col;
            fixed2 uv = IN.uv_MainTex * _TextureSize.xy;
            float bottomHeight = 65.0;
            float halfWidth = _LabelDimensions.x * 0.5;
        
            // Define positions and scales for each text and texture
            float2 positionsAndScales[12] = {
              float2(0, 0), float2(halfWidth, bottomHeight),
              float2(halfWidth, 0), float2(halfWidth, bottomHeight * 0.5),
              float2(halfWidth, bottomHeight * 0.5), float2(halfWidth, bottomHeight * 0.5),
              float2(0, bottomHeight), float2(_LabelDimensions.x, _LabelDimensions.y * 0.3611 - bottomHeight),
              float2(0, _LabelDimensions.y * 0.8611), float2(_LabelDimensions.x, _LabelDimensions.y * 0.1389),
              float2(0, _LabelDimensions.y * 0.5 - 50), float2(300, 100)
            };

            finalColor *= ProcessTexture(RenderTexture(_QRCodeTex, uv, positionsAndScales[0], positionsAndScales[1], col));
            finalColor *= ProcessTexture(RenderTexture(_TextTexture1, uv, positionsAndScales[2], positionsAndScales[3], col));
            finalColor *= ProcessTexture(RenderTexture(_TextTexture2, uv, positionsAndScales[4], positionsAndScales[5], col));
            finalColor *= ProcessTexture(RenderTexture(_TextTexture3, uv, positionsAndScales[6], positionsAndScales[7], col));
            finalColor *= ProcessTexture(RenderTexture(_CompanyText, uv, positionsAndScales[8], positionsAndScales[9], col));
            finalColor *= ProcessTexture(RenderTexture(_UPCABarcode, uv, positionsAndScales[10], positionsAndScales[11], col));
            finalColor *= ProcessTexture(RenderTexture(_ProductText, uv, float2(0, _LabelDimensions.y * 0.7222), float2(halfWidth, _LabelDimensions.y * 0.1389), col));
            finalColor *= ProcessTexture(RenderTexture(_SubheadingText, uv, float2(halfWidth, _LabelDimensions.y * 0.7222), float2(halfWidth, _LabelDimensions.y * 0.0694), col));
            finalColor *= ProcessTexture(RenderTexture(_LabelNameText, uv, float2(halfWidth, _LabelDimensions.y * 0.7917), float2(halfWidth, _LabelDimensions.y * 0.0694), col));
            finalColor *= ProcessTexture(RenderTexture(_PkNumText, uv, float2(halfWidth, _LabelDimensions.y * 0.8611), float2(halfWidth, _LabelDimensions.y * 0.1389), col));

            o.Albedo = finalColor.rgb;
            o.Alpha = finalColor.a;
        }
        ENDCG
    }
          FallBack "Diffuse"
}