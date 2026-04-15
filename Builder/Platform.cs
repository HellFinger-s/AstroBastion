using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class Platform : MonoBehaviour
{
    public bool isOccupied = false;
    public Transform towerBuildPlace;
    public Transform cubeBuildPlace;

    [SerializedDictionary, SerializeField]
    private SerializedDictionary<MaterialKeys, Material> materials;

    public Cube parentCube;
    public BaseTower connectedTower;
    public Cube connectedCube;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }


    public bool isFree()
    {
        return connectedCube == null && connectedTower == null;
    }


    public void SetMaterial(MaterialKeys key)
    {
        if (isFree() || key == MaterialKeys.normal)
        {
            meshRenderer.material = materials[key];
            if (key == MaterialKeys.normal)
            {
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BaseEnemy> (out BaseEnemy enemy))
        {
            parentCube.Destroy();
        }
    }
}
