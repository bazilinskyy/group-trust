﻿using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;


public class RecenterXROrigin : MonoBehaviour
{
    [SerializeField] private Transform m_recenterTo;
    [Tooltip("If no RecenterTo Transform has been set, it will search for this Tag")]
    [SerializeField] [TagSelector] private string m_recenterToTag = "Target_XROrigin";
    [SerializeField] private KeyCode m_recenterKey = KeyCode.Keypad0;
    private Transform _xrCamera;
    private XROrigin _xrOrigin;


    private void Awake()
    {
        GetRequiredComponents();
    }


    private void GetRequiredComponents()
    {
        _xrOrigin = GetComponent<XROrigin>();

        if (_xrOrigin == null)
        {
            return;
        }

        _xrCamera = _xrOrigin.GetComponentInChildren<Camera>().transform;
    }


    [ContextMenu(nameof(RecenterAndFlatten))]
    public void RecenterAndFlatten()
    {
        FindObjectWithTag();
        RecenterPosition(true);
        RecenterRotation();

        Debug.LogFormat("SOSXR: We just ran {0}", nameof(RecenterAndFlatten));
    }


    [ContextMenu(nameof(RecenterWithoutFlatten))]
    public void RecenterWithoutFlatten()
    {
        FindObjectWithTag();
        RecenterPosition(false);
        RecenterRotation();

        Debug.LogFormat("SOSXR: We just ran {0}", nameof(RecenterWithoutFlatten));
    }


    private void FindObjectWithTag()
    {
        if (m_recenterTo != null)
        {
            return;
        }

        m_recenterTo = transform.root.FindChildByTag(m_recenterToTag); // Go to the root GameObject, then search back downwards until you find something with this tag.

        if (m_recenterTo == null)
        {
            Debug.LogWarningFormat("SOSXR: We don't have anything in our scene with the tag {0}, are you sure that it is defined?", m_recenterToTag);
        }
    }


    private void RecenterPosition(bool flatten)
    {
        var distanceDiff = m_recenterTo.transform.position - _xrCamera.position;
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
        var rotationAngleY = m_recenterTo.transform.rotation.eulerAngles.y - _xrCamera.transform.rotation.eulerAngles.y;

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