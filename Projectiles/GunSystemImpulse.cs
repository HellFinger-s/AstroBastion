using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystemImpulse : BaseProjectile
{
    public float scalingSpeed;
    public float maxScale;

    public void Update()
    {
        transform.localScale += Vector3.one * scalingSpeed * Time.deltaTime;
        if (transform.localScale.x > maxScale)
        {
            Destroy();
        }
    }

    private void Destroy()
    {
        transform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }
}
