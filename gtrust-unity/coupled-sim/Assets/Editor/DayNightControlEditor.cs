using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


[CustomEditor(typeof(DayNightControl))]
public class DayNightControlEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myTarget = (DayNightControl) target;

        DrawDefaultInspector();

        if (GUILayout.Button("Night"))
        {
            myTarget.InitNight();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        if (GUILayout.Button("Day"))
        {
            myTarget.InitDay();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}