// Import necessary libraries
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

// UxManager class responsible for handling user interface interactions
public class UxManager : MonoBehaviour, IPointerDownHandler
{
    public Image audioToggler;
    public Sprite noVolume;
    public Sprite yesVolume;

    // Global volume checker
    private bool isMuted = false;

    void Awake()
    {
        Input.backButtonLeavesApp = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // Implement the IPointerDown interface method
    public void OnPointerDown(PointerEventData eventData)
    {
        // Check if the clicked object is the "Play" button
        if (gameObject.name == "PlayButton")
        {
            // Load the "GameSelection" scene
            SceneManager.LoadScene("GameSelection");
        }
        // Check if the clicked object is the "Statistics" button
        else if (gameObject.name == "StatsButton")
        {
            // Load the "Statistics" scene
            SceneManager.LoadScene("Statistics");
        }
        // Check if the clicked object is the "Exit" button
        else if (gameObject.name == "ExitButton")
        {
            // Quit the application
            Application.Quit();
        }
        // Check if the clicked object is the "AudioToggler" button
        else if(gameObject.name == "AudioToggler")
        {
            Mute();
        }
        // Check if the clicked object is the "Slot1" button
        else if (gameObject.name == "Slot1")
        {
            // Load the "Level" scene
            SceneManager.LoadScene("Level");
        }
        // Check if the clicked object is the "BackButton" button
        else if (gameObject.name == "BackButton")
        {
            SceneManager.LoadScene("Main");
        }
    }

    // Mute or unmute the application's audio sources
    public void Mute()
    {
        // Toggle the mute state
        isMuted = !isMuted;

        // Set the audio volume based on the mute state
        AudioListener.volume = isMuted ? 0 : 1;

        if (isMuted == true) 
        {
            audioToggler.sprite = noVolume;
        } 
        else if (isMuted == false) 
        {
            audioToggler.sprite = yesVolume;
        }
    }
}