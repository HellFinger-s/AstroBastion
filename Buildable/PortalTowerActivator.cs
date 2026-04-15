using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTowerActivator : MonoBehaviour
{
    public PortalTower portal;

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError("Djsdjvjdsnvjkdsv");
        if (other.TryGetComponent<PlayerControl>(out PlayerControl playerControl))
        {
            portal.StartTeleport(playerControl);
        }
    }
}
