using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protectable : MonoBehaviour
{
    public LevelManager levelManager;
    public int health;

    private void Start()
    {
        LUbus.GetInstance().UpdateProtectableHP(health, true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
        {
            health -= enemy.planetDamage;
            if (health < 0)
            {
                health = 0;
            }
            LUbus.GetInstance().UpdateProtectableHP(health);
            if (health == 0)
            {
                levelManager.ProtectableDead();
            }
        }
    }
}
