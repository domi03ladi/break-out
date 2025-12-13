using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{

    [SerializeField]
    private GameObject explosionPrefab;

    public void Explode()
    {
        if (explosionPrefab == null)
        {
            Debug.LogWarning("Explosion Prefab is missing " + gameObject.name);
            return;
        }

        GameObject explosionInstance = Instantiate(
            explosionPrefab,
            transform.position,
            Quaternion.identity
        );

        ParticleSystem ps = explosionInstance.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }

        float totalDuration = ps != null ? ps.main.duration + ps.main.startLifetime.constantMax : 5f;
        Destroy(explosionInstance, totalDuration);
    }
}
