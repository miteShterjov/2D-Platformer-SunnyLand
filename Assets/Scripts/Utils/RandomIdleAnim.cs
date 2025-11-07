using UnityEngine;

namespace TopDown.Misc
{
    public class RandomIdleAnim : MonoBehaviour
    {
        private Animator _animator;

        void Awake()
        {
            _animator = GetComponent<Animator>();

        }

        void Start()
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            _animator.Play(stateInfo.fullPathHash, -1, Random.Range(0f, 1f));
        }
    }
}