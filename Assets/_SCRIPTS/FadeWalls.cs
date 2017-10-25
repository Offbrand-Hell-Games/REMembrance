using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeWalls : MonoBehaviour
{
    public Material Wall_Material;
    Material mat;
    public BoxCollider WallColider;

    //float alpha = 0.0f;
    void Start()
    {
        mat = GetComponent<Renderer>().material;
		WallColider = GetComponent<BoxCollider>();
    }
    void Fade()
    {
        StartCoroutine(FadeAway());
    }
    void Disable_Collision()
    {
        StartCoroutine(DisableWalls());
    }

    public IEnumerator DisableWalls()
    {
        WallColider.enabled = false;
        yield return new WaitForSeconds(2.0f);
        WallColider.enabled = true;
        yield return null;
    }
    public IEnumerator FadeAway()
    {

        mat.CopyPropertiesFromMaterial(Wall_Material);
        mat.SetFloat("_Mode", 3f);
        //mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");

        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
        yield return new WaitForSeconds(2.0f);
        
        mat.CopyPropertiesFromMaterial(Wall_Material);
        yield return null;
    }
}
