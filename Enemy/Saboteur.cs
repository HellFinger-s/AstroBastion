using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saboteur : MonoBehaviour
{
    public float impulseInterval;
    public SaboteurImpulse impulse;

    private float timer = 0f;
    
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > impulseInterval)
        {
            timer = 0f;
            impulse.StartImpulse();
        }
    }
}
