using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the sprite moves
    public float separationDistance = 1f; // Minimum distance to maintain from other players
    public float separationForce = 5f; // Strength of the separation force
    public GameObject xMarkerPrefab; // Prefab for the "X" marker
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isSelected = false; // Track if the player is selected
    private GameObject currentXMarker;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        // Set the initial target position to the sprite's current position
        targetPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Disable gravity
        Debug.Log("Behaviour script has started."); // Test log message
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected)
        {
            // Detect mouse click and set target position
            if (Input.GetMouseButtonDown(1))
            {
                // SetTargetPosition() is now called from SelectionManager
            }
        }

        // Move the sprite towards the target position
        MoveToTarget();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        Debug.Log($"{gameObject.name} selected: {selected}");
        GetComponent<SpriteRenderer>().color = selected ? Color.green : Color.white;
    }

    public void SetTargetPosition(Vector3 newTarget)
    {
        targetPosition = newTarget;
        isMoving = true;

        // Highlight the destination with an "X"
        HighlightDestination(newTarget);
    }

    void MoveToTarget()
    {
        if (isMoving)
        {
            // Calculate the direction to the target
            Vector2 direction = (targetPosition - transform.position).normalized;
            Vector2 separation = CalculateSeparation();

            // Combine the movement towards the target and the separation force
            Vector2 movement = direction * moveSpeed + separation * separationForce;

            // Move the sprite towards the target position using physics
            rb.velocity = movement;

            // Check if the sprite has reached the target position
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
                rb.velocity = Vector2.zero;

                // Remove the "X" marker once the sprite reaches the target
                if (currentXMarker != null)
                {
                    Destroy(currentXMarker);
                }
            }
        }
    }

    void HighlightDestination(Vector3 position)
    {
        // Destroy the previous "X" marker if it exists
        if (currentXMarker != null)
        {
            Destroy(currentXMarker);
        }

        // Instantiate a new "X" marker at the target position
        currentXMarker = Instantiate(xMarkerPrefab, position, Quaternion.identity);

        // Adjust the z position to be slightly lower than the player
        Vector3 markerPosition = currentXMarker.transform.position;
        markerPosition.z = transform.position.z + 1; // Set Z position to be lower than the player
        currentXMarker.transform.position = markerPosition;
    }

    Vector2 CalculateSeparation()
    {
        Vector2 separation = Vector2.zero;
        int count = 0;

        foreach (Behaviour other in FindObjectsOfType<Behaviour>())
        {
            if (other != this)
            {
                float distance = Vector2.Distance(transform.position, other.transform.position);
                if (distance < separationDistance)
                {
                    Vector2 diff = (Vector2)(transform.position - other.transform.position);
                    diff /= distance; // Weight by distance
                    separation += diff;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            separation /= count;
        }

        return separation;
    }
}
