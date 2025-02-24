using UnityEngine;
using VehicleBehaviour;


public class WheelRotator : MonoBehaviour
{
    public Transform FrontLeft;
    public Transform FrontRight;
    public Transform RearLeft;
    public Transform RearRight;

    public float WheelDiameter = 0.65f;

    private Suspension FrontLeftSuspension;
    private Suspension FrontRightSuspension;
    private Suspension RearLeftSuspension;
    private Suspension RearRightSuspension;

    private float RotationSpeed;
    private float speed = 0;


    private void Start()
    {
        FrontLeft.GetComponent<WheelCollider>().wheelDampingRate = 1000;
        FrontRight.GetComponent<WheelCollider>().wheelDampingRate = 1000;
        RearLeft.GetComponent<WheelCollider>().wheelDampingRate = 1000;
        RearRight.GetComponent<WheelCollider>().wheelDampingRate = 1000;

        FrontLeftSuspension = FrontLeft.GetComponent<Suspension>();
        FrontRightSuspension = FrontRight.GetComponent<Suspension>();
        RearLeftSuspension = RearLeft.GetComponent<Suspension>();
        RearRightSuspension = RearRight.GetComponent<Suspension>();

        FrontLeftSuspension.enabled = false;
        FrontRightSuspension.enabled = false;
        RearLeftSuspension.enabled = false;
        RearRightSuspension.enabled = false;
    }


    private void Update()
    {
        speed = Mathf.Clamp(GetComponent<Rigidbody>().velocity.magnitude, -30, 30); // Not Needed but might be useful, if rotation looks weird because of framerate

        if (speed < 0.05f && speed < 0.05f)
        {
            speed = 0f;
        }

        RotationSpeed = 360f * speed / 3.6f / Mathf.PI / WheelDiameter;

        //Front Left
        FrontLeftSuspension.wheelModel.transform.Rotate(RotationSpeed * Time.deltaTime, 0, 0);
        //Front Right
        FrontRightSuspension.wheelModel.transform.Rotate(RotationSpeed * Time.deltaTime, 0, 0);
        //Rear Left
        RearLeftSuspension.wheelModel.transform.Rotate(RotationSpeed * Time.deltaTime, 0, 0);
        //Rear Right
        RearRightSuspension.wheelModel.transform.Rotate(RotationSpeed * Time.deltaTime, 0, 0);
    }
}