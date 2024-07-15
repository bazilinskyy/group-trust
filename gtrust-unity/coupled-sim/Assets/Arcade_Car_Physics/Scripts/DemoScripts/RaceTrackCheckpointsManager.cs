/*
 * This code is part of Arcade Car Physics for Unity by Saarg (2018)
 *
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */

using UnityEngine;
using VehicleBehaviour;


public class RaceTrackCheckpointsManager : MonoBehaviour
{
    [SerializeField] private CheckPointEvent[] checkPoints;

    [SerializeField] private Ghost _ghost;

    private int _lastCP = 0;
    private GhostRecorder _recorder;
    private float _startTime = -1.0f;


    private void StartRace(WheelVehicle vehicle)
    {
        _startTime = Time.realtimeSinceStartup;

        _lastCP = 1;

        Debug.Log("Race start!");

        if (vehicle != null)
        {
            _recorder = new GhostRecorder(60.0f, 10, ref vehicle);
            StartCoroutine(_recorder.RecordCoroutine());
        }
        else
        {
            _recorder = null;
        }

        if (_ghost != null)
        {
            _ghost.LoadData(vehicle.name);
            _ghost.RestartGhost();
        }
    }


    public void OnCheckPointEnter(CheckPointEvent cpEvent, Collider other)
    {
        if (_lastCP == checkPoints.Length - 1 && checkPoints[checkPoints.Length - 1] == cpEvent)
        {
            // This is the finish
            Debug.Log(Time.realtimeSinceStartup - _startTime);

            var vehicle = other.GetComponentInParent<WheelVehicle>();

            _recorder.Stop();
            _recorder.Save(vehicle.name);

            if (checkPoints[checkPoints.Length - 1] == checkPoints[0])
            {
                // If it's a loop start a new timer
                StartRace(vehicle);
            }
        }
        else if (checkPoints[0] == cpEvent && _lastCP != 1)
        {
            // This is the start
            StartRace(other.GetComponentInParent<WheelVehicle>());
        }
        else if (_lastCP < checkPoints.Length && checkPoints[_lastCP] == cpEvent)
        {
            // This is the next logical CP
            Debug.Log("CP: " + _lastCP);

            _lastCP++;
        }
    }
}