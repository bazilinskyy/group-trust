using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;


// TODO(jacek): Probably a good idea to move all the PlayerAvatar syncing in one place and just register them from here
[Serializable]
public class AIPedestrianSyncSystem
{
    public enum Mode
    {
        None,
        Host,
        Client
    }


    [SerializeField] private PedestrianDesc[] AIPedestrians;

    private UNetHost _host;

    private List<AvatarPose> _poses = new();

    private List<PlayerAvatar> Pedestrians = new();


    public void InitHost(UNetHost host)
    {
        _host = host;

        foreach (var p in AIPedestrians)
        {
            var ped = p.Pedestrian;
            Pedestrians.Add(ped.GetComponent<PlayerAvatar>());
            ped.Init(p.Path);
            ped.GetComponent<Animator>().enabled = true;
        }
    }


    public void InitClient(MessageDispatcher dispatcher)
    {
        dispatcher.AddStaticHandler((int) MsgId.S_UpdateAIPedestrianPoses, ClientHandleUpdatePoses);

        foreach (var p in AIPedestrians)
        {
            var ped = p.Pedestrian;
            Pedestrians.Add(ped.GetComponent<PlayerAvatar>());
            ped.GetComponent<AIPedestrian>().enabled = false;
        }
    }


    private void ClientHandleUpdatePoses(ISynchronizer sync, int srcPlayerId)
    {
        var msg = NetMsg.Read<UpdateAIPedestrianPosesMessage>(sync);

        for (var i = 0; i < msg.Poses.Count; i++)
        {
            Pedestrians[i].ApplyPose(msg.Poses[i]);
        }
    }


    public List<AvatarPose> GatherPoses()
    {
        _poses.Clear();

        foreach (var ped in Pedestrians)
        {
            _poses.Add(ped.GetPose());
        }

        return _poses;
    }


    public void UpdateHost()
    {
        _host.BroadcastUnreliable(new UpdateAIPedestrianPosesMessage
        {
            Poses = GatherPoses()
        });
    }


    [Serializable]
    private struct PedestrianDesc
    {
        public AIPedestrian Pedestrian;
        public WaypointCircuit Path;
    }


    private struct UpdateAIPedestrianPosesMessage : INetMessage
    {
        public int MessageId => (int) MsgId.S_UpdateAIPedestrianPoses;

        public List<AvatarPose> Poses;


        public void Sync<T>(T synchronizer) where T : ISynchronizer
        {
            synchronizer.SyncListSubmessage(ref Poses);
        }
    }
}