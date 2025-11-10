using UnityEngine;

public class Finishline : MonoBehaviour
{
    public static readonly int anim_param_finish = Animator.StringToHash("active");
    private Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Finish Line Reached!");
            // Implement level completion logic here, such as loading the next level or displaying a victory screen.
            animator.SetTrigger(anim_param_finish);
        }
    }
}
