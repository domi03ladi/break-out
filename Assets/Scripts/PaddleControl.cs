using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleControl : MonoBehaviour
{

    [SerializeField] private float paddleSpeed = 35f;

    // playing field boundaries
    [SerializeField] private float leftBoundary = -9.0f;
    [SerializeField] private float rightBoundary = 9.0f;

    private float halfPaddleWidth;
    private float minX;             // left border
    private float maxX;             // right border


    void Start()
    {
        // getting the center of the paddle
        halfPaddleWidth = transform.localScale.x / 2f;

        minX = leftBoundary + halfPaddleWidth;
        maxX = rightBoundary - halfPaddleWidth;
    }


    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        float horizontal = Input.GetAxis("Horizontal");

        // calculate new position
        Vector3 newPosition = transform.position;
        newPosition.x += horizontal * paddleSpeed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

        transform.position = newPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody ballRb = collision.rigidbody;

        if (ballRb != null)
        {
            // Get the exact point where the ball hits the paddle
            Vector3 hitpoint = collision.contacts[0].point;
            float hitfactor = (hitpoint.x - transform.position.x) / transform.localScale.x;
            Vector3 newDirection = new Vector3(hitfactor, 1, 0).normalized; // 1 is y direction (upwards), z is 0
            ballRb.velocity = newDirection; // overwritting the physics (rigidbody) to refkect on impact from player controll
        }

    }
}