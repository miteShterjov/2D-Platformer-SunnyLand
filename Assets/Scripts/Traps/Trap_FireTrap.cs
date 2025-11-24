using System;
using UnityEngine;

public class Trap_FireTrap : MonoBehaviour
{
    public bool IsActive { get => isActive; set => isActive = value; }
    [SerializeField] private GameObject[] fireTraps;
    [SerializeField] private bool isActive = true;
    private static readonly int anim_active_trigger = Animator.StringToHash("active");
    private Animator animator;
    private Collider2D trapCollider;

    void Awake()
    {
        
    }

    void Update()
    {
        TriggerAnimEvent(IsActive);
        TriggerCollider(IsActive);
        TriggerLayerChange(IsActive);
    }

    private void TriggerLayerChange(bool isActive)
    {
        foreach (GameObject fireTrap in fireTraps)
        {
            fireTrap.gameObject.layer = isActive ? LayerMask.NameToLayer("Default") : LayerMask.NameToLayer("Ground");
        }
    }

    private void TriggerAnimEvent(bool isActive)
    {
        foreach (GameObject fireTrap in fireTraps)
        {
            animator = fireTrap.GetComponent<Animator>();
            if (animator != null) animator.SetBool(anim_active_trigger, isActive);
            else Debug.LogWarning("Animator component not found on fireTrap GameObject.");
        }
    }

    private void TriggerCollider(bool isActive)
    {
        foreach(GameObject fireTrap in fireTraps)
        {
            trapCollider = fireTrap.GetComponent<Collider2D>();
            if (trapCollider != null) trapCollider.enabled = isActive;
            else Debug.LogWarning("Collider2D component not found on fireTrap GameObject.");
        }
    }

}
