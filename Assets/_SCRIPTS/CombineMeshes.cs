using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// Author: Corwin Belser
/// Combines the meshes of all instances for each gameObject provided
public class CombineMeshes : MonoBehaviour {

    public GameObject[] MESHES_TO_COMBINE;

	// Use this for initialization
	void Start () {
        /* I know...I know...This is not a good function call... */
        /* Get a list of all game objects in the scene */
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject objToMeshCombine in MESHES_TO_COMBINE)
        {
            /* Pass all gameObjects with a matching name to CombineMesh() */
            CombineMesh(allObjects.Where(o => o.name == objToMeshCombine.name).ToArray());
        }
	}
	
    private void CombineMesh( GameObject[] allGameObjectsWithMesh )
    {
        Debug.Log("Combining " + allGameObjectsWithMesh.Length + " meshes of " + allGameObjectsWithMesh[0].name);

        /* Get a reference to the material used by these gameObjects */
        Material material = allGameObjectsWithMesh[0].GetComponentInChildren<MeshRenderer>().material;

        /* Get the MeshFilter component of each of the gameObjects */
        MeshFilter[] meshFilters = new MeshFilter[allGameObjectsWithMesh.Length];
        for (int i = 0; i < allGameObjectsWithMesh.Length; i++)
        {
            //Debug.Log(allGameObjectsWithMesh[i].GetComponentInChildren<MeshFilter>());
            meshFilters[i] = allGameObjectsWithMesh[i].GetComponentInChildren<MeshFilter>();
        }

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        /* Turn off each gameObject after pulling out its mesh data */
        for (int i = 0; i < meshFilters.Length; i++ )
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        /* Combine all the meshes to a single mesh, assigning it to the an Empty GameObject */
        GameObject master = new GameObject(allGameObjectsWithMesh[0].name + "_MASTER_MESH");
        master.AddComponent<MeshFilter>();
        master.AddComponent<MeshRenderer>();
        master.GetComponent<MeshFilter>().mesh = new Mesh();
        master.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        master.GetComponent<MeshRenderer>().material = material;
        master.SetActive(true);
    }
}
