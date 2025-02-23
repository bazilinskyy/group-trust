using SOSXR;
using UnityEngine;


public class Instantiator : MonoBehaviour
{
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private float m_delay = 1f;
    [DisableEditing] [SerializeField] private GameObject m_instantiatedObject;


    private void Start()
    {
        Invoke(nameof(InstantiateObject), m_delay);
    }


    private void InstantiateObject()
    {
        m_instantiatedObject = Instantiate(m_prefab, transform.position, transform.rotation);
    }
}