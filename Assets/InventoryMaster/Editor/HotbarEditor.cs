using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(Hotbar))]
public class HotbarEditor : Editor
{

    SerializedProperty slotsInTotal;
    SerializedProperty keyCodesForSlots;
    Hotbar hotbar;

    void OnEnable()
    {
        hotbar = target as Hotbar;
        slotsInTotal = serializedObject.FindProperty("slotsInTotal");
        keyCodesForSlots = serializedObject.FindProperty("keyCodesForSlots");
        slotsInTotal.intValue = hotbar.getSlotsInTotal();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUILayout.BeginVertical("Box");
        slotsInTotal.intValue = hotbar.getSlotsInTotal();
        for (int i = 0; i < slotsInTotal.intValue; i++)
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(keyCodesForSlots.GetArrayElementAtIndex(i), new GUIContent("Slot " + (i + 1)));
            GUILayout.EndVertical();
        }
        serializedObject.ApplyModifiedProperties();
        GUILayout.EndVertical();
    }
}
