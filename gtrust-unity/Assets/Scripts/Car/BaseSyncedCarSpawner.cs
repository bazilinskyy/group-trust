﻿using System.Collections;
using UnityEngine;


public class BaseSyncedCarSpawner : CarSpawnerBase
{
    protected override IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSeconds(0);
        var aICar = Spawn(spawnParams, false);
        aICar.name = name + " Instance";

        foreach (var waypoint in spawnParams.Track.Waypoints)
        {
            var speedSettings = waypoint.GetComponent<SpeedSettings>();

            if (speedSettings != null)
            {
                speedSettings.targetAICar = aICar;
            }
        }

        foreach (var cb in spawnParams.customBehaviours)
        {
            cb.Init(aICar);
            aICar.CustomBehaviours.Add(cb);
        }
    }
}