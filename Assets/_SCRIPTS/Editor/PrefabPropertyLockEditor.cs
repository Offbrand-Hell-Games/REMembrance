using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrefabPropertyLock))]
[CanEditMultipleObjects]
public class PrefabPropertyLockEditor : Editor {

    PrefabPropertyLock t;
    SerializedObject GetTarget;
    SerializedProperty ThisList;

    public void OnEnable()
    {
        t = (PrefabPropertyLock)target;
        GetTarget = new SerializedObject(t);
        ThisList = GetTarget.FindProperty("prefabProperties");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        for (int i = 0; i < ThisList.arraySize; i++)
        {
            SerializedProperty prefabProperty = ThisList.GetArrayElementAtIndex(i);

            EditorGUILayout.LabelField(t.prefabProperties[i].orientation);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(prefabProperty.FindPropertyRelative("passable"));
            EditorGUILayout.PropertyField(prefabProperty.FindPropertyRelative("impassable"));
            EditorGUILayout.PropertyField(prefabProperty.FindPropertyRelative("doorway"));
            EditorGUILayout.PropertyField(prefabProperty.FindPropertyRelative("door"));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
        GetTarget.ApplyModifiedProperties();
    }
}
