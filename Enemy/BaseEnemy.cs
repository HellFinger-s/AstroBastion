using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AYellowpaper.SerializedCollections;

public enum EnemyTypes
{
    Soldier,
    Ox,
    Fly,
    Sprinter,
    Desperate,
    Styer,
    Bully,
    SpaceFort,
    Provocateur,
    Saboteur,
    RepairTeam,
    Chameleon
}


public enum Priority
{
    Provocateur,
    PolicyBased,
    Ignore
}


public class BaseEnemy : MonoBehaviour
{
    public bool engineActive = true;
    public bool isActiveWeaponSystem = true;
    public float basicMaxHealth;
    public float basicMaxSpeed;
    public float rotationSpeed;
    public Priority priority = Priority.PolicyBased;
    public Shield shield;
    public Stealth stealth;

    [Header("Explosion on death")]
    public bool needExplodeParticleOnDeath = true;
    public float particleScale = 5f;
    
    [HideInInspector]
    public int poolIndex = 0;
    public EnemyTypes keyName;
    public float waypointAccuracy = 30f;
    public List<Vector3> waypoints;
    [HideInInspector]
    public int pathIndex = 0;
    
    public int currentWaypointIndex = 1;
    public float reward = 100;
    public int planetDamage = 2;
    public GameObject engineFlamesParent;
    public ParticleSystem repairParticles;
    [Header("Resist")]
    public float kineticResist;
    public float energyResist;

    public List<BaseTower> detectedTowers = new List<BaseTower> { };
    public float speedMultiplier = 1f;
    private Vector3 offset;
    private float maxSpeed;
    private Coroutine engineRestartCoroutine;
    private bool destroyed = false;
    protected Rigidbody rb;

    private float scaledMaxHealth;
    private float currentHealth;
    private float currentReward;
    private float slowdownTimer = 0f;
    private float slowDownduration = 2.5f;
    private float slowDownMultiplier = 1f;
    private float engineOverheatTimer = 0f;
    private float overheatDuration = 2f;
    private float gunSystemRebootTimer = 0f;
    private float gunSystemRebootDuration = 0f;
    private bool slowDown = false;
    private bool engineOverheat = false;
    private bool gunSystemReboot;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Setup(EnemySetup parameters, int waveNumber)
    {
        if (parameters.useShield)
        {
            shield.gameObject.SetActive(true);
            shield.maxValue = parameters.shieldCapacity;
            shield.regenerationSpeed = parameters.regenerationSpeed;
            shield.ResetShield();
        }

        if (parameters.overrideSpeed)
        {
            maxSpeed = parameters.newMaxSpeed;
        }
        else
        {
            maxSpeed = basicMaxSpeed;
        }

        engineOverheat = false;
        slowDown = false;
        engineOverheatTimer = 0f;
        slowdownTimer = 0f;
        if (stealth is not null)
        {
            stealth.Enable();
        }
        Grow(waveNumber);
    }

    private void Update()
    {
        if (slowDown)
        {
            slowdownTimer += Time.deltaTime;
            if (slowdownTimer > slowDownduration)
            {
                speedMultiplier /= slowDownMultiplier;
                slowdownTimer = 0f;
                slowDown = false;
            }
        }
        
        if (engineOverheat)
        {
            engineOverheatTimer += Time.deltaTime;
            if (engineOverheatTimer > overheatDuration)
            {
                speedMultiplier = 1;
                engineOverheatTimer = 0f;
                engineOverheat = false;
            }
        }

        if (gunSystemReboot)
        {
            gunSystemRebootTimer += Time.deltaTime;
            if (gunSystemRebootTimer > gunSystemRebootDuration)
            {
                isActiveWeaponSystem = true;
                gunSystemRebootTimer = 0f;
                gunSystemReboot = false;
            }
        }

        Tick();
    }

    protected virtual void Tick() { }

    private void FixedUpdate()
    {
        if (engineActive)
        {
            Move();
        }
    }

    private void Move()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        Vector3 target = waypoints[currentWaypointIndex];
        Vector3 toTarget = target - rb.position;

        if (toTarget.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(toTarget);
            Quaternion newRot = Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newRot);
        }

        float step = maxSpeed * speedMultiplier * Time.fixedDeltaTime;
        Vector3 forward = rb.rotation * Vector3.forward;
        rb.MovePosition(rb.position + forward * step);

        if ((rb.position - target).sqrMagnitude < waypointAccuracy)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
                Destroy();
        }
    }

    private void Grow(int waveNumber)
    {
        scaledMaxHealth = basicMaxHealth * Mathf.Pow(1.02f, waveNumber - 1);
        currentReward = reward * Mathf.Pow(1.01f, waveNumber - 1);
    }

    public void ReceiveDamage(Damage damageComponent, Collider collider)
    {
        if (damageComponent.damageType == DamageType.kinetic)
        {
            currentHealth -= damageComponent.value * damageComponent.multiplier * (1 - kineticResist);
        }
        else
        {
            currentHealth -= damageComponent.value * damageComponent.multiplier * (1 - energyResist);
        }
        DamageVisualizer.GetInstance().ShowDamage(collider.ClosestPoint(transform.position),
                damageComponent.gameObject.transform.position - transform.position,
                damageComponent.value * damageComponent.multiplier,
                DamageVisType.PureDamage,
                damageComponent.multiplier > 1);
        if (currentHealth <= 0)
        {
            Destroy();
        }
    }

    public void SetSpeedMultiplier(float speedMultiplier)
    {
        this.speedMultiplier = speedMultiplier;
    }

    public void MultiplySpeedMultiplier(float value)
    {
        speedMultiplier *= value;
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
        destroyed = true;
        ExplosionParticlesManager.GetInstance().SpawnParticle(transform.position, particleScale);
        NotifyTowers();
        ResourceManager.GetInstance().IncreaseResources(currentReward);
        PoolManager.GetInstance().ReleaseEnemy(this);
    }

    public void SetPath(List<Vector3> path, Vector3 newOffset, int pathIndex, bool startPositionOverride, Vector3 startPosition)
    {
        if (waypoints is not null)
        {
            waypoints.Clear();
        }
        waypoints = new List<Vector3>(path);
        this.pathIndex = pathIndex;
        offset = newOffset;
        for (int i = 0; i < waypoints.Count; i++)
        {
            waypoints[i] = waypoints[i] + offset;
        }
        if (startPositionOverride)
        {
            transform.position = startPosition;
        }
        else
        {
            transform.position = waypoints[0];
        }
        transform.forward = (waypoints[1] - transform.position).normalized;
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BaseProjectile>(out BaseProjectile projectileComponent))
        {
            ApplyModifiers(projectileComponent);
        }
        if (other.TryGetComponent<Damage>(out Damage damageComponent))
        {
            ReceiveDamage(damageComponent, other);
            if (stealth is not null && stealth.IsActive())
            {
                stealth.Disable();
            }
        }
        
    }

    private void ApplyModifiers(BaseProjectile projectile)
    {
        if (projectile.effects != null)
        {
            // Move this to a separate manager
            foreach(var key in projectile.effects.Keys)
            {
                if (projectile.effects[key].modifierEnabled)
                {
                    if (key == ProjectileEffectsNames.Collapse)
                    {
                        if (Dice.RollChance(projectile.effects[key].percent))
                        {
                            Destroy();
                        }
                    }

                    if (key == ProjectileEffectsNames.SlowDown)
                    {
                        slowdownTimer = 0f;
                        if (!slowDown)
                        {
                            slowDownMultiplier = 1 - projectile.effects[key].percent / 100f;
                            speedMultiplier *= slowDownMultiplier;
                            slowDown = true;
                        }                        
                    }

                    if (key == ProjectileEffectsNames.BackTeleport)
                    {
                        currentWaypointIndex -= 20;
                        if (currentWaypointIndex < 0)
                        {
                            currentWaypointIndex = 0;
                        }
                        transform.position = waypoints[currentWaypointIndex];
                        engineActive = true;
                    }

                    if (key == ProjectileEffectsNames.GunSystemsReboot)
                    {
                        gunSystemRebootTimer = 0f;
                        if (isActiveWeaponSystem)
                        {
                            isActiveWeaponSystem = false;
                            gunSystemReboot = true;
                            gunSystemRebootDuration = projectile.effects[key].value;
                        }
                    }

                    if (key == ProjectileEffectsNames.EngineOverheat && !engineOverheat)
                    {
                        if (Dice.RollChance(projectile.effects[key].percent))
                        {
                            Debug.Log("OVERHEAT");
                            engineOverheat = true;
                            speedMultiplier = 0f;
                        }
                    }
                }
            }
        }
    }

    public void DisableEngines(float seconds)
    {
        if (engineRestartCoroutine is not null)
        {
            StopCoroutine(engineRestartCoroutine);
            engineRestartCoroutine = null;
        }
        engineRestartCoroutine = StartCoroutine(RestartEngines(seconds));
        engineActive = false;
        engineFlamesParent.SetActive(false);
    }

    private IEnumerator RestartEngines(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        engineActive = true;
        engineFlamesParent.SetActive(true);
    }

    public float GetPathPassing()
    {
        return (float) currentWaypointIndex / waypoints.Count;
    }

    public float GetSpeed()
    {
        return maxSpeed * speedMultiplier;
    }

    public float GetBasicSpeed()
    {
        return basicMaxSpeed * speedMultiplier;
    }

    public void DetectedTower(BaseTower tower)
    {
        if (!destroyed)
        {
            detectedTowers.Add(tower);
        }
    }

    public void RemoveTower(BaseTower tower)
    {
        detectedTowers.Remove(tower);
    }

    
    private void NotifyTowers()
    {
        for (int i = 0; i < detectedTowers.Count; i++)
        {
            detectedTowers[i].EnemyDestroyed(this);
        }
        detectedTowers.Clear();
    }

    public void Refresh()
    {
        engineActive = true;
        isActiveWeaponSystem = true;
        speedMultiplier = 1f;
        currentHealth = scaledMaxHealth;
        if (engineRestartCoroutine is not null)
        {
            StopCoroutine(engineRestartCoroutine);
            engineRestartCoroutine = null;
        }
        currentWaypointIndex = 1;
        destroyed = false;
    }

    public bool isDestroyed()
    {
        return destroyed;
    }

    public void Repair(int value)
    {
        if (!repairParticles.isPlaying)
        {
            repairParticles.Play();
        }
        currentHealth += value;
        if (currentHealth > scaledMaxHealth)
        {
            currentHealth = scaledMaxHealth;
        }
    }

    public void Teleport(int delta)
    {
        if (engineActive)
        {
            currentWaypointIndex -= delta;
            if (currentWaypointIndex < 0)
            {
                currentWaypointIndex = 0;
            }
            rb.position = waypoints[currentWaypointIndex];
        }
    }

    public float GetScaledMaxHealth()
    {
        return scaledMaxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetScaledReward()
    {
        return currentReward;
    }
}
