using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Bomber : NetworkBehaviour {

	public GameObject bomb;
	public GameObject explosion;
	
	public Vector2[] m_path;
	
	[SyncVar]
	public int m_pathIndex = -1;
	
	public int speed = 3;
	
	float bombTimer;
	
	Quaternion targetRotation;
	
	Vector2 CalcVelocity()
	{
		Vector2 v = (m_path[m_pathIndex+1] - m_path[m_pathIndex]) * speed;
		GetComponent<Rigidbody2D>().velocity = v;
		return v;
	}
	
	
	public void FollowPath(Vector2[] path)
	{
		if (path.Length < 2)
		{
			NetworkServer.Destroy(gameObject);
			return;
		}
		
		m_path = path;
		m_pathIndex = 0;
		
		transform.position = path[0];
		Vector2 v = CalcVelocity();
		targetRotation = Quaternion.FromToRotation(transform.right, v);
		transform.rotation = targetRotation * transform.rotation;
	}
	
	void Update()
	{
		if (m_pathIndex == -1)
			return;
		
		if (Time.time > bombTimer)
		{
			bombTimer = Time.time + 0.2f;
			GameObject b = (GameObject)GameObject.Instantiate(bomb, transform.position, transform.rotation);
		}
		
		if (!NetworkServer.active)
			return;
			
		float dist = (m_path[m_pathIndex+1] - (Vector2)transform.position).magnitude;
		if (dist < 0.4f)
		{
			
			m_pathIndex += 1;
			if (m_pathIndex == m_path.Length-1)
			{
				m_pathIndex = -1;
				Invoke("KillMe", 2.0f);
				return;
			}
			Vector2 v = CalcVelocity();
			targetRotation = Quaternion.FromToRotation(transform.right, v);
			transform.rotation = targetRotation * transform.rotation;
		}
	}
	
	void KillMe()
	{
		NetworkServer.Destroy(gameObject);
	}

	// Unity.GeneratedNetworkCode
	public static Vector2[] _ReadArrayVector2_None(NetworkReader reader)
	{
		int num = (int)reader.ReadUInt16();
		if (num == 0)
		{
			return new Vector2[0];
		}
		Vector2[] array = new Vector2[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = reader.ReadVector2();
		}
		return array;
	}

	public static void _WriteArrayVector2_None(NetworkWriter writer, Vector2[] value)
	{
		if (value == null)
		{
			writer.Write((ushort)0);
			return;
		}
		ushort value2 = (ushort)value.Length;
		writer.Write(value2);
		ushort num = 0;
		while ((int)num < value.Length)
		{
			writer.Write(value[(int)num]);
			num += 1;
		}
	}
	
}
