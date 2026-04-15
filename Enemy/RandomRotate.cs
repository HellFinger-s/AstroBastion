using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotate : MonoBehaviour
{
    public float speed = 0.5f;

    public Vector3 rotateVector;

    private void Awake()
    {
        rotateVector = rotateVector.normalized;
    }

    public void Update()
    {
        transform.Rotate(rotateVector, speed * Time.deltaTime);
    }
}
