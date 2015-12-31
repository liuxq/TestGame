using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[CustomEditor(typeof(Tooltip))]
public class TooltipEditor : Editor
{

    Tooltip tooltip;
    SerializedProperty tooltipWidth;
    SerializedProperty tooltipHeight;
    SerializedProperty showTooltipIcon;
    SerializedProperty showTooltipName;
    SerializedProperty showTooltipDesc;
    SerializedProperty tooltipIconPosX;
    SerializedProperty tooltipIconPosY;
    SerializedProperty tooltipNamePosX;
    SerializedProperty tooltipNamePosY;
    SerializedProperty tooltipDescPosX;
    SerializedProperty tooltipDescPosY;
    SerializedProperty tooltipDescSizeX;
    SerializedProperty tooltipDescSizeY;
    SerializedProperty tooltipIconSize;

    int imageTypeIndexForTooltip = 2;
    int fontStyleOfTooltipName = 0;
    int fontStyleOfTooltipDesc = 0;

    bool foldOutIcon;
    bool foldOutName;
    bool foldOutDesc;

    void OnEnable()
    {
        tooltip = target as Tooltip;
        tooltip.setVariables();
        tooltipWidth = serializedObject.FindProperty("tooltipWidth");
        tooltipHeight = serializedObject.FindProperty("tooltipHeight");
        showTooltipIcon = serializedObject.FindProperty("showTooltipIcon");
        showTooltipName = serializedObject.FindProperty("showTooltipName");
        showTooltipDesc = serializedObject.FindProperty("showTooltipDesc");
        tooltipIconPosX = serializedObject.FindProperty("tooltipIconPosX");
        tooltipIconPosY = serializedObject.FindProperty("tooltipIconPosY");
        tooltipNamePosX = serializedObject.FindProperty("tooltipNamePosX");
        tooltipNamePosY = serializedObject.FindProperty("tooltipNamePosY");
        tooltipDescPosX = serializedObject.FindProperty("tooltipDescPosX");
        tooltipDescPosY = serializedObject.FindProperty("tooltipDescPosY");
        tooltipDescSizeX = serializedObject.FindProperty("tooltipDescSizeX");
        tooltipDescSizeY = serializedObject.FindProperty("tooltipDescSizeY");
        tooltipIconSize = serializedObject.FindProperty("tooltipIconSize");
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical("Box");
        if (!Application.isPlaying)
        {
            serializedObject.Update();
            toolTipSettings();
            serializedObject.ApplyModifiedProperties();
        }
        else
            EditorGUILayout.HelpBox("You cannot modify the Tooltip in playmode!", MessageType.Warning);
        GUILayout.EndVertical();
    }

    void toolTipSettings()
    {
        GUILayout.BeginVertical("Box");
        EditorGUI.BeginChangeCheck();
        
        tooltipWidth.intValue = EditorGUILayout.IntSlider("Tooltip Width:", tooltipWidth.intValue, 0, 500);                                 //intslider for the tooltip width
        tooltipHeight.intValue = EditorGUILayout.IntSlider("Tooltip Height:", tooltipHeight.intValue, 0, 500);

        EditorGUI.indentLevel++;
        tooltip.tooltipBackground.sprite = (Sprite)EditorGUILayout.ObjectField("Source Image", tooltip.tooltipBackground.sprite, typeof(Sprite), true);
        tooltip.tooltipBackground.color = EditorGUILayout.ColorField("Color", tooltip.tooltipBackground.color);
        tooltip.tooltipBackground.material = (Material)EditorGUILayout.ObjectField("Material", tooltip.tooltipBackground.material, typeof(Material), true);
        string[] imageTypes = new string[4]; imageTypes[0] = "Filled"; imageTypes[1] = "Simple"; imageTypes[2] = "Sliced"; imageTypes[3] = "Tiled";
        imageTypeIndexForTooltip = EditorGUILayout.Popup("Image Type", imageTypeIndexForTooltip, imageTypes, EditorStyles.popup);
        if (imageTypeIndexForTooltip == 0) { tooltip.tooltipBackground.type = Image.Type.Filled; imageTypeIndexForTooltip = 0; }
        else if (imageTypeIndexForTooltip == 1) { tooltip.tooltipBackground.type = Image.Type.Simple; imageTypeIndexForTooltip = 1; }
        else if (imageTypeIndexForTooltip == 2) { tooltip.tooltipBackground.type = Image.Type.Sliced; imageTypeIndexForTooltip = 2; }
        else if (imageTypeIndexForTooltip == 3) { tooltip.tooltipBackground.type = Image.Type.Tiled; imageTypeIndexForTooltip = 3; }
        tooltip.tooltipBackground.fillCenter = EditorGUILayout.Toggle("Fill Center", tooltip.tooltipBackground.fillCenter);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();
        if (EditorGUI.EndChangeCheck())
        {
            tooltip.updateTooltip();
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical("Box");
        showTooltipIcon.boolValue = EditorGUILayout.Toggle("Item Icon", showTooltipIcon.boolValue);                                          //if itemicon in tooltip activated
        if (showTooltipIcon.boolValue)
        {
            tooltip.updateTooltip();
            EditorGUI.indentLevel++;
            foldOutIcon = EditorGUILayout.Foldout(foldOutIcon, "Settings");
            if (foldOutIcon)
            {                
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel++;
                tooltipIconPosX.intValue = EditorGUILayout.IntSlider("Position X:", tooltipIconPosX.intValue, 0, tooltipWidth.intValue);
                tooltipIconPosY.intValue = EditorGUILayout.IntSlider("Position Y:", tooltipIconPosY.intValue, 0, -tooltipHeight.intValue);
                tooltipIconSize.intValue = EditorGUILayout.IntSlider("Size:", tooltipIconSize.intValue, 0, 150);
                EditorGUI.indentLevel--;
                if (EditorGUI.EndChangeCheck())
                {
                    tooltip.updateTooltip();
                }
            }
            EditorGUI.indentLevel--;
        }
        else
            tooltip.updateTooltip();
        GUILayout.EndVertical();

        EditorGUILayout.Space();
        GUILayout.BeginVertical("Box");
        showTooltipName.boolValue = EditorGUILayout.Toggle("Item Name", showTooltipName.boolValue);                                     //if you want a itemname in the tooltip the following options are there
        if (showTooltipName.boolValue)
        {
            tooltip.updateTooltip();
            EditorGUI.indentLevel++;
            foldOutName = EditorGUILayout.Foldout(foldOutName, "Settings");
            if (foldOutName)
            {
                EditorGUI.BeginChangeCheck();
                tooltipNamePosX.intValue = EditorGUILayout.IntSlider("Position X:", tooltipNamePosX.intValue, 0, tooltipWidth.intValue);
                tooltipNamePosY.intValue = EditorGUILayout.IntSlider("Position Y:", tooltipNamePosY.intValue, 0, -tooltipHeight.intValue);
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;
                EditorGUILayout.TextArea("Character", EditorStyles.boldLabel);
                tooltip.tooltipNameText.font = (Font)EditorGUILayout.ObjectField("Font", tooltip.tooltipNameText.font, typeof(Font), true);                                                 //objectfield for the fonts
                string[] fontTypes = new string[4]; fontTypes[0] = "Normal"; fontTypes[1] = "Bold"; fontTypes[2] = "Italic"; fontTypes[3] = "BoldAndItalic";        //different fonttypes in a string array
                fontStyleOfTooltipName = EditorGUILayout.Popup("Font Style", fontStyleOfTooltipName, fontTypes, EditorStyles.popup);                        //a popout with the fontstyles
                if (fontStyleOfTooltipName == 0) { tooltip.tooltipNameText.fontStyle = FontStyle.Normal; fontStyleOfTooltipName = 0; }                                  //sets the fontstyle in text 
                else if (fontStyleOfTooltipName == 1) { tooltip.tooltipNameText.fontStyle = FontStyle.Bold; fontStyleOfTooltipName = 1; }
                else if (fontStyleOfTooltipName == 2) { tooltip.tooltipNameText.fontStyle = FontStyle.Italic; fontStyleOfTooltipName = 2; }
                else if (fontStyleOfTooltipName == 3) { tooltip.tooltipNameText.fontStyle = FontStyle.BoldAndItalic; fontStyleOfTooltipName = 3; }
                tooltip.tooltipNameText.fontSize = EditorGUILayout.IntField("Font Size", tooltip.tooltipNameText.fontSize);                                                                 //fontsize for the itemName
                tooltip.tooltipNameText.lineSpacing = EditorGUILayout.FloatField("Line Spacing", tooltip.tooltipNameText.lineSpacing);                                                      //linespacing for the itemName
                tooltip.tooltipNameText.supportRichText = EditorGUILayout.Toggle("Rich Text", tooltip.tooltipNameText.supportRichText);                                                     //support rich text for the itemName
                tooltip.tooltipNameText.color = EditorGUILayout.ColorField("Color", tooltip.tooltipNameText.color);                                                                         //colorfield for different colors for the text
                tooltip.tooltipNameText.material = (Material)EditorGUILayout.ObjectField("Material", tooltip.tooltipNameText.material, typeof(Material), true);                             //material objectfield for the itemname
                EditorGUI.indentLevel--;
                if (EditorGUI.EndChangeCheck())
                {
                    tooltip.updateTooltip();
                }
            }
            EditorGUI.indentLevel--;
        }
        else
            tooltip.updateTooltip();

        GUILayout.EndVertical();


        EditorGUILayout.Space();
        GUILayout.BeginVertical("Box");
        showTooltipDesc.boolValue = EditorGUILayout.Toggle("Item Description", showTooltipDesc.boolValue);                                                 //toggle for the itemdescription
        if (showTooltipDesc.boolValue)
        {
            tooltip.updateTooltip();
            EditorGUI.indentLevel++;
            foldOutDesc = EditorGUILayout.Foldout(foldOutDesc, "Settings");
            if (foldOutDesc)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel++;
                tooltipDescPosX.intValue = EditorGUILayout.IntSlider("Position X:", tooltipDescPosX.intValue, 0, tooltipWidth.intValue);
                tooltipDescPosY.intValue = EditorGUILayout.IntSlider("Position Y:", tooltipDescPosY.intValue, 0, -tooltipHeight.intValue);
                tooltipDescSizeX.intValue = EditorGUILayout.IntSlider("Size X:", tooltipDescSizeX.intValue, 0, tooltipWidth.intValue);
                tooltipDescSizeY.intValue = EditorGUILayout.IntSlider("Size Y:", tooltipDescSizeY.intValue, 0, tooltipHeight.intValue);
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;
                EditorGUILayout.TextArea("Character", EditorStyles.boldLabel);
                tooltip.tooltipDescText.font = (Font)EditorGUILayout.ObjectField("Font", tooltip.tooltipDescText.font, typeof(Font), true);                                                 //objectfield for the fonts
                string[] fontTypes = new string[4]; fontTypes[0] = "Normal"; fontTypes[1] = "Bold"; fontTypes[2] = "Italic"; fontTypes[3] = "BoldAndItalic";        //different fonttypes in a string array
                fontStyleOfTooltipDesc = EditorGUILayout.Popup("Font Style", fontStyleOfTooltipDesc, fontTypes, EditorStyles.popup);                        //a popout with the fontstyles
                if (fontStyleOfTooltipDesc == 0) { tooltip.tooltipDescText.fontStyle = FontStyle.Normal; fontStyleOfTooltipDesc = 0; }                                  //sets the fontstyle in text 
                else if (fontStyleOfTooltipDesc == 1) { tooltip.tooltipDescText.fontStyle = FontStyle.Bold; fontStyleOfTooltipDesc = 1; }
                else if (fontStyleOfTooltipDesc == 2) { tooltip.tooltipDescText.fontStyle = FontStyle.Italic; fontStyleOfTooltipDesc = 2; }
                else if (fontStyleOfTooltipDesc == 3) { tooltip.tooltipDescText.fontStyle = FontStyle.BoldAndItalic; fontStyleOfTooltipDesc = 3; }
                tooltip.tooltipDescText.fontSize = EditorGUILayout.IntField("Font Size", tooltip.tooltipDescText.fontSize);                                                                 //fontsize for the itemDesc
                tooltip.tooltipDescText.lineSpacing = EditorGUILayout.FloatField("Line Spacing", tooltip.tooltipDescText.lineSpacing);                                                      //linespacing for the itemDesc
                tooltip.tooltipDescText.supportRichText = EditorGUILayout.Toggle("Rich Text", tooltip.tooltipDescText.supportRichText);                                                     //support rich text for the itemDesc
                tooltip.tooltipDescText.color = EditorGUILayout.ColorField("Color", tooltip.tooltipDescText.color);                                                                         //colorfield for different colors for the text
                tooltip.tooltipDescText.material = (Material)EditorGUILayout.ObjectField("Material", tooltip.tooltipDescText.material, typeof(Material), true);                             //material objectfield for the itemname
                EditorGUI.indentLevel--;

                EditorGUI.indentLevel--;
                if (EditorGUI.EndChangeCheck())
                {
                    tooltip.updateTooltip();
                }
            }
            EditorGUI.indentLevel--;
        }
        else
            tooltip.updateTooltip();

        GUILayout.EndVertical();

    }

}
