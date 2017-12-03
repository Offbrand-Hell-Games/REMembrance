// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/RenderOnZTestFAIL"
{
 Properties
 {
 _Color("Tint",Color) = (1,1,1,1)
 }

 Subshader
 {
   Pass
   {
    Stencil
    {
        Ref 1
        Comp Equal
    } 

   CGPROGRAM

   #pragma vertex vert
   #pragma fragment frag


   struct input
   {

   float4 pos : POSITION;

   };

   struct v2f
   {

   float4 pos : SV_POSITION;
   };

   float4 _Color;

   v2f vert(input i)
   {
   v2f o;

   o.pos = UnityObjectToClipPos(i.pos); //unity_matrix_mvp goes first. these are matrices, orders in multiplications matters

   return o;

   }

   float4 frag(v2f i) : COLOR
   {
   return _Color;
   }

   ENDCG

   }
 }
 }