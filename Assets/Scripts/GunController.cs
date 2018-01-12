using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GunController : NetworkBehaviour
{
    [SerializeField]
    GunType equippedGun;

    [SyncVar]
    public int ammo;
    [SyncVar]
    public int maxAmmo;
    public Transform muzzle;

    float subtractOffset;

    public GunType EquippedGun
    {
        get
        {
            return equippedGun;
        }
        set
        {
            this.equippedGun = value;
            ammo = 0;
            maxAmmo = equippedGun.startMaxAmmo;
        }
    }

    public void Fire(string team)
    {
        if (equippedGun != null && ammo > 0)
        {
            if(equippedGun.splash) //shotgun or similar
            {
                for(int i=0; i < equippedGun.splashCount; i++)
                {
                    switch (equippedGun.splashCount)
                    {
                        case 3:
                            subtractOffset = .5f;
                            break;
                        case 4:
                            subtractOffset = 1f;
                            break;
                        case 5:
                            subtractOffset = 2f;
                            break;
                    }

                    GameObject bullet = Instantiate(equippedGun.bulletPrefab, muzzle.position, muzzle.rotation);
                    bullet.transform.Rotate(new Vector3(0f, 0f, (i - subtractOffset)));
                    bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * equippedGun.shotSpeed;
                    bullet.GetComponent<Missile>().damage = equippedGun.damage;
                    bullet.GetComponent<Missile>().team = team;
                    NetworkServer.Spawn(bullet);
                }
            }
            else //one shot, one bullet
            {
                GameObject bullet = Instantiate(equippedGun.bulletPrefab, muzzle.position, muzzle.rotation);
                bullet.transform.Rotate(new Vector3(0f, 0f, (Random.Range(-equippedGun.spread, equippedGun.spread))));
                bullet.GetComponent<Rigidbody2D>().velocity = muzzle.right * equippedGun.shotSpeed;
                bullet.GetComponent<Missile>().damage = equippedGun.damage;
                bullet.GetComponent<Missile>().team = team;
                NetworkServer.Spawn(bullet);
            }
            
            ammo--;
        }
    }

    [Server]
    public void Reload()
    {
        ammo = maxAmmo;
    }

    private void Awake() //for testing
    {
        maxAmmo = equippedGun.startMaxAmmo;
    }
}
