using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaProjectile : BaseProjectile
{
    public float lifetime = 5f;
    public float speed = 700f;
    public float explosionDelay = 3f;
    [HideInInspector]
    public float scalingFactor = 0.5f;

    private float timer = 0f;
    private bool mooving = true;
    private bool scaling = false;


    private void Update()
    {
        if (mooving)
        {
            timer += Time.deltaTime;
            transform.position += transform.forward * speed * Time.deltaTime;
            if (timer > lifetime)
            {
                Explosion();
            }
        }
        
        if (scaling)
        {
            transform.localScale += Vector3.one * scalingFactor * Time.deltaTime;
        }
    }


    private void Explosion()
    {
        mooving = false;
        scaling = true;
        StartCoroutine(Destroy(explosionDelay));
    }


    private IEnumerator Destroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        parentPool.ReleaseProjectile(this);
        timer = 0f;
        transform.localScale = Vector3.one;
        mooving = true;
        scaling = false;
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        Explosion();
    }
}
