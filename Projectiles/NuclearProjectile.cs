using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NuclearProjectile : BaseProjectile
{
    public float lifetime = 5f;
    public float speed = 50f;
    //[HideInInspector]
    public float accelerationValue = 0f;
    public float rotationSpeed = 5f;

    private float timer = 0f;

    private float currentSpeed;

    [HideInInspector]
    public BaseEnemy target;

    private Vector3 lookDirection;

    private float checkTimer = 0f;
    private float CheckInterval = 0.5f;
    private bool releasing = false;

    private void Awake()
    {
        currentSpeed = speed;
    }


    private void Update()
    {
        if (!releasing)
        {
            if (target)
            {
                lookDirection = target.transform.position - transform.position;
                if (checkTimer > CheckInterval)
                {
                    CheckTarget();
                }
            }
            else
            {
                lookDirection = transform.forward;
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDirection, transform.up), rotationSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            transform.position += transform.forward * currentSpeed * Time.deltaTime;
            if (accelerationValue > 1)
            {
                currentSpeed += accelerationValue * Time.deltaTime;
            }
            if (timer > lifetime)
            {
                Destroy();
            }
        }
    }

    private void Destroy()
    {
        // Rocket explosion particles
        if (!releasing)
        {
            releasing = true;
            timer = 0f;
            parentPool.ReleaseProjectile(this);
            accelerationValue = -1f;
            currentSpeed = speed;
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
        if (!target.isActiveAndEnabled)
        {
            Destroy();
        }
    }
}
