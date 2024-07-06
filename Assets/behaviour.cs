using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Behaviour : MonoBehaviour
{
    public GameObject xMarkerPrefab; // Prefab for the "X" marker
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isSelected = false; // Track if the player is selected
    private GameObject currentXMarker;
    private NavMeshAgent navAgent;

    void Start()
    {
        // Set the initial target position to the sprite's current position
        targetPosition = transform.position;
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false; // Disable rotation for 2D
        navAgent.updateUpAxis = false; // Disable up-axis for 2D
        Debug.Log("Behaviour script has started."); // Test log message
    }

    void Update()
    {
        if (isSelected)
        {
            // Detect mouse click and set target position
            if (Input.GetMouseButtonDown(1))
            {
                SelectionManager.Instance.MoveSelectedPlayersToPosition(GetTargetPosition());
            }
        }

        // Update rotation to face movement direction
        if (navAgent.velocity.sqrMagnitude > 0.1f)
        {
            UpdateRotation();
        }

        // Check if the sprite has reached the target position
        if (isMoving && !navAgent.pathPending)
        {
            if (navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f)
                {
                    isMoving = false;

                    // Remove the "X" marker once the sprite reaches the target
                    if (currentXMarker != null)
                    {
                        Destroy(currentXMarker);
                    }
                }
            }
        }
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

        // Set the target position for the NavMeshAgent
        navAgent.SetDestination(targetPosition);

        // Highlight the destination with an "X"
        HighlightDestination(newTarget);
    }

    Vector3 GetTargetPosition()
    {
        // Get the mouse position in world coordinates
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure the target position is on the same z-plane as the sprite
        return mousePosition;
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

    void UpdateRotation()
    {
        Vector3 direction = navAgent.velocity.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90); // Adjust by 90 degrees so the top of the sprite faces the movement direction
    }
}
