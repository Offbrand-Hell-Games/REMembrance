using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// Author: Corwin Belser
/// Editor extension for displaying active state of walls for a tile prefab
[CustomEditor(typeof(PrefabActiveTogglePropertyLock))]
[CanEditMultipleObjects]
public class PrefabActiveTogglePropertyLockEditor : Editor {

    PrefabActiveTogglePropertyLock prefabPropertyLock;
    SerializedObject serializedObject;
    SerializedProperty prefabProperties;

    /// <summary>
    /// Called when script is added to component
    /// Stores reference to attached PrefabPropertyLock class
    /// </summary>
    public void OnEnable()
    {
        prefabPropertyLock = (PrefabActiveTogglePropertyLock)target;
        serializedObject = new SerializedObject(prefabPropertyLock);
        prefabProperties = serializedObject.FindProperty("prefabProperties");
    }

    /// <summary>
    /// Called when gameObject displayed in inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        for (int i = 0; i < prefabProperties.arraySize; i++)
        {
            SerializedProperty prefabProperty = prefabProperties.GetArrayElementAtIndex(i);

            EditorGUILayout.LabelField(prefabPropertyLock.prefabProperties[i].orientation);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(prefabProperty.FindPropertyRelative("passable"));
            EditorGUILayout.PropertyField(prefabProperty.FindPropertyRelative("impassable"));
            EditorGUILayout.PropertyField(prefabProperty.FindPropertyRelative("doorway"));
            EditorGUILayout.PropertyField(prefabProperty.FindPropertyRelative("door"));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
        serializedObject.ApplyModifiedProperties();
        prefabPropertyLock.UpdateActiveStates();
    }
}
