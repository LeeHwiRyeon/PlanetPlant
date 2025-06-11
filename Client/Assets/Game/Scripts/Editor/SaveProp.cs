using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class SaveProp : EditorWindow {
    [MenuItem("Custom/SaveProp")]
    public static void SaveMantlePieces()
    {
        string folderPath = "Assets/Game/Prefabs/Planets"; //폴더 경로
        string folderSavePath = "Assets/Game/Prefabs/Planets/Data"; //폴더 경로
        if (!Directory.Exists(folderSavePath)) {
            Directory.CreateDirectory(folderSavePath);
        }
        string[] prefabPaths = Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories);

        foreach (string path in prefabPaths) {
            GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

            if (prefab != null) {
                var planet = prefab.GetComponent<Planet>();
                if (planet != null) {
                    string savePath = $"{folderSavePath}/{planet.name}_mantle.json";
                    var mantlePieces = new List<MantlePiece>(planet.MantlePieces);
                    string json = JsonConvert.SerializeObject(mantlePieces, Formatting.Indented);
                    File.WriteAllText(savePath, json);
                }
            }
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Custom/LoadProp")]
    public static void LoadMantlePieces()
    {
        string folderPath = "Assets/Game/Prefabs/Planets"; //폴더 경로
        string folderLoadPath = "Assets/Game/Prefabs/Planets/Data"; //폴더 경로
        string[] prefabPaths = Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories);

        foreach (string path in prefabPaths) {
            var planetPrefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (planetPrefab != null) {
                var planet = planetPrefab.GetComponent<Planet>();
                if (planet != null) {
                    string mantlePiecesPath = $"{folderLoadPath}/{planet.name}_mantle.json";
                    if (File.Exists(mantlePiecesPath)) {
                        string json = File.ReadAllText(mantlePiecesPath);
                        var mantlePieces = JsonConvert.DeserializeObject<List<MantlePiece>>(json);
                        var prefab = Instantiate(planetPrefab);
                        var planetInstance = prefab.GetComponent<Planet>();
                        if (mantlePieces.Count != planetInstance.MantlePieces.Count) {
                            planetInstance.MantlePieces = new List<MantlePiece>(mantlePieces.Count);
                            for (int i = 0; i < mantlePieces.Count; i++) {
                                planetInstance.MantlePieces.Add(new MantlePiece());
                            }
                        }
                        for (int i = 0; i < mantlePieces.Count; i++) {
                            var mantlePiece = mantlePieces[i];
                            planetInstance.MantlePieces[i].기본타일 = mantlePiece.기본타일;
                            planetInstance.MantlePieces[i].화산폭팔변경 = mantlePiece.화산폭팔변경;
                        }

                        string savePath = Path.Combine(Path.GetDirectoryName(path), planet.name + ".prefab");
                        PrefabUtility.SaveAsPrefabAsset(prefab, savePath);
                        DestroyImmediate(prefab);
                        Debug.Log(string.Format("{0} prefab updated with MantlePieces data", planet.name));
                    } else {
                        Debug.LogWarning(string.Format("{0}'s MantlePieces file not found", planet.name));
                    }
                }
            }
        }

        AssetDatabase.Refresh();
    }
}
