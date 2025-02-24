using UnityEngine;


public class ShadowToggler : MonoBehaviour
{
    public Light[] SoftLights;
    public Light[] HardLights;


    private void Start()
    {
        if (gameObject.GetComponent<Camera>() == null)
        {
            Debug.Log("No Camera Found");
        }
    }


    private void OnPreRender()
    {
        foreach (var l in SoftLights)
        {
            l.shadows = LightShadows.None;
        }

        foreach (var l in HardLights)
        {
            l.shadows = LightShadows.None;
        }
    }


    private void OnPostRender()
    {
        foreach (var l in SoftLights)
        {
            l.shadows = LightShadows.Soft;
        }

        foreach (var l in HardLights)
        {
            l.shadows = LightShadows.Hard;
        }
    }
}