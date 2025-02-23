using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using static Varjo.XR.VarjoEyeTracking;
using CommonUsages = UnityEngine.XR.CommonUsages;
using InputDevice = UnityEngine.XR.InputDevice;


public enum GazeDataSource
{
    InputSubsystem,
    GazeAPI
}


public class EyeTracking : MonoBehaviour
{
    [Header("KeyCodes")]
    [SerializeField] private KeyCode m_calibrationRequestKey = KeyCode.Keypad1;
    [SerializeField] private KeyCode m_toggleGazeTarget = KeyCode.Keypad2;
    [SerializeField] private KeyCode m_toggleFixationPointKey = KeyCode.Keypad3;
    [SerializeField] private KeyCode m_setOutputFilterTypeKey = KeyCode.Keypad4;
    [Tooltip("Will poll Varjo functions: IsGazeAllowed() and IsGazeCalibrated() simultaneously into one neat package")]
    [SerializeField] private KeyCode m_canWeUseGazeKey = KeyCode.Keypad5;

    [Header("Settings")]
    [SerializeField] private GazeDataSource m_gazeDataSource = GazeDataSource.InputSubsystem;
    [SerializeField] private GazeCalibrationMode m_gazeCalibrationMode = GazeCalibrationMode.OneDot;
    [SerializeField] private GazeOutputFilterType m_gazeOutputFilterType = GazeOutputFilterType.Standard;
    [SerializeField] private GazeOutputFrequency m_gazeOutputFrequency = GazeOutputFrequency.MaximumSupported;
    [SerializeField] private Camera m_XRCamera;

    [Header("Fixation point")]
    [SerializeField] private Transform m_fixationPointTransform;

    [Header("Gaze target")]
    [Tooltip("Gaze point indicator")]
    [SerializeField] private GameObject m_gazeTarget;
    [SerializeField] private MeshRenderer m_gazeRenderer;
    [Tooltip("Gaze ray radius")]
    [SerializeField] private float m_gazeTargetRadius = 0.01f;
    [Tooltip("Gaze point distance if not hit anything")]
    [SerializeField] private float m_noHitGazeTargetDistance = 15f;
    [Tooltip("Gaze target offset towards viewer")]
    [SerializeField] private float m_targetOffset = 0.2f;

    private readonly List<InputDevice> _devices = new();
    private Vector3 _direction;
    private float _distance;
    private Eyes _eyes;
    private Vector3 _fixationPoint;
    private GazeData _gazeData;
    private RaycastHit _hit;
    private InputDevice _inputDevice;
    private Vector3 _leftEyeTrackingPosition;
    private Quaternion _leftEyeTrackingRotation;
    private Vector3 _rayOrigin;
    private Vector3 _rightEyeTrackingPosition;
    private Quaternion _rightEyeTrackingRotation;

    // SOSXR: Added FocusName property
    public string FocusName { get; private set; }


    private void Start()
    {
        SetGazeOutputFrequency(m_gazeOutputFrequency);
    }


    private void OnEnable()
    {
        GetDevice();
    }


    private void GetDevice()
    {
        if (_inputDevice.isValid)
        {
            return;
        }

        InputDevices.GetDevicesAtXRNode(XRNode.CenterEye, _devices);
        _inputDevice = _devices.FirstOrDefault();
    }


    private void Update()
    {
        if (Input.GetKeyDown(m_toggleFixationPointKey))
        {
            m_fixationPointTransform.gameObject.SetActive(!m_fixationPointTransform.gameObject.activeInHierarchy);

            Debug.Log("SOSXR: Fixation point is now: " + (m_fixationPointTransform.gameObject.activeInHierarchy ? "visible" : "hidden"));
        }

        if (Input.GetKeyDown(m_calibrationRequestKey))
        {
            RequestGazeCalibration(m_gazeCalibrationMode);

            Debug.Log("SOSXR: Gaze calibration requested");
        }

        if (Input.GetKeyDown(m_setOutputFilterTypeKey))
        {
            SetGazeOutputFilterType(m_gazeOutputFilterType);

            Debug.Log("SOSXR: Gaze output filter type is now: " + GetGazeOutputFilterType());
        }

        if (Input.GetKeyDown(m_canWeUseGazeKey))
        {
            Debug.Log("SOSXR: Can we use gaze? " + CanWeUseGaze());
        }

        if (Input.GetKeyDown(m_toggleGazeTarget))
        {
            m_gazeTarget.SetActive(!m_gazeTarget.activeInHierarchy);

            Debug.Log("SOSXR: Gaze target is now: " + (m_gazeTarget.activeInHierarchy ? "visible" : "hidden"));
        }

        FocusName = _hit.collider != null ? _hit.collider.name : "NULL"; // With _hit.transform.name you'd get the info of the RigidBody, where we want info on the Collider. 

        // Debug.LogFormat("We hit {0}", FocusName);
    }


    private void LateUpdate()
    {
        GetEyeData();

        SphereCast();
    }


    private bool CanWeUseGaze()
    {
        return IsGazeAllowed() && IsGazeCalibrated();
    }


    /// <summary>
    ///     Get gaze data if gaze is allowed and calibrated
    /// </summary>
    private void GetEyeData()
    {
        if (!CanWeUseGaze())
        {
            return;
        }

        GetDevice();

        // m_gazeTarget.SetActive(true);

        if (m_gazeDataSource == GazeDataSource.InputSubsystem)
        {
            GetGazeDataFromInputSubsystem();
        }
        else if (m_gazeDataSource == GazeDataSource.GazeAPI)
        {
            GetGazeDataFromGazeApi();
        }
    }


    private void GetGazeDataFromInputSubsystem()
    {
        if (!_inputDevice.TryGetFeatureValue(CommonUsages.eyesData, out _eyes))
        {
            return;
        }

        if (_eyes.TryGetFixationPoint(out _fixationPoint) && m_fixationPointTransform != null)
        {
            m_fixationPointTransform.localPosition = _fixationPoint;
        }

        _rayOrigin = m_XRCamera.transform.position;

        _direction = (m_fixationPointTransform.position - m_XRCamera.transform.position).normalized;
    }


    private void GetGazeDataFromGazeApi()
    {
        _gazeData = GetGaze();

        if (_gazeData.status == GazeStatus.Invalid)
        {
            Debug.LogWarning("GazeStatus is Invalid");

            return;
        }

        _rayOrigin = m_XRCamera.transform.TransformPoint(_gazeData.gaze.origin); // Set gaze origin as raycast origin

        _direction = m_XRCamera.transform.TransformDirection(_gazeData.gaze.forward); // Set gaze direction as raycast direction

        m_fixationPointTransform.position = _rayOrigin + _direction * _gazeData.focusDistance; // Fixation point can be calculated using ray origin, direction and focus distance
    }


    private void SphereCast()
    {
        if (Physics.SphereCast(_rayOrigin, m_gazeTargetRadius, _direction, out _hit)) // Raycast to world from XR Camera position towards fixation point
        {
            m_gazeTarget.transform.position = _hit.point - _direction * m_targetOffset; // Put target on gaze raycast position with offset towards user

            m_gazeTarget.transform.LookAt(_rayOrigin, Vector3.up); // Make gaze target point towards user

            _distance = _hit.distance; // Scale gaze-target with distance so it appears to be always same size

            m_gazeTarget.transform.localScale = Vector3.one * _distance;
        }
        else // If gaze ray didn't hit anything, the gaze target is shown at fixed distance
        {
            m_gazeTarget.transform.position = _rayOrigin + _direction * m_noHitGazeTargetDistance;
            m_gazeTarget.transform.LookAt(_rayOrigin, Vector3.up);
            m_gazeTarget.transform.localScale = Vector3.one * m_noHitGazeTargetDistance;
        }
    }
}