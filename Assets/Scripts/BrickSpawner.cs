
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    public GameObject brick;
    public float width;
    public float height;
    public float columnGap = 0.1f;
    public float rowGap = 0.1f;

    private Vector3 highestPoint;
    // Start is called before the first frame update
    void Start()
    {
        // Assuming transform.position is the desired bottom-left corner of the grid area.
        highestPoint = new Vector3(transform.position.x, transform.position.y + height, transform.position.z); 

        float maxX = highestPoint.x + width;
        float maxY = transform.position.y; // Bottom edge Y

        float brickWidth = brick.GetComponent<Renderer>().bounds.size.x;
        float brickHeight = brick.GetComponent<Renderer>().bounds.size.y;


        for (float x = highestPoint.x; x < maxX; x += brickWidth + columnGap)
        {
            for (float y = highestPoint.y; y > maxY; y -= brickHeight + rowGap)
            {
                // Calculate the center position of the brick
                Instantiate(brick, new Vector3(x + brickWidth / 2, y - brickHeight / 2 , transform.position.z), transform.rotation);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        highestPoint = new Vector3(transform.position.x, transform.position.y + height, transform.position.z); 
        // Top-Left Corner is highestPoint
        Vector3 topLeft = highestPoint; 

        // Define the other corners based on the size (width, height)
        Vector3 topRight = new Vector3(topLeft.x + width, topLeft.y, topLeft.z);
        Vector3 bottomLeft = new Vector3(topLeft.x, topLeft.y - height, topLeft.z);
        Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y, topLeft.z);

        // Draw the lines for the bounding box
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
