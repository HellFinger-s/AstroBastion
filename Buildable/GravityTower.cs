using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityTower : BaseTower
{
    private float timer = 0f;
    private float teleportTimer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (HasEnemy() && timer > betweenShotsSeconds && isActive)
        {
            Shoot();
        }

        if (leftUpgradeLevel > 0)
        {
            teleportTimer += Time.deltaTime;
            if (teleportTimer > 10f)
            {
                TeleportEnemies();
                teleportTimer = 0f;
            }
            
        }
    }

    private void Shoot()
    {
        BaseProjectile projectile = TakeProjectile();
        projectile.transform.position = shotStartPlaces[0].position;
        projectile.transform.rotation = Quaternion.LookRotation(transform.up, transform.forward);
        projectile.gameObject.SetActive(true);
        projectile.effects[ProjectileEffectsNames.SlowDown].modifierEnabled = true;
        projectile.effects[ProjectileEffectsNames.SlowDown].percent = CalculateSlowdownPercent();
        timer = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleOnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleOnTriggerExit(other);
    }

    private int CalculateTeleportingEnemyCount()
    {
        return leftUpgradeLevel * 2;
    }

    private int CalculateSlowdownPercent()
    {
        return 30 + rightUpgradeLevel * 5;
    }

    private void TeleportEnemies()
    {
        for (int i = 0; i < CalculateTeleportingEnemyCount(); i++)
        {
            if (i % 2 == 0 && provocateurs.Count > 0)
            {
                int index = Random.Range(0, provocateurs.Count);
                provocateurs[index].Teleport(5);
            }
            else if (enemies.Count > 0)
            {
                int index = Random.Range(0, enemies.Count);
                enemies[index].Teleport(5);
            }
        }
    }
}
