using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleControl : MonoBehaviour
{

    [SerializeField] private float paddleSpeed = 35f;
    [SerializeField] private AudioClip hitSound;

    private AudioSource audioSource;

    private Rigidbody rb;

    // playing field boundaries
    [SerializeField] private float leftBoundary = -9.0f;
    [SerializeField] private float rightBoundary = 9.0f;

    private float halfPaddleWidth;
    private float minX;             // left border
    private float maxX;             // right border


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>(); // Rigidbody holen

        // SICHERHEIT: Das Rigidbody muss Kinematisch sein (wird vom Code gesteuert)
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }


    void Start()
    {
        // getting the center of the paddle
        halfPaddleWidth = transform.localScale.x / 2f;

        minX = leftBoundary + halfPaddleWidth;
        maxX = rightBoundary - halfPaddleWidth;
    }

    void FixedUpdate()
    {
        if (Time.timeScale == 0f)
            return;

        float horizontal = Input.GetAxis("Horizontal");

        Vector3 targetPosition = transform.position;
        targetPosition.x += horizontal * paddleSpeed * Time.fixedDeltaTime;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);

        if (rb != null)
        {
            rb.MovePosition(targetPosition);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody ballRb = collision.rigidbody;

        if (ballRb != null)
        {

            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }

            // Get the exact point where the ball hits the paddle
            Vector3 hitpoint = collision.contacts[0].point;
            float hitfactor = (hitpoint.x - transform.position.x) / transform.localScale.x;
            Vector3 newDirection = new Vector3(hitfactor, 1, 0).normalized; // 1 is y direction (upwards), z is 0
            ballRb.velocity = newDirection; // overwritting the physics (rigidbody) to refkect on impact from player controll
        }

    }
}