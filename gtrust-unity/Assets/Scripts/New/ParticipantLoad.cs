﻿using UnityEngine;


// This script ensures that the avatar connecting MVN Analyze and Unity doesn't get destroyed when loading a new scene.
public class ParticipantLoad : MonoBehaviour
{
    private static bool created = false;


    private void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(gameObject);
            created = true;
        }
    }
}