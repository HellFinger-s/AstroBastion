using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyTower : BaseTower
{
    public float timer = 0f;


    private void Update()
    {

        timer += Time.deltaTime;
        if (HasEnemy() && timer > betweenShotsSeconds && isActive)
        {
            Shoot();
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
    }

    private void OnTriggerExit(Collider other)
    {
        HandleOnTriggerExit(other);
    }
}
