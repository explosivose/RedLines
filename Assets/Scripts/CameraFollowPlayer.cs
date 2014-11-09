using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour 
{
	public Vector3 offset;
	public Vector3 defaultRotation;
	
	
	
	void Update () 
	{
		Transform playertr = Player.Instance.transform;
		if (Player.Instance.isDead)
		{
			Quaternion rotation = Quaternion.LookRotation(playertr.position - transform.position);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 4f);
		}
		else
		{
			Vector3 target = playertr.position + offset;

			transform.position = target;
			
			float V = Player.Instance.thrust/Player.Instance.maxThrust;//Input.GetAxisRaw("Vertical");
	
			Quaternion rotation = Quaternion.Euler(defaultRotation);
			rotation *= Quaternion.AngleAxis(V * 45f, playertr.transform.forward);
			rotation *= Quaternion.AngleAxis(V * 10f, transform.right);
			transform.rotation = Quaternion.Lerp (transform.rotation, rotation, Time.deltaTime);
			
		}
		
	}
	
}