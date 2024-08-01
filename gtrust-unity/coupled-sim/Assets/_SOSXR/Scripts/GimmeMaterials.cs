using System.Collections.Generic;
using UnityEngine;


public class GimmeMaterials : MonoBehaviour
{
    [SerializeField] private Renderer[] m_childRenderers; // array

    [SerializeField] private List<Material> m_materials = new();

    [SerializeField] private List<string> m_materialNames = new();

    [SerializeField] private Material Buildings_DS_Atlas;
    [SerializeField] private Material Misc_DS_Atlas;


    private void Reset()
    {
        m_childRenderers = GetComponentsInChildren<Renderer>();
    }


    [ContextMenu(nameof(GetUniqueMaterials))]
    private void GetUniqueMaterials()
    {
        m_materials.Clear();

        foreach (var rend in m_childRenderers)
        {
            foreach (var mat in rend.sharedMaterials)
            {
                if (!m_materials.Contains(mat))
                {
                    m_materials.Add(mat);
                }

                if (!m_materialNames.Contains(mat.name))
                {
                    m_materialNames.Add(mat.name);
                }
            }
        }
    }
}