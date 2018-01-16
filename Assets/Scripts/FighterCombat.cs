using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Linq;

public class FighterCombat : NetworkBehaviour
{
    public GameObject turret;
    public FighterType ft;
    public GunController gunController;
    public GunType gun; //for testing purposes

    public delegate void TakeDamageDelegate(int side, int damage);
    public delegate void DieDelegate();
    public delegate void RespawnDelegate();

    [SyncEvent(channel = 1)]
    public event TakeDamageDelegate EventTakeDamage;

    [SyncEvent]
    public event DieDelegate EventDie;

    [SyncEvent]
    public event RespawnDelegate EventRespawn;

    [SyncVar]
    public int health = 100;

    [SyncVar]
    public int energy = 100;

    [SyncVar]
    public bool alive = true;

    [SyncVar]
    public bool sprinting = false;

    [SyncVar]
    public string fighterName;

    [SyncVar]
    public string fighterType;

    [SyncVar]
    public string team;

    float regenTimer;

    [Server]
    public void InitializeFromFighterType(FighterType newFT)
    {
        ft = newFT;
        fighterType = ft.fighterTypeName;
        health = ft.maxHealth;
        energy = ft.maxEnergy;
        fighterName = Guid.NewGuid().ToString().Substring(0,8);
        team = "team-" + fighterName;

        GetComponent<SpriteRenderer>().sprite = ft.skinBody;
    }

    public override void OnStartClient()
    {
        if (NetworkServer.active)
            return;

        //TODO
        FighterType found = FighterTypeManager.Lookup(fighterType);
        ft = found;
        GetComponent<SpriteRenderer>().sprite = ft.skinBody;
    }

    private void Awake() // for testing purposes
    {
        gun = gunController.EquippedGun;
    }

    [ServerCallback]
    void Update()
    {
        if (!alive)
        {
            return;
        }

        // energy recharges over time
        if (Time.time > regenTimer)
        {
            if (energy < ft.maxEnergy && !sprinting)
            {
                energy += ft.energyRegen;
            }
            regenTimer = Time.time + 0.1f;
        }

    }

    [Server]
    public void GotHit(int side, int damage, string team)
    {
        if (team != null && this.team == team)
        {
            return;
        }

        EventTakeDamage(side, damage);
        TakeDamage(damage);
    }


    [Server]
    public void TakeDamage(int amount)
    {
        if (!alive)
            return;

        if (health > amount)
        {
            health -= amount;
        }
        else
        {
            health = 0;
            alive = false;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            // play explosion!
            EventDie();
        }
    }

    public void Fire()
    {
        if (PlayGame.GetComplete())
            return;

        if (!alive)
            return;

        gunController.Fire(team);
    }

    [Command]
    public void CmdFire()
    {
        Fire();
    }

    [Server]
    void Reload()
    {
        if (PlayGame.GetComplete())
            return;

        if (!alive)
            return;

        gunController.Reload();
    }

    [Command]
    public void CmdReload()
    {
        Reload();
    }

    [Command]
    public void CmdKillSelf()
    {
        TakeDamage(1000000);
    }

    public override void OnStartLocalPlayer()
    {
    }
}
