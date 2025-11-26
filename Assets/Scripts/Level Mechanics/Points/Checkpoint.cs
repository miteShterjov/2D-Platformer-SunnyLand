using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Sprite checkpointON;
    [SerializeField] private Sprite checkpointOFF;
    private bool wasActive = false;
    private bool isActive = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (wasActive) return;
            if (isActive) return;

            GameManager.Instance.SetActiveCheckpoint(this);
            EnableCheckpoint();
        }
    }

    private void EnableCheckpoint()
    {
        isActive = true;
        GetComponent<SpriteRenderer>().sprite = checkpointON;
    }

    public void DisableCheckpoint()
    {
        isActive = false;
        wasActive = true;
        GetComponent<SpriteRenderer>().sprite = checkpointOFF;
    }
}
