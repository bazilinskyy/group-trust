using UnityEngine;
using UnityStandardAssets.Utility;


public class AIPedestrian : MonoBehaviour
{
    public float moveSpeed = 1.6f;
    public float animationBlendFactor = 1f;
    public float HeightDampingFactor = 0.05f;
    public float SpeedDampingFactor = 0.05f;
    public Animator animator;
    private WaypointProgressTracker _tracker;
    private float currentBlendFactor = 1f;


    public void Init(WaypointCircuit circuit)
    {
        enabled = true;
        _tracker = GetComponent<WaypointProgressTracker>();
        _tracker.enabled = true;
        _tracker.Init(circuit);

        foreach (var waypoint in circuit.Waypoints)
        {
            var speedSettings = waypoint.GetComponent<PedestrianWaypoint>();

            if (speedSettings != null)
            {
                speedSettings.target = this;
            }
        }

        currentBlendFactor = animationBlendFactor;
//        animator.SetBool("Walking", moveSpeed > 0.01f);
    }


    // Smoothing rate dictates the proportion of source remaining after one second
    public static float Damp(float source, float target, float smoothing, float dt)
    {
        return Mathf.Lerp(source, target, 1 - Mathf.Pow(smoothing, dt));
    }


    private void Update()
    {
        currentBlendFactor = Damp(currentBlendFactor, animationBlendFactor, SpeedDampingFactor, Time.deltaTime);
        animator.SetFloat("Speed", currentBlendFactor);
        var steer = Quaternion.LookRotation(_tracker.target.position - transform.position, Vector3.up).eulerAngles;
        var rot = transform.eulerAngles;
        rot.y = steer.y;
        transform.eulerAngles = rot;
        var pos = transform.position;
        pos += transform.forward * moveSpeed * Time.deltaTime;

        if (Physics.Raycast(pos + Vector3.up, Vector3.down, out var hitInfo, 2))
        {
            pos.y = Damp(pos.y, hitInfo.point.y, HeightDampingFactor, Time.deltaTime);
        }

        transform.position = pos;
    }


    private void OnTriggerEnter(Collider other)
    {
        var speedSettings = other.GetComponent<PedestrianWaypoint>();

        if (speedSettings == null || (speedSettings.target != null && speedSettings.target != this))
        {
            return;
        }

        moveSpeed = speedSettings.targetSpeed;
        animationBlendFactor = speedSettings.targetBlendFactor;
    }
}