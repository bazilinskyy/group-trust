using UnityEngine;


[RequireComponent(typeof(CarTrafficLight))]
[ExecuteAlways]
public class GizmoDrawer_TrafficLight : MonoBehaviour
{
    [SerializeField] private float m_addedHeight = 5f;
    [SerializeField] private float m_radius = 0.35f;
    private CarTrafficLight _carTrafficLight;


    private void Awake()
    {
        _carTrafficLight = GetComponent<CarTrafficLight>();
    }


    private void OnDrawGizmos()
    {
        var newPosition = transform.position;
        newPosition.y += m_addedHeight;

        if (_carTrafficLight.State == LightState.GREEN)
        {
            Gizmos.color = Color.green;
        }
        else if (_carTrafficLight.State == LightState.RED)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.yellow;
        }

        Gizmos.DrawSphere(newPosition, m_radius);
    }
}