using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(LightProbePlacer))]
public class LightProbePlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myTarget = (LightProbePlacer) target;
        DrawDefaultInspector();

        if (GUILayout.Button("Place Light Probes"))
        {
            myTarget.PlaceLightProbes();
        }
    }
}