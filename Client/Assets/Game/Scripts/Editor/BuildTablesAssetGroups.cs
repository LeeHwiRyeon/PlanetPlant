using System;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

public class BuildTablesAssetGroups : MonoBehaviour {
    [MenuItem("Build/Build Asset Groups")]
    public static void BuildAssetGroups()
    {
        BuildAssetGroups(AddressableAssetGroupNames.Planets, "t:GameObject", typeof(GameObject), new string[] {
            "Assets/Game/Prefabs/Planets",
        });
        BuildAssetGroups(AddressableAssetGroupNames.Plants, "t:GameObject", typeof(GameObject), new string[] {
            "Assets/Game/Prefabs/Plants",
        });
        BuildAssetGroups(AddressableAssetGroupNames.Sounds, "t:AudioClip", typeof(AudioClip), new string[] {
            "Assets/Game/Resource/Sounds/PPSound",
        });
        BuildAssetGroups(AddressableAssetGroupNames.Effects, "t:GameObject", typeof(GameObject), new string[] {
            "Assets/Game/Prefabs/PPEffect",
        });
    }

    public static void BuildAssetGroups(string groupName, string findType, Type type, string[] assetPaths)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var group = settings.FindGroup(groupName);

        var guids = AssetDatabase.FindAssets(findType, assetPaths);
        foreach (var guid in guids) {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath(assetPath, type);
            if (asset == null) {
                continue;
            }
            var entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(assetPath), group);
            entry.address = asset.name;
            //entry.SetLabel("Tables", true, true);
        }

        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
    }


}