using UnityEngine;

public class Trap_SpikedBall : MonoBehaviour
{
    [SerializeField] private float pushForce = 5f;
    [SerializeField] private Rigidbody2D spikeRigidbody;

    void Start()
    {
        Vector2 pushVector = new Vector2(pushForce, 0);
        spikeRigidbody.AddForce(pushVector, ForceMode2D.Impulse);
    }
}
