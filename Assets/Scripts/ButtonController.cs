using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private PlayerController playerController;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
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
    }

    public void OnPointerUp(PointerEventData eventData)
    {
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
    }
}
