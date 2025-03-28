﻿using UnityEngine;
using UnityEngine.XR;


public class CameraCounter : MonoBehaviour
{
    [SerializeField] private KeyCode m_recenterKey = KeyCode.Alpha0;
    [SerializeField] private bool m_allowRecentering = false;

    private Transform _childCamera;
    private XRInputSubsystem _xrInputSubsystem;


    private void Awake()
    {
        _xrInputSubsystem = new XRInputSubsystem();
        
    }


    private void Update()
    {
        if (_childCamera == null && transform.GetChild(0) != null)
        {
            _childCamera = transform.GetChild(0);
        }
        
        if (!m_allowRecentering)
        {
            return;
        }

        if (Input.GetKeyDown(m_recenterKey))
        {
            _xrInputSubsystem.TryRecenter();
        }
    }


    private void LateUpdate()
    {
        if (_childCamera == null)
        {
            return;
        }
        
        var invertedPosition = -_childCamera.localPosition;
        transform.localPosition = invertedPosition;
    }
}