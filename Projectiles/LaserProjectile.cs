using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserProjectile : BaseProjectile
{
    public float lifetime;
    public float speed = 1000;

    private float timer = 0f;


    private void Update()
    {
        timer += Time.deltaTime;
        transform.position += transform.forward * speed * Time.deltaTime;
        if (timer > lifetime)
        {
            Destroy();
        }
    }

    private void Destroy()
    {
        parentPool.ReleaseProjectile(this);
        timer = 0f;
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        Destroy();
    }
}
