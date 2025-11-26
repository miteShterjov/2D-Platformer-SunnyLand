using UnityEngine;

public class Cherry_PickUp : MonoBehaviour, IPickUp
{
    [SerializeField] private float healthBoostAmount = 1f;
    private static readonly int anim_param_pickup = Animator.StringToHash("isPickedUp");

    public void OnObjectPickup(GameObject player)
    {
        player.GetComponent<PlayerStats>()?.RegenerateHealth((int)healthBoostAmount);
        GetComponent<Animator>().SetBool(anim_param_pickup, true);
    }
}
