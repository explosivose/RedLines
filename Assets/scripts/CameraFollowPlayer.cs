using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour 
{
	
	public Transform player;
	
	public Vector3 minPositionOffset = new Vector3(0f, 0f, 2f);
	public Vector3 maxPositionOffset = new Vector3(-5f, 0f, -10f);
	public Vector3 minLookOffset = Vector3.zero;
	public Vector3 maxLookOffset = new Vector3(10f, 0f, 0f);
	
	public float posDamping = 8f;
	public float angDamping = 2f;

	private Vector3 realOffset;
	private Vector3 lastPosition;
	private Vector3 nextPosition;
	
	void Start ()
	{

		if (player == null) Debug.LogError("Player not set in inspector!");
			
			
		lastPosition = transform.position;
		nextPosition = player.position;
	}
	
	
	
	void FixedUpdate () 
	{
		realOffset = player.position - transform.position;
		if (player.GetComponent<Player>().IsDead )
			DeathCam ();
		else
			ChaseCam ();
	}
	
	float pc;
	void ChaseCam()
	{
		posDamping = player.GetComponent<Player>().thrust;
		
		// player speed as a percentage of the maximum attainable speed
		// note that the maximum attainable speed can change... hence need for "hyperlerp" var
		pc =  GameManager.Instance.HyperLerp;
		
		// returns  1 if the player is on the positive side of the Z axis
		// returns -1 if the player is on the negative side of the Z axis
		float lr = Mathf.Sign (player.position.z);
		
		// calculate camera position
		Vector3 closest = player.position + minPositionOffset * lr;
		Vector3 furthest = player.position + maxPositionOffset * lr;
		nextPosition = Vector3.Lerp (closest, furthest, pc);
		
		transform.position = Vector3.Slerp (lastPosition, nextPosition, Time.deltaTime * posDamping);
		lastPosition = transform.position;

		// calculate camera angle
		Vector3 closestLookDir = (player.position + minLookOffset) - transform.position;
		Vector3 furthestLookDir = (player.position + maxLookOffset) - transform.position;
		Vector3 lookDir = Vector3.Lerp(closestLookDir, furthestLookDir, pc);
		Quaternion lookRot = Quaternion.LookRotation(lookDir);
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * angDamping);
	}
	
	void ChaseCamOld()
	{
		
		transform.position = Vector3.Slerp (lastPosition, nextPosition, Time.deltaTime * posDamping);
		Vector3 lookTarget = player.position + (transform.right + minLookOffset) ;
		Vector3 lookDirection = lookTarget - transform.position;
		Quaternion lookRotation = Quaternion.LookRotation (lookDirection);
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * angDamping);
	}
	
	void DeathCam()
	{
		nextPosition = player.position + Vector3.back * 50f;
		if (transform.position != lastPosition) 
			lastPosition = transform.position;
		
		transform.position = Vector3.Slerp (lastPosition, nextPosition, Time.deltaTime * posDamping);
		transform.LookAt (player.position);
	}
}
