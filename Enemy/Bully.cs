using System.Collections;
using UnityEngine;
using System.Collections.Generic;



public class Bully : BaseEnemy, IReceiveTowers
{
    [Header("Bully")]
    public Vector2 waitingTimeBorders;
    public float attackInterval;
    public float proximityToTower;
    public int projectilePoolCapacity;
    public BullyProjectile projectile;
    public List<Transform> shotStartPlaces;
    public List<BaseTower> towersToAttack;

    private float timer = 0f;
    private bool canAttack = false;
    private int currentTargetIndex = 0;
    private Queue<BullyProjectile> projectilesPool = new Queue<BullyProjectile> { };

    private void Start()
    {
        for (int i = 0; i < projectilePoolCapacity; i++)
        {
            BullyProjectile temp = Instantiate(projectile);
            temp.bully = this;
            temp.gameObject.SetActive(false);
            projectilesPool.Enqueue(temp);
        }
        StartCoroutine(StartHunt());
    }

    protected override void Tick()
    {
        if (isActiveWeaponSystem && canAttack && towersToAttack.Count > 0)
        {
            timer += Time.deltaTime;
            //Debug.LogError(currentTargetIndex);
            if (!towersToAttack[currentTargetIndex].gameObject.activeSelf)
            {
                towersToAttack.RemoveAt(currentTargetIndex);
                currentTargetIndex = GetNewTowerIndex();
                if (currentTargetIndex == -1)
                {
                    //Debug.LogError("Index is -1");
                    ReturnToWay();
                    canAttack = false;
                    timer = 0f;
                }
                else
                {
                    TeleportToTower(towersToAttack[currentTargetIndex]);
                }
            }
            if (timer > attackInterval)
            {
                Attack();
            }
        }

    }


    private IEnumerator StartHunt()
    {
        yield return new WaitForSeconds(Random.Range(waitingTimeBorders.x, waitingTimeBorders.y));
        canAttack = true;
    }

    private int GetNewTowerIndex()
    {
        if (towersToAttack.Count == 0)
            return -1;

        int index = 0;

        for (int i = 1; i < towersToAttack.Count; i++)
        {
            if ((transform.position - towersToAttack[i].transform.position).sqrMagnitude < (transform.position - towersToAttack[index].transform.position).sqrMagnitude)
            {
                index = i;
            }
        }
        return index;
    }

    private void TeleportToTower(BaseTower target)
    {
        engineActive = false;
        Debug.LogError("Telep");
        Vector3 randomDir = Quaternion.AngleAxis(
            Random.Range(0f, 60f),
            Random.onUnitSphere) * target.transform.up * proximityToTower;
        transform.position = target.transform.position + randomDir;
        transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
    }

    private void ReturnToWay()
    {
        while (currentWaypointIndex + 1 < waypoints.Count &&
       (waypoints[currentWaypointIndex + 1] - transform.position).sqrMagnitude <
       (waypoints[currentWaypointIndex] - transform.position).sqrMagnitude)
        {
            currentWaypointIndex++;
        }
        rb.position = waypoints[currentWaypointIndex];
        engineActive = true;
    }

    public void AddTower(BaseTower tower)
    {
        if (!towersToAttack.Contains(tower))
        {
            towersToAttack.Add(tower);
            if (towersToAttack.Count == 1)
            {
                currentTargetIndex = 0;
                TeleportToTower(tower);
            }
            canAttack = true;
        }

        
    }

    public void RemoveTower(BaseTower tower)
    {
        if (towersToAttack.Contains(tower))
        {
            towersToAttack.Remove(tower);
            if (towersToAttack.Count == 0)
            {
                currentTargetIndex = 0;
                canAttack = false;
                timer = 0f;
            }
        }
    }

    private void Attack()
    {
        projectilesPool.Peek().transform.position = shotStartPlaces[0].position;
        projectilesPool.Peek().transform.rotation = shotStartPlaces[0].transform.rotation;
        projectilesPool.Peek().gameObject.SetActive(true);
        projectilesPool.Dequeue();
        projectilesPool.Peek().transform.position = shotStartPlaces[1].position;
        projectilesPool.Peek().transform.rotation = shotStartPlaces[1].transform.rotation;
        projectilesPool.Peek().gameObject.SetActive(true);
        projectilesPool.Dequeue();
        timer = 0f;
    }

    public void ReleaseProjectile(BullyProjectile projectile)
    {
        projectilesPool.Enqueue(projectile);
    }
}