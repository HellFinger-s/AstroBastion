using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : BaseTower
{
    public Transform footing;
    public Transform gun;

    public int burstLength = 3;
    public float betweenShotInBurstSeconds = 0.2f;

    public float footingRotationSpeed = 80f;
    public float gunRotationSpeed = 80f;

    public float maxLaserLength = 100f;
    public LayerMask laserRaycastMask;
    public Transform laser;

    public float timer = 0f;

    private bool laserActive = false;
    private bool bursting = true;
    private int shotsCount = 0;
    private int burstCount = 0;

    private float laserVisualizeUpdateThreshold = 0.1f;


    private void Update()
    {
        RotateTower();

        timer += Time.deltaTime;

        if (bursting && !laserActive && HasEnemy() && timer > betweenShotInBurstSeconds && isActive)
        {
            Shoot();
        }
        if (laserActive && timer > laserVisualizeUpdateThreshold)
        {
            LaserVisualize();
            timer = 0f;
        }
    }

    private void Shoot()
    {
        if (IsFocusedOnTarget())
        {
            if (leftUpgradeLevel > 0 && burstCount > CalculateRequireBurstsCount())
            {
                laserActive = true;
                Laser();
                return;
            }
            shotsCount++;
            BaseProjectile projectile = TakeProjectile();
            projectile.transform.position = shotStartPlaces[0].position;
            projectile.transform.rotation = gun.transform.rotation;
            projectile.gameObject.SetActive(true);
            timer = 0f;
            if (Dice.RollChance(CalculateShieldIgnoreChance()))
            {
                projectile.effects[ProjectileEffectsNames.ShieldIgnore].modifierEnabled = true;
            }
            if (shotsCount == burstLength)
            {
                shotsCount = 0;
                bursting = false;
                burstCount++;
                StartCoroutine(WaitUntilNextBurst(betweenShotsSeconds));
            }
        }
    }

    private IEnumerator WaitUntilNextBurst(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        bursting = true;
    }

    private void Laser()
    {
        LaserVisualize();
        StartCoroutine(DisableLaser(1f));
    }

    private void LaserVisualize()
    {
        if (Physics.Raycast(shotStartPlaces[0].position, gun.forward, out RaycastHit hit, maxLaserLength, laserRaycastMask, QueryTriggerInteraction.Collide))
        {
            //Debug.Log(hit.collider.gameObject.name);
            float distance = Vector3.Magnitude(gun.InverseTransformPoint(hit.point) - gun.InverseTransformPoint(shotStartPlaces[0].position));
            Debug.DrawLine(shotStartPlaces[0].position, hit.point, Color.blue, 5f);
            Debug.Log("dist: " + distance);
            laser.transform.localScale = new Vector3(0.2f, 0.2f, distance / 2f);
            laser.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("no hit");
            laser.transform.localScale = new Vector3(0.2f, 0.2f, maxLaserLength / 5f);
            laser.gameObject.SetActive(true);
        }
    }

    private IEnumerator DisableLaser(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        laser.transform.localScale = Vector3.one * 0.2f;
        laser.gameObject.SetActive(false);
        laserActive = false;
        timer = 0f;
        burstCount = 0;
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
        if (isActive && !laserActive && HasEnemy())
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
        Vector3 shooterPos = shotStartPlaces[0].position;

        Vector3 enemyPos = enemy.transform.position;
        Vector3 enemyVel = enemy.transform.forward * enemy.GetSpeed();

        Vector3 toEnemy = enemyPos - shooterPos;
        float dist = toEnemy.magnitude;

        float t = dist / Mathf.Max(0.01f, projectile.GetSpeed());

        return enemyPos + enemyVel * t;
    }

    private int CalculateShieldIgnoreChance()
    {
        return 10 * rightUpgradeLevel;
    }

    private int CalculateRequireBurstsCount()
    {
        return 1 * (5 - leftUpgradeLevel);
    }
}
