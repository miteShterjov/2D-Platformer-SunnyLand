using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out IPickUp pickUp)) pickUp.OnObjectPickup();
    }
}
