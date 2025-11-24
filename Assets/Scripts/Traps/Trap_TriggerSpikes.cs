using UnityEngine;

public class Trap_TriggerSpikes : MonoBehaviour
{
    public bool IsTriggered { get => isTriggered; set => isTriggered = value; }

    [Header("Spike Trigger Settings")]
    [SerializeField] private float spikeSpeed = 5f;
    [SerializeField] private float hitCheckDistance = 0.65f;
    [SerializeField] private GameObject spikeDestroyAnim;
    [SerializeField] private LayerMask groundMask;
    // [SerializeField] private float destroyDelay = 0.2f;

    private bool isTriggered;

    void Update()
    {
        if (IsTriggered) TriggerSpikeMovement();
        HasColisionOccurred();
    }

    private void TriggerSpikeMovement()
    {
        // start moveing the spike down
        transform.Translate(Vector3.down * Time.deltaTime * spikeSpeed);
    }

    private void HasColisionOccurred()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, hitCheckDistance, groundMask);
        if (hit.collider != null)
        {
            Instantiate(spikeDestroyAnim, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Instantiate(spikeDestroyAnim, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * hitCheckDistance);
    }
}
