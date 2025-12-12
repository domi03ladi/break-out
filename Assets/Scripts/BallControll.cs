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

    [Header("Squash & Stretch")]
    [SerializeField] private Transform ballModel;
    [SerializeField] private float squashReturnSpeed = 2f;

    [Header("Impact Squash Settings")]
    [SerializeField] private Vector3 impactSquash = new Vector3(1.5f, 0.7f, 1.2f);

    private AudioSource audioSource;


    private float fixedSpeed = 8f;
    private const float MIN_VERTICAL_SPEED = 1.0f;

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
        // Wenn kinematisch (eingefroren oder wartend), keine Geschwindigkeit berechnen
        if (m_Rigidbody.isKinematic)
        {
            return;
        }

        Vector3 currentVelocity = m_Rigidbody.velocity;

        // Nur prüfen, wenn der Ball sich überhaupt bewegt
        if (currentVelocity.sqrMagnitude > 0.01f)
        {
            // 1. Verhindern, dass der Ball horizontal stecken bleibt
            // Wir prüfen, ob die Y-Geschwindigkeit (hoch/runter) zu klein ist
            if (Mathf.Abs(currentVelocity.y) < MIN_VERTICAL_SPEED)
            {
                // Wir bestimmen das Vorzeichen (geht er gerade leicht hoch oder runter?)
                // Wenn er exakt 0 ist (horizontal), schicken wir ihn nach unten (-1)
                float sign = (currentVelocity.y == 0) ? -1f : Mathf.Sign(currentVelocity.y);

                // Wir erzwingen die minimale Y-Geschwindigkeit
                currentVelocity.y = sign * MIN_VERTICAL_SPEED;
            }

            // 2. Konstante Geschwindigkeit erzwingen
            // Hier nutzen wir den korrigierten Vektor und normalisieren ihn wieder
            m_Rigidbody.velocity = currentVelocity.normalized * fixedSpeed;
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

    void OnCollisionEnter(Collision collision)
    {
        StopAllCoroutines();
        StartCoroutine(ImpactSquash());
        GameObject hitObject = collision.gameObject;

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

    private IEnumerator ImpactSquash()
    {
        Vector3 squashed = impactSquash;
        Vector3 normal = Vector3.one;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * squashReturnSpeed;
            ballModel.localScale = Vector3.Lerp(squashed, normal, t);
            yield return null;
        }
    }
}

