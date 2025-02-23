using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


[Serializable]
public struct Rating
{
    [Header("Don't edit these manually")]
    public long CurrentUnixTimeSeconds;
}


/// <summary>
///     The class handles setting a haptic reminder every X seconds, so that participants don't forget to provide a rating.
///     Then they press and hold a button (suggested is to use the trigger), to indicate trust / no trust
/// </summary>
public class EmperorsRating : MonoBehaviour
{
    [Header("Both hands: left first, then right")]
    [SerializeField] private List<XRBaseController> m_controllers;
    [SerializeField] private List<InputActionReference> m_buttonPressRefs;
    [SerializeField] private List<InputActionReference> m_buttonReleaseRefs;

    [Header("Reminder Haptics")]
    [Tooltip("Seconds between haptic reminders.")]
    [SerializeField] [Range(10, 120)] private int m_reminderHapticsInterval = 30;
    [Tooltip("Duration of the haptic reminder pulse.")]
    [SerializeField] [Range(0.1f, 5f)] private float m_reminderHapticsDuration = 2.5f;
    [Tooltip("Intensity of the haptic reminder pulse.")]
    [SerializeField] [Range(0.1f, 2f)] private float m_reminderHapticsIntensity = 0.5f;

    [Header("Active Haptics")]
    [Tooltip("Seconds between active haptic feedback.")]
    [SerializeField] [Range(0.1f, 1f)] private float m_activeHapticsInterval = 0.5f;
    [Tooltip("Duration of the active haptic pulse.")]
    [SerializeField] [Range(0.1f, 2.5f)] private float m_activeHapticsDuration = 0.15f;
    [Tooltip("Intensity of the active haptic pulse.")]
    [SerializeField] [Range(0.1f, 2f)] private float m_activeHapticsIntensity = 0.25f;

    [Header("Debug: make up random measurements instead of controller rotations")]
    [SerializeField] private bool m_debug = true;

    public Rating CurrentRating = new();

    private Coroutine _hapticsActiveCR;
    private Coroutine _hapticsReminderCR;
    private Coroutine _measureRotationCR;


    private void Awake()
    {
        CurrentRating = new Rating();
    }


    private void Start()
    {
        if (_hapticsReminderCR != null)
        {
            StopCoroutine(_hapticsReminderCR);
        }

        _hapticsReminderCR = StartCoroutine(HapticsCR(m_reminderHapticsInterval, m_reminderHapticsIntensity, m_reminderHapticsDuration));
    }


    private void OnEnable()
    {
        foreach (var buttonPressRef in m_buttonPressRefs)
        {
            buttonPressRef.action.performed += ButtonPressed;
            buttonPressRef.action.Enable();
        }

        foreach (var buttonReleaseRef in m_buttonReleaseRefs)
        {
            buttonReleaseRef.action.performed += ButtonReleased;
            buttonReleaseRef.action.Enable();
        }

        Haptic(); // Test
    }


    [ContextMenu(nameof(Haptic))]
    private void Haptic(float amplitude = 1, float duration = 1)
    {
        foreach (var controller in m_controllers)
        {
            controller.SendHapticImpulse(amplitude, duration);
        }
    }


    /// <summary>
    ///     Coroutine to handle repeated sending of haptic impulses at specified intervals.
    /// </summary>
    private IEnumerator HapticsCR(float interval, float amplitude, float duration)
    {
        for (;;)
        {
            yield return new WaitForSeconds(interval);
            Haptic(amplitude, duration);
        }
    }


    /// <summary>
    ///     Is fired when the button (trigger) is pressed down, starting the rotation measurement and the coroutine for the
    ///     'active haptics'.
    /// </summary>
    private void ButtonPressed(InputAction.CallbackContext context)
    {
        ButtonPressed();
    }


    [ContextMenu(nameof(ButtonPressed))]
    private void ButtonPressed()
    {
        Debug.Log("You pressed the button. Well done.");

        if (_measureRotationCR != null)
        {
            Debug.LogWarning("The measure CR wasn't null when we wanted to start a new measurement, this doesn't seem to be ok?");
            StopCoroutine(_measureRotationCR);
            _measureRotationCR = null;
        }

        _measureRotationCR = StartCoroutine(MeasureRotationCR());

        if (_hapticsActiveCR != null)
        {
            Debug.LogWarning("The active haptics coroutine wasn't null when we pressed the (trigger) button anew, this doesn't seem to be ok?");
            StopCoroutine(_hapticsActiveCR);
            _hapticsActiveCR = null;
        }

        _hapticsActiveCR = StartCoroutine(HapticsCR(m_activeHapticsInterval, m_activeHapticsIntensity, m_activeHapticsDuration));
    }


    /// <summary>
    ///     Coroutine to continuously measure the rotation of the controller.
    /// </summary>
    private IEnumerator MeasureRotationCR()
    {
        for (;;)
        {
            CurrentRating = new Rating();
            CurrentRating.CurrentUnixTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();

            yield return null;
        }
    }


    /// <summary>
    ///     Handles the button release event, stopping the rotation measurement and the 'active haptics'.
    /// </summary>
    private void ButtonReleased(InputAction.CallbackContext context)
    {
        ButtonReleased();
    }


    [ContextMenu(nameof(ButtonReleased))]
    private void ButtonReleased()
    {
        Debug.Log("Released the button");

        CurrentRating = new Rating();

        if (_hapticsActiveCR != null)
        {
            StopCoroutine(_hapticsActiveCR);
            _hapticsActiveCR = null;
        }
        else
        {
            Debug.LogWarning("The active haptics were already turned off when I released the button, that doesn't seem right.");
        }

        if (_measureRotationCR != null)
        {
            StopCoroutine(_measureRotationCR);
            _measureRotationCR = null;
        }
        else
        {
            Debug.LogWarning("The measurement coroutine had already been stopped prior to our call to stop it (on the release of the trigger button), this doesn't seem correct");
        }
    }


    /// <summary>
    ///     Disables the input actions and stops all coroutines when the object is disabled.
    /// </summary>
    private void OnDisable()
    {
        foreach (var buttonPressRef in m_buttonPressRefs)
        {
            buttonPressRef.action.performed -= ButtonPressed;
            buttonPressRef.action.Disable();
        }

        foreach (var buttonReleaseRef in m_buttonReleaseRefs)
        {
            buttonReleaseRef.action.performed -= ButtonReleased;
            buttonReleaseRef.action.Disable();
        }

        StopAllCoroutines();

        _hapticsActiveCR = null;
        _measureRotationCR = null;
        _hapticsReminderCR = null;
    }
}