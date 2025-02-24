using UnityEngine;


public abstract class CustomBehaviour : MonoBehaviour
{
    public abstract void Init(AICar aiCar);


    public abstract void Trigger(CustomBehaviourData data);
}