Shader "Custom/ShowBehindTarget" {
    SubShader {
        Tags {"Queue"="Geometry"}
        Pass {
            Stencil {
                Ref 1
                Comp equal
            }
        }
//        Pass {
//            Stencil {
//                Ref 1
//                Comp NotEqual
//                Fail Keep
//            }
//
//                ZWrite on
//        }
    } 
}