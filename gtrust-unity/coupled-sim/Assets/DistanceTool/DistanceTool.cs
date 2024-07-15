/*
 * Created by Wes McDermott - 2011 - the3dninja.com/blog
 */

using UnityEngine;


[ExecuteInEditMode]
public class DistanceTool : MonoBehaviour
{
    public string distanceToolName = "";
    public Color lineColor = Color.yellow;
    public bool initialized = false;
    public string initialName = "Distance Tool";
    public Vector3 startPoint = Vector3.zero;
    public Vector3 endPoint = new(0, 1, 0);
    public float distance;
    public float gizmoRadius = 0.1f;
    public bool scaleToPixels = false;
    public int pixelPerUnit = 128;


    private void OnEnable()
    {
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = lineColor;
        Gizmos.DrawWireSphere(startPoint, gizmoRadius);
        Gizmos.DrawWireSphere(endPoint, gizmoRadius);
        Gizmos.DrawLine(startPoint, endPoint);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = lineColor;
        Gizmos.DrawWireSphere(startPoint, gizmoRadius);
        Gizmos.DrawWireSphere(endPoint, gizmoRadius);
        Gizmos.DrawLine(startPoint, endPoint);
    }
}