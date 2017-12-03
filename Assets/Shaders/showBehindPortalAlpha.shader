Shader "Custom/ShowBehindPortalAlpha" {
	Properties {
        _Color ("Main Color", Color) = (1,1,1,0)
    }
    SubShader {
        Tags {"RenderType"="Opaque" "Queue"="Geometry-10"}
        ZWrite Off

        
        Stencil {
            Ref 1
            Comp always
            Pass replace
        }
       

        CGPROGRAM
        #pragma surface surf Lambert alpha:f

        float4 _Color;
        struct Input {
            float4 color : COLOR;
        };
        void surf (Input IN, inout SurfaceOutput o) {
            o.Albedo = _Color.rgb;
            o.Normal = half3(0,0,-1);
            o.Alpha = _Color.a;
        }
        ENDCG
    }
   
}