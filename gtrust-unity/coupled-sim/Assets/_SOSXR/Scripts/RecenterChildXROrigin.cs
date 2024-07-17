using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;


public class RecenterChildXROrigin : MonoBehaviour
{
    [SerializeField] private Transform m_recenterTo;
    [SerializeField] private KeyCode m_recenterKey = KeyCode.Keypad0;
    private Transform _xrCamera;
    private XROrigin _xrOrigin;


    private void Awake()
    {
        GetRequiredComponents();
    }


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
        GetRequiredComponents();
        RecenterPosition(true);
        RecenterRotation();

        Debug.LogFormat("SOSXR: We just ran {0}", nameof(RecenterAndFlatten));
    }


    [ContextMenu(nameof(RecenterWithoutFlatten))]
    public void RecenterWithoutFlatten()
    {
        GetRequiredComponents();
        RecenterPosition(false);
        RecenterRotation();

        Debug.LogFormat("SOSXR: We just ran {0}", nameof(RecenterWithoutFlatten));
    }


    

    private void RecenterPosition(bool flatten)
    {
        var distanceDiff = m_recenterTo.position - _xrCamera.position;
        _xrOrigin.transform.position += distanceDiff;

        if (flatten && _xrOrigin.CurrentTrackingOriginMode == TrackingOriginModeFlags.Floor)
        {
            Debug.LogWarning("SOSXR: You want to flatten the _xrCamera on the _xrRig, but the CurrenTrackingOrigin-mode on the aforementioned Rig is set to 'floor', which doesn't allow setting the Y component");

            return;
        }

        if (flatten)
        {
            _xrOrigin.transform.position = _xrOrigin.transform.position.Flatten();
        }
    }


    private void RecenterRotation()
    {
        var rotationAngleY = m_recenterTo.rotation.eulerAngles.y - _xrCamera.transform.rotation.eulerAngles.y;

        _xrOrigin.transform.Rotate(0, rotationAngleY, 0);
    }


    private void Update()
    {
        if (Input.GetKeyDown(m_recenterKey))
        {
            RecenterWithoutFlatten();
            Debug.Log("SOSXR: RecenterWithoutFlatten via key");
        }
    }
}