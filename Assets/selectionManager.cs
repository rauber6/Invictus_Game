using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    public Texture2D selectionTexture;
    private bool isSelecting = false;
    private bool isUIDragging = false; // Flag to indicate if a UI drag is happening
    private Vector3 mousePosition1;

    private List<Behaviour> selectedPlayers = new List<Behaviour>();
    private Vector3 groupTarget;

    void Update()
    {
        if (isUIDragging)
        {
            Debug.Log("UI Dragging, skipping selection");
            return; // Ignore input if a UI drag is happening
        }

        // Start selecting
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                isSelecting = true;
                mousePosition1 = Input.mousePosition;
                Debug.Log("Selection started at: " + mousePosition1);
            }
            else
            {
                Debug.Log("Click on UI element detected, not starting selection");
            }
        }

        // End selecting
        if (Input.GetMouseButtonUp(0))
        {
            if (isSelecting)
            {
                isSelecting = false;
                Debug.Log("Selection ended at: " + Input.mousePosition);

                // Deselect all players first
                foreach (Behaviour player in selectedPlayers)
                {
                    player.SetSelected(false);
                }

                selectedPlayers.Clear();

                foreach (Behaviour player in FindObjectsOfType<Behaviour>())
                {
                    if (IsWithinSelectionBounds(player.gameObject))
                    {
                        selectedPlayers.Add(player);
                        player.SetSelected(true); // Custom method to set the player as selected
                        Debug.Log($"{player.gameObject.name} is selected.");
                    }
                    else
                    {
                        Debug.Log($"{player.gameObject.name} is NOT within selection bounds.");
                    }
                }
            }
        }

        // Set group target when right mouse button is clicked
        if (Input.GetMouseButtonDown(1) && selectedPlayers.Count > 0)
        {
            SetGroupTarget();
        }
    }

    void OnGUI()
    {
        if (isSelecting)
        {
            // Create a rect from both mouse positions
            var rect = Utils.GetScreenRect(mousePosition1, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    public bool IsWithinSelectionBounds(GameObject gameObject)
    {
        var camera = Camera.main;
        var viewportBounds = Utils.GetViewportBounds(camera, mousePosition1, Input.mousePosition);
        bool isWithinBounds = viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));

        Debug.Log($"{gameObject.name} is within selection bounds: {isWithinBounds}");
        return isWithinBounds;
    }

    void SetGroupTarget()
    {
        // Get the mouse position in world coordinates
        groupTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        groupTarget.z = 0;

        // Calculate the centroid of the selected players
        Vector3 centroid = Vector3.zero;
        foreach (var player in selectedPlayers)
        {
            centroid += player.transform.position;
        }
        centroid /= selectedPlayers.Count;

        // Set individual targets for each player
        foreach (var player in selectedPlayers)
        {
            Vector3 offset = player.transform.position - centroid;
            Vector3 individualTarget = groupTarget + offset;
            player.SetTargetPosition(individualTarget);
        }
    }

    public void SetUIDragging(bool isDragging)
    {
        Debug.Log("SetUIDragging: " + isDragging);
        isUIDragging = isDragging;
    }
}
