using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Barmetler.RoadSystem
{
    [CustomEditor(typeof(RoadSystem))]
    public class RoadSystemEditor : Editor
    {
        private int presetSelectedIndex = 0;
        private RoadSystem roadSystem;


        private void OnSceneGUI()
        {
            Draw();
        }


        private void Draw()
        {
            if (roadSystem.ShowDebugInfo)
            {
                var edges = roadSystem.GetGraphEdges();
                Handles.color = Color.blue;
                var style = new GUIStyle();
                style.normal.textColor = Color.magenta;

                foreach (var road in roadSystem.Roads)
                {
                    foreach (var segment in Enumerable.Range(0, road.NumSegments))
                    {
                        var points = road.GetPointsInSegment(segment).Select(e => road.transform.TransformPoint(e)).ToArray();
                        Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2f);
                    }
                }

                foreach (var edge in edges)
                {
                    Handles.DrawLine(edge.start, edge.end, 2f);

                    if (roadSystem.ShowEdgeWeights)
                    {
                        Handles.Label((edge.start + edge.end) / 2, "Cost: " + edge.cost, style);
                    }
                }
            }
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.BeginHorizontal();
            var options = MeshConversion.MeshOrientation.Presets.Keys.ToList();
            presetSelectedIndex = EditorGUILayout.Popup("Source Orientation Preset", presetSelectedIndex, options.ToArray());

            if (GUILayout.Button("Set All Roads"))
            {
                var generators = roadSystem.GetComponentsInChildren<RoadMeshGenerator>();
                var group = Undo.GetCurrentGroup();
                Undo.RecordObjects(generators, "Change Source Orientation Preset on all Roads");

                foreach (var g in generators)
                {
                    g.settings.SourceOrientation.Preset = options[presetSelectedIndex];
                }

                roadSystem.RebuildAllRoads();
                Undo.CollapseUndoOperations(group);
            }

            GUILayout.EndHorizontal();


            if (GUILayout.Button("Construct Graph"))
            {
                roadSystem.ConstructGraph();
                EditorUtility.SetDirty(roadSystem);
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("Rebuild All Roads"))
            {
                roadSystem.RebuildAllRoads();
                EditorUtility.SetDirty(roadSystem);
                SceneView.RepaintAll();
            }
        }


        private void OnEnable()
        {
            roadSystem = (RoadSystem) target;
        }
    }
}