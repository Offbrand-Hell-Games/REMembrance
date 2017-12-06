Shader "Custom/MapClear"
{
	SubShader {
	 ColorMask 0
	 Pass {
	  	Stencil {
	      Ref 0
	      Comp always
	      Pass replace
	 	}
	 }
    }
}