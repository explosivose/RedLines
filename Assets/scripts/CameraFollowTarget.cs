using UnityEngine;
using System.Collections;

public class CameraFollowTarget : MonoBehaviour 
{
	
	public Transform target;
	public float followSpeed = 8f;
	public float maxCameraDistance = 20f;
	public float minCameraDistance = 2f;
	public float minLookOffset = 2.5f;
	public float cameraSmoothing = 5f;
	public float minCameraSize = 3.5f;
	public float maxCameraSize = 5f;
	public float cameraShake = 1f;

	private Vector3 lastPosition;
	private Vector3 nextPosition;
	private float lastMoveTime;


	void Start ()
	{
		lastMoveTime = 0f;
		lastPosition = transform.position;
		nextPosition = target.transform.position;
	}



	void FixedUpdate () 
	{


		if (GameManager.Instance.State == GameManager.GameState.GameOver )
				DeathCam ();
		else
				ChaseCam ();
	}

	float previousTargetSpeed = 0f;
	Vector3 cameraShakeOffset = Vector3.zero;
	void ChaseCam()
	{
		float targetSpeed = target.rigidbody2D.velocity.magnitude;
		float targetDeltaSpeed = (targetSpeed - previousTargetSpeed) / Time.fixedDeltaTime;
		previousTargetSpeed = targetSpeed;

		cameraShakeOffset = Random.onUnitSphere * targetDeltaSpeed * cameraShake;

		target.audio.volume = Mathf.Abs(targetDeltaSpeed) * 0.02f;

		if (minLookOffset > 0f) 
		{
			if ( target.rigidbody2D.velocity.x < 0f)
				minLookOffset = -minLookOffset;
		}
		if (minLookOffset < 0f) 
		{
			if ( target.rigidbody2D.velocity.x > 0f)
				minLookOffset = -minLookOffset;
		}

		if (targetSpeed > maxCameraDistance)
			targetSpeed = maxCameraDistance;
		if (targetSpeed < minCameraDistance)
			targetSpeed = minCameraDistance;
		nextPosition = target.position + Vector3.back *  ((maxCameraDistance+minCameraDistance)-targetSpeed);
		if (transform.position != lastPosition) 
		{
			lastPosition = transform.position;
			lastMoveTime = Time.time;
		}
		transform.position = Vector3.Slerp (lastPosition, nextPosition, Time.deltaTime * followSpeed);
		Vector3 lookTarget = target.position + (transform.right * minLookOffset) + cameraShakeOffset;
		Vector3 lookDirection = lookTarget - transform.position;
		Quaternion lookRotation = Quaternion.LookRotation (lookDirection);
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * cameraSmoothing);
	}

	void DeathCam()
	{
		nextPosition = target.position + Vector3.back * 50f;
		if (transform.position != lastPosition) 
		{
			lastPosition = transform.position;
			lastMoveTime = Time.time;
		}
		transform.position = Vector3.Slerp (lastPosition, nextPosition, (Time.time - lastMoveTime) * followSpeed);
		transform.LookAt (target.position);
	}
}
