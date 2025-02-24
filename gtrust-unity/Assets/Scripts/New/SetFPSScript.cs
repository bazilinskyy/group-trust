﻿using UnityEngine;
using UnityEngine.UI;


// This script is partially copied from http://wiki.unity3d.com/index.php/FramesPerSecond && https://answers.unity.com/questions/46745/how-do-i-find-the-frames-per-second-of-my-game.html
// It is used to compute the amount of Frames that are drawn per second in Unity.
public class SetFPSScript : MonoBehaviour
{
    public Text fps;
    public float m_refreshTime = 0.5f;
    private int m_frameCounter = 0;
    private float m_lastFramerate = 0.0f;
    private float m_timeCounter = 0.0f;


    // Update is called once per frame
    private void Update()
    {
        if (m_timeCounter < m_refreshTime)
        {
            m_timeCounter += Time.deltaTime;
            m_frameCounter++;
        }
        else
        {
            //This code will break if you set your m_refreshTime to 0, which makes no sense.
            m_lastFramerate = m_frameCounter / m_timeCounter;
            m_frameCounter = 0;
            m_timeCounter = 0.0f;
        }

        fps.enabled = !NetworkingManager.Instance.hideGui;
        SetFPS();
    }


    private void SetFPS()
    {
        fps.text = "FPS: " + Mathf.Round(m_lastFramerate);
        
    }
}