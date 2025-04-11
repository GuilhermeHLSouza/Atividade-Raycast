using UnityEngine;

public class DestroyPlayerOnTouch : MonoBehaviour
{
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Destroy(other.gameObject);
        }
    }
}
