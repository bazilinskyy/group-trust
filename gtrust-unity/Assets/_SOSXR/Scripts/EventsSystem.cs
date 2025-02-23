using System;
using UnityEngine;


public class EventsSystem : MonoBehaviour
{
    public static Action HaveArrived;
    public static Action NearlyThere;
    public static Action OnOurWay;
    public static Action RanRedLight;


    public void InvokeOnOurWay()
    {
        Debug.Log(nameof(InvokeOnOurWay));
        OnOurWay?.Invoke();
    }


    public void InvokeRanRedLight()
    {
        Debug.Log(nameof(InvokeRanRedLight));
        RanRedLight?.Invoke();
    }


    public void InvokeNearlyThere()
    {
        Debug.Log(nameof(InvokeNearlyThere));
        NearlyThere?.Invoke();
    }


    public void InvokeHaveArrived()
    {
        Debug.Log(nameof(InvokeHaveArrived));
        HaveArrived?.Invoke();
    }
}