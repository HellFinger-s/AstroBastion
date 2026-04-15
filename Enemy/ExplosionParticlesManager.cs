using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionParticlesManager : LazySingleton<ExplosionParticlesManager>
{
    public int poolCapacity = 100;
    public ExplosionParticle explosionParticle;

    private Queue<ExplosionParticle> pool = new Queue<ExplosionParticle> { };


    protected override bool CheckDependencies()
    {
        return true;
    }

    protected override void OnDependenciesReady()
    {
        for (int i = 0; i < poolCapacity; i++)
        {
            var particle = GameObject.Instantiate(explosionParticle);
            pool.Enqueue(particle);
            particle.Init(this);
            particle.gameObject.SetActive(false);
        }
        return;
    }

    public void ReturnParticle(ExplosionParticle particle)
    {
        pool.Enqueue(particle);
    }

    public void SpawnParticle(Vector3 position, float scale)
    {
        pool.Peek().gameObject.SetActive(true);
        pool.Peek().Play(position, scale);
        pool.Dequeue();
    }
}
