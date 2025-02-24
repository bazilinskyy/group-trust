﻿//base class for client and server implementation

public abstract class NetworkSystem
{
    protected NetState _currentState;
    protected TransitionPhase _transitionPhase;


    public abstract void Update();


    public abstract void FixedUpdate();


    public abstract void OnGUI(bool RunTrialSequenceAutomatically);


    public abstract void Shutdown();


    protected enum TransitionPhase
    {
        None,
        LoadingLevel,
        // Level load AsyncOperation completes as soon as the level is loaded,
        // but before objects are spawned, so we wait an additional frame for those.
        WaitingForAwakes
    }


    protected enum NetState
    {
        Disconnected,
        Client_Connecting,
        Lobby,
        InGame
    }
}