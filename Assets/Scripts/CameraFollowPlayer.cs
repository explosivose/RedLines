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
		else if (CubeMaster.Instance.HyperJumpEnter)
		{
			Vector3 target = playertr.position + Vector3.back * 2f + Vector3.right * 0.5f;
			Vector3 look = new Vector3(0f,0f,-90f);
			Quaternion targetrot = Quaternion.Euler(look);
			
			transform.position = Vector3.Slerp(
				transform.position,
				target,
				Time.deltaTime * 8f);
			
			transform.rotation = Quaternion.Slerp(
				transform.rotation,
				targetrot,
				Time.deltaTime * 2f);
		}
		else
		{
			Vector3 target = playertr.position + offset;

			transform.position = Vector3.Slerp(
				transform.position,
				target,
				Time.deltaTime * 4f);
			
			float V = Player.Instance.thrust/Player.Instance.maxThrust;//Input.GetAxisRaw("Vertical");
	
			Quaternion rotation = Quaternion.Euler(defaultRotation);
			rotation *= Quaternion.AngleAxis(V * 45f, playertr.transform.forward);
			rotation *= Quaternion.AngleAxis(V * 10f, transform.right);
			transform.rotation = Quaternion.Lerp (transform.rotation, rotation, Time.deltaTime * 2f);
			
		}
		
	}
	
}