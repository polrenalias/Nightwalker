// Import necessary libraries
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

// ButtonController class responsible for handling button interactions
public class ButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // References to game objects in the scene
    public GameObject pauseMenu;
    public GameObject pauseButton;
    public GameObject movementControls;
    public GameObject actionControls;

    // Reference to the player controller
    private PlayerController playerController;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Find the PlayerController object in the scene
        playerController = FindObjectOfType<PlayerController>();
    }

    // Called when a pointer is pressed on the object
    public void OnPointerDown(PointerEventData eventData)
    {
        // Check which button was pressed based on its name
        if (gameObject.name == "LeftButton")
        {
            playerController.SetMovingLeft(true);
        }
        else if (gameObject.name == "RightButton")
        {
            playerController.SetMovingRight(true);
        }
        else if (gameObject.name == "JumpButton")
        {
            playerController.SetJumping(true);
        }
        else if (gameObject.name == "AttackButton")
        {
            playerController.SetAttacking(true);
        }
        else if (gameObject.name == "DefendButton")
        {
            playerController.SetDefending(true);
        }
        else if (gameObject.name == "PauseButton")
        {
            // Pause the game
            Time.timeScale = 0;
            pauseButton.SetActive(false);
            movementControls.SetActive(false);
            actionControls.SetActive(false);
            pauseMenu.SetActive(true);
        }
        else if (gameObject.name == "ResumeButton")
        {
            // Resume the game
            Time.timeScale = 1;
            pauseButton.SetActive(true);
            movementControls.SetActive(true);
            actionControls.SetActive(true);
            pauseMenu.SetActive(false);
        }
        else if (gameObject.name == "ExitButton")
        {
            // Load the "Main" scene
            Time.timeScale = 1;
            pauseButton.SetActive(true);
            movementControls.SetActive(true);
            actionControls.SetActive(true);
            SceneManager.LoadScene("Main");
        }
    }

    // Called when a pointer is released on the object
    public void OnPointerUp(PointerEventData eventData)
    {
        // Check which button was released based on its name
        if (gameObject.name == "LeftButton")
        {
            playerController.SetMovingLeft(false);
        }
        else if (gameObject.name == "RightButton")
        {
            playerController.SetMovingRight(false);
        }
        else if (gameObject.name == "JumpButton")
        {
            playerController.SetJumping(false);
        }
        else if (gameObject.name == "AttackButton")
        {
            playerController.SetAttacking(false);
        }
        else if (gameObject.name == "DefendButton")
        {
            playerController.SetDefending(false);
        }
    }
}
