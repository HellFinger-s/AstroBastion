using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;


public enum TargetPolicy
{
    queue,
    nearest,
    further,
    dying,
    freshest,
    fastest,
    slowest
}

public class BaseTower : Buildable, IReleaseProjectile
{
    [Space]
    [Space]
    [Space]
    [Header("BaseTower settings")]
    public int currentLevel = 1;
    public int nextLevelCost = 200;
    public BuildableTypes nextTowerKey;
    public bool usingTargetPolicy = true;
    public ParticleSystem repairParticles;
    public TargetPolicy policy = TargetPolicy.queue;
    public int leftUpgradeLevel = 0;
    public int rightUpgradeLevel = 0;
    public Sprite leftUpgradeIcon;
    public Sprite rightUpgradeIcon;
    public int[] leftUpgradeCost = new int[3] { 100, 100, 100 };
    public int[] rightUpgradeCost = new int[3] { 100, 100, 100 };
    public BuildableTypes leftLevel4Tower;
    public BuildableTypes rightLevel4Tower;
    public Platform connectedPlatform;
    public GameObject attackAreaVisualizer;
    [SerializedDictionary, SerializeField]
    private SerializedDictionary<MaterialKeys, Material> materials;
    [SerializeField]
    private List<MeshRenderer> meshRenderers = new List<MeshRenderer> { };
    public HPVisualizer hpVisualizer;

    [Space]
    [Space]
    [Space]
    [Header("Localization")]
    public LocalizationKeys nameKey;
    public LocalizationKeys descKey;

    [Space]
    [Space]
    [Space]
    [Header("Tower Logic")]
    public bool isActive = false;
    public int currentHealth;
    public int maxHealth;
    public float rebootTime;
    private List<EnemyPath> enemyPaths;


    public float betweenShotsSeconds = 0.2f;

    public BaseProjectile projectile;

    public List<BaseEnemy> enemies = new List<BaseEnemy> { };
    public List<BaseEnemy> provocateurs = new List<BaseEnemy> { };

    protected float focusAngle = 10f;

    public BaseEnemy currentEnemy;

    public List<Transform> shotStartPlaces;
    private List<BaseProjectile> projectilesPool = new List<BaseProjectile> { };
    public int poolCapacity = 10;
    private int freeProjectileIndex = 0;

    private List<string> leftLocalizationEndings = new List<string> { "_ULeft_L1_desc", "_ULeft_L2_desc", "_ULeft_L3_desc", "_ULeft_L3_desc" };
    private List<string> rightLocalizationEndings = new List<string> { "_URight_L1_desc", "_URight_L2_desc", "_URight_L3_desc", "_URight_L3_desc" };

    private Coroutine rebootCoroutine;

    public void PlayerDestroy()
    {
        Destroy();
    }

    public virtual void Destroy()
    {
        isActive = false;
        currentHealth = maxHealth;
        hpVisualizer.gameObject.SetActive(false);
        connectedPlatform.connectedTower = null;
        connectedPlatform = null;
        transform.parent = null;
        SetAttackAreaVisibility(false);
        leftUpgradeLevel = 0;
        rightUpgradeLevel = 0;
        PoolManager.GetInstance().ReleaseBuild(this);
    }

    public void InitTower(Warlord warlord)
    {
        currentHealth = maxHealth;
        SetEnemyPaths(warlord);
        InitPool();
    }

    public void ActivateTower()
    {
        isActive = true;
    }

    public virtual void Built()
    {

    }

    public void InitPool()
    {
        if (projectile is not null)
        {
            

            for (int i = 0; i < poolCapacity; i++)
            {
                projectilesPool.Add(GameObject.Instantiate(projectile, Vector3.zero, Quaternion.identity));    
                projectilesPool[i].poolIndex = i;
                projectilesPool[i].parentPool = this;
                projectilesPool[i].gameObject.SetActive(false);
            }
        }

    }

    protected BaseProjectile TakeProjectile()
    {
        if (freeProjectileIndex < poolCapacity)
        {
            return projectilesPool[freeProjectileIndex++];
        }
        else
        {
            Debug.LogWarning("Pool overheat");
            return projectilesPool[0];
        }
    }

    public void ReleaseProjectile(BaseProjectile projectileToRelease)
    {
        BaseProjectile tempProjectile = projectilesPool[--freeProjectileIndex];
        tempProjectile.poolIndex = projectileToRelease.poolIndex;
        projectilesPool[tempProjectile.poolIndex] = tempProjectile;
        projectilesPool[freeProjectileIndex] = projectileToRelease;
        projectileToRelease.poolIndex = freeProjectileIndex;

        /*if (projectileToRelease.effects != null)
        {
            foreach (var key in projectileToRelease.effects.Keys)
            {
                projectileToRelease.effects[key].modifierEnabled = false;
            }
        }*/

        if (projectileToRelease.TryGetComponent<Damage>(out Damage damageComponent))
        {
            damageComponent.multiplier = 1;
        }
    }

    public void ReceiveDamage(Damage damageComponent, Collider collider)
    {
        currentHealth -= damageComponent.value;
        hpVisualizer.gameObject.SetActive(true);
        hpVisualizer.SetMaterial(currentHealth, maxHealth);
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

    public void ChangePolicy(TargetPolicy newPolicy)
    {
        policy = newPolicy;
    }

    public void HandleOnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BaseEnemy>(out BaseEnemy enemyComponent))
        {
            //Debug.Log(!enemyComponent.isDestroyed() + "  " + enemyComponent.name + "  " + other.GetInstanceID(), enemyComponent.gameObject);
            if (!enemyComponent.isDestroyed() && !enemies.Contains(enemyComponent) && !provocateurs.Contains(enemyComponent))
            {
                enemyComponent.DetectedTower(this);
                AddEnemy(enemyComponent);
            }
        }
        
        if (other.TryGetComponent<Damage>(out Damage damageComponent))
        {
            //Debug.LogError(other.gameObject.name);
            LUbus.GetInstance().TowerAttacked();
            ReceiveDamage(damageComponent, other);
        }
    }

    public void HandleOnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<BaseEnemy>(out BaseEnemy enemyComponent))
        {
            enemyComponent.RemoveTower(this);
            RemoveEnemy(enemyComponent);
        }
    }

    private void AddEnemy(BaseEnemy addable)
    {
        if (addable.priority == Priority.Provocateur)
        {
            provocateurs.Add(addable);
        }
        else if (addable.priority == Priority.PolicyBased)
        {
            enemies.Add(addable);
        }
    }

    private void RemoveEnemy(BaseEnemy removable)
    {
        if (removable.priority == Priority.Provocateur && provocateurs.Contains(removable))
        {
            provocateurs.Remove(removable);
        }
        else if (removable.priority == Priority.PolicyBased && enemies.Contains(removable))
        {
            enemies.Remove(removable);
        }
    }

    public BaseEnemy GetTarget()
    {
        if (provocateurs.Count > 0)
        {
            return provocateurs[0];
        }
        switch (policy)
        {
            case TargetPolicy.queue:
                return enemies[0];
            case TargetPolicy.nearest:
                return FindNearest();
            case TargetPolicy.further:
                return FindFurther();
            case TargetPolicy.dying:
                return FindDying();
            case TargetPolicy.freshest:
                return FindFreshest();
            case TargetPolicy.fastest:
                return FindFastest();
            case TargetPolicy.slowest:
                return FindSlowest();
            default:
                return enemies[0];
        }
    }


    public bool HasEnemy()
    {
        int count = 0;
        for (int i = 0; i < provocateurs.Count; i++)
        {
            if (Vector3.Angle(provocateurs[i].transform.position - transform.position, transform.up) < 90f)
            {
                count++;
            }
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].stealth is null || (enemies[i].stealth is not null && !enemies[i].stealth.IsActive()))
            {
                if (Vector3.Angle(enemies[i].transform.position - transform.position, transform.up) < 90f)
                {
                    count++;
                }
            }
        }
        return count > 0;
    }

    private BaseEnemy FindNearest()
    {
        BaseEnemy temp = enemies[0];
        for (int i = 1; i < enemies.Count; i++)
        {
            if ((enemies[i].stealth is null || (enemies[i].stealth is not null && !enemies[i].stealth.IsActive()))
                && (transform.position - enemies[i].transform.position).sqrMagnitude < (transform.position - temp.transform.position).sqrMagnitude)
            {
                temp = enemies[i];
            }
        }
        return temp;
    }

    private BaseEnemy FindFurther()
    {
        BaseEnemy temp = enemies[0];
        for (int i = 1; i < enemies.Count; i++)
        {
            if ((enemies[i].stealth is null || (enemies[i].stealth is not null && !enemies[i].stealth.IsActive()))
                && (transform.position - enemies[i].transform.position).sqrMagnitude > (transform.position - temp.transform.position).sqrMagnitude)
            {
                temp = enemies[i];
            }
        }
        return temp;
    }

    private BaseEnemy FindDying()
    {
        BaseEnemy temp = enemies[0];
        for (int i = 1; i < enemies.Count; i++)
        {
            if ((enemies[i].stealth is null || (enemies[i].stealth is not null && !enemies[i].stealth.IsActive()))
                && enemies[i].GetCurrentHealth() < temp.GetScaledMaxHealth())
            {
                temp = enemies[i];
            }
        }
        return temp;
    }

    private BaseEnemy FindFreshest()
    {
        BaseEnemy temp = enemies[0];
        for (int i = 1; i < enemies.Count; i++)
        {
            if ((enemies[i].stealth is null || (enemies[i].stealth is not null && !enemies[i].stealth.IsActive()))
                && enemies[i].GetCurrentHealth() > temp.GetScaledMaxHealth())
            {
                temp = enemies[i];
            }
        }
        return temp;
    }

    private BaseEnemy FindSlowest()
    {
        BaseEnemy temp = enemies[0];
        for (int i = 1; i < enemies.Count; i++)
        {
            if ((enemies[i].stealth is null || (enemies[i].stealth is not null && !enemies[i].stealth.IsActive()))
                && enemies[i].GetSpeed() < temp.GetSpeed())
            {
                temp = enemies[i];
            }
        }
        return temp;
    }

    private BaseEnemy FindFastest()
    {
        BaseEnemy temp = enemies[0];
        for (int i = 1; i < enemies.Count; i++)
        {
            if ((enemies[i].stealth is null || (enemies[i].stealth is not null && !enemies[i].stealth.IsActive()))
                && enemies[i].GetSpeed() > temp.GetSpeed())
            {
                temp = enemies[i];
            }
        }
        return temp;
    }

    public void IncreaseLevel()
    {
        currentLevel++;
    }

    public int IncreaseLeftUpgradeLevel()
    {
        leftUpgradeLevel++;
        OnIncreasedLeftUpgradeLevel();
        return leftUpgradeLevel;
    }

    public virtual void OnIncreasedLeftUpgradeLevel() { }


    public int IncreaseRightUpgradeLevel()
    {
        rightUpgradeLevel++;
        OnIncreasedRightUpgradeLevel();
        return rightUpgradeLevel;
    }

    public virtual void OnIncreasedRightUpgradeLevel() { }

    public void MomentalRepair()
    {
        if (!repairParticles.isPlaying)
        {
            repairParticles.Play();
        }
        currentHealth = maxHealth;
        hpVisualizer.gameObject.SetActive(false);
    }

    public void Repair(int value)
    {
        if (currentHealth < maxHealth)
        {
            if (!repairParticles.isPlaying)
            {
                repairParticles.Play();
            }
            currentHealth += value;
            hpVisualizer.SetMaterial(currentHealth, maxHealth);
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
                hpVisualizer.gameObject.SetActive(false);
            }
        }
    }

    public virtual Dictionary<string, string> GetSubstitutions(IButtonTextTemplate template)
    {
        Dictionary<string, string> substitutions = new Dictionary<string, string> { };

        if (template is BuildableTextTemplate)
        {
            if (projectile.TryGetComponent<Damage>(out Damage component))
            {
                substitutions.Add(TextMarks.Damage.ToString(), string.Format("{0}", component.value));
                substitutions.Add(TextMarks.DamageType.ToString(), Localizer.GetInstance().Localize(component.damageType.ToString()));
            }
            substitutions.Add(TextMarks.FireRate.ToString(), string.Format("{0:f2}", (betweenShotsSeconds).ToString()));
            substitutions.Add(TextMarks.HP.ToString(), maxHealth.ToString());
            
        }

        return substitutions;
    }

    public string GetUpgradeLocalizationKey(bool isLeft = false)
    {
        if (isLeft)
        {
            Debug.Log(leftUpgradeLevel);
            return keyName.ToString() + leftLocalizationEndings[leftUpgradeLevel];
        }
        return keyName.ToString() + rightLocalizationEndings[rightUpgradeLevel];
    }

    public void SetMaterial(MaterialKeys key)
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material = materials[key];
        }
    }

    public void SetAttackAreaVisibility(bool visibility)
    {
        if (attackAreaVisualizer)
        {
            attackAreaVisualizer.SetActive(visibility);
            if (enemyPaths.Count > 0)
            {
                if (visibility)
                {
                    for (int i = 0; i < enemyPaths.Count; i++)
                    {
                        enemyPaths[i].ShowAttackAreaOnPath(transform.position, transform.up, attackAreaVisualizer.transform.localScale.x / 2);
                    }
                }
                else
                {
                    for (int i = 0; i < enemyPaths.Count; i++)
                    {
                        enemyPaths[i].SetDefaultPathColor();
                    }
                }
            }
        }
    }

    public void SetEnemyPaths(Warlord warlord)
    {
        enemyPaths = warlord.paths;
    }

    public void EnemyDestroyed(BaseEnemy enemy)
    {
        switch (enemy.priority)
        {
            case Priority.Provocateur:
                provocateurs.Remove(enemy);
                break;
            case Priority.PolicyBased:
                enemies.Remove(enemy);
                break;
            case Priority.Ignore:
                break;
            default:
                Debug.Log("Unknown priority");
                break;
        }
    }

    public int GetLeftUpgradeCost()
    {
        if (leftUpgradeLevel < 3)
        {
            return leftUpgradeCost[leftUpgradeLevel];
        }
        return 0;
    }

    public int GetRightUpgradeCost()
    {
        if (rightUpgradeLevel < 3)
        {
            return rightUpgradeCost[rightUpgradeLevel];
        }
        return 0;
    }

    public void Reboot()
    {
        Debug.Log("reboot");
        if (isActive)
        {
            isActive = false;
            StartCoroutine(RebootEnumerator());
        }
    }

    private IEnumerator RebootEnumerator()
    {
        yield return new WaitForSeconds(rebootTime);
        isActive = true;
    }

    public int GetPlayerDestroyCashback()
    {
        return Mathf.CeilToInt(fullCost * currentHealth / maxHealth * cashbackPercent);
    }
}
