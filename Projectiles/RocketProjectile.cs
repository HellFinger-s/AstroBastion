using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProjectile : BaseProjectile
{
    public float lifetime = 5f;
    public float speed = 700f;
    public float rotationSpeed = 5f;

    private float timer = 0f;

    [HideInInspector]
    public BaseEnemy targetEnemy;

    private Vector3 lookDirection;
    public Vector3 prevTargetPosition;

    private float checkTimer = 0f;
    private float CheckInterval = 0.5f;
    private bool releasing = false;

    private void Update()
    {
        if (!releasing)
        {
            checkTimer += Time.deltaTime;
            if (targetEnemy)
            {
                lookDirection = targetEnemy.transform.position - transform.position;
                if (checkTimer > CheckInterval)
                {
                    CheckTarget();
                }
            }
            else
            {
                lookDirection = prevTargetPosition - transform.position;
                if ((transform.position - prevTargetPosition).sqrMagnitude < 1e-3)
                {
                    Destroy();
                }
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDirection, transform.up), rotationSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            transform.position += transform.forward * speed * Time.deltaTime;
            if (timer > lifetime)
            {
                Destroy();
            }
        }
    }

    private void Destroy()
    {
        if (!releasing)
        {
            releasing = true;
            parentPool.ReleaseProjectile(this);
            timer = 0f;
            releasing = false;
            gameObject.SetActive(false);
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        Destroy();
    }

    private void CheckTarget()
    {
        if (!targetEnemy.gameObject.activeSelf)
        {
            targetEnemy = null;
        }
        else
        {
            prevTargetPosition = targetEnemy.transform.position;
        }
    }
}
