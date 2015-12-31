using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class CreateBlueprintDatabase
{
    public static BlueprintDatabase asset;                                                  //The List of all Blueprints

#if UNITY_EDITOR
    public static BlueprintDatabase createBlueprintDatabase()                                    //creates a new BlueprintDatabase(new instance)
    {
        asset = ScriptableObject.CreateInstance<BlueprintDatabase>();                       //of the ScriptableObject Blueprint

        AssetDatabase.CreateAsset(asset, "Assets/InventoryMaster/Resources/BlueprintDatabase.asset");            //in the Folder Assets/Resources/BlueprintDatabase.asset
        AssetDatabase.SaveAssets();                                                         //and than saves it there
        asset.blueprints.Add(new Blueprint());
        return asset;
    }
#endif



}
