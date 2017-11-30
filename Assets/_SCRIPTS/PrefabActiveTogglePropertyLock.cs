using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Author: Corwin Belser
/// Allows active states to be stored for wall types in tile prefabs,
///   preventing them from being overriden when the base prefab is updated
[ExecuteInEditMode]
public class PrefabActiveTogglePropertyLock : MonoBehaviour {

    /// <summary>
    /// string constants for doing string.Contains to find wall components for a given orientation
    /// </summary>
    public static class Orientation
    {
        public const string North = "N";
        public const string South = "S";
        public const string East = "E";
        public const string West = "W";
    }

    /// <summary>
    /// string constants for for doing string.Contains to find individual wall components
    /// </summary>
    /// <TODO> _Door will match both _Doorway & _Door</TODO>
    public static class WallType
    {
        public const string Passable = "_Passable";
        public const string Impassable = "_Impassable";
        public const string Doorway = "_Doorway";
        public const string Door = "_Door";
    }

    /// <summary>
    /// Holds Active states of wall types for an orientation
    /// </summary>
    [System.Serializable]
    public class PrefabProperty
    {
        public string orientation;
        public bool passable;
        public bool impassable;
        public bool doorway;
        public bool door;

        public override string ToString()
        {
            return orientation + "\r\n"
                + "\t" + "passable: " + passable.ToString() + "\r\n"
                + "\t" + "impassable: " + impassable.ToString() + "\r\n"
                + "\t" + "doorway: " + doorway.ToString() + "\r\n"
                + "\t" + "door: " + door.ToString() + "\r\n";
        }
    }
    public PrefabProperty[] prefabProperties; /* Array of orientations present on the prefab */

    /// <summary>
    /// Creates a PrefabProperty for an orientation, setting the Active state of the wall components
    /// </summary>
    /// <param name="orientation"> The Orientation string const</param>
    /// <returns></returns>
    private PrefabProperty ReadActiveState(string orientation)
    {
        PrefabProperty prefabProperty = new PrefabProperty();
        prefabProperty.orientation = orientation;

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.name.Contains(orientation))
            {
                if (child.name.Contains(WallType.Passable))
                    prefabProperty.passable = child.activeSelf;
                else if (child.name.Contains(WallType.Impassable))
                    prefabProperty.impassable = child.activeSelf;
                else if (child.name.Contains(WallType.Doorway))
                    prefabProperty.doorway = child.activeSelf;
                else if (child.name.Contains(WallType.Door))
                    prefabProperty.door = child.activeSelf;
            }
        }
        return prefabProperty;
    }

    /// <summary>
    /// Overwrites the active states of wall components for all wall orientations
    /// </summary>
    private void WriteActiveStates()
    {
        foreach (PrefabProperty prefabProperty in prefabProperties)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (child.name.Contains(prefabProperty.orientation))
                {
                    if (child.name.Contains(WallType.Passable))
                        child.SetActive(prefabProperty.passable);
                    else if (child.name.Contains(WallType.Impassable))
                        child.SetActive(prefabProperty.impassable);
                    else if (child.name.Contains(WallType.Doorway))
                        child.SetActive(prefabProperty.doorway);
                    else if (child.name.Contains(WallType.Door))
                        child.SetActive(prefabProperty.door);
                }
            }
        }
    }

    /// <summary>
    /// Initializes the prefabProperties variable, inserting a PrefabProperty instance for each instance found
    /// </summary>
    public void SetupPrefabProperties()
    {
        /* Determine how many orientations are attached to this prefab */
        int orientationCount = 0;
        if (name.Contains(Orientation.North)) orientationCount++;
        if (name.Contains(Orientation.South)) orientationCount++;
        if (name.Contains(Orientation.East)) orientationCount++;
        if (name.Contains(Orientation.West)) orientationCount++;

        if (orientationCount == 0)
            return;

        /* Initialize each orientation */
        prefabProperties = new PrefabProperty[orientationCount];
        int index = 0;
        if (name.Contains(Orientation.North))
        {
            prefabProperties[index] = ReadActiveState(Orientation.North);
            index++;
        }
            
        if (name.Contains(Orientation.South))
        {
            prefabProperties[index] = ReadActiveState(Orientation.South);
            index++;
        }
        if (name.Contains(Orientation.East))
        {
            prefabProperties[index] = ReadActiveState(Orientation.East);
            index++;
        }
        if (name.Contains(Orientation.West))
        {
            prefabProperties[index] = ReadActiveState(Orientation.West);
            index++;
        }
    }

    /// <summary>
    /// Called when script is started. Initializes if not already
    /// </summary>
    public void Start()
    {
        if (prefabProperties == null)
            SetupPrefabProperties();
    }

    /// <summary>
    /// Called from PrefabPropertyLockEditor when a change is made
    /// </summary>
    public void UpdateActiveStates()
    {
        if (prefabProperties != null && prefabProperties.Length != 0)
        {
            foreach (PrefabProperty prefabProperty in prefabProperties)
                WriteActiveStates();
        }
    }
}
