using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DelayedColliderEnabler : MonoBehaviour
{
    [SerializeField] private float m_delay = 1.0f;
    [SerializeField] private List<Collider> m_colliders = new();


    private void OnValidate()
    {
        foreach (var coll in m_colliders.Where(coll => coll.enabled))
        {
            coll.enabled = false;
            Debug.LogFormat("Collider {0} needs to be disabled prior to starting game, otherwise bad things happen.", coll.name);
        }
    }


    private void Start()
    {
        Invoke(nameof(SetEnabled), m_delay);
    }


    private void SetEnabled()
    {
        foreach (var coll in m_colliders)
        {
            coll.enabled = true;
            Debug.LogFormat("Collider {0} enabled", coll.name);
        }
    }
}