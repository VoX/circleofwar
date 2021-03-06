﻿using UnityEngine;
using UnityEngine.Networking;

public class Missile : NetworkBehaviour {

	public GameObject explosion;
	public int damage;
	public string team;
	
	Vector3 startPos;	
	float deathTimer;
    [SerializeField]
    float lifeTime;
	
	public override void OnStartClient()
	{
		GameObject exp = Instantiate(explosion, transform.position, Quaternion.identity);
		Destroy(exp, 1.0f);
	}
	
	public override void OnStartServer()
	{
		startPos = transform.position;
		deathTimer = Time.time + lifeTime;
	}
	
	[ServerCallback]
	void Update()
	{
		if (Time.time > deathTimer)
		{
			deathTimer = 0;
			NetworkServer.Destroy(gameObject);
		}
	}
	
	[ServerCallback]
	void OnTriggerEnter2D(Collider2D collider)
	{
		if (deathTimer == 0)
			return;
		
		bool destroyMe = false;		
		
		var fc = collider.gameObject.GetComponentInParent<FighterController>();
		if (fc != null)
		{
			int side = GetHitSide(startPos, collider.gameObject.transform.position, collider.gameObject.transform.right);
			fc.GotHit(side, damage, team);
			destroyMe = true;
			//Debug.Log ("Angle: " + angle + " AngleDir: " + angleDir + " side: " + side);			
		}

		Exploder ex = collider.gameObject.GetComponent<Exploder>();
		if (ex != null)
		{
			ex.Explode();
			destroyMe = true;
		}
		
		// destroy missile
		if (destroyMe || collider.gameObject.layer == 11)
		{
			deathTimer = 0;
			NetworkServer.Destroy(gameObject);
		}
	}
	
	public override void OnNetworkDestroy()
	{
		// create explosion
		GameObject exp = Instantiate(explosion, transform.position, Quaternion.identity);
		Destroy(exp, 1.0f);	
	}
	
	// left is positive. right is negative
	public static float AngleDir(Vector2 A, Vector2 B)
	{
		return -A.x * B.y + A.y * B.x;
	}
	
	static public int GetHitSide(Vector3 startPos, Vector3 targetPos, Vector3 targetForward)
	{
		Vector3 v = (startPos - targetPos).normalized;
		Vector3 front = targetForward;
		int side = 0;
		
		// left is +, right is -
		float angleDir = AngleDir(v, front);
		
		// behind = 180, front = 0
		float angle = Vector3.Angle(front, v);
		if (angle < 30)
		{
			side = 0; //front
		} else if (angle > 150)
		{
			side = 2; // back
		}
		else
		{
			if (angleDir > 0)
			{
				side = 3; // left
			}
			else
			{
				side = 1; //right
			}
		}
		return side;
	}
}
