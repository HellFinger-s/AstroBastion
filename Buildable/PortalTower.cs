using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTower : BaseTower
{
    [Header("PortalTower")]
    [HideInInspector]
    public PortalTowersDispatcher dispatcher;
    public float teleportThreshold = 5f;
    public float basicTimeout = 12f;
    public int basicHealthPayment = 20;
    public Transform playerTeleportPlace;
    [HideInInspector]
    public PortalTower connectedPortal;
    public GameObject teleportActivator;

    private PlayerControl teleportingPlayer;
    private GameObject connectionVisualizer;

    private float reloadTimer = 0f;
    private bool isReloading = false;

    private void Update()
    {
        if (isReloading)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer > CalculateTimeout())
            {
                isReloading = false;
                teleportActivator.SetActive(true);
                reloadTimer = 0f;
            }
        }
    }

    public override void Built()
    {
        base.Built();
        PortalTower freeTower = dispatcher.GetFreePortal();
        teleportActivator.SetActive(true);
        if (freeTower is not null)
        {
            connectedPortal = freeTower;
            connectedPortal.SetPair(this);
            dispatcher.AddPare(freeTower, this);
            dispatcher.SetFreePortal(null);
        }
        else
        {
            dispatcher.SetFreePortal(this);
        }
    }

    public void StartTeleport(PlayerControl player)
    {
        if (connectedPortal is not null && connectedPortal.IsReady())
        {
            teleportingPlayer = player;
            Teleport();
        }
    }

    private void Teleport()
    {
        teleportingPlayer.PlayWarpParticles();
        teleportingPlayer.controlLocked = true;
        teleportingPlayer.FlushInput();
        connectedPortal.SetTeleportingPlayer(teleportingPlayer);
        teleportingPlayer.ReceiveDamage(CalculateHealthPayment());
        teleportingPlayer.transform.position = connectedPortal.playerTeleportPlace.position;
        //teleportingPlayer.transform.rotation = connectedPortal.playerTeleportPlace.rotation;
        teleportingPlayer.controlLocked = false;
        Reload();
        connectedPortal.Reload();
    }

    public void SetPair(PortalTower tower)
    {
        connectedPortal = tower;
    }

    public void RemovePair()
    {
        connectedPortal = null;
        connectionVisualizer = null;
    }

    public void FindNewPair()
    {
        PortalTower freeTower = dispatcher.GetFreePortal();
        if (freeTower is not null)
        {
            connectedPortal = freeTower;
            connectedPortal.SetPair(this);
            dispatcher.AddPare(freeTower, this);
            dispatcher.SetFreePortal(null);
        }
        else
        {
            dispatcher.SetFreePortal(this);
        }
    }

    public void SetTeleportingPlayer(PlayerControl player)
    {
        teleportingPlayer = player;
    }

    public override void Destroy()
    {
        base.Destroy();
        if (connectionVisualizer is not null)
        {
            dispatcher.ReleaseVisualizer(connectionVisualizer);
        }
        connectionVisualizer = null;
        connectedPortal.RemovePair();
        connectedPortal.FindNewPair();
        StopAllCoroutines();
        teleportingPlayer.controlLocked = false;
        teleportingPlayer = null;
    }

    private int CalculateHealthPayment()
    {
        return basicHealthPayment - 5 * rightUpgradeLevel;
    }

    private float CalculateTimeout()
    {
        return basicTimeout - 2 * leftUpgradeLevel;
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleOnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleOnTriggerExit(other);
    }

    public void SetConnectionVisualizer(GameObject visualizer)
    {
        connectionVisualizer = visualizer;
    }
    public GameObject GetConnectionVisualizer()
    {
        return connectionVisualizer;
    }

    public override Dictionary<string, string> GetSubstitutions(IButtonTextTemplate template)
    {
        Dictionary<string, string> substitutions = new Dictionary<string, string> { };

        if (template is BuildableTextTemplate)
        {
            substitutions.Add(TextMarks.PortalPayment.ToString(), basicHealthPayment.ToString());
            substitutions.Add(TextMarks.Timeout.ToString(), string.Format("0:F0",CalculateTimeout()));
            substitutions.Add(TextMarks.HP.ToString(), maxHealth.ToString());
        }

        return substitutions;
    }

    public bool IsReady()
    {
        return !isReloading;
    }

    public void Reload()
    {
        isReloading = true;
        teleportActivator.SetActive(false);
    }
}
