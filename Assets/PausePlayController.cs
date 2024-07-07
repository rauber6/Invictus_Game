using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PausePlayController : MonoBehaviour
{
    public TMP_Text buttonText;
    private bool isPaused = false;

    void Start()
    {
        // Assign the Button component
        Button pausePlayButton = GetComponent<Button>();
        pausePlayButton.onClick.AddListener(TogglePausePlay);

        // Set initial button text
        UpdateButtonText();
    }

    void TogglePausePlay()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // Pause the game
        }
        else
        {
            Time.timeScale = 1f; // Resume the game
        }

        UpdateButtonText();
    }

    void UpdateButtonText()
    {
        buttonText.text = isPaused ? "Play" : "Pause";
    }
}
