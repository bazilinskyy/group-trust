using System.Collections.Generic;
using UnityEngine;


public class Colorer : MonoBehaviour
{
    [SerializeField] private Renderer m_renderer;
    [SerializeField] private List<Color> m_color = new();


    private void Awake()
    {
        GetRenderer();
    }


    private void GetRenderer()
    {
        if (m_renderer != null)
        {
            return;
        }

        m_renderer = GetComponentInChildren<Renderer>();
    }


    private void Start()
    {
        Colourize();
    }


    private void Colourize()
    {
        if (m_color == null || m_color.Count == 0 || m_renderer == null)
        {
            return;
        }

        m_renderer.material.color = m_color[Random.Range(0, m_color.Count)];
    }
}