Shader "Custom/AlphaOnZtestFail" {
	Properties {
		 _Color("Tint",Color) = (1,1,1,1)
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		
		Pass {
			Stencil
   			{
   			    Ref 1
   			    Comp Equal
   			}

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
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
				
				float4 frag(v2f IN) : COLOR {
					return _Color;
				}
			ENDCG
		}
	}
}