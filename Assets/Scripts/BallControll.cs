using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour
{
    // Kein Singleton mehr nötig, da der GameManager ihn direkt erstellt

    Rigidbody m_Rigidbody;
    public float m_Thrust = 20f;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.AddForce(Vector3.down); //inital push to the Ball (RB) so it starts moving without gravity
    }

    // Wir können Update() löschen, da keine Input-Logik mehr benötigt wird

    void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        float fixedSpeed = 10f;
        rb.velocity = rb.velocity.normalized * fixedSpeed;
    }
}