using UnityEngine;

public class Gem_PickUp : MonoBehaviour, IPickUp
{
    private static readonly int anim_param_pickup = Animator.StringToHash("isPickedUp");
    
    public void OnObjectPickup(GameObject player)
    {
        GameManager.Instance.AddGems();
        GetComponent<Animator>().SetBool(anim_param_pickup, true);
    }
}
