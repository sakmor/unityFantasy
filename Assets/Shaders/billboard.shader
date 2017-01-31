// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Cg  shader for billboards" {
   Properties {
    [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    [MaterialToggle] _isToggled("isToggle", Float) = 0
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)		
    _Xoffset ("X_Offset",Float)=0
    _Xscale ("X_scale",Float)=1


   }
   SubShader {
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
    
      Pass {   
         ZWrite Off // don't write to depth buffer 
         Lighting Off
         Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
         #pragma target 2.0
         #pragma multi_compile _ PIXELSNAP_ON
         #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
         #include "UnityCG.cginc"

         // User-specified uniforms            
         uniform sampler2D _MainTex;     
         uniform float _Xoffset;
         uniform float _Xscale;
         uniform float _isToggled;

         struct vertexInput {
            float4 vertex : POSITION;
            float4 tex : TEXCOORD0;
            fixed4 color    : COLOR;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
            fixed4 color    : COLOR;
         };
         
         fixed4 _Color;
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
            
          //distance falloff
            float3 viewDirW = _WorldSpaceCameraPos - mul((float4x4)unity_ObjectToWorld, input.vertex);
            float viewDist = length(viewDirW);
          
   
            if(_isToggled==1){
              output.pos = mul(UNITY_MATRIX_P,  
              mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
            - float4(input.vertex.x, input.vertex.y, 0.0, 0.0) * float4(_Xscale, 1.0, 1.0, 1.0));
            output.pos.x+=_Xoffset+(_Xscale/33); //sakmor:我也不知道為什麼是34
            
            }else{
              output.pos = mul(UNITY_MATRIX_P,  
              mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
            - float4(input.vertex.x, input.vertex.y, 0.0, 0.0) * float4(1.0, 1.0, 1.0, 1.0));
 
            }

 
            output.tex = input.tex;
       
            output.color = input.color * _Color;
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            fixed4 c = tex2D(_MainTex, float2(input.tex.xy)) * input.color;
            return c;
         }
 
         ENDCG
      }
   }
}