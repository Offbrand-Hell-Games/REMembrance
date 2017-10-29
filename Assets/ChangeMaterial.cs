using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour {

    public Material Invisible, Highlight;
    Renderer thisRend;

	// Use this for initialization
	void Start () {
        thisRend = this.gameObject.GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ChangeMat()
    {
        StartCoroutine(ChangeMaterials());
    }

    public IEnumerator ChangeMaterials()
    {
        thisRend.material = Highlight;
        yield return new WaitForSeconds(3.0f);
        thisRend.material = Invisible;
        yield return null;
    }
}
