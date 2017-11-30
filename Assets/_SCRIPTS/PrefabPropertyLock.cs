using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Author: Corwin Belser
/// Allows properties to be added to a list, preventing them from being overriden
///     when a prefab change is applied
[ExecuteInEditMode]
public class PrefabPropertyLock : MonoBehaviour {

    public Vector3 test;
    public Dictionary<string, bool> ActiveStates;

    public void Start()
    {
        if (ActiveStates == null)
        {
            ActiveStates = new Dictionary<string, bool>();

            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                ActiveStates.Add(child.name, child.activeSelf);
            }
        }
    }

    public void Update()
    {
        foreach (string name in ActiveStates.Keys) {
            Transform child = transform.Find(name);
            if (child != null) {
                bool value;
                if (ActiveStates.TryGetValue(name, out value))
                    child.gameObject.SetActive(value);
            }
        }
    }
}
