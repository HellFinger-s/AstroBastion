using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform player;

    public float rotationSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            transform.position = player.position;

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                player.rotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
