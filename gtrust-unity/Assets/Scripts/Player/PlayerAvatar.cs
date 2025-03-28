﻿using System;
using System.Collections.Generic;
using System.IO;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using VehicleBehaviour;


public enum HMISlot
{
    None,
    Hood,
    Top,
    Windshield
}


// stores Transform state for every "bone" of an Avatar
// The pose represents both a pedestrian and car (for simplicity)
// so we have some redundancy
public struct AvatarPose : INetSubMessage
{
    public List<Vector3> LocalPositions;
    public List<Quaternion> LocalRotations;
    public BlinkerState Blinkers;
    public bool FrontLights;
    public bool StopLights;


    public void DeserializeFrom(BinaryReader reader)
    {
        LocalPositions = reader.ReadListVector3();
        LocalRotations = reader.ReadListQuaternion();
        Blinkers = (BlinkerState) reader.ReadInt32();
        FrontLights = reader.ReadBoolean();
        StopLights = reader.ReadBoolean();
    }


    public void DeserializeFrom(ref DataStreamReader reader)
    {
        LocalPositions = reader.ReadListVector3();
        LocalRotations = reader.ReadListQuaternion();
        Blinkers = (BlinkerState) reader.ReadInt();
        FrontLights = reader.ReadBoolean();
        StopLights = reader.ReadBoolean();
    }


    public void SerializeTo(BinaryWriter writer)
    {
        writer.Write(LocalPositions);
        writer.Write(LocalRotations);
        writer.Write((int) Blinkers);
        writer.Write(FrontLights);
        writer.Write(StopLights);
    }


    private IEnumerable<string> CsvEnumerator()
    {
        for (var i = 0; i < LocalPositions.Count; i++)
        {
            var pos = LocalPositions[i];
            var rot = LocalRotations[i];

            yield return $"{pos.x},{pos.y},{pos.z},{rot.x},{rot.y},{rot.z},{rot.w},{(int) Blinkers}";
        }
    }


    private IEnumerable<string> CsvEnumeratorNoBlinkers()
    {
        for (var i = 0; i < LocalPositions.Count; i++)
        {
            var pos = LocalPositions[i];
            var rot = LocalRotations[i];

            yield return $"{pos.x},{pos.y},{pos.z},{rot.x},{rot.y},{rot.z},{rot.w}";
        }
    }
}


public enum AvatarType
{
    Pedestrian,
    Driver,
    Driven_Passenger
}


// These are used for more than Players (AI cars have these as well)
public class PlayerAvatar : MonoBehaviour
{
    [Header("Controls")]
    public ModeElements VRModeElements;
    public ModeElements FlatModeElements;
    public ModeElements SuiteModeElements;
    public ModeElements NoModeElements;

    [Header("Player")]
    [FormerlySerializedAs("LocalModeElements")]
    public ModeElements HostDrivenAIElements;
    public ModeElements PlayerAsDriver;
    public ModeElements PlayerAsPassenger;

    [Header("Driver")]
    public ModeElements AV;
    public ModeElements MDV;

    [Header("Audio")]
    public EngineSoundManager Internal;
    public EngineSoundManager External;

    [Header("SOSXR")]
    [SerializeField] private bool m_instantiateXRRig = true;
    [SerializeField] private GameObject m_xrRigPrefab = null;
    [SerializeField] private Transform m_xrRigParent = null;

    [Header("Sync these over the network")]
    public Transform[] SyncTransforms; // SOSXR : These are the transforms that are synced over the network

    [Header("Other")]
    public Camera[] cameras;
    public HMIAnchors HMISlots;
    public AvatarType Type;

    [SerializeField] private CarBlinkers _carBlinkers;
    public GameObject stopLights;
    public GameObject frontLights;


    private readonly List<Vector3> _pos = new();
    private readonly List<Quaternion> _rot = new();
    public CarBlinkers CarBlinkers => _carBlinkers;


    //set up an Avatar (disabling and enabling needed components) for different control methods
    public void Initialize(bool isRemote, PlayerSystem.InputMode inputMode, PlayerSystem.ControlMode controlMode, PlayerSystem.VehicleType vehicleType, int cameraIndex = -1)
    {
        if (isRemote)
        {
            InitializeRemote();
        }
        else
        {
            InitializeLocal(inputMode, controlMode, vehicleType, cameraIndex);
        }
    }


    private void InitializeLocal(PlayerSystem.InputMode inputMode, PlayerSystem.ControlMode controlMode, PlayerSystem.VehicleType vehicleType, int cameraIndex)
    {
        InitializeCamera(inputMode, cameraIndex);

        var modeElements = InitializeInputMode(inputMode);

        SetupModeElements(modeElements);

        modeElements = InitializeControlMode(controlMode);

        SetupModeElements(modeElements);

        modeElements = InitializeVehicle(vehicleType);

        SetupModeElements(modeElements);
    }


    private void InitializeCamera(PlayerSystem.InputMode inputMode, int cameraIndex)
    {
        if (cameraIndex >= 0)
        {
            if ((m_instantiateXRRig && m_xrRigPrefab != null && inputMode == PlayerSystem.InputMode.VR) || inputMode == PlayerSystem.InputMode.Suite)
            {
                var rig = Instantiate(m_xrRigPrefab, m_xrRigParent);

                cameras[cameraIndex] = rig.GetComponentInChildren<Camera>();

                rig.transform.parent = cameras[cameraIndex].transform.parent; // This will probably be the 'CameraParent'
                rig.transform.localPosition = Vector3.zero;
                rig.transform.localRotation = Quaternion.identity;

                var cam = rig.GetComponentInChildren<Camera>();
                cameras[cameraIndex] = cam;

                Debug.Log("SOSXR: Instantiated XR_Origin rig for XR, instead of enabling the default camera.");
            }
            else
            {
                var go = new GameObject("SOSXR: Instantiated Normal Camera");
                go.transform.parent = m_xrRigParent;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                cameras[cameraIndex] = go.AddComponent<Camera>();
                cameras[cameraIndex].nearClipPlane = 0.06f;
                go.AddComponent<AudioListener>();
            }
        }
    }


    private ModeElements InitializeInputMode(PlayerSystem.InputMode inputMode)
    {
        ModeElements modeElements;

        switch (inputMode)
        {
            case PlayerSystem.InputMode.Suite:
                modeElements = SuiteModeElements;

                break;
            case PlayerSystem.InputMode.Flat:
                modeElements = FlatModeElements;

                break;
            case PlayerSystem.InputMode.VR:
                modeElements = VRModeElements;

                break;
            case PlayerSystem.InputMode.None:

                modeElements = NoModeElements;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(inputMode), inputMode, null);
        }

        return modeElements;
    }


    private ModeElements InitializeControlMode(PlayerSystem.ControlMode controlMode)
    {
        ModeElements modeElements;

        switch (controlMode)
        {
            case PlayerSystem.ControlMode.Driver:
                if (Internal != null)
                {
                    Internal.enabled = true;
                }

                modeElements = PlayerAsDriver;

                break;
            case PlayerSystem.ControlMode.HostAI:
                if (External != null)
                {
                    External.enabled = true;
                }

                modeElements = HostDrivenAIElements;

                break;
            case PlayerSystem.ControlMode.Passenger:
                if (Internal != null)
                {
                    Internal.enabled = true;
                }

                modeElements = PlayerAsPassenger;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(controlMode), controlMode, null);
        }

        return modeElements;
    }


    private ModeElements InitializeVehicle(PlayerSystem.VehicleType vehicleType)
    {
        ModeElements modeElements;

        switch (vehicleType)
        {
            case PlayerSystem.VehicleType.AV:
                modeElements = AV;

                break;
            case PlayerSystem.VehicleType.MDV:
                modeElements = MDV;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(vehicleType), vehicleType, null);
        }

        return modeElements;
    }


    private void InitializeRemote()
    {
        if (External != null)
        {
            External.enabled = true;
        }

        GetComponentInChildren<Rigidbody>().isKinematic = true;
        GetComponentInChildren<Rigidbody>().useGravity = false;

        foreach (var wc in GetComponentsInChildren<WheelCollider>())
        {
            wc.enabled = false;
        }

        foreach (var su in GetComponentsInChildren<Suspension>())
        {
            su.enabled = false;
        }

        if (_carBlinkers == null)
        {
            _carBlinkers = FindObjectOfType<CarBlinkers>();
        }

        if (_carBlinkers == null)
        {
            Debug.LogFormat("SOSXR: In our PlayerAvatar, a passenger doesn't carry their own blinkers, so they needed to find some 'out there'. We want to be sure we got the correct blinkers: {0}", _carBlinkers.name);
        }

        if (stopLights == null)
        {
            Debug.Log("SOSXR: We don't yet have any stoplights, but also no way to get some new ones.");
        }

        if (frontLights == null)
        {
            Debug.Log("SOSXR: We don't yet have any front lights, but also no way to get some new ones. Maybe they're on a car somewhere?");
        }
    }


    private static void SetupModeElements(ModeElements modeElements)
    {
        if (modeElements.gameObjects != null)
        {
            foreach (var go in modeElements.gameObjects)
            {
                if (go != null)
                {
                    go.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("SOSXR: A GameObject mode-element was expected here, but they are null");
                }
            }
        }

        if (modeElements.monoBehaviours != null)
        {
            foreach (var mb in modeElements.monoBehaviours)
            {
                if (mb != null)
                {
                    mb.enabled = true;
                }
                else
                {
                    Debug.LogWarning("SOSXR: A MonoBehaviour mode-element was expected here, but they are null.");
                }
            }
        }

        if (modeElements.collider != null)
        {
            modeElements.collider.enabled = true;
        }
        /*
        if (modeElements.disabledGameObjects != null)
        {
            foreach (var go in modeElements.disabledGameObjects)
            {
                go.SetActive(false);
            }
        }
        if (modeElements.disabledMonoBehaviours != null)
        {
            foreach (var mb in modeElements.disabledMonoBehaviours)
            {
                mb.enabled = false;
            }
        }
        */
    }


    /// <summary>
    ///     This happens on the Host
    /// </summary>
    /// <returns></returns>
    public AvatarPose GetPose()
    {
        _pos.Clear();
        _rot.Clear();
        _pos.Add(transform.position);
        _rot.Add(transform.rotation);

        for (var i = 0; i < SyncTransforms.Length; i++)
        {
            var trans = SyncTransforms[i];
            _pos.Add(trans.localPosition);
            _rot.Add(trans.localRotation);
        }

        return new AvatarPose
        {
            LocalPositions = _pos,
            LocalRotations = _rot,
            Blinkers = _carBlinkers == null ? BlinkerState.None : _carBlinkers.State,
            FrontLights = frontLights != null && frontLights.activeSelf,
            StopLights = stopLights != null && stopLights.activeSelf
        };
    }


    /// <summary>
    ///     This happens on the Client
    /// </summary>
    /// <param name="pose"></param>
    public void ApplyPose(AvatarPose pose)
    {
        transform.position = pose.LocalPositions[0];
        transform.rotation = pose.LocalRotations[0];

        for (var i = 0; i < SyncTransforms.Length; i++)
        {
            var trans = SyncTransforms[i];
            trans.localPosition = pose.LocalPositions[i + 1];
            trans.localRotation = pose.LocalRotations[i + 1];
        }

        if (_carBlinkers != null)
        {
            _carBlinkers.SwitchToState(pose.Blinkers);
        }

        if (frontLights != null)
        {
            frontLights.SetActive(pose.FrontLights);
        }

        if (stopLights != null)
        {
            stopLights.SetActive(pose.StopLights);
        }
    }


    internal void SetBreakLights(bool breaking)
    {
        stopLights.SetActive(breaking);
    }


    [Serializable]
    public struct ModeElements
    {
        [Header("Enabled elements")]
        public GameObject[] gameObjects;
        public MonoBehaviour[] monoBehaviours;
        public Collider collider;
        /*
        [Header("Disabled elements")]
        public GameObject[] disabledGameObjects;
        public MonoBehaviour[] disabledMonoBehaviours;
        */
    }


    // defines HMI to be spawned on the car
    [Serializable]
    public struct HMIAnchors
    {
        public Transform Hood;
        public Transform Top;
        public Transform Windshield;
        [NonSerialized]
        public HMI HoodHMI;
        [NonSerialized]
        public HMI TopHMI;
        [NonSerialized]
        public HMI WindshieldHMI;


        private HMI Spawn(HMI prefab, ref HMI instance, Transform parent)
        {
            if (instance != null)
            {
                Destroy(instance);
            }

            instance = Instantiate(prefab, parent);
            instance.transform.localPosition = default;
            instance.transform.localRotation = Quaternion.identity;

            return instance;
        }


        public HMI Spawn(HMISlot slot, HMI prefab)
        {
            switch (slot)
            {
                default:
                case HMISlot.None:
                    Assert.IsFalse(true);

                    return null;
                case HMISlot.Hood:
                    return Spawn(prefab, ref HoodHMI, Hood);
                case HMISlot.Top:
                    return Spawn(prefab, ref TopHMI, Top);
                case HMISlot.Windshield:
                    return Spawn(prefab, ref WindshieldHMI, Windshield);
            }
        }
    }
}