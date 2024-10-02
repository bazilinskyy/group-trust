using System.Collections.Generic;
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


    private InputMode PlayerInputMode = InputMode.VR;
    [SerializeField] private PlayerAvatar m_pedestrianPrefab;
    [SerializeField] private PlayerAvatar[] m_passengerPrefabs;
    [SerializeField] private PlayerAvatar[] m_driverPrefabs;

[TagSelector] [SerializeField] private string m_carTag = "ManualCar";
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

    [SerializeField] private List<AvatarPose> _poses = new();

  [SerializeField]  private PlayerAvatar[] Player2Avatar = new PlayerAvatar[UNetConfig.MaxPlayers];

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
        // Assert.IsNotNull(tracker);

        if (aiCar == null || tracker == null)
        {
            return;
        }
        
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
                return m_driverPrefabs[carIdx]; // SOSXR
            case SpawnPointType.PlayerInAIControlledCar:
                return m_driverPrefabs[carIdx];
            case SpawnPointType.PlayerPassivePassenger:
                return m_passengerPrefabs[0]; // SOSXR: Why only index 0?
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

        if (spawnPoint.Type is SpawnPointType.PlayerControllingCar or SpawnPointType.PlayerInAIControlledCar)
        {
            LocalPlayer.GetComponentInChildren<Speedometer>().enabled = true;
            Debug.Log("SOSXR: I'm enabling the Speedometer for the driven car. This is probably good, although not very pretty.");
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
        else if (layerChanger.Length == 0)
        {
            Debug.LogWarning("I didn't find a layerchanger");
        }
        else
        {
            if (layerChanger[0] == null)
            {
                Debug.LogWarning("I found a null layerchanger");
            }
            else
            {
                Debug.Log("I found a good layerchanger");
                layerChanger[0].SetLayer();
            }
            
        }
    }


    public void SpawnRemotePlayer(SpawnPoint spawnPoint, int player, ExperimentRoleDefinition role)
    {
        var remotePlayer = SpawnAvatar(spawnPoint, GetAvatarPrefab(spawnPoint.Type, role.carIdx), player, role);

        // DisableRemoteXROriginParts(remotePlayer);

        // DisableRemoteXRRecenterer(remotePlayer);
        
        DestroyRemoteXRRecenterer(remotePlayer);
        
        
        remotePlayer.Initialize(true, InputMode.None, ControlMode.HostAI, spawnPoint.VehicleType);
    }


    private void DestroyRemoteXRRecenterer(PlayerAvatar remotePlayer)
    {
        var recenterer = remotePlayer.GetComponentInChildren<RecenterChildXROrigin>();

        if (recenterer == null)
        {
            Debug.LogWarning("SOSXR: I couldn't find the remote RecenterChildXROrigin to destroy. This is not good.");

            return;
        }

        Destroy(recenterer);
    }


    private void DisableRemoteXRRecenterer(PlayerAvatar remotePlayer)
    {
        var recenterer = remotePlayer.GetComponentInChildren<RecenterChildXROrigin>();

        if (recenterer == null)
        {
            Debug.LogWarning("SOSXR: I couldn't find the remote RecenterChildXROrigin to disable. This is not good.");

            return;
        }

        recenterer.enabled = false;
    }


    private static void DisableRemoteXROriginParts(PlayerAvatar remotePlayer)
    {
        var remoteXROrigin = remotePlayer.GetComponentInChildren<XROrigin>();

        if (remoteXROrigin == null)
        {
            Debug.LogError("SOSXR: I couldn't find the remote XROrigin. This is not good.");

            return;
        }

        foreach (Transform child in remoteXROrigin.transform)
        {
            child.gameObject.SetActive(false);
            Debug.Log("SOSXR: I'm disabling a child of the remote XROrigin. This is probably good.");
        }

        remoteXROrigin.gameObject.SetActive(false);
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
            var drivenCar = GameObject.FindGameObjectWithTag(m_carTag);

            if (drivenCar != null)
            {
                avatar.transform.parent = drivenCar.transform;
            }
            else
            {
                Debug.LogErrorFormat("SOSXR: For avatar {0} we're supposed to find a car with the tag {1}, but we couldn't find one in the scene. This is not good.", avatar.name, m_carTag);

                return avatar;
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
            
            // Debug.LogFormat("SOSXR: I'm gathering poses for avatar {0} with {1} LocalPositions. I think this only happens on the Host?", avatar.name,  _poses[^1].LocalPositions.Count);
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

                // Debug.LogFormat("SOSXR: I'm applying poses for Avatar {0}, but not the local player. And only on Clients?", Avatars[i].name);
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