using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            // 1. Die zentrale Funktion im GameManager aufrufen
            // Der GameManager muss das Singleton-Muster verwenden (GameManager.Instance)
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseLife();
            }

            // 2. Den Ball deaktivieren oder zerstören, damit er neu gestartet werden kann
            // Du kannst den Ball auch direkt zerstören, damit der GameManager einen neuen erstellt.
            Destroy(other.gameObject);
        }
    }

}
