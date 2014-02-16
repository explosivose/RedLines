using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour 
{
	public Vector3 offset;
	public Vector3 defaultRotation;
	private Transform player;
	
	
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	
	void FixedUpdate () 
	{
		if (Player.isDead)
		{
			Quaternion rotation = Quaternion.LookRotation(player.position - transform.position);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 4f);
		}
		else
		{
			Vector3 target = player.position + offset;
			transform.position = Vector3.Lerp (transform.position, target, Time.deltaTime * 8f);
			
			float V = Input.GetAxis("Vertical");
			Player playerScript = player.GetComponent<Player>();
			//float V = player.rigidbody.velocity.magnitude / playerScript.maxSpeed;
			
			Quaternion rotation = Quaternion.Euler(defaultRotation);
			rotation *= Quaternion.AngleAxis(-V * 22.5f, player.transform.forward);
			rotation *= Quaternion.AngleAxis(V * 60f, transform.right);
			transform.rotation = Quaternion.Lerp (transform.rotation, rotation, Time.deltaTime);
			
		}

	}
	
}
