using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct TrafficLightEvent
{
    //miningful description of cycle event
    public string name;
    //time that has to pass since previous event before activating this event
    public float deltaTime;

    //sections on which state change will be applied on
    public CarSection[] carSections;
    public PedestrianSection[] pedestrianSections;

    //state to be set on the carSections and pedestrainSections
    public LightState state;
}


public enum LightState
{
    GREEN,
    BLINK_GREEN,
    RED,
    YELLOW,
    RED_AND_YELLOW,
    LOOP_BACK
}


//defines and executes traffic light cycle described as a sequence of TrafficLightEvents
public class TrafficLightsManager : MonoBehaviour
{
    [SerializeField]
    private TrafficLightEvent[] initialStreetLightSetup;
    [SerializeField]
    private TrafficLightEvent[] streetLightEvents;
    public int CurrentIndex = 0;
    public float CurrentTimer = 0;
    private bool initialized = false;


    public void UpdateHost(List<int> initiallyTriggeredEvents, List<int> triggeredEvents)
    {
        if (!initialized)
        {
            for (var i = 0; i < initialStreetLightSetup.Length; i++)
            {
                initiallyTriggeredEvents.Add(CurrentIndex);
                TriggerEvent(i, true);
            }

            initialized = true;
        }

        while (CurrentTimer >= streetLightEvents[CurrentIndex].deltaTime && CurrentIndex < streetLightEvents.Length)
        {
            triggeredEvents.Add(CurrentIndex);
            TriggerEvent(CurrentIndex, false);
            CurrentTimer -= streetLightEvents[CurrentIndex].deltaTime;
            CurrentIndex++;

            if (CurrentIndex >= streetLightEvents.Length)
            {
                CurrentIndex = 0;
            }
        }

        CurrentTimer += Time.deltaTime;
    }


    public void TriggerEvent(int idx, bool initialSetup)
    {
        if (initialSetup)
        {
            ChangeCarLightsState(ref initialStreetLightSetup[idx]);
            ChangePedestrianLightsState(ref initialStreetLightSetup[idx]);
        }
        else
        {
            ChangeCarLightsState(ref streetLightEvents[idx]);
            ChangePedestrianLightsState(ref streetLightEvents[idx]);
        }
    }


    private void ChangeCarLightsState(ref TrafficLightEvent streetLightEvent)
    {
        if (streetLightEvent.carSections.Length == 0)
        {
            return;
        }

        foreach (var section in streetLightEvent.carSections)
        {
            foreach (var light in section.carStreetLights)
            {
                switch (streetLightEvent.state)
                {
                    case LightState.GREEN:
                        light.TurnGreen();

                        break;

                    case LightState.YELLOW:
                        light.TurnYellow();

                        break;

                    case LightState.RED:
                        light.TurnRed();

                        break;

                    case LightState.RED_AND_YELLOW:
                        light.TurnRedAndYellow();

                        break;

                    case LightState.LOOP_BACK:
                        break;
                }
            }
        }
    }


    private void ChangePedestrianLightsState(ref TrafficLightEvent streetLightEvent)
    {
        if (streetLightEvent.pedestrianSections.Length == 0)
        {
            return;
        }

        foreach (var section in streetLightEvent.pedestrianSections)
        {
            foreach (var light in section.pedestrianLights)
            {
                switch (streetLightEvent.state)
                {
                    case LightState.GREEN:
                        light.TurnGreen();

                        break;
                    case LightState.RED:
                        light.TurnRed();

                        break;

                    case LightState.BLINK_GREEN:
                        light.TurnBlink();

                        break;
                    case LightState.LOOP_BACK:
                        break;
                }
            }
        }
    }
}