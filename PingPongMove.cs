using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongMove : MonoBehaviour
{
    [SerializeField] private float distance = 10f;
    [SerializeField] private float speed = 1f;

    private Vector3 _startLocalPosition;

    private void Awake()
    {
        _startLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        float offset = Mathf.PingPong(Time.time * speed, distance * 2f) - distance;

        Vector3 newPos = _startLocalPosition;
        newPos.y += offset;

        transform.localPosition = newPos;
    }
}
