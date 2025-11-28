using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleControl : MonoBehaviour
{

    [SerializeField] private float paddleSpeed = 30f;

    // Spielfeldränder: Im Inspector auf die X-Positionen der Wände einstellen
    [SerializeField] private float leftBoundary = -9.0f;
    [SerializeField] private float rightBoundary = 9.0f;

    private float halfPaddleWidth;  // Halbe Breite des Paddles
    private float minX;             // Endgültige linke Grenze für den Mittelpunkt des Paddles
    private float maxX;             // Endgültige rechte Grenze für den Mittelpunkt des Paddles


    void Start()
    {
        // 1. Die X-Skalierung gibt die volle Breite an. Wir benötigen die Hälfte davon.
        halfPaddleWidth = transform.localScale.x / 2f;

        // 2. Berechnung der endgültigen Mittelpunkt-Grenzen (nur einmal nötig!)
        // minX: Linke Wand + halbe Breite (Mittelpunkt darf nicht über diesen Punkt hinaus)
        minX = leftBoundary + halfPaddleWidth;
        // maxX: Rechte Wand - halbe Breite (Mittelpunkt darf nicht über diesen Punkt hinaus)
        maxX = rightBoundary - halfPaddleWidth;

        // Optional: Hilfreich zur Fehlersuche.
        // Debug.Log("Begrenzungen gesetzt: minX=" + minX + ", maxX=" + maxX);
    }


    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        float horizontal = Input.GetAxis("Horizontal");

        // Neue Position berechnen
        Vector3 newPosition = transform.position;
        newPosition.x += horizontal * paddleSpeed * Time.deltaTime;

        // X begrenzen (Hier verwenden wir die einmal berechneten minX/maxX Werte)
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