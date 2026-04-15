using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class HPVisualizer : MonoBehaviour
{
    [SerializeField, SerializedDictionary]
    private SerializedDictionary<MaterialKeys, Material> materials;

    public MeshRenderer meshRenderer;

    public void SetMaterial(int currentHealth, int maxHealth)
    {
        if ((float)currentHealth / maxHealth > 0.7)
        {
            meshRenderer.material = materials[MaterialKeys.highHP];
        }
        else if ((float)currentHealth / maxHealth > 0.3)
        {
            meshRenderer.material = materials[MaterialKeys.midHP];
        }
        else
        {
            meshRenderer.material = materials[MaterialKeys.lowHP];
        }
    }
}
