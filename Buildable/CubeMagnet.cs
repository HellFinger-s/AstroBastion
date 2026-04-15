using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMagnet : MonoBehaviour
{
    public Platform platform;

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Cube>(out Cube cube))
        {
            platform.connectedCube = cube;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Cube>(out Cube cube))
        {
            platform.connectedCube = null;
        }
    }
}
