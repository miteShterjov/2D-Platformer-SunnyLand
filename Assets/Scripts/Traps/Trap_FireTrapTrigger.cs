using System;
using UnityEngine;

public class Trap_FireTrapTrigger : MonoBehaviour
{
    [SerializeField] private Sprite leaverOn;
    [SerializeField] private Sprite leaverOff;

    private bool playerInRange = false;
    
    private SpriteRenderer spriteRenderer;
    private InputSystem_Actions inputActions;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        inputActions = new InputSystem_Actions();
        
        inputActions.Enable();
    }

    void Start()
    {
        inputActions.Player.Interact.performed += ctx => PlayerInteractionEvent();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void PlayerInteractionEvent()
    {
        if (!playerInRange) return;
        GetComponentInParent<Trap_FireTrap>().IsActive = !GetComponentInParent<Trap_FireTrap>().IsActive;
        UpdateLeaverSprite();
        
    }

    private void UpdateLeaverSprite()
    {
        if (spriteRenderer.sprite == leaverOff) spriteRenderer.sprite = leaverOn;
        else spriteRenderer.sprite = leaverOff;
    }
}
