using UnityEngine;

public class ObjectDestroy : MonoBehaviour
{
    public void OnDestroyObject() => Destroy(this.gameObject);
}