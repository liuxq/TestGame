using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(EquipmentSystem))]
public class EquipmentSystemEditor : Editor
{

    SerializedProperty slotsInTotal;
    SerializedProperty itemTypeOfSlots;
    EquipmentSystem eS;
    void OnEnable()
    {
        eS = target as EquipmentSystem;
        slotsInTotal = serializedObject.FindProperty("slotsInTotal");
        itemTypeOfSlots = serializedObject.FindProperty("itemTypeOfSlots");

    }

    public override void OnInspectorGUI()
    {
        eS.getSlotsInTotal();
        serializedObject.Update();
        GUILayout.BeginVertical("Box");
        for (int i = 0; i < slotsInTotal.intValue; i++)
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(itemTypeOfSlots.GetArrayElementAtIndex(i), new GUIContent("Slot " + (i + 1)));
            GUILayout.EndVertical();
        }
        serializedObject.ApplyModifiedProperties();
        GUILayout.EndVertical();
    }

}
