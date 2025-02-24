/*
 * This code is part of Arcade Car Physics for Unity by Saarg (2018)
 *
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */

using UnityEngine;


namespace VehicleBehaviour
{
    [RequireComponent(typeof(WheelVehicle))]
    [RequireComponent(typeof(AudioSource))]
    public class EngineSoundManager : MonoBehaviour
    {
        [Header("AudioClips")]
        public AudioClip starting;
        public AudioClip rolling;
        public AudioClip stopping;

        [Header("pitch parameter")]
        public float flatoutSpeed = 20.0f;
        [Range(0.0f, 3.0f)]
        public float minPitch = 0.7f;
        [Range(0.0f, 0.1f)]
        public float pitchSpeed = 0.05f;
        [Range(0.0f, 1f)]
        public float volume = 1;

        [SerializeField] private AudioSource m_source;
        private IVehicle _vehicle;


        private void Start()
        {
            if (m_source == null)
            {
                m_source = GetComponent<AudioSource>();
            }

            var aiCar = GetComponent<AICar>();

            if (aiCar.isActiveAndEnabled)
            {
                _vehicle = aiCar;
            }
            else
            {
                _vehicle = GetComponent<WheelVehicle>();
            }
        }


        private void Update()
        {
            if (_vehicle.Handbrake && m_source.clip == rolling)
            {
                m_source.volume = volume;
                m_source.clip = stopping;
                m_source.loop = false;
                m_source.Play();
            }

            if (!_vehicle.Handbrake && (m_source.clip == stopping || m_source.clip == null))
            {
                m_source.volume = volume;
                m_source.clip = starting;
                m_source.Play();
                m_source.loop = false;
                m_source.pitch = 1;
            }

            if (!_vehicle.Handbrake && !m_source.isPlaying)
            {
                Debug.LogFormat("SOSXR: Play that funky music white boy, play that funky music right. Handbrake: {0}, IsPlaying: {1}", _vehicle.Handbrake, m_source.isPlaying);
                
                m_source.volume = volume;
                m_source.clip = rolling;
                m_source.loop = true;
                m_source.Play();

            }

            if (m_source.clip == rolling)
            {
                m_source.pitch = Mathf.Lerp(m_source.pitch, minPitch + Mathf.Abs(_vehicle.Speed) / flatoutSpeed, pitchSpeed);
            }
        }
    }
}