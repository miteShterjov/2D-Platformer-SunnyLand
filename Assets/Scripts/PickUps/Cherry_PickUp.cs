using UnityEngine;

public class Cherry_PickUp : MonoBehaviour, IPickUp
{
    public static readonly int anim_param_pickup = Animator.StringToHash("isPickedUp");
    
    public void OnObjectPickup()
    {
        print("When I pick up the cherry, the player's health goes up by 1.");

        GetComponent<Animator>().SetBool(anim_param_pickup, true);
    }
}
