using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	public float maxSpeed  = 10f;
	public float turnSpeed = 100f;
	public Transform deathsplosion;
	public static bool isDead = false;
	
	private Vector3 direction = Vector3.zero;
	private Vector3 targetPosition = Vector3.zero;

	
	
	// Use this for initialization
	void Start () 
	{
		targetPosition = transform.position;
	}
	
	void Update () 
	{
		if (isDead) 
		{
			Screen.lockCursor = false;
			return;
		}
		movementUpdate();
		MouseCursor();
	}
	
	void MouseCursor()
	{
		if (Screen.lockCursor == false)
		{
			Time.timeScale = 0f;
			if (Input.anyKey)
				Screen.lockCursor = true;
		}
		else
		{
			Time.timeScale = 1f;
		}
	}
	
	void movementUpdate()
	{
		// decide which input to take
		// read mouse axis and other axis inputs
		// use the one with the larger magnitude
		float mx = Input.GetAxis ("Mouse X");	// mouse
		float ha = Input.GetAxis("Horizontal"); // other axis
		float h = ha;
		if (Mathf.Abs(mx) > Mathf.Abs(ha)) h = mx;
		float my = -Input.GetAxis ("Mouse Y");
		float va = -Input.GetAxis("Vertical");
		float v = va;
		if (Mathf.Abs(my) > Mathf.Abs(va)) v = my;
		
		// calculate rotation based on input
		
		float maxrot = 45f;
		Vector3 vrot = transform.rotation.eulerAngles;
		
		// left/right rotation
		vrot.y += h * turnSpeed * Time.deltaTime;
		if (vrot.y >  90f+maxrot) vrot.y =  90f+maxrot; // clamp between 90+max and 90-max
		if (vrot.y <  90f-maxrot) vrot.y =  90f-maxrot;
		
		// up/down rotation
		vrot.z += v * turnSpeed * Time.deltaTime;
		if (vrot.z > maxrot      && vrot.z <  90f) vrot.z = maxrot;
		if (vrot.z < 360f-maxrot && vrot.z > 270f) vrot.z = 360f-maxrot;
		
		// calculate movement based on rotation
		
		// need to avoid the wrap around from 0deg to 359deg for movement
		// i.e. 359deg becomes -1deg
		if (vrot.z > 270f) vrot.z -= 360f;
		
		float x = Rescale(vrot.y, 90f+maxrot, 90f-maxrot, 1f, -1f);
		float y = Rescale(vrot.z, maxrot, -maxrot, 1f, -1f);
		// move direction relative to camera orientation
		direction = Camera.main.transform.TransformDirection(new Vector3(x, -y));
		direction.z = 0f;	// remove any Z component added by TransformDirection()
		targetPosition += direction * maxSpeed * Time.deltaTime;
		
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 16f);
		transform.rotation = Quaternion.Euler(vrot);
		
	}
	
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Death" && !isDead)
		{
			Death();
		}
	}
	
	void Death()
	{
		Instantiate(deathsplosion, transform.position, transform.rotation);
		maxSpeed = 0f;
		//rigidbody.useGravity = true;
		isDead = true;
	}
	
	// Rescale a value from old range to new range
	// i.e. 5 is between 1 (min) and 10 (max)
	// Rescale(ref 5, 1, 10, 30, 50) --> 40
	private float Rescale(float value, float oldMax, float oldMin, float newMax, float newMin)
	{
		float oldRange = oldMax - oldMin;
		float newRange = newMax - newMin;
		return (((value - oldMin)*newRange)/oldRange)+newMin;
	}
}
