﻿using System;
using UnityEngine;
using UnityEngine.Networking;

public class FighterController : NetworkBehaviour
{
    GameObject pickup;
    bool cooldown;

    [SerializeField]
    public Gun equippedGun;

    [SerializeField]
    Transform muzzleLoc;
    
    public FighterType ft;

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

    public float energy = 0f;

    [SyncVar]
    public bool alive = true;

    public bool sprinting = false;

    [SyncVar]
    public string fighterName;

    [SyncVar]
    public string fighterType;

    [SyncVar]
    public string team;

    // server movement
    Vector2 moveDirection;

    bool firingWeapon;
    bool m_focus = true;

    public Transform upperBody;
    public GameObject explosion;
    public GameObject tracks;
    Rigidbody2D physBody;

    public float trackTimer;

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
        if (!fighterActionsAllowed())
            return;

        if (pickup == null)
            return;

        if (pickup.GetComponent<Gun>() != null)
        {
            Gun oldGun = equippedGun;
            if (oldGun != null)
            {
                oldGun.transform.SetParent(null);
                oldGun.isRendererEnabled = true;
                oldGun.SetMuzzle(null);
                
                equippedGun = pickup.GetComponent<Gun>();
                equippedGun.transform.SetParent(gameObject.transform);
                equippedGun.isRendererEnabled = false;
                equippedGun.SetMuzzle(muzzleLoc);
            }
            else
            {
                equippedGun = pickup.GetComponent<Gun>();
                equippedGun.transform.SetParent(gameObject.transform);
                equippedGun.isRendererEnabled = false;
                equippedGun.SetMuzzle(muzzleLoc);
            }
        }
    }

    [Server]
    public void InitializeFromFighterType(FighterType newFT)
    {
        ft = newFT;
        fighterType = ft.fighterTypeName;
        health = ft.maxHealth;
        energy = ft.maxEnergy;
        fighterName = Guid.NewGuid().ToString().Substring(0, 8);
        team = "team-" + fighterName;

        GetComponent<SpriteRenderer>().sprite = ft.skinBody;
    }

    public override void OnStartClient()
    {
        if (NetworkServer.active)
            return;

        FighterType found = FighterTypeManager.Lookup(fighterType);
        ft = found;
        GetComponent<SpriteRenderer>().sprite = ft.skinBody;
    }

    void Awake()
    {
        physBody = GetComponent<Rigidbody2D>();
        physBody.centerOfMass = new Vector2(0, 0);
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
            physBody.drag = 100;
            physBody.angularDrag = 100;
            // play explosion!
            EventDie();
        }
    }

    bool fighterActionsAllowed()
    {
        return !PlayGame.GetComplete() && alive;
    }

    [Command]
    public void CmdFire()
    {
        if(equippedGun == null || !fighterActionsAllowed())
        {
            return;
        }
        equippedGun.Fire(team);
    }

    [Command]
    public void CmdReload()
    {
        if (equippedGun == null || !fighterActionsAllowed())
        {
            return;
        }
        equippedGun.Reload();
    }
    
    [ClientCallback]
    void FixedUpdate()
    {
        HandleClientMovement();
        HandleClientEnergy();
    }

    [Client]
    void HandleClientEnergy()
    {
        var newenergy = energy + (sprinting ? -ft.sprintRate : ft.energyRegenRate);
        if (newenergy > 0 && newenergy < ft.maxEnergy)
        {
            energy = newenergy;
        }
        if (energy < ft.sprintEnergy)
        {
            sprinting = false;
        }
    }

    void HandleClientMovement()
    {
        if (!isLocalPlayer || !fighterActionsAllowed())
        {
            return;
        }

        physBody.drag = moveDirection.sqrMagnitude > .1f ? 20 : 5;
        physBody.AddForce(moveDirection * ft.acceleration * (sprinting ? 3250 : 2000));
    }

    void OnApplicationFocus(bool value)
    {
        m_focus = value;
    }

    Quaternion TrackRotate(Vector2 moveVect)
    {
        float degrees;
        if (moveVect.x < 0)
        {
            degrees = 360 - (Mathf.Atan2(moveVect.x, moveVect.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            degrees = Mathf.Atan2(moveVect.x, moveVect.y) * Mathf.Rad2Deg;
        }
        return Quaternion.AngleAxis(degrees, Vector3.back);
    }

    [ClientCallback]
    void Update()
    {
        if (alive && Time.time > trackTimer && physBody.velocity.magnitude > 0.01f)
        {
            GameObject footprint = Instantiate(tracks, transform.position, TrackRotate(physBody.velocity));
            Destroy(footprint, 1.5f);
            trackTimer = Time.time + 0.25f;
        }

        if (!isLocalPlayer)
        {
            Vector3 trotationVector = new Vector3(0, 0, physBody.rotation);
            upperBody.transform.rotation = Quaternion.Euler(trotationVector);
            return;
        }

        UpdateCamera();

        if (!m_focus)
            return;

        if (!alive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!sprinting)
            {
                sprinting = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (sprinting)
            {
                sprinting = false;
            }
        }

        HandleMovementInput();

        if (Input.GetMouseButtonDown(0))
        {
            if (!firingWeapon)
            {
                firingWeapon = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (firingWeapon)
            {
                firingWeapon = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            CmdInteract();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            CmdReload();
        }

        Vector3 mouse_pos = Input.mousePosition;
        mouse_pos.z = 0.0f;
        Vector3 object_pos = Camera.main.WorldToScreenPoint(transform.position);
        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = mouse_pos.y - object_pos.y;
        float angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
        Vector3 rotationVector = new Vector3(0, 0, angle);
        upperBody.transform.rotation = Quaternion.Euler(rotationVector);
        physBody.MoveRotation(angle);

        if (firingWeapon)
        {
            CmdFire();
        }
    }

    void HandleMovementInput()
    {
        float xMoveForce = Input.GetAxis("Horizontal");
        float yMoveForce = Input.GetAxis("Vertical");
        if (xMoveForce > 0) { xMoveForce = 1; }
        if (xMoveForce < 0) { xMoveForce = -1; }
        if (yMoveForce > 0) { yMoveForce = 1; }
        if (yMoveForce < 0) { yMoveForce = -1; }
        Vector2 moveForce = new Vector2(xMoveForce, yMoveForce);
        
        moveDirection = moveForce.normalized;
    }

    void UpdateCamera()
    {
        Vector3 cpos = physBody.position;
        cpos.z = Camera.main.transform.position.z;
        Camera.main.transform.position = cpos;
    }
}
