using UnityEngine;
using UnityEditor;
using System.IO;

public class RemoveCannon : EditorWindow {
    [MenuItem("Custom/Remove Cannon")]
    public static void RemoveCannonFromPrefabs()
    {
        string folderPath = "Assets/Game/Prefabs/Planets"; //폴더 경로
        string[] prefabPaths = Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories);

        foreach (string path in prefabPaths) {
            GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

            if (prefab != null) {
                Transform[] transforms = prefab.GetComponentsInChildren<Transform>(true);

                foreach (Transform transform in transforms) {
                    if (transform.gameObject.name == "Cannon") //Cannon 이름으로 찾기
                    {
                        DestroyImmediate(transform.gameObject, true);
                        PrefabUtility.SavePrefabAsset(prefab);
                        Debug.Log("Cannon removed from: " + prefab.name);
                    }
                }
            }
        }

        AssetDatabase.Refresh();
    }
}