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
        // Warte für die definierte Zeit
        yield return new WaitForSeconds(startDelay);

        m_Rigidbody.isKinematic = false;
        m_Rigidbody.velocity = Vector3.zero;

        // NEU: Zufällige diagonale Startrichtung berechnen
        // 1. Seitliche Richtung: Zufällig nach links oder rechts (nutzt minDirectionX)
        float x = Random.Range(minDirectionX, 1f) * (Random.value < 0.5f ? -1f : 1f);

        // 2. Vertikale Richtung: Nach oben (positive Y), da der Ball unten startet.
        // Wenn du den Ball von der Mitte nach unten starten lassen willst, verwende Vector3.down, 
        // aber dann musst du die Blöcke darunter anordnen. Hier verwenden wir Vector3.up (y=1).
        float y = 1f;

        Vector3 initialDirection = new Vector3(x, y, 0f).normalized;

        // Wende Kraft an
        // Wir verwenden jetzt launchMagnitude als Gesamtkraft, nicht nur für Y.
        m_Rigidbody.AddForce(initialDirection * launchMagnitude, ForceMode.VelocityChange);
    }
}