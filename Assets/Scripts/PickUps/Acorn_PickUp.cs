using UnityEngine;

public class Acorn_PickUp : MonoBehaviour
{
    [SerializeField] private float staminaBoostAmount = 1f;
    private static readonly int anim_param_pickup = Animator.StringToHash("isPickedUp");

    public void OnObjectPickup(GameObject player)
    {
        player.GetComponent<PlayerStats>()?.RegenerateStamina(staminaBoostAmount);
        GetComponent<Animator>().SetBool(anim_param_pickup, true);
    }
}
