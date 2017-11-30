using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Author: Corwin Belser
/// Allows properties to be added to a list, preventing them from being overriden
///     when a prefab change is applied
[ExecuteInEditMode]
public class PrefabPropertyLock : MonoBehaviour {

    public static class Orientation
    {
        public const string North = "N";
        public const string South = "S";
        public const string East = "E";
        public const string West = "W";
    }

    public static class WallType
    {
        public const string Passable = "_Passable";
        public const string Impassable = "_Impassable";
        public const string Doorway = "_Doorway";
        public const string Door = "_Door";
    }

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
    public PrefabProperty[] prefabProperties;

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
                if (child.name.Contains(WallType.Impassable))
                    prefabProperty.impassable = child.activeSelf;
                if (child.name.Contains(WallType.Doorway))
                    prefabProperty.doorway = child.activeSelf;
                if (child.name.Contains(WallType.Door))
                    prefabProperty.door = child.activeSelf;
            }
        }
        return prefabProperty;
    }

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
                    if (child.name.Contains(WallType.Impassable))
                        child.SetActive(prefabProperty.impassable);
                    if (child.name.Contains(WallType.Doorway))
                        child.SetActive(prefabProperty.doorway);
                    if (child.name.Contains(WallType.Door))
                        child.SetActive(prefabProperty.door);
                }
            }
        }
    }

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

    public void Start()
    {
        if (prefabProperties == null)
            SetupPrefabProperties();
    }

    public void Update()
    {
        foreach (PrefabProperty prefabProperty in prefabProperties)
            WriteActiveStates();
    }
}
