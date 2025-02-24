using UnityEngine;


public class DisableTracking : MonoBehaviour
{
    public Camera cam;
    private Vector3 startPos;


    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
        startPos = transform.localPosition;
    }


    private void Update()
    {
        transform.localPosition = startPos - cam.transform.localPosition;
        transform.localRotation = Quaternion.Inverse(cam.transform.localRotation);
    }
}