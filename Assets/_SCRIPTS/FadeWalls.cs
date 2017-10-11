using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeWalls : MonoBehaviour
{
    public Material Wall_Material;
    Material mat;

    float alpha = 0.0f;
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }
    void Fade()
    {

        StartCoroutine(FadeAway());
    }
    public IEnumerator FadeAway()
    {
        print("MADE IT HERE");
        mat.CopyPropertiesFromMaterial(Wall_Material);
        mat.SetFloat("_Mode", 3f);
        //mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");

        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
        yield return new WaitForSeconds(3.0f);
        print("Made it here");
        mat.CopyPropertiesFromMaterial(Wall_Material);
        yield return null;
    }
}
