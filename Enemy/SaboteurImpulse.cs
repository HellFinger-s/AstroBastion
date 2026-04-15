using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaboteurImpulse : MonoBehaviour
{
    public Transform parent;
    public float scaleFactor;
    public float growTime;

    private bool grow = false;
    private float timer = 0f;

    private void Update()
    {
        if (grow)
        {
            timer += Time.deltaTime;
            parent.localScale += Vector3.one * scaleFactor * Time.deltaTime;
            if (timer > growTime)
            {
                grow = false;
                timer = 0f;
                parent.localScale = Vector3.zero;
            }    
        }
    }

    public void StartImpulse()
    {
        grow = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BaseTower>(out BaseTower tower))
        {
            Debug.LogError("Detected");
            tower.Reboot();
        }
    }
}
