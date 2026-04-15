using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailgunTower : BaseTower
{
    public Transform footing;
    public Transform gun;


    public float footingRotationSpeed = 80f;
    public float gunRotationSpeed = 80f;

    public float timer = 0f;


    private void Update()
    {
        RotateTower();

        timer += Time.deltaTime;
        if (HasEnemy() && timer > betweenShotsSeconds && isActive)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (IsFocusedOnTarget())
        {
            RailgunProjectile projectile = (RailgunProjectile) TakeProjectile();
            projectile.transform.position = shotStartPlaces[0].position;
            projectile.transform.rotation = gun.transform.rotation;
            projectile.gameObject.SetActive(true);
            timer = 0f;
            projectile.destroyChance = CalculateDestroyChance();
            if (Dice.RollChance(CalculateCritChance()))
            {
                if (projectile.TryGetComponent<Damage>(out Damage damage))
                {
                    damage.multiplier = CalculateCritMultiplier();
                }
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        HandleOnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleOnTriggerExit(other);
    }

    private void RotateTower()
    {
        if (isActive && HasEnemy())
        {
            currentEnemy = GetTarget();
            Vector3 toTarget = GetPredictedAimPoint(currentEnemy) - gun.position;

            Vector3 horizontalDir = Vector3.ProjectOnPlane(toTarget, footing.up).normalized;
            if (horizontalDir != Vector3.zero)
            {
                Quaternion targetBaseRotation = Quaternion.LookRotation(horizontalDir, footing.up);
                footing.rotation = Quaternion.RotateTowards(
                    footing.rotation,
                    targetBaseRotation,
                    footingRotationSpeed * Time.deltaTime
                );
            }

            //Vector3 localToTarget = transform.InverseTransformDirection(toTarget);
            float angle = Vector3.Angle(horizontalDir, toTarget);
            //float targetPitchAngle = Mathf.Atan2(localToTarget.y, localToTarget.z) * Mathf.Rad2Deg;
            Quaternion targetGunLocalRotation = Quaternion.Euler(-angle, 0f, 0f);
            gun.localRotation = Quaternion.RotateTowards(
                gun.localRotation,
                targetGunLocalRotation,
                gunRotationSpeed * Time.deltaTime
            );
        }
    }

    private bool IsFocusedOnTarget()
    {
        Vector3 aimPoint = GetPredictedAimPoint(currentEnemy);
        Vector3 toAim = aimPoint - gun.position;

        Debug.DrawLine(gun.transform.position, aimPoint, Color.red, 5f);

        if (toAim.sqrMagnitude < 0.0001f) return true;

        return Vector3.Angle(toAim, gun.forward) < focusAngle;
    }

    private Vector3 GetPredictedAimPoint(BaseEnemy enemy)
    {
        
        return enemy.transform.position;
    }

    private int CalculateDestroyChance()
    {
        return 10 * leftUpgradeLevel;
    }

    private int CalculateCritChance()
    {
        return 15 * rightUpgradeLevel;
    }

    private int CalculateCritMultiplier()
    {
        return (int) (1.5 + 0.5 * rightUpgradeLevel);
    }
}
