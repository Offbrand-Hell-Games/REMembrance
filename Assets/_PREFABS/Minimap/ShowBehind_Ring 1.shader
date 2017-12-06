Shader "Custom/MapRevealerPlane" {
    SubShader {
        Tags {"Queue"="Geometry-10"}
        ZWrite Off
        ColorMask 0

        Pass {
            Stencil {
                Ref 1
                Comp always
                Pass replace
            }
        }
    } 
}