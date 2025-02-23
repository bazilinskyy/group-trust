using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;


public class RecenterChildXROrigin : MonoBehaviour
{
    [SerializeField] private Transform m_recenterTo;
    [SerializeField] private KeyCode m_recenterKey = KeyCode.Keypad0;
    private Transform _xrCamera;
    private XROrigin _xrOrigin;


    private void GetRequiredComponents()
    {
        if (_xrOrigin != null)
        {
            return;
        }

        _xrOrigin = GetComponentInChildren<XROrigin>();

        if (_xrOrigin == null)
        {
            return;
        }

        _xrCamera = _xrOrigin.GetComponentInChildren<Camera>().transform;
    }


    [ContextMenu(nameof(RecenterAndFlatten))]
    public void RecenterAndFlatten()
    {
        Recenter(true);
    }


    [ContextMenu(nameof(RecenterWithoutFlatten))]
    public void RecenterWithoutFlatten()
    {
        Recenter(false);
    }


    private void Recenter(bool flatten)
    {
        GetRequiredComponents();

        if (m_recenterTo == null || _xrCamera == null)
        {
            Debug.LogWarning("SOSXR: We can't recenter the position, because either the RecenterTo object or the _xrCamera is null");

            return;
        }

        var distanceDiff = m_recenterTo.transform.position - _xrCamera.position;
        _xrOrigin.transform.position += distanceDiff;

        if (flatten && _xrOrigin.CurrentTrackingOriginMode != TrackingOriginModeFlags.Floor) // SOSXR: You want to flatten the _xrCamera on the _xrRig, but if the CurrenTrackingOrigin-mode on the aforementioned Rig is set to 'floor', which doesn't allow setting the Y component, you cannot flatten it.
        {
            _xrOrigin.transform.position = _xrOrigin.transform.position.Flatten();
        }

        var rotationAngleY = m_recenterTo.transform.rotation.eulerAngles.y - _xrCamera.transform.rotation.eulerAngles.y;
        _xrOrigin.transform.Rotate(0, rotationAngleY, 0);

        Debug.Log("SOSXR: We just ran RecenterCenter");
    }


    private void Update()
    {
        if (Input.GetKeyDown(m_recenterKey))
        {
            RecenterWithoutFlatten();
        }
    }
}