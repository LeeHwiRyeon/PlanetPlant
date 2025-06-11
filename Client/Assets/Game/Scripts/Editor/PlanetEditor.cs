using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor {
    private static GameObject SelectedObject;
    private void OnEnable()
    {
        if (Application.isPlaying) {
            return;
        }

        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null && SelectedObject != selectedObject) {
            SelectedObject = selectedObject;
            var planet = SelectedObject.GetComponent<Planet>();
            if (planet != null) {
                planet.ApplyShaderSettings();
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Apply Shader Settings")) {
            var planet = Selection.activeGameObject.GetComponent<Planet>();
            if (planet != null) {
                planet.ApplyShaderSettings();
            }
        }
    }


}