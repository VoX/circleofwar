using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class FighterMovement : NetworkBehaviour
{
    Vector2 moveSpeed;

    // for client movement command throftling
    Vector2 oldMoveForce = new Vector2(0,0);

    // server movement
    [SyncVar]
    Vector2 moving;

    public bool autoMove = false;

    bool firingWeapon;
    bool m_focus = true;

    [SyncVar]
    public float upperBodyAngle;

    public Transform upperBody;
    public GameObject explosion;
    public GameObject tracks;
    public FighterCombat fc;
    public InteractBasic interactZone;

    public float trackTimer;

    float fireWeaponTimer;
    float turrentSendTimer = 0.0f;
    float turrentSendDelay = 0.1f;

    void Awake()
    {
        autoDeathTime = Time.time + Random.Range(5, 60);
    }

    void Update()
    {
        if (NetworkServer.active)
            UpdateServer();

        if (NetworkClient.active)
            UpdateClient();
    }

    void UpdateServer()
    {
        if (!fc.alive)
        {
            GetComponent<Rigidbody2D>().drag = 5;
            GetComponent<Rigidbody2D>().angularDrag = 5;
            return;
        }
        // update x movement
        if (moving.x > 0)
        {
            moveSpeed.x += fc.ft.acceleration;
            if (moveSpeed.x >= fc.ft.topRunSpeed)
            {
                moveSpeed.x = fc.ft.topRunSpeed;
            }
        }
        else if (moving.x < 0)
        {
            moveSpeed.x -= fc.ft.acceleration;
            if (moveSpeed.x <= -fc.ft.topRunSpeed)
            {
                moveSpeed.x = -fc.ft.topRunSpeed;
            }
        }
        else
        {
            moveSpeed.x *= 0.18f;
        }

        // update y movement
        if (moving.y > 0)
        {
            moveSpeed.y += fc.ft.acceleration;
            if (moveSpeed.y >= fc.ft.topRunSpeed)
            {
                moveSpeed.y = fc.ft.topRunSpeed;
            }
        }
        else if (moving.y < 0)
        {
            moveSpeed.y -= fc.ft.acceleration;
            if (moveSpeed.y <= -fc.ft.topRunSpeed)
            {
                moveSpeed.y = -fc.ft.topRunSpeed;
            }
        }
        else
        {
            moveSpeed.y *= 0.18f;
        }
        
        Vector2 d = new Vector2(moveSpeed.x, moveSpeed.y);
        GetComponent<Rigidbody2D>().velocity = d;

        if (autoMove)
        {
            AutoMove();
        }
    }

    void OnApplicationFocus(bool value)
    {
        m_focus = value;
    }



    void UpdateClient()
    {
        if (fc.alive && Time.time > trackTimer && GetComponent<Rigidbody2D>().velocity.magnitude > 0.001f)
        {
            GameObject footprint = (GameObject)GameObject.Instantiate(tracks, transform.position, TrackRotate(GetComponent<Rigidbody2D>().velocity));
            GameObject.Destroy(footprint, 1.5f);
            trackTimer = Time.time + 0.25f;
        }

        if (!isLocalPlayer)
        {
            Vector3 trotationVector = new Vector3(0, 0, upperBodyAngle);
            upperBody.transform.rotation = Quaternion.Euler(trotationVector);
            return;
        }

        if (!m_focus)
            return;

        if (!fc.alive)
        {
            return;
        }

        HandlePlayerMovement();

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

        if (Input.GetKey(KeyCode.F1))
        {
            fc.CmdKillSelf();
        }

        if (Input.GetKey(KeyCode.E))
        {
            interactZone.Interact();
        }

        if (Input.GetKey(KeyCode.R))
        {
            fc.
                CmdReload();
        }

        // keep camera on me
        Vector3 cpos = transform.position;
        cpos.z = Camera.main.transform.position.z;
        Camera.main.transform.position = cpos;

        Vector3 mouse_pos = Input.mousePosition;
        mouse_pos.z = 0.0f;
        Vector3 object_pos = Camera.main.WorldToScreenPoint(transform.position);
        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = mouse_pos.y - object_pos.y;
        float angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
        Vector3 rotationVector = new Vector3(0, 0, angle);
        upperBody.transform.rotation = Quaternion.Euler(rotationVector);

        // point upper body at mouse
        if (Time.time > turrentSendTimer)
        {
            CmdRotateUpperBody(angle);
            turrentSendTimer = Time.time + turrentSendDelay;
        }

        if (firingWeapon)
        {
            FireWeapon();
        }
    }

    void FireWeapon()
    {
        if (Time.time > fireWeaponTimer)
        {
            fireWeaponTimer = Time.time + fc.gunController.EquippedGun.fireRate;
            fc.CmdFire();
        }
    }

    void HandlePlayerMovement()
    {
        float xMoveForce = Input.GetAxis("Horizontal");
        float yMoveForce = Input.GetAxis("Vertical");
        if (xMoveForce > 0) { xMoveForce = 1; }
        if (xMoveForce < 0) { xMoveForce = -1; }
        if (yMoveForce > 0) { yMoveForce = 1; }
        if (yMoveForce < 0) { yMoveForce = -1; }
        Vector2 moveForce = new Vector2(xMoveForce, yMoveForce);
        moveForce = moveForce.normalized;

        if (oldMoveForce != moveForce)
        {
            CmdWalk(moveForce);
            oldMoveForce = moveForce;
        }
    }

    [Server]
    void RotateUpperBody(float angle)
    {
        if (!fc.alive)
            return;

        if (PlayGame.GetComplete())
            return;

        Vector3 rotationVector = new Vector3(0, 0, angle);
        upperBody.transform.rotation = Quaternion.Euler(rotationVector);
        upperBodyAngle = angle;
    }

    [Command]
    public void CmdRotateUpperBody(float angle)
    {
        RotateUpperBody(angle);
    }

    [Server]
    void Walk(Vector2 moving)
    {
        if (PlayGame.GetComplete() || !fc.alive)
        {
            this.moving.x = 0;
            this.moving.y = 0;
            return;
        }

        this.moving = moving;
    }

    [Command]
    public void CmdWalk(Vector2 moving)
    {
        Walk(moving);
    }

    float autoMoveTimer = 0;
    float autoTurret;
    float autoDeathTime;

    [Server]
    void AutoMove()
    {
        if(Time.time > autoDeathTime)
        {
            fc.TakeDamage(50000);
        }
        if (Time.time > autoMoveTimer)
        {
            autoTurret += Random.Range(-90, 90);
            RotateUpperBody(autoTurret);

            float moveX = (Random.Range(-10, 10) * 0.1f);
            float moveY = (Random.Range(-10, 10) * 0.1f);
            Vector2 movement = new Vector2(moveX, moveY);
            movement.Normalize();

            if (fc.gunController.ammo == 0)
                fc.gunController.Reload();

            Walk(movement);
            autoMoveTimer = Time.time + Random.Range(.5f, 4f);
        }
        FireWeapon();
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

}
