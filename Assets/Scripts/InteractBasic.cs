using UnityEngine;
using UnityEngine.Networking;

public class InteractBasic : NetworkBehaviour
{
    GameObject pickup;

    void OnTriggerStay2D(Collider2D col)
    {
        if(col.CompareTag("Item"))
        {
            pickup = col.gameObject;
        }
    }

    public void Interact()
    {
        if (PlayGame.GetComplete())
            return;

        if (pickup == null)
            return;

        if (pickup.GetComponent<GunPickup>() != null)
        {
            Debug.Log(GetComponentInChildren<GunController>());
            GetComponentInChildren<GunController>().equippedGun = pickup.GetComponent<GunPickup>().gun;
            Destroy(pickup);
        }
    }
}
