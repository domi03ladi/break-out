using UnityEngine;

public class EyeFollowPhysics : MonoBehaviour
{
    [Tooltip("Tag of the object to follow.")]
    public string targetTag = "Ball";
    [Tooltip("Maximum rotation from the initial forward direction.")]
    public float maxAngle = 30f;

    private Transform target;
    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;
        FindTarget();
    }

    void Update()
    {
        if (target == null)
        {
            FindTarget();
            if (target == null) return;
        }

        // Local position of target relative to the eye
        Vector3 localPos = transform.InverseTransformPoint(target.position);

        // Clamp local position to max angle
        // Using simple proportional mapping to maxAngle
        float clampedX = Mathf.Clamp(localPos.y, -1f, 1f) * maxAngle; // pitch
        float clampedY = Mathf.Clamp(localPos.x, -1f, 1f) * maxAngle; // yaw

        // Apply rotation relative to initial rotation
        Quaternion rotation = initialRotation * Quaternion.Euler(-clampedX, clampedY, 0f);
        transform.rotation = rotation;
    }

    private void FindTarget()
    {
        GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
        if (targetObject != null)
        {
            target = targetObject.transform;
            Rigidbody rb = target.GetComponent<Rigidbody>();
            if (rb != null)
                rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }
}
