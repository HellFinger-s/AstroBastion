using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTowerProjectile : BaseProjectile
{
    public float lifetime;
    public float speed = 1000;

    private float timer = 0f;

    public override void Setup(float additiveSpeed)
    {
        speed += additiveSpeed;
    }

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
        if (other.TryGetComponent<Shield>(out Shield shield))
        {
            if (!effects.ContainsKey(ProjectileEffectsNames.ShieldIgnore) || !effects[ProjectileEffectsNames.ShieldIgnore].enabled)
            {
                Destroy();
            }
        }
        else
        {
            Destroy();
        }
    }

    public override float GetSpeed()
    {
        return speed;
    }
}
