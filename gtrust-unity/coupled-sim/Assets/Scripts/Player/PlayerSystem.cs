﻿using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityStandardAssets.Utility;


//spawns, initializes and manages avatar at runtime
public class PlayerSystem : MonoBehaviour
{
    public enum ControlMode
    {
        Driver,
        Passenger,
        HostAI
    }


    public enum InputMode
    {
        Flat,
        VR,
        Suite,
        None
    }


    public enum VehicleType
    {
        MDV,
        AV
    }


    public InputMode PlayerInputMode;
    [FormerlySerializedAs("_AvatarPrefab")] [SerializeField] private PlayerAvatar m_pedestrianPrefab;
    [SerializeField] private PlayerAvatar[] m_passengerPrefabs;
    [FormerlySerializedAs("_AvatarPrefabDriver")] [SerializeField] private PlayerAvatar[] m_driverPrefabs;


    // [NonSerialized]
    public PlayerAvatar LocalPlayer;

    // Avatars contains both Drivers and Pedestrians (in arbitrary order)
    //[NonSerialized]
    public List<PlayerAvatar> Avatars = new();
    //[NonSerialized]
    public List<PlayerAvatar> Cars = new();
    //[NonSerialized]
    public List<PlayerAvatar> Pedestrians = new();
    //[NonSerialized]
    public List<PlayerAvatar> Passengers = new();

    private readonly List<AvatarPose> _poses = new();

    private readonly PlayerAvatar[] Player2Avatar = new PlayerAvatar[UNetConfig.MaxPlayers];

    private HMIManager _hmiManager;
    public PlayerAvatar PedestrianPrefab => m_pedestrianPrefab;


    public PlayerAvatar GetAvatar(int player)
    {
        return Player2Avatar[player];
    }


    private void Awake()
    {
        _hmiManager = FindObjectOfType<HMIManager>();
    }


    public void ActivatePlayerAICar()
    {
        var aiCar = LocalPlayer.GetComponent<AICar>();
        var tracker = LocalPlayer.GetComponent<WaypointProgressTracker>();
        Assert.IsNotNull(tracker);
        aiCar.enabled = true;
        tracker.enabled = true;

        foreach (var waypoint in tracker.Circuit.Waypoints)
        {
            var speedSettings = waypoint.GetComponent<SpeedSettings>();

            if (speedSettings != null)
            {
                speedSettings.targetAICar = aiCar;
            }
        }
    }


    private PlayerAvatar GetAvatarPrefab(SpawnPointType type, int carIdx)
    {
        switch (type)
        {
            case SpawnPointType.PlayerControlledPedestrian:
                return m_pedestrianPrefab;
            case SpawnPointType.PlayerControllingCar:
            case SpawnPointType.PlayerInAIControlledCar:
                return m_driverPrefabs[carIdx];
            case SpawnPointType.PlayerPassivePassenger:
                return m_passengerPrefabs[0];
            default:
                Assert.IsFalse(true, $"Invalid SpawnPointType: {type}");

                return null;
        }
    }


    public void SpawnLocalPlayer(SpawnPoint spawnPoint, int player, ExperimentRoleDefinition role)
    {
        var isPassenger = spawnPoint.Type == SpawnPointType.PlayerInAIControlledCar;
        LocalPlayer = SpawnAvatar(spawnPoint, GetAvatarPrefab(spawnPoint.Type, role.carIdx), player, role);
        LocalPlayer.Initialize(false, PlayerInputMode, isPassenger ? ControlMode.Passenger : ControlMode.Driver, spawnPoint.VehicleType, spawnPoint.CameraIndex);

        if (isPassenger)
        {
            var waypointFollow = LocalPlayer.GetComponent<WaypointProgressTracker>();
            Assert.IsNotNull(waypointFollow);
            waypointFollow.Init(role.AutonomousPath);
            LocalPlayer.gameObject.layer = LayerMask.NameToLayer(role.AutonomousIsYielding ? "Yielding" : "Car");

            var hmiControl = LocalPlayer.GetComponent<ClientHMIController>();
            hmiControl.Init(_hmiManager);
        }

        ChangeLayerForFace();
    }


    private void ChangeLayerForFace()
    {
        var layerChanger = LocalPlayer.GetComponentsInChildren<LayerChanger>();

        if (layerChanger == null)
        {
            Debug.LogWarning("I didn't find a layerchanger");
        }
        else if (layerChanger.Length > 1)
        {
            Debug.LogWarning("I found too many layerchangers");
        }
        else
        {
            Debug.Log("I found the exact good amount of layerchangers");
            layerChanger[0].SetLayer();
        }
    }


    public void SpawnRemotePlayer(SpawnPoint spawnPoint, int player, ExperimentRoleDefinition role)
    {
        var remotePlayer = SpawnAvatar(spawnPoint, GetAvatarPrefab(spawnPoint.Type, role.carIdx), player, role);

        // DisableRemoteXROriginParts(remotePlayer);
        
        DestroyRemoteXROrigins(remotePlayer);

        remotePlayer.Initialize(true, InputMode.None, ControlMode.HostAI, spawnPoint.VehicleType);
    }

    private static void DestroyRemoteXROrigins(PlayerAvatar remotePlayer)
    {
        var remoteXROrigin = remotePlayer.GetComponentInChildren<XROrigin>();

        if (remoteXROrigin == null)
        {
            Debug.LogError("SOSXR: I couldn't find the remote XROrigin. This is not good.");
            return;
        }

        Debug.Log("SOSXR: I'm destroying the remote XROrigin. This is probably good.");
        Destroy(remoteXROrigin.gameObject);
    }

    
    private static void DisableRemoteXROriginParts(PlayerAvatar remotePlayer)
    {
        var remoteXROrigin = remotePlayer.GetComponentInChildren<XROrigin>();

        /*
        if (remotePlayer == null)
        {
            return;
        }
        remotePlayer.gameObject.SetActive(false);
        if (!remotePlayer.gameObject.activeInHierarchy)
        {
            return; // This is of course a bit bs, but a good way to quickly see what happens if we disable the entire XROrigin Gameobject.
        }
        */

        remoteXROrigin.transform.GetChild(0).gameObject.SetActive(false);
        Debug.LogWarning("SOSXR: I'm disabling the first child of the remote player. That sounds very wrong.");

        var recenter = remoteXROrigin.GetComponent<RecenterXROrigin>();
        recenter.enabled = false;
        Debug.Log("SOSXR: Disabling remote RecenterXROrigin");
    }


    public List<PlayerAvatar> GetAvatarsOfType(AvatarType type)
    {
        switch (type)
        {
            case AvatarType.Pedestrian: return Pedestrians;
            case AvatarType.Driver: return Cars;
            case AvatarType.Driven_Passenger: return Passengers;
            default:
                Assert.IsFalse(true, $"No avatar collection for type {type}");

                return null;
        }
    }


    private PlayerAvatar SpawnAvatar(SpawnPoint spawnPoint, PlayerAvatar prefab, int player, ExperimentRoleDefinition role)
    {
        var avatar = Instantiate(prefab);
        avatar.transform.position = spawnPoint.position;
        avatar.transform.rotation = spawnPoint.rotation;

        if (prefab.Type == AvatarType.Driven_Passenger)
        {
            var carTag = "ManualCar";
            var drivenCar = GameObject.FindGameObjectWithTag(carTag);

            if (drivenCar != null)
            {
                avatar.transform.parent = drivenCar.transform;
            }
            else
            {
                Debug.LogErrorFormat("SOSXR: For avatar {0} we're supposed to find a car with the tag {1}, but we couldn't find one in the scene. This is not good.", avatar.name, carTag);
            }
        }

        var cameraSetup = spawnPoint.Point.GetComponent<CameraSetup>();

        if (cameraSetup != null)
        {
            foreach (var cam in avatar.GetComponentsInChildren<Camera>())
            {
                cam.fieldOfView = cameraSetup.fieldOfView;
                cam.transform.localRotation = Quaternion.Euler(cameraSetup.rotation);
            }
        }

        Avatars.Add(avatar);
        GetAvatarsOfType(avatar.Type).Add(avatar);
        Player2Avatar[player] = avatar;

        if (role.HoodHMI != null)
        {
            _hmiManager.AddHMI(avatar.HMISlots.Spawn(HMISlot.Hood, role.HoodHMI));
        }

        if (role.TopHMI != null)
        {
            _hmiManager.AddHMI(avatar.HMISlots.Spawn(HMISlot.Top, role.TopHMI));
        }

        if (role.WindshieldHMI != null)
        {
            _hmiManager.AddHMI(avatar.HMISlots.Spawn(HMISlot.Windshield, role.WindshieldHMI));
        }

        return avatar;
    }


    public List<AvatarPose> GatherPoses()
    {
        _poses.Clear();

        foreach (var avatar in Avatars)
        {
            _poses.Add(avatar.GetPose());
        }

        return _poses;
    }


    public void ApplyPoses(List<AvatarPose> poses)
    {
        for (var i = 0; i < Avatars.Count; i++)
        {
            var avatar = Avatars[i];

            if (avatar != LocalPlayer)
            {
                Avatars[i].ApplyPose(poses[i]);
            }
        }
    }


    //displays controller selection GUI
    public void SelectModeGUI()
    {
        GUILayout.Label($"Mode: {PlayerInputMode}");

        if (GUILayout.Button("Suite mode"))
        {
            PlayerInputMode = InputMode.Suite;
        }

        if (GUILayout.Button("HMD mode"))
        {
            PlayerInputMode = InputMode.VR;
        }

        if (GUILayout.Button("Keyboard mode"))
        {
            PlayerInputMode = InputMode.Flat;
        }

        if (GUILayout.Button("None"))
        {
            PlayerInputMode = InputMode.None;
        }
    }


    public void SelectMode(InputMode inputMode)
    {
        PlayerInputMode = inputMode;
    }


    private void OnDisable()
    {
        Avatars = null;
        Cars = null;
        Pedestrians = null;
        Passengers = null;
    }
}