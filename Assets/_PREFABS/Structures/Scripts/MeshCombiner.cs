using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// Author: Corwin Belser
/// Combines the meshes of all instances for each gameObject provided
public class MeshCombiner : MonoBehaviour {

    public GameObject[] ROOMS;
    public MeshFilter[] MESHES_TO_IGNORE;

	// Use this for initialization
	void Start () {
        foreach (GameObject room in ROOMS)
        {
            LocateAndCombineMeshes(room);
        }
	}

    private void LocateAndCombineMeshes(GameObject room)
    {
        /* Find all meshes in a single room */
        MeshFilter[] meshFilters = room.GetComponentsInChildren<MeshFilter>(false);
        if (meshFilters.Length == 0)
        {
            Debug.Log(room.name + " Had no MeshFilters to combine!");
        }
        Debug.Log("Combining meshes for " + room.name);

        /* Group them by name, creating groups of same meshes */
        IEnumerable<IGrouping<string, GameObject>> meshGroups = meshFilters.GroupBy(mf => mf.name, mf => mf.gameObject);

        foreach (IGrouping<string, GameObject> meshGroup in meshGroups)
        {
            if (!MESHES_TO_IGNORE.Any(m => m.name == meshGroup.Key))
                CombineMeshes(meshGroup.ToArray(), room);
            else
                Debug.Log("Ignoring mesh " + meshGroup.Key + " for mesh combination.");
        } 
    }
	
    private void CombineMeshes( GameObject[] allGameObjectsWithMesh, GameObject room )
    {
        if (allGameObjectsWithMesh.Length == 0)
        {
            Debug.Log("CombineMeshes() was not given any meshes!");
        }
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

        /* Combine all the meshes to a single mesh, assigning it to the parent gameObject */
        GameObject master = new GameObject(allGameObjectsWithMesh[0].name + "_MASTER_MESH");
        master.AddComponent<MeshFilter>();
        master.AddComponent<MeshRenderer>();
        master.GetComponent<MeshFilter>().mesh = new Mesh();
        master.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        master.GetComponent<MeshRenderer>().material = material;
        master.transform.SetParent(room.transform);
        master.SetActive(true);
    }
}
