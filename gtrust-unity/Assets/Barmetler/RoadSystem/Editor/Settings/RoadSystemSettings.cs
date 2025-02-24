using System;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace Barmetler.RoadSystem
{
    [CreateAssetMenu(fileName = "RoadSystemSettings", menuName = "Barmetler/RoadSystemSettings")]
    public class RoadSystemSettings : ScriptableObject
    {
        [SerializeField] private RoadSettings roadSettings = new();
        [SerializeField] private IntersectionSettings intersectionSettings = new();

        [SerializeField] private bool drawNavigatorDebug = false;
        [SerializeField] private bool drawNavigatorDebugPoints = false;
        [SerializeField] private bool autoCalculateNavigator = false;

        internal static RoadSystemSettings instance = null;

        public const string settingsFolderPath = "Assets/Settings/Editor";
        public const string settingsPath = "Assets/Settings/Editor/RoadSystemSettings.asset";

        public bool DrawBoundingBoxes => roadSettings.drawBoundingBoxes;
        public bool UseRayCast => roadSettings.useRayCast;
        public bool CopyHitNormal => roadSettings.copyHitNormal;

        public GameObject NewRoadPrefab
        {
            get => roadSettings.newRoadPrefab;
            set
            {
                roadSettings.newRoadPrefab = value;
                EditorUtility.SetDirty(this);
            }
        }

        public GameObject NewIntersectionPrefab
        {
            get => intersectionSettings.newIntersectionPrefab;
            set
            {
                intersectionSettings.newIntersectionPrefab = value;
                EditorUtility.SetDirty(this);
            }
        }

        public bool DrawNavigatorDebug
        {
            get => drawNavigatorDebug;
            set
            {
                drawNavigatorDebug = value;
                EditorUtility.SetDirty(this);
            }
        }

        public bool DrawNavigatorDebugPoints
        {
            get => drawNavigatorDebugPoints;
            set
            {
                drawNavigatorDebugPoints = value;
                EditorUtility.SetDirty(this);
            }
        }

        public bool AutoCalculateNavigator
        {
            get => autoCalculateNavigator;
            set
            {
                autoCalculateNavigator = value;
                EditorUtility.SetDirty(this);
            }
        }

        public static RoadSystemSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = AssetDatabase.LoadAssetAtPath<RoadSystemSettings>(settingsPath);
                }

                if (instance == null)
                {
                    instance = CreateInstance<RoadSystemSettings>();
                    Directory.CreateDirectory(settingsFolderPath);
                    AssetDatabase.CreateAsset(instance, settingsPath);
                    AssetDatabase.SaveAssets();
                }

                return instance;
            }
        }

        internal static SerializedObject SerializedInstance => new(Instance);


        [Serializable]
        private class RoadSettings
        {
            [Tooltip("Draw bounding boxes around bezier segments?")]
            public bool drawBoundingBoxes = false;
            [Tooltip("When extending the road, whether to place it at the intersection of the mouse with the scene's geometry.")]
            public bool useRayCast = true;
            [Tooltip("If useRayCast is enabled, should the new road segment copy the surface normal of the intersection?")]
            public bool copyHitNormal = false;

            [Tooltip("The Prefab to use when creating a new road.")]
            public GameObject newRoadPrefab;
        }


        [Serializable]
        private class IntersectionSettings
        {
            [Tooltip("The Prefab to use when creating a new intersection.")]
            public GameObject newIntersectionPrefab;
        }
    }
}