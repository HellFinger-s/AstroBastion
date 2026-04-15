using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairTeam : MonoBehaviour
{
    public float repairInterval = 1f;
    public int repairValue = 10;
    public BaseEnemy parent;

    private List<BaseEnemy> nearbyEnemies = new List<BaseEnemy> { };
    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > repairInterval)
        {
            foreach(BaseEnemy enemy in nearbyEnemies)
            {
                if (!enemy.isDestroyed())
                {
                    enemy.Repair(repairValue);
                }
            }
            timer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BaseEnemy>(out BaseEnemy enemy) && enemy != parent)
        {
            nearbyEnemies.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<BaseEnemy>(out BaseEnemy enemy) && enemy != parent && nearbyEnemies.Contains(enemy))
        {
            nearbyEnemies.Remove(enemy);
        }
    }
}
