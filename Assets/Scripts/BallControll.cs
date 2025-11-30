using UnityEngine;
using System.Collections;

public class BallControl : MonoBehaviour
{
    Rigidbody m_Rigidbody;

    
    [SerializeField] private float startDelay = 1.0f;
    [SerializeField] private float launchMagnitude = 10f;
    [SerializeField] private float minDirectionX = 0.5f;

    private float fixedSpeed = 8f;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        m_Rigidbody.isKinematic = true;
        LaunchDelayed();
    }

    void FixedUpdate()
    {
        if (m_Rigidbody.isKinematic)
        {
            return;
        }

        if (m_Rigidbody.velocity.sqrMagnitude > 0.01f)
        {
            m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * fixedSpeed;
        }
    }

    // Method is used by GameManager to start the ball
    public void LaunchDelayed()
    {
        StartCoroutine(DelayedLaunchRoutine());
    }

    private IEnumerator DelayedLaunchRoutine()
    {
        // Deleay
        yield return new WaitForSeconds(startDelay);

        // Release the ball and start
        m_Rigidbody.isKinematic = false; // active physics
        m_Rigidbody.velocity = Vector3.zero; // delete old speed

        // Force to push the ball down
        m_Rigidbody.AddForce(Vector3.down * launchMagnitude, ForceMode.VelocityChange);
    }
}