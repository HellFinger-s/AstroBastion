using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronicTower : BaseTower, IReceiveTowers
{
    [System.Serializable]
    public class Connection
    {
        public GameObject connectionVisualizer;
        public ElectronicTower from;
        public ElectronicTower to;
    }

    public BaseProjectile gunSystemImpulse;
    
    public List<GameObject> connectionVisualizers = new List<GameObject> { };
    public List<GameObject> freeIndicators = new List<GameObject> { };
    public Transform connectionStartPlace;


    private List<Connection> connections = new List<Connection> { };
    private List<ElectronicTower> detectedTowers = new List<ElectronicTower> { };
    private bool generateImpulses = false;
    private float timer = 0f;
    private float impulseTimer = 0f;
    private float impulseTimeout = 0f;

    private void Update()
    {

        timer += Time.deltaTime;
        if (HasEnemy() && timer > betweenShotsSeconds && isActive)
        {
            Shoot();
        }

        if (generateImpulses)
        {
            impulseTimer += Time.deltaTime;
            if (impulseTimer > impulseTimeout)
            {
                gunSystemImpulse.transform.position = shotStartPlaces[0].position; ;
                gunSystemImpulse.transform.rotation = Quaternion.LookRotation(transform.up, transform.forward);
                gunSystemImpulse.gameObject.SetActive(true);
                gunSystemImpulse.effects[ProjectileEffectsNames.GunSystemsReboot].modifierEnabled = true;
                gunSystemImpulse.effects[ProjectileEffectsNames.GunSystemsReboot].value = CalculateGunRebootDuration();
                impulseTimer = 0f;
            }
        }
    }

    private void Shoot()
    {
        BaseProjectile projectile = TakeProjectile();
        projectile.transform.position = shotStartPlaces[0].position;
        projectile.transform.rotation = Quaternion.LookRotation(transform.up, transform.forward);
        projectile.gameObject.SetActive(true);
        timer = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleOnTriggerEnter(other);
        if (other.TryGetComponent<BaseEnemy> (out BaseEnemy enemy))
        {
            if (enemy.stealth is not null)
            {
                enemy.stealth.Disable();
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        HandleOnTriggerExit(other);
    }

    public override void OnIncreasedLeftUpgradeLevel()
    {
        if (!generateImpulses)
        {
            generateImpulses = true;
            impulseTimeout = CalcualteImpulseTimeout();
        }
    }

    public override void OnIncreasedRightUpgradeLevel()
    {
        CheckConnections();
    }

    private void CheckConnections()
    {
        Debug.LogError("Call CheckConnections", gameObject);
        UpdateFreeIndicators();
        for (int i = connections.Count; i < GetAvailableConnectionsCount(); i++)
        {
            if (detectedTowers.Count == 0)
            {
                return;
            }
            Connection newConnection = new Connection();
            newConnection.from = this;
            newConnection.to = FindNearestTower();
            Debug.LogError(newConnection.to == null);
            if (newConnection.to is null)
            {
                return;
            }
            newConnection.connectionVisualizer = connectionVisualizers[0];
            detectedTowers.Remove(newConnection.to);
            connectionVisualizers[0].transform.position = connectionStartPlace.position;
            connectionVisualizers[0].transform.LookAt(newConnection.to.connectionStartPlace);
            connectionVisualizers[0].transform.localScale = new Vector3(1f,
                1f,
                (newConnection.to.connectionStartPlace.position - newConnection.from.connectionStartPlace.position).magnitude / 2);
            connectionVisualizers[0].SetActive(true);
            connectionVisualizers.RemoveAt(0);
            newConnection.to.CreateConnection(newConnection);
            connections.Add(newConnection);
        }
        UpdateFreeIndicators();
    }

    public void AddTower(BaseTower tower)
    {
        if (tower is ElectronicTower electronicTower && electronicTower != this)
        {
            detectedTowers.Add(electronicTower);
            CheckConnections();
        }
    }

    public void RemoveTower(BaseTower tower)
    {
        if (tower is ElectronicTower electronicTower && detectedTowers.Contains(electronicTower))
        {
            detectedTowers.Remove(electronicTower);
        }
    }

    public override void Destroy()
    {
        ClearAllConnections();
        generateImpulses = false;
        timer = 0f;
        impulseTimer = 0f;
        base.Destroy();
    }

    public void BreakConnection(Connection connection)
    {
        connections.Remove(connection);
        if (connection.from == this)
        {
            connectionVisualizers.Add(connection.connectionVisualizer);
            connection.connectionVisualizer.SetActive(false);
        }
        CheckConnections();
    }

    private void ClearAllConnections()
    {
        var copy = new List<Connection>(connections);
        connections.Clear();
        foreach (Connection con in copy)
        {
            if (con.to != this)
            {
                con.to.BreakConnection(con);
            }
            else
            {
                con.from.BreakConnection(con);
            }
            connectionVisualizers.Add(con.connectionVisualizer);
            con.connectionVisualizer.SetActive(false);
        }
        UpdateFreeIndicators();
    }

    public void CreateConnection(Connection connection)
    {
        connections.Add(connection);
        detectedTowers.Remove(connection.from);
        UpdateFreeIndicators();
    }

    public int GetAvailableConnectionsCount()
    {
        return rightUpgradeLevel;
    }

    private float CalcualteImpulseTimeout()
    {
        return 12 - leftUpgradeLevel * 2;
    }

    private float CalculateGunRebootDuration()
    {
        return 2 + leftUpgradeLevel;
    }

    private ElectronicTower FindNearestTower()
    {
        if (detectedTowers.Count == 0)
            return null;
        int index = -1;
        for (int i = 0; i < detectedTowers.Count; i++)
        {
            if (detectedTowers[i].GetConnectionsCount() == detectedTowers[i].GetAvailableConnectionsCount())
            {
                continue;
            }
            if (index == -1)
            {
                index = i;
            }
            if ((transform.position - detectedTowers[index].transform.position).sqrMagnitude > (transform.position - detectedTowers[i].transform.position).sqrMagnitude)
            {
                index = i;
            }
        }
        if (index == -1)
        {
            return null;
        }
        
        return detectedTowers[index];
    }

    public int GetConnectionsCount()
    {
        return connections.Count;
    }

    private void UpdateFreeIndicators()
    {
        for (int i = 0; i < freeIndicators.Count; i++)
        {
            if (i < GetAvailableConnectionsCount() - GetConnectionsCount())
            {
                freeIndicators[i].SetActive(true);
            }
            else
            {
                freeIndicators[i].SetActive(false);
            }
        }
    }
}
