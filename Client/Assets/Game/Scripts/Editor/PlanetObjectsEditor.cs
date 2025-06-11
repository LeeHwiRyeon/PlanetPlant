using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(PlanetObjects))]
public class PlanetObjectsEditor : Editor {
    private static GameObject SelectedObject;
    private void OnEnable()
    {
        if (Application.isPlaying) {
            return;
        }

        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null && SelectedObject != selectedObject) {
            SelectedObject = selectedObject;
            var planetObjects = SelectedObject.GetComponent<PlanetObjects>();
            if (planetObjects != null) {
                GameObject currentEditingPrefab = GetCurrentEditingPrefabInstance();
                if (currentEditingPrefab == null) {
                    return;
                }

                var planet = currentEditingPrefab.GetComponent<Planet>();
                planetObjects.ApplySettings(planet);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("위치조정")) {
            var planetObjects = Selection.activeGameObject.GetComponent<PlanetObjects>();
            if (planetObjects != null) {
                GameObject currentEditingPrefab = GetCurrentEditingPrefabInstance();
                var planet = currentEditingPrefab.GetComponent<Planet>();
                planetObjects.ApplySettings(planet);
            }
        }
    }

    public GameObject GetCurrentEditingPrefabInstance()
    {
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null) {
            return prefabStage.prefabContentsRoot;
        }
        return null;
    }


}