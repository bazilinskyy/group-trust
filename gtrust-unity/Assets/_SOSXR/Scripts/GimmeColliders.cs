using UnityEngine;


public class GimmeColliders : MonoBehaviour
{
    [SerializeField] private Collider[] m_childColliders;
    [SerializeField] private MeshCollider[] m_meshColliders;
    [SerializeField] private BoxCollider[] m_boxColliders;


    private void Reset()
    {
        m_childColliders = GetComponentsInChildren<Collider>();
        m_meshColliders = GetComponentsInChildren<MeshCollider>();
        m_boxColliders = GetComponentsInChildren<BoxCollider>();
    }


    [ContextMenu(nameof(TurnOffColliders))]
    private void TurnOffColliders()
    {
        foreach (var coll in m_childColliders)
        {
            coll.enabled = false;
        }
    }


    [ContextMenu(nameof(TurnOnColliders))]
    private void TurnOnColliders()
    {
        foreach (var coll in m_childColliders)
        {
            coll.enabled = true;
        }
    }


    [ContextMenu(nameof(MakeMeshCollidersConvex))]
    private void MakeMeshCollidersConvex()
    {
        foreach (var coll in m_meshColliders)
        {
            coll.convex = true;
        }
    }
}