using UnityEngine;
using System.Collections;

public class BallControl : MonoBehaviour
{
    Rigidbody m_Rigidbody;

    [SerializeField] private float startDelay = 1.0f;
    [SerializeField] private float launchMagnitude = 10f;

    [Header("Sounds")]
    [SerializeField] private AudioClip destroyBrickSound;
    [SerializeField] private AudioClip hitWallSound;

    [Header("Visual Effects")]
    [SerializeField] private Renderer ballRenderer; // Der Renderer auf dem Ball-Objekt
    [SerializeField] private Material fireMaterial; // Dein 'Fire_Ball' Material Asset
    [SerializeField] private Material frozenMaterial; // Dein 'Frozen_Ball' Material Asset

    private AudioSource audioSource;


    private float fixedSpeed = 8f;

    private Vector3 savedVelocity; // To store direction and speed
    private bool isFrozen = false; // To track state

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        m_Rigidbody.isKinematic = true;
        LaunchDelayed();
        SetBallVisual(fireMaterial);
    }

    void FixedUpdate()
    {
        // If kinematic (frozen or waiting to start), do not calculate speed
        if (m_Rigidbody.isKinematic)
        {
            return;
        }

        // Enforce constant speed
        if (m_Rigidbody.velocity.sqrMagnitude > 0.01f)
        {
            m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * fixedSpeed;
        }
    }

    [ContextMenu("Toggle Freeze")]
    public void ToggleFreeze()
    {
        if (isFrozen)
        {
            // Unfreeze: Restore Physics and Velocity
            m_Rigidbody.isKinematic = false;
            m_Rigidbody.velocity = savedVelocity; 
            isFrozen = false;
            SetBallVisual(fireMaterial);
        }
        else
        {
            // Freeze: Save Velocity and Stop Physics
            savedVelocity = m_Rigidbody.velocity; 
            m_Rigidbody.isKinematic = true;
            m_Rigidbody.velocity = Vector3.zero; // Visually stop immediately
            isFrozen = true;
            SetBallVisual(frozenMaterial);
        }
    }

    // Füge dies zu BallControl hinzu:
    void OnCollisionEnter(Collision collision)
    {
        // Die 'Collision'-Variable enthält Informationen über das getroffene Objekt.
        GameObject hitObject = collision.gameObject;

        // Überprüfe, ob das getroffene Objekt ein Block ist (oder ein anderes relevantes Tag hat).
        if (hitObject.CompareTag("Cube"))
        {
            if (audioSource != null && destroyBrickSound != null)
            {
                audioSource.PlayOneShot(destroyBrickSound);
            }

        } else if (hitObject.CompareTag("Border"))
        {
            if (audioSource != null && hitWallSound != null)
            {
                audioSource.PlayOneShot(hitWallSound);
            }
        }
    }


    private void SetBallVisual(Material material)
    {
        if (ballRenderer != null && material != null)
        {
            ballRenderer.material = material;
        }
    }

    public void LaunchDelayed()
    {
        StartCoroutine(DelayedLaunchRoutine());
    }

    private IEnumerator DelayedLaunchRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        m_Rigidbody.isKinematic = false;
        m_Rigidbody.velocity = Vector3.zero;

        float x = 0f;
        float y = -1f;

        Vector3 initialDirection = new Vector3(x, y, 0f).normalized;

        m_Rigidbody.AddForce(initialDirection * launchMagnitude, ForceMode.VelocityChange);
    }
}