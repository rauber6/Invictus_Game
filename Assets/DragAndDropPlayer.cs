using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropPlayer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject playerPrefab; // Assign the player prefab in the inspector

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private SelectionManager selectionManager;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        selectionManager = FindObjectOfType<SelectionManager>(); // Find the SelectionManager in the scene
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f; // Make the icon semi-transparent while dragging
        canvasGroup.blocksRaycasts = false; // Allow raycasts to pass through the icon
        if (selectionManager != null)
        {
            selectionManager.SetUIDragging(true); // Notify the SelectionManager that a UI drag is happening
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; // Move the icon with the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f; // Make the icon fully opaque
        canvasGroup.blocksRaycasts = true; // Block raycasts

        // Convert screen position to world position
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0;

        // Instantiate the player prefab at the drop position
        Instantiate(playerPrefab, worldPosition, Quaternion.identity);

        // Reset the icon's position
        rectTransform.anchoredPosition = Vector2.zero;

        if (selectionManager != null)
        {
            selectionManager.SetUIDragging(false); // Notify the SelectionManager that the UI drag has ended
        }
    }
}
