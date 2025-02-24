﻿using System;
using UnityEngine;


[Serializable]
public struct EnviromentalLightData
{
    public Color skyColor;
    public Color equatorColor;
    public Color groundColor;
    public Color directionalColor;
    public Vector3 rotation;
    public float intensity;
}


//helper script that allows switching between two (day/night) lightning/environment settings
//scene lightmaps have to be rebaked manually after such a switch
public class DayNightControl : MonoBehaviour
{
    public GameObject[] lamps;
    public Light directionalLight;

    public EnviromentalLightData dayLight;
    public EnviromentalLightData nightLight;

    public Material daySkybox;
    public Material nightSkybox;

    [Header("SOSXR")]
    [SerializeField] private bool m_changeSkybox = false;


    public void InitNight()
    {
        ChangeLight(nightLight);
        ChangeSkybox(nightSkybox);
        ToggleLights(true);
    }


    public void InitDay()
    {
        ChangeLight(dayLight);
        ChangeSkybox(daySkybox);
        ToggleLights(false);
    }


    private void ToggleLights(bool toggle)
    {
        foreach (var lamp in lamps)
        {
            lamp.GetComponentInChildren<Light>().enabled = toggle;
        }
    }


    private void ChangeLight(EnviromentalLightData data)
    {
        RenderSettings.ambientEquatorColor = data.equatorColor;
        RenderSettings.ambientSkyColor = data.skyColor;
        RenderSettings.ambientGroundColor = data.groundColor;
        directionalLight.color = data.directionalColor;
        directionalLight.transform.rotation = Quaternion.Euler(data.rotation);
        directionalLight.intensity = data.intensity;
    }


    private void ChangeSkybox(Material data)
    {
        if (m_changeSkybox)
        {
            RenderSettings.skybox = data;
        }
    }
}