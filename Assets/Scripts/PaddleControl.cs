using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleControl : MonoBehaviour
{

    [SerializeField] private float paddleSpeed = 30f;
    [SerializeField] private float minX = -7.5f;  // linke Grenze
    [SerializeField] private float maxX = 7.5f;   // rechte Grenze

    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        float horizontal = Input.GetAxis("Horizontal");

        // Neue Position berechnen
        Vector3 newPosition = transform.position +
                              new Vector3(horizontal * paddleSpeed * Time.deltaTime, 0f, 0f);

        // X begrenzen
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
