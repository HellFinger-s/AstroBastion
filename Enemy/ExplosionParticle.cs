using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionParticle : MonoBehaviour
{
    private ExplosionParticlesManager parent;
    public ParticleSystem particle;

    public void Init(ExplosionParticlesManager manager)
    {
        parent = manager;
    }

    private void OnParticleSystemStopped()
    {
        parent.ReturnParticle(this);
    }

    public void Play(Vector3 position, float scale)
    {
        transform.position = position;
        transform.localScale = Vector3.one * scale;
        particle.Play();
    }
}
