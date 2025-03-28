﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityStandardAssets.Utility;
using Object = UnityEngine.Object;


[Serializable]
public class AICarSyncSystem
{
    public enum Mode
    {
        None,
        Host,
        Client
    }


    public AICar[] Prefabs;
    private UNetHost _host;

    private Mode _mode;

    private List<AvatarPose> _poses = new();

    // Make sure to update the way these cars are logged if we
    // ever start supporting removal
    [NonSerialized]
    public List<PlayerAvatar> Cars = new();


    public void InitHost(UNetHost host)
    {
        _mode = Mode.Host;
        _host = host;
    }


    public void InitClient(MessageDispatcher dispatcher)
    {
        _mode = Mode.Client;
        dispatcher.AddStaticHandler((int) MsgId.S_SpawnAICar, ClientHandleSpawnAICar);
        dispatcher.AddStaticHandler((int) MsgId.S_UpdateAICarPoses, ClientHandleUpdatePoses);
    }


    private int FindPrefabIndex(AICar prefab)
    {
        for (var i = 0; i < Prefabs.Length; i++)
        {
            if (Prefabs[i] == prefab)
            {
                return i;
            }
        }

        return -1;
    }


    public AICar Spawn(CarSpawnParams parameters, bool yielding)
    {
        Assert.AreEqual(Mode.Host, _mode, "Only host can spawn synced objects");
        var prefabIdx = FindPrefabIndex(parameters.Car);
        Assert.AreNotEqual(-1, prefabIdx, $"The prefab {parameters.Car} was not added to NetworkingManager -> AICarSyncSystem -> Prefabs");
        var aiCar = Object.Instantiate(Prefabs[prefabIdx], parameters.SpawnPoint.position, parameters.SpawnPoint.rotation);
        aiCar.gameObject.layer = LayerMask.NameToLayer(yielding ? "Yielding" : "Car");
        aiCar.enabled = true;
        var waypointProgressTracker = aiCar.GetComponent<WaypointProgressTracker>();
        waypointProgressTracker.enabled = true;
        waypointProgressTracker.Init(parameters.Track);
        var paint = aiCar.GetComponent<CarConfigurator>();
        paint.ChangeParameters(parameters);
        var avatar = aiCar.GetComponent<PlayerAvatar>();
        avatar.Initialize(false, PlayerSystem.InputMode.None, PlayerSystem.ControlMode.HostAI, parameters.VehicleType);
        var color = parameters.color;
        var rb = aiCar.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.GetComponent<Rigidbody>().useGravity = true;
        Cars.Add(avatar);

        _host.BroadcastReliable(new SpawnAICarMsg
        {
            PrefabIdx = prefabIdx,
            Position = parameters.SpawnPoint.position,
            Rotation = parameters.SpawnPoint.rotation,
            SpawnPassenger = parameters.SpawnPassenger,
            Labeled = parameters.Labeled,
            SpawnDriver = parameters.SpawnDriver,
            VehicleType = (int) parameters.VehicleType,
            Color = new Vector3(color.r, color.g, color.b)
        });

        return aiCar;
    }


    private void ClientHandleSpawnAICar(ISynchronizer sync, int srcPlayerId)
    {
        var msg = NetMsg.Read<SpawnAICarMsg>(sync);
        var go = Object.Instantiate(Prefabs[msg.PrefabIdx], msg.Position, msg.Rotation);
        var avatar = go.GetComponent<PlayerAvatar>();
        var color = msg.Color;
        avatar.GetComponent<CarConfigurator>().ChangeParameters(msg);
        avatar.Initialize(true, PlayerSystem.InputMode.None, PlayerSystem.ControlMode.HostAI, (PlayerSystem.VehicleType) msg.VehicleType);
        Cars.Add(avatar);
    }


    private void ClientHandleUpdatePoses(ISynchronizer sync, int srcPlayerId)
    {
        var msg = NetMsg.Read<UpdateAICarPosesMsg>(sync);

        for (var i = 0; i < msg.Poses.Count; i++)
        {
            if (Cars[i] == null)
            {
                Debug.LogWarning("SOSXR: I don't have a car here");

                continue;
            }

            Cars[i].ApplyPose(msg.Poses[i]);
        }
    }


    public List<AvatarPose> GatherPoses()
    {
        _poses.Clear();

        foreach (var car in Cars)
        {
            _poses.Add(car.GetPose());
        }

        return _poses;
    }


    public void UpdateHost()
    {
        _host.BroadcastUnreliable(new UpdateAICarPosesMsg
        {
            Poses = GatherPoses()
        });
    }


    public struct SpawnAICarMsg : INetMessage
    {
        public int PrefabIdx;
        public Vector3 Position;
        public Quaternion Rotation;
        public bool SpawnDriver;
        public bool SpawnPassenger;
        public int VehicleType;
        public Vector3 Color;
        public bool Labeled;
        public int MessageId => (int) MsgId.S_SpawnAICar;


        public void Sync<T>(T synchronizer) where T : ISynchronizer
        {
            synchronizer.Sync(ref PrefabIdx);
            synchronizer.Sync(ref Position);
            synchronizer.Sync(ref Rotation);
            synchronizer.Sync(ref SpawnDriver);
            synchronizer.Sync(ref SpawnPassenger);
            synchronizer.Sync(ref VehicleType);
            synchronizer.Sync(ref Color);
            synchronizer.Sync(ref Labeled);
        }
    }


    private struct UpdateAICarPosesMsg : INetMessage
    {
        public List<AvatarPose> Poses;
        public int MessageId => (int) MsgId.S_UpdateAICarPoses;


        public void Sync<T>(T synchronizer) where T : ISynchronizer
        {
            synchronizer.SyncListSubmessage(ref Poses);
        }
    }
}