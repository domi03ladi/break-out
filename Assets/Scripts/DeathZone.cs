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
        Debug.Log(other.gameObject.name + " entered DeathZone");
        if (other.CompareTag("Ball"))
        {
            // Function is used in GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseLife();
            }

            // destroy ball to create a new one
            Destroy(other.gameObject);
        }else if (other.CompareTag("Answers"))
        {
            Destroy(other.gameObject);
        }
    }

}
