using UnityEngine;

public class Trap_FallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallDelay = 0.5f;
    [SerializeField] private float destroyDelay = 5f;
    [SerializeField] private float scanRange = 1f;
    [SerializeField] Transform scanner;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (IsPlayerInRange()) Invoke("MakePlatformFall", fallDelay);
    }

    void MakePlatformFall()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        Destroy(gameObject, destroyDelay);
    }

    public bool IsPlayerInRange()
    {
        Collider2D hit = Physics2D.OverlapCircle(scanner.position, scanRange, LayerMask.GetMask("Player"));
        return hit != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(scanner.position, scanRange);
    }
}
