using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrefabPropertyLock))]
[CanEditMultipleObjects]
public class PrefabPropertyLockEditor : Editor {

    SerializedProperty ActiveStates;

    public void OnEnable()
    {
        ActiveStates = serializedObject.FindProperty("test");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(ActiveStates);
        serializedObject.ApplyModifiedProperties();
    }
}
