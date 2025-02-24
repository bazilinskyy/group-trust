﻿using System.Collections;
using UnityEngine;


//broadcast visual syncing network message and displays red bar as a visual synchronization marker (used to syncing gameplay videos captured on different devices)
public class VisualSyncManager : MonoBehaviour
{
    private IEnumerator coroutine;


    public void DoHostGUI(Host host)
    {
        if (!NetworkingManager.Instance.hideGui && GUILayout.Button("Visual syncing"))
        {
            host.BroadcastMessage(new VisualSyncMessage());
            DisplayMarker();
        }
    }


    public void DisplayMarker()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = ShowMarkerCoroutine();
        StartCoroutine(coroutine);
    }


    private IEnumerator ShowMarkerCoroutine()
    {
        GetComponent<Renderer>().enabled = true;

        yield return new WaitForSeconds(1f);
        GetComponent<Renderer>().enabled = false;
    }
}