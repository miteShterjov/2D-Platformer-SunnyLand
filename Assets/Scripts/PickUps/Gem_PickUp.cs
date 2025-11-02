using UnityEngine;

public class Gem_PickUp : MonoBehaviour, IPickUp
{
    public static readonly int anim_param_pickup = Animator.StringToHash("isPickedUp");
    
    public void OnObjectPickup()
    {
        print("When I pick up the gem, the player's gem bag will increase by 1. First we need to implement the gem bag.");
        GetComponent<Animator>().SetBool(anim_param_pickup, true);
    }
}
