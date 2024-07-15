using System.Collections.Generic;
using System.Linq;
using UnityEditor;


namespace Barmetler.RoadSystem
{
    [CustomEditor(typeof(Intersection))]
    public class IntersectionEditor : Editor
    {
        private List<Road> affectedRoads;
        private Intersection intersection;


        private void OnEnable()
        {
            intersection = (Intersection) target;
            intersection.Invalidate();
            affectedRoads = intersection.AnchorPoints.Select(e => e.GetConnectedRoad()).Where(e => e).ToList();
            Undo.undoRedoPerformed += OnUndoRedo;
        }


        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
        }


        public void OnUndoRedo()
        {
            affectedRoads.ForEach(e => e.OnValidate());
        }


        private void OnSceneGUI()
        {
            if (intersection.transform.hasChanged)
            {
                intersection.transform.hasChanged = false;
                intersection.Invalidate();
            }
        }
    }
}