using UnityEngine;
using UnityEngine.Networking;

public class InteractBasic : NetworkBehaviour
{
    GameObject pickup;
    bool cooldown;

    void OnTriggerStay2D(Collider2D col)
    {
        if (cooldown == false)
        {
            if (col.CompareTag("Item"))
            {
                pickup = col.gameObject;
                Invoke("ResetPickup", 0.3f);
            }
            Invoke("ResetCooldown", 0.5f);
            cooldown = true;
        }
    }

    void ResetPickup()
    {
        pickup = null;
    }

    void ResetCooldown()
    {
        cooldown = false;
    }

    [Command]
    public void CmdInteract()
    {
        if (PlayGame.GetComplete())
            return;

        if (pickup == null)
            return;

        if (pickup.GetComponent<GunPickup>() != null)
        {
            GunType oldGun = GetComponentInChildren<GunController>().EquippedGun;
            if (oldGun != null)
            {
                GameObject newPickup = Instantiate(oldGun.pickupPrefab, transform.position, Quaternion.identity);
                NetworkServer.Spawn(newPickup);
                GetComponentInChildren<GunController>().EquippedGun = pickup.GetComponent<GunPickup>().gun;
                GetComponent<FighterCombat>().gun = GetComponentInChildren<GunController>().EquippedGun;
                Destroy(pickup);
            }
            else
            {
                GetComponentInChildren<GunController>().EquippedGun = pickup.GetComponent<GunPickup>().gun;
                GetComponent<FighterCombat>().gun = GetComponentInChildren<GunController>().EquippedGun;
                Destroy(pickup);
            }
        }
    }
}
