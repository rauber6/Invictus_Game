using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    private List<Behaviour> selectedPlayers = new List<Behaviour>();
    private bool isUIDragging = false; // Track if a UI drag is happening

    private bool isSelecting = false;
    private Vector3 mousePosition1;
    private Vector3 mousePosition2;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (isUIDragging)
        {
            return; // Do not select while dragging UI
        }

        if (Input.GetMouseButtonDown(0))
        {
            isSelecting = true;
            mousePosition1 = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false;
            SelectObjects();
        }

        if (isSelecting)
        {
            mousePosition2 = Input.mousePosition;
        }
    }

    void OnGUI()
    {
        if (isSelecting)
        {
            // Create a rectangle with the mouse positions
            Rect rect = Utils.GetScreenRect(mousePosition1, mousePosition2);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    public void SelectPlayer(Behaviour player)
    {
        if (!selectedPlayers.Contains(player))
        {
            selectedPlayers.Add(player);
            player.SetSelected(true);
        }
    }

    public void DeselectPlayer(Behaviour player)
    {
        if (selectedPlayers.Contains(player))
        {
            selectedPlayers.Remove(player);
            player.SetSelected(false);
        }
    }

    public void DeselectAll()
    {
        foreach (var player in selectedPlayers)
        {
            player.SetSelected(false);
        }
        selectedPlayers.Clear();
    }

    public void MoveSelectedPlayersToPosition(Vector3 targetPosition)
    {
        if (selectedPlayers.Count == 0) return;

        // Calculate the center point of the selected players
        Vector3 centerPoint = Vector3.zero;
        foreach (var player in selectedPlayers)
        {
            centerPoint += player.transform.position;
        }
        centerPoint /= selectedPlayers.Count;

        // Calculate and set the target position for each player based on its offset from the center
        foreach (var player in selectedPlayers)
        {
            Vector3 offset = player.transform.position - centerPoint;
            player.SetTargetPosition(targetPosition + offset);
        }
    }

    public void SetUIDragging(bool isDragging)
    {
        isUIDragging = isDragging;
    }

    private void SelectObjects()
    {
        Bounds selectionBounds = Utils.GetViewportBounds(Camera.main, mousePosition1, mousePosition2);
        DeselectAll();

        foreach (var player in FindObjectsOfType<Behaviour>())
        {
            if (selectionBounds.Contains(Camera.main.WorldToViewportPoint(player.transform.position)))
            {
                SelectPlayer(player);
            }
        }
    }
}
