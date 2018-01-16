using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public float speedX;
    public float speedY;

    void Start()
    {
        InvokeRepeating("ChangeDirections", 0, 5.0f);
    }

    void Update () {
        Vector3 pos = transform.position;

        if (transform.position.y < -11 || transform.position.y > 6)
        {
            pos.x += speedX * Time.deltaTime;
        }
        else
        {
            pos.y += speedY * Time.deltaTime;
        }

        transform.position = pos;

    }

    private void ChangeDirections()
    {
        speedX = speedX * -1;
        speedY = speedY * -1;
    }
}
