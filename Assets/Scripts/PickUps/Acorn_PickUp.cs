using UnityEngine;

public class Acorn_PickUp : MonoBehaviour
{
    public static readonly int anim_param_pickup = Animator.StringToHash("isPickedUp");
    
    public void OnObjectPickup()
    {
        print("When I pick up the acorn, the player's stamina goes up by 1.");

        GetComponent<Animator>().SetBool(anim_param_pickup, true);
    }
}
