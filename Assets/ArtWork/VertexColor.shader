﻿Shader "Vertex Color" {
	Properties {
		_MainTex ("Base (RBG)" , 2D) = "white" {}
	}
	SubShader {
		Pass {
			Lighting On
			ColorMaterial AmbientAndDiffuse
			SetTexture [_MainTex] {
				combine texture * primary DOUBLE
			}
		}
	}
}