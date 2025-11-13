using UnityEngine;

public class Startline : MonoBehaviour
{
    public static readonly int anim_param_start = Animator.StringToHash("triggered");

    [SerializeField] private bool isFliped = false;

    private Animator animator;
    
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (isFliped)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        animator.SetTrigger(anim_param_start);
        Debug.Log("Start Line Crossed!");
    }
}
