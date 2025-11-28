
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    public GameObject brick;
    public float width;
    public float height;
    public float columnGap = 0.1f;
    public float rowGap = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 highestPoint = new Vector3(transform.position.x - width / 2, transform.position.y + height / 2, transform.position.z);
        float maxX = highestPoint.x + width;
        float maxY = highestPoint.y - height;
        float brickWidth = brick.GetComponent<Renderer>().bounds.size.x;
        float brickHeight = brick.GetComponent<Renderer>().bounds.size.y;

        for (float x = highestPoint.x; x < maxX; x += brickWidth + columnGap)
        {
            for (float y = highestPoint.y; y > maxY; y -= brickHeight + rowGap)
            {
                Instantiate(brick, new Vector3(x + brickWidth / 2, y - brickHeight / 2, transform.position.z), transform.rotation);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
