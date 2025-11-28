using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour
{

    Rigidbody m_Rigidbody;
    public float m_Thrust = 20f;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        m_Rigidbody.AddForce(Vector3.down); //inital push to the Ball (RB) so it starts moving without gravity
    }

    void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        float fixedSpeed = 10f;
        rb.velocity = rb.velocity.normalized * fixedSpeed;
    }
}