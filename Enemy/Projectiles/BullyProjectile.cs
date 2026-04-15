using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullyProjectile : MonoBehaviour
{
    public float lifetime;
    public float speed = 1000;
    public Bully bully;

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
        bully.ReleaseProjectile(this);
        timer = 0f;
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        Destroy();
    }
}
