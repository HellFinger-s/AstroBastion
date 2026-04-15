using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTower_L3 : BaseTower
{
    public Transform footing;
    public Transform gun;


    public float footingRotationSpeed = 3f;
    public float gunRotationSpeed = 3f;

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
            BaseProjectile projectile1 = TakeProjectile();
            projectile1.transform.position = shotStartPlaces[0].position;
            projectile1.transform.rotation = gun.transform.rotation;
            projectile1.gameObject.SetActive(true);
            BaseProjectile projectile2 = TakeProjectile();
            projectile2.transform.position = shotStartPlaces[1].position;
            projectile2.transform.rotation = gun.transform.rotation;
            projectile2.gameObject.SetActive(true);
            timer = 0f;
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

        //Debug.DrawLine(gun.transform.position, aimPoint, Color.red, 5f);

        if (toAim.sqrMagnitude < 0.0001f) return true;

        return Vector3.Angle(toAim, gun.forward) < focusAngle;
    }

    private Vector3 GetPredictedAimPoint(BaseEnemy enemy)
    {
        Vector3 shooterPos = (shotStartPlaces[0].position + shotStartPlaces[1].position) / 2;

        Vector3 enemyPos = enemy.transform.position;
        Vector3 enemyVel = enemy.transform.forward * enemy.GetSpeed();

        Vector3 toEnemy = enemyPos - shooterPos;
        float dist = toEnemy.magnitude;

        float t = dist / Mathf.Max(0.01f, projectile.GetSpeed());

        return enemyPos + enemyVel * t;
    }
}
