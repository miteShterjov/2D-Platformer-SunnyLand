using UnityEngine;
using UnityEngine.Events;

// Attach to the child GameObject that has IsTrigger = true
public class ChildTriggerCaller : MonoBehaviour
{
    [SerializeField] GameObject deathEffect;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GetComponentInParent<Trap_TriggerSpikes>().IsTriggered = true;
            gameObject.transform.SetParent(null);
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);            
        }
    }
}
