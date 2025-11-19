using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedSpeed : MonoBehaviour
{

    Rigidbody m_Rigidbody;
    public float m_Thrust = 20f;


    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.AddForce(Vector3.down); //inital push to the Ball (RB) so it starts moving without gravity
    }

    // pyhsics are manipulated in the fixed update -> compensates for framerate
    void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        float fixedSpeed = 10f;
        rb.velocity = rb.velocity.normalized * fixedSpeed;
    }
}
